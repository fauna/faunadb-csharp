using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Reflection;
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
        private Version httpVersion;

        public const string StreamingPath = "stream";
        public const HttpMethodKind StreamingHttpMethod = HttpMethodKind.Post;

        internal DefaultClientIO(HttpClient client, AuthenticationHeaderValue authHeader, LastSeen lastSeen, Uri endpoint, TimeSpan? timeout, Version httpVersion)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (authHeader == null)
            {
                throw new ArgumentNullException(nameof(authHeader));
            }
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (lastSeen == null)
            {
                throw new ArgumentNullException(nameof(lastSeen));
            }
            this.client = client;
            this.authHeader = authHeader;
            this.lastSeen = lastSeen;
            this.endpoint = endpoint;
            this.clientTimeout = timeout;
#if NETSTANDARD2_1
            this.httpVersion = httpVersion == null ? new Version(2, 0) : httpVersion;
#else
            this.httpVersion = httpVersion == null ? new Version(1, 1) : httpVersion;
#endif
        }

        public DefaultClientIO(string secret, Uri endpoint, TimeSpan? timeout = null, HttpClient httpClient = null, Version httpVersion = null)
            : this(httpClient ?? CreateClient(), AuthHeader(secret), new LastSeen(), endpoint, timeout, httpVersion)
        { }

        public IClientIO NewSessionClient(string secret) =>
            new DefaultClientIO(client, AuthHeader(secret), lastSeen, endpoint, clientTimeout, httpVersion);

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null) =>
            DoRequestAsync(method, path, data, query, queryTimeout);

        public Task<StreamingRequestResult> DoStreamingRequest(string data, IReadOnlyDictionary<string, string> query = null) =>
            DoStreamingRequestAsync(data, query);

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
            message.Headers.Add("X-FaunaDB-API-Version", "4");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct(EnvironmentEditor.Create()));
            message.Version = httpVersion;

            var last = lastSeen.Txn;
            if (last.HasValue)
            {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            TimeSpan? timeout = queryTimeout ?? clientTimeout ?? client.Timeout;
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

        async Task<StreamingRequestResult> DoStreamingRequestAsync(string data, IReadOnlyDictionary<string, string> query = null)
        {
            var path = StreamingPath;
            var dataString = data == null ?  null : new StringContent(data, Encoding.UTF8, "application/json");
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
                path = $"{path}?{queryString}";
            
            var startTime = DateTime.UtcNow;

            var message = new HttpRequestMessage(new HttpMethod(StreamingHttpMethod.Name()), $"{endpoint}{path}");
            message.Content = dataString;
            message.Headers.Authorization = authHeader;
            message.Headers.Add("X-FaunaDB-API-Version", "4");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct(EnvironmentEditor.Create()));
            message.Version = httpVersion;
            message.SetTimeout(Timeout.InfiniteTimeSpan);
            
            var last = lastSeen.Txn;
            if (last.HasValue)
            {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            var httpResponse = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
            
            Stream response = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var endTime = DateTime.UtcNow;

            if (httpResponse.Headers.Contains("X-Txn-Time")) {
                // there shouldn't ever be more than one...
                var time = httpResponse.Headers.GetValues("X-Txn-Time").First();

                lastSeen.SetTxn(Convert.ToInt64(time));
            }

            var errorContent = String.Empty;
            if (!httpResponse.IsSuccessStatusCode)
            {
                StreamReader streamReader = new StreamReader(response);
                errorContent = await streamReader.ReadLineAsync();
            }

            return new StreamingRequestResult(query, data, response, (int)httpResponse.StatusCode, errorContent, ToDictionary(httpResponse.Headers), startTime, endTime);
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
        /// Adds a header with Bearer Auth Token.
        /// </summary>
        static AuthenticationHeaderValue AuthHeader(string authToken)
        {
            return new AuthenticationHeaderValue("Bearer", authToken);
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
