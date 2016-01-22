using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using FaunaDB.Errors;
using FaunaDB.Values;

namespace FaunaDB.Client
{
    /// <summary>
    /// Handles actual I/O for a Client. This can be changed for testing.
    /// </summary>
    public interface IClientIO
    {
        Task<HttpResponseMessage> DoRequest(HttpRequestMessage rq);
    }

    class DefaultClientIO : IClientIO
    {
        // HttpClient is IDisposable, but we don't dispose of it.
        // http://stackoverflow.com/questions/15705092/do-httpclient-and-httpclienthandler-have-to-be-disposed
        readonly HttpClient client = new HttpClient();

        public DefaultClientIO(Uri domain, TimeSpan timeout, string user, string password)
        {
            client.BaseAddress = domain;
            client.Timeout = timeout;
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", AuthString(user, password));
        }

        public Task<HttpResponseMessage> DoRequest(HttpRequestMessage rq) =>
            client.SendAsync(rq);

        // NetworkCredentials doesn't work, so have to do this.
        // Based on: http://stackoverflow.com/questions/19851474/networkcredential-working-for-post-but-not-get
        static string AuthString(string user, string password)
        {
            var cred = user == null ? password : $"{user}:{password}";
            byte[] bytes = Encoding.ASCII.GetBytes(cred);
            string base64 = Convert.ToBase64String(bytes);
            return "Basic " + base64;
        }
    }

    /// <summary>
    /// Directly communicates with FaunaDB via JSON.
    /// </summary>
    public class Client
    {
        readonly IClientIO clientIO;

        /// <summary>
        /// Username passed into constructor.
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Password passed into constructor.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Called every time a response comes back from the server.
        /// </summary>
        //todo: this is called whether there's an error or not, do we want to do better?
        //todo: what to do in case of timeout?
        public event EventHandler<RequestResult> OnResponse;

        /// <param name="domain">Base URL for the FaunaDB server.</param>
        /// <param name="scheme">Scheme of the FaunaDB server. Should be "http" or "https".</param>
        /// <param name="port">Port of the FaunaDB server.</param>
        /// <param name="timeout">Timeout. Defaults to 1 minute.</param>
        /// <param name="user">
        /// User name for authorization.
        /// This is useful for root access to a cloud account.
        /// Otherwise, you should use the Key API to get a password and just use the <c>password</c> parameter.
        /// </param>
        /// <param name="password">Auth token for the FaunaDB server.</param>
        /// <param name="clientIO">Optional IInnerClient. Used only for testing.</param>"> 
        public Client(
            string domain = "rest.faunadb.com",
            string scheme = "https",
            int? port = null,
            TimeSpan? timeout = null,
            string user = null,
            string password = null,
            IClientIO clientIO = null)
        {
            if (port == null)
                port = scheme == "https" ? 443 : 80;

            User = user;
            Password = password;

            this.clientIO = clientIO ??
                new DefaultClientIO(new Uri(scheme + "://" + domain + ":" + port), timeout ?? TimeSpan.FromSeconds(60), user, password);
        }

        #region HTTP methods
        /// <summary>
        /// HTTP <c>GET</c>. See the <see href="https://faunadb.com/documentation/rest">docs</see>.
        /// </summary>
        /// <param name="path">
        /// Path relative to the <c>domain</c> this was constructed with.
        /// You can also use a Ref here and it will implicitly conert.
        /// </param>
        /// <param name="query">Values to be converted to URL parameters.</param>
        /// <exception cref="FaunaException"/>
        public Task<Value> Get(string path, IReadOnlyDictionary<string, string> query = null) =>
            Execute(HttpMethodKind.Get, path, query: query);

        /// <summary>
        /// HTTP <c>POST</c>. See the <see href="https://faunadb.com/documentation/rest">docs</see>.
        /// </summary>
        /// <param name="path">
        /// Path relative to the <c>domain</c> this was constructed with.
        /// You can also use a Ref here and it will implicitly convert.
        /// </param>
        /// <param name="data">Value to be converted to request JSON.</param>
        /// <exception cref="FaunaException"/>
        public Task<Value> Post(string path, Value data = null) =>
            Execute(HttpMethodKind.Post, path, data: data);

