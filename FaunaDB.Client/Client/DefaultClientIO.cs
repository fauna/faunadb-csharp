using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FaunaDB.Collections;

namespace FaunaDB.Client
{
    /// <summary>
    /// Default client that handles all http connections using <see cref="HttpClient"/>.
    /// </summary>
    class DefaultClientIO : IClientIO
    {
        readonly HttpClient client;
        readonly AuthenticationHeaderValue authHeader;

        private LastSeen lastSeen;

        internal DefaultClientIO(HttpClient client, AuthenticationHeaderValue authHeader, LastSeen lastSeen)
        {
            this.client = client;
            this.authHeader = authHeader;
            this.lastSeen = lastSeen;
        }

        public DefaultClientIO(string secret, Uri endpoint, TimeSpan timeout)
            : this(CreateClient(endpoint, timeout), AuthHeader(secret), new LastSeen())
        { }

        public IClientIO NewSessionClient(string secret) =>
            new DefaultClientIO(client, AuthHeader(secret), lastSeen);

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null) =>
            DoRequestAsync(method, path, data, query);

        async Task<RequestResult> DoRequestAsync(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null)
        {
            var dataString = data == null ?  null : new StringContent(data);
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
                path = $"{path}?{queryString}";

            var startTime = DateTime.UtcNow;

            var message = new HttpRequestMessage(new HttpMethod(method.Name()), path);
            message.Content = dataString;
            message.Headers.Authorization = authHeader;

            var last = lastSeen.Txn;
            if (last.HasValue) {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            var httpResponse = await client.SendAsync(message).ConfigureAwait(false);

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

        static HttpClient CreateClient(Uri endpoint, TimeSpan timeout)
        {
            var client = new HttpClient();
            client.BaseAddress = endpoint;
            client.Timeout = timeout;
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-FaunaDB-API-Version", "2.1");

            return client;
        }
    }
}
