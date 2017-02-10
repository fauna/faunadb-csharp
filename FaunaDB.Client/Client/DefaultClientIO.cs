using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FaunaDB.Collections;

namespace FaunaDB.Client
{
    /// <summary>
    /// Default client that handles all http connections using <see cref="HttpClient"/>.
    /// </summary>
    class DefaultClientIO : IClientIO
    {
        readonly HttpClient client;
        readonly string authHeader;

        internal DefaultClientIO(HttpClient client, string secret)
        {
            this.authHeader = AuthString(secret);
            this.client = client;
        }

        public DefaultClientIO(Uri domain, TimeSpan timeout, string secret)
        {
            client = new HttpClient();
            client.BaseAddress = domain;
            client.Timeout = timeout;
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            authHeader = AuthString(secret);
        }

        public IClientIO NewSessionClient(string secret) =>
            new DefaultClientIO(client, secret);

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
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var httpResponse = await client.SendAsync(message).ConfigureAwait(false);

            string response;

            if (httpResponse.Content.Headers.ContentEncoding.Any(encoding => encoding == "gzip"))
                response = await DecompressGZip(httpResponse.Content).ConfigureAwait(false);
            else
                response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var endTime = DateTime.UtcNow;

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
            new ImmutableDictionary<string, IEnumerable<string>>(headers);

        /// <summary>
        /// Encodes secret string using base 64.
        /// </summary>
        static string AuthString(string secret)
        {
            var bytes = Encoding.ASCII.GetBytes(secret);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Convert query parameters to a URL string.
        /// </summary>
        static string QueryString(IReadOnlyDictionary<string, string> query)
        {
            // Can't just do `new NameValueCollection()` because the one returned by ParseQueryString has a different `ToString` implementation.
            var q = HttpUtility.ParseQueryString("");
            foreach (var kv in query)
                q[kv.Key] = kv.Value;
            return q.ToString();
        }
    }
}