        /// <summary>
        /// Like <c>Post</c>, but a <c>PUT</c> request.
        /// </summary>
        /// <exception cref="FaunaException"/>
        public Task<Value> Put(string path, Value data = null) =>
            Execute(HttpMethodKind.Put, path, data: data);

        /// <summary>
        /// Like <c>Post</c>, but a <c>PATCH</c> request.
        /// </summary>
        /// <exception cref="FaunaException"/>
        public Task<Value> Patch(string path, Value data = null) =>
            Execute(HttpMethodKind.Patch, path, data: data);

        /// <summary>
        /// Like <c>Post</c>, but a <c>DELETE</c> request.
        /// </summary>
        /// <exception cref="FaunaException"/>
        public Task<Value> Delete(string path) =>
            Execute(HttpMethodKind.Delete, path);

        /// <summary>
        /// Use the FaunaDB query API.
        /// </summary>
        /// <param name="expression">Expression generated by methods of <see cref="Query"/>.</param>
        /// <exception cref="FaunaException"/>
        public Task<Value> Query(Query expression) =>
            Execute(HttpMethodKind.Post, "", data: (Value) expression);

        /// <summary>
        /// Ping FaunaDB.
        /// See the <see href="https://faunadb.com/documentation/rest#other">docs</see>. 
        /// </summary>
        /// <exception cref="FaunaException"/>
        public async Task<string> Ping(string scope = null, int? timeout = null) =>
            (string) await Get("ping", ImmutableUtil.DictWithoutNullValues(
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("timeout", timeout?.ToString()))).ConfigureAwait(false);
        #endregion

        async Task<Value> Execute(HttpMethodKind action, string path, Value data = null, IReadOnlyDictionary<string, string> query = null)
        {
            var startTime = DateTime.UtcNow;
            /*
            `ConfigureAwait(false)` should be used on on all `await` calls in the FaunaDB package.
            http://stackoverflow.com/questions/13489065/best-practice-to-call-configureawait-for-all-server-side-code
            http://blog.stephencleary.com/2012/02/async-and-await.html
            https://channel9.msdn.com/Series/Three-Essential-Tips-for-Async/Async-library-methods-should-consider-using-Task-ConfigureAwait-false-
            http://www.tugberkugurlu.com/archive/the-perfect-recipe-to-shoot-yourself-in-the-foot-ending-up-with-a-deadlock-using-the-c-sharp-5-0-asynchronous-language-features
            */
            var responseHttp = await PerformRequest(action, path, data, query).ConfigureAwait(false);
            var responseText = await responseHttp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var endTime = DateTime.UtcNow;
            var responseContent = Value.FromJson(responseText);

            var rr = new RequestResult(
                this,
                action, path, query, data,
                responseContent, responseHttp.StatusCode, responseHttp.Headers,
                startTime, endTime);

            FireEvent(rr);

            FaunaException.RaiseForStatusCode(rr);
            return ((ObjectV) responseContent)["resource"];
        }

        Task<HttpResponseMessage> PerformRequest(HttpMethodKind action, string path, Value data, IReadOnlyDictionary<string, string> query)
        {
            var dataString = data == null ?  null : new StringContent(data.ToJson());
            var queryString = query == null ? null : QueryString(query);
            if (queryString != null)
                path = $"{path}?{queryString}";
            return clientIO.DoRequest(new HttpRequestMessage(new HttpMethod(action.Name()), path) { Content = dataString });
        }

        void FireEvent(RequestResult rr)
        {
            // MSDN recommends making a copy to prevent race conditions. (https://msdn.microsoft.com/en-us/library/w369ty8x.aspx)
            var handler = OnResponse;
            if (handler != null)
                handler(this, rr);
        }

        /// <summary>
        /// Convert query parameters to a URL string.
        /// </summary>
        internal static string QueryString(IReadOnlyDictionary<string, string> query)
        {
            // Can't just do `new NameValueCollection()` because the one returned by ParseQueryString has a different `ToString` implementation.
            var q = HttpUtility.ParseQueryString("");
            foreach (var kv in query)
                q[kv.Key] = kv.Value;
            return q.ToString();
        }
    }
}
