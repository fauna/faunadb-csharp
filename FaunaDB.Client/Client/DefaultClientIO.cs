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
using FaunaDB.Errors;

namespace FaunaDB.Client
{
    /// <summary>
    /// Default client that handles all http connections using <see cref="HttpClient"/>.
    /// </summary>
    internal class DefaultClientIO : IClientIO
    {
        private readonly Uri endpoint;
        private readonly TimeSpan? clientTimeout;

        private readonly HttpClient client;
        private readonly AuthenticationHeaderValue authHeader;

        private readonly IReadOnlyDictionary<string, string> customHeaders;

        private LastSeen lastSeen;
        private Version httpVersion;

        public const string StreamingPath = "stream";
        public const HttpMethodKind StreamingHttpMethod = HttpMethodKind.Post;

        public static Builder Builder()
        {
            return new Builder();
        }

        internal DefaultClientIO(Builder builder)
        {
            if (builder.AuthHeader == null)
            {
                builder.Secret.AssertNotNull(nameof(builder.Secret));
            }

            builder.Endpoint.AssertNotNull(nameof(builder.Endpoint));

            this.client = builder.Client ?? CreateClient();
            this.authHeader = builder.AuthHeader ?? AuthHeader(builder.Secret);
            this.lastSeen = builder.LastSeen ?? new LastSeen();
            this.endpoint = builder.Endpoint;
            this.clientTimeout = builder.Timeout;
            this.customHeaders = builder.CustomHeaders;

#if NETSTANDARD2_1
            this.httpVersion = builder.HttpVersion == null ? new Version(2, 0) : builder.HttpVersion;
#else
            this.httpVersion = builder.HttpVersion == null ? new Version(1, 1) : builder.HttpVersion;
#endif
        }

        public IClientIO NewSessionClient(string secret)
        {
            return Builder()
                    .SetClient(client)
                    .SetAuthHeader(AuthHeader(secret))
                    .SetLastSeen(lastSeen)
                    .SetEndpoint(endpoint)
                    .SetTimeout(clientTimeout)
                    .SetHttpVersion(httpVersion)
                    .SetCustomHeaders(customHeaders)
                    .Build();
        }

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null) =>
            DoRequestAsync(method, path, data, query, queryTimeout);

        public Task<StreamingRequestResult> DoStreamingRequest(string data, IReadOnlyDictionary<string, string> query = null) =>
            DoStreamingRequestAsync(data, query);

        private async Task<RequestResult> DoRequestAsync(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null)
        {
            var dataString = data == null ? null : new StringContent(data);
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
            {
                path = $"{path}?{queryString}";
            }

            var startTime = DateTime.UtcNow;

            var message = new HttpRequestMessage(new HttpMethod(method.Name()), $"{endpoint}{path}");
            message.Content = dataString;
            message.Headers.Authorization = authHeader;
            message.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Add("X-FaunaDB-API-Version", "4");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct(EnvironmentEditor.Create()));
            if (httpVersion.ToString() == "1.1")
            {
                message.Headers.Add("Keep-Alive", "3000");
            }

            message.Version = httpVersion;

            // adding custom headers provided during the client creation
            if (customHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in customHeaders)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

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
            {
                response = await DecompressGZip(httpResponse.Content).ConfigureAwait(false);
            }
            else
            {
                response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            var endTime = DateTime.UtcNow;

            if (httpResponse.Headers.Contains("X-Txn-Time"))
            {
                // there shouldn't ever be more than one...
                var time = httpResponse.Headers.GetValues("X-Txn-Time").First();

                lastSeen.SetTxn(Convert.ToInt64(time));
            }

            return new RequestResult(method, path, query, data, response, (int)httpResponse.StatusCode, ToDictionary(httpResponse.Headers), startTime, endTime);
        }

        private async Task<StreamingRequestResult> DoStreamingRequestAsync(string data, IReadOnlyDictionary<string, string> query = null)
        {
            var path = StreamingPath;
            var dataString = data == null ? null : new StringContent(data, Encoding.UTF8, "application/json");
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
            {
                path = $"{path}?{queryString}";
            }

            var startTime = DateTime.UtcNow;

            var message = new HttpRequestMessage(new HttpMethod(StreamingHttpMethod.Name()), $"{endpoint}{path}");
            message.Content = dataString;
            message.Headers.Authorization = authHeader;
            message.Headers.Add("X-FaunaDB-API-Version", "4");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct(EnvironmentEditor.Create()));
            message.Version = httpVersion;
            message.SetTimeout(Timeout.InfiniteTimeSpan);

            // adding custom headers provided during the client creation
            if (customHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in customHeaders)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            var last = lastSeen.Txn;
            if (last.HasValue)
            {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            var httpResponse = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

            Stream response = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var endTime = DateTime.UtcNow;

            if (httpResponse.Headers.Contains("X-Txn-Time"))
            {
                // there shouldn't ever be more than one...
                var time = httpResponse.Headers.GetValues("X-Txn-Time").First();

                lastSeen.SetTxn(Convert.ToInt64(time));
            }

            var errorContent = string.Empty;
            if (!httpResponse.IsSuccessStatusCode)
            {
                StreamReader streamReader = new StreamReader(response);
                errorContent = await streamReader.ReadLineAsync();
            }

            return new StreamingRequestResult(query, data, response, (int)httpResponse.StatusCode, errorContent, ToDictionary(httpResponse.Headers), startTime, endTime);
        }

        private static async Task<string> DecompressGZip(HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzip))
                    {
                        return await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        private static IReadOnlyDictionary<string, IEnumerable<string>> ToDictionary(HttpResponseHeaders headers) =>
            headers.ToDictionary(k => k.Key, v => v.Value);

        /// <summary>
        /// Adds a header with Bearer Auth Token.
        /// </summary>
        private static AuthenticationHeaderValue AuthHeader(string authToken)
        {
            return new AuthenticationHeaderValue("Bearer", authToken);
        }

        /// <summary>
        /// Convert query parameters to a URL string.
        /// </summary>
        private static string QueryString(IReadOnlyDictionary<string, string> query)
        {
            var keyValues = query.Select((keyValue) =>
            {
                return string.Format("{0}={1}",
                                     WebUtility.UrlEncode(keyValue.Key),
                                     WebUtility.UrlEncode(keyValue.Value));
            });

            return string.Join("&", keyValues);
        }

        private static HttpClient CreateClient() =>
            new HttpClient(new TimeoutHandler());
    }

    /// <summary>
    /// Http Client proper timing out
    /// </summary>
    internal static class HttpRequestExtensions
    {
        private static string TimeoutPropertyKey = "RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            request.AssertNotNull(nameof(request));
            request.Properties[TimeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            request.AssertNotNull(nameof(request));

            if (request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout)
            {
                return timeout;
            }

            return null;
        }
    }

    internal class TimeoutHandler : DelegatingHandler
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

            using (var source = NewCancellationTokenSource(timeout.Value, cancellationToken))
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
