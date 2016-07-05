using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FaunaDB.Collections;

namespace FaunaDB.Client
{
    class DefaultClientIO : IClientIO
    {
        // HttpClient is IDisposable, but we don't dispose of it.
        // http://stackoverflow.com/questions/15705092/do-httpclient-and-httpclienthandler-have-to-be-disposed
        readonly HttpClient client = new HttpClient();

        public DefaultClientIO(Uri domain, TimeSpan timeout, string secret)
        {
            client.BaseAddress = domain;
            client.Timeout = timeout;
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", AuthString(secret));
        }

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null) =>
            DoRequestAsync(method, path, data, query);

        async Task<RequestResult> DoRequestAsync(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null)
        {
            var dataString = data == null ?  null : new StringContent(data);
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
                path = $"{path}?{queryString}";

            var startTime = DateTime.UtcNow;

            var httpResponse = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method.Name()), path) { Content = dataString });//.ConfigureAwait(false);
            var response = await httpResponse.Content.ReadAsStringAsync();//.ConfigureAwait(false);

            var endTime = DateTime.UtcNow;

            return new RequestResult(method, path, query, data, response, (int)httpResponse.StatusCode, ToDictionary(httpResponse.Headers), startTime, endTime);
        }

        static IReadOnlyDictionary<string, IEnumerable<string>> ToDictionary(HttpResponseHeaders headers)
        {
            OrderedDictionary<string, IEnumerable<string>> dic = new OrderedDictionary<string, IEnumerable<string>>();

            foreach (var kv in headers)
                dic.Add(kv.Key, kv.Value);

            return dic;
        }

        // NetworkCredentials doesn't work, so have to do this.
        // Based on: http://stackoverflow.com/questions/19851474/networkcredential-working-for-post-but-not-get
        static string AuthString(string secret)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(secret);
            string base64 = Convert.ToBase64String(bytes);
            return $"Basic {base64}";
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
