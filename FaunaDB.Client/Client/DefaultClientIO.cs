using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FaunaDB.Collections;

namespace FaunaDB.Client
{
    /// <summary>
    /// Default client that handles all http connections using <see cref="HttpClient"/>.
    /// </summary>
    class DefaultClientIO : IClientIO
    {
        readonly Uri endpoint;
        readonly TimeSpan? clientTimeout;

        readonly HttpClient client;
        readonly AuthenticationHeaderValue authHeader;

        private LastSeen lastSeen;

        internal DefaultClientIO(HttpClient client, AuthenticationHeaderValue authHeader, LastSeen lastSeen, Uri endpoint, TimeSpan? timeout)
        {
            this.client = client;
            this.authHeader = authHeader;
            this.lastSeen = lastSeen;
            this.endpoint = endpoint;
            this.clientTimeout = timeout;
        }

        public DefaultClientIO(string secret, Uri endpoint, TimeSpan? timeout = null, HttpClient httpClient = null)
            : this(httpClient ?? CreateClient(), AuthHeader(secret), new LastSeen(), endpoint, timeout)
        { }

        public IClientIO NewSessionClient(string secret) =>
            new DefaultClientIO(client, AuthHeader(secret), lastSeen, endpoint, clientTimeout);

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null) =>
            DoRequestAsync(method, path, data, query, queryTimeout);

        async Task<RequestResult> DoRequestAsync(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null)
        {
            var dataString = data == null ?  null : new StringContent(data);
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
                path = $"{path}?{queryString}";

            var startTime = DateTime.UtcNow;

            var message = new HttpRequestMessage(new HttpMethod(method.Name()), $"{endpoint}{path}");
            message.Content = dataString;
            message.Headers.Authorization = authHeader;
            message.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Add("X-FaunaDB-API-Version", "2.7");
            message.Headers.Add("X-Fauna-Driver", "csharp");

            var last = lastSeen.Txn;
            if (last.HasValue)
            {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            TimeSpan? timeout = queryTimeout ?? clientTimeout;
            if (timeout.HasValue)
            {
                message.SetTimeout(timeout);
                message.Headers.Add("X-Query-Timeout", timeout.Value.TotalMilliseconds.ToString());
            }

            var httpResponse = await client.SendAsync(message, CancellationToken.None).ConfigureAwait(false);

            string response;

            if (httpResponse.Content.Headers.ContentEncoding.Any(encoding => encoding == "gzip"))
                response = await DecompressGZip(httpResponse.Content).ConfigureAwait(false);
            else
                response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var endTime = DateTime.UtcNow;

            if (httpResponse.Headers.Contains("X-Txn-Time")) {
                // there shouldn't ever be more than one...
                var time = httpResponse.Headers.GetValues("X-Txn-Time").First();

                lastSeen.SetTxn(Convert.ToInt64(time));
            }

            return new RequestResult(method, path, query, data, response, (int)httpResponse.StatusCode, ToDictionary(httpResponse.Headers), startTime, endTime);
        }

        static async Task<string> DecompressGZip(HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzip))
                        return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
        }

        static IReadOnlyDictionary<string, IEnumerable<string>> ToDictionary(HttpResponseHeaders headers) =>
            headers.ToDictionary(k => k.Key, v => v.Value);

        /// <summary>
        /// Encodes secret string using base64.
        /// </summary>
        static AuthenticationHeaderValue AuthHeader(string secret)
        {
            var bytes = Encoding.ASCII.GetBytes(secret);
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }

        /// <summary>
        /// Convert query parameters to a URL string.
        /// </summary>
        static string QueryString(IReadOnlyDictionary<string, string> query)
        {
            var keyValues = query.Select((keyValue) =>
            {
                return string.Format("{0}={1}",
                                     WebUtility.UrlEncode(keyValue.Key),
                                     WebUtility.UrlEncode(keyValue.Value));
            });

            return string.Join("&", keyValues);
        }

        static HttpClient CreateClient() =>
            new HttpClient(new TimeoutHandler());        
    }

    /// <summary>
    /// Http Client proper timing out
    /// </summary>

    static class HttpRequestExtensions
    {
        private static string TimeoutPropertyKey = "RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Properties[TimeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout)
                return timeout;

            return null;
        }
    }

    class TimeoutHandler : DelegatingHandler
    {
        public TimeoutHandler()
        {
            base.InnerHandler = new HttpClientHandler();
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timeout = request.GetTimeout();

            if (!timeout.HasValue)
            {
                return await base.SendAsync(request, cancellationToken);
            }
                
            using(var source = NewCancellationTokenSource(timeout.Value, cancellationToken))
            {
                try
                {
                    return await base.SendAsync(request, source.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }
            }
            
        }

        private CancellationTokenSource NewCancellationTokenSource(TimeSpan timeout, CancellationToken token)
        {
            var source = CancellationTokenSource.CreateLinkedTokenSource(token);
            source.CancelAfter(timeout);
            return source;
        }
    }
}
