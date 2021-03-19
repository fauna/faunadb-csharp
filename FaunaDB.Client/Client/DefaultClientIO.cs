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

        public const string StreamingPath = "stream";
        public const HttpMethodKind StreamingHttpMethod = HttpMethodKind.Post;

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
            message.Headers.Add("X-Fauna-Driver", "csharp");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct());

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
            message.Headers.Add("X-Fauna-Driver", "csharp");
            message.Headers.Add("X-Driver-Env", RuntimeEnvironmentHeader.Construct());
            
            var last = lastSeen.Txn;
            if (last.HasValue)
            {
                message.Headers.Add("X-Last-Seen-Txn", last.Value.ToString());
            }

            var httpResponse = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);
            
            Stream response = await httpResponse.Content.ReadAsStreamAsync();

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

    internal class RuntimeEnvironmentHeader
    {
        private string runtime;
        private string driverVersion;
        private string operatingSystem;
        private string environment;

        private static RuntimeEnvironmentHeader instance;

        private RuntimeEnvironmentHeader() {}

        private void GatherEnvironmentInfo()
        {
            this.environment = GetRuntimeEnvironment();
            this.operatingSystem = GetOperatingSystemName();
            this.runtime = GetCurrentRuntime();
            this.driverVersion = Assembly.Load(new AssemblyName("FaunaDB.Client")).GetName().Version.ToString();
        }

        private static string GetOperatingSystemName()
        {
#if NETCOREAPP
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            return "Unknown";
#else
            // if we're under one of the .net frameworks than OS is Windows
            return "Windows";
#endif
        }

        private static string GetCurrentRuntime()
        {
#if NET45
            return ".net framework 4.5";
#elif NET451
            return ".net framework 4.51";
#elif NET46
            return ".net framework 4.6";
#elif NET461
            return ".net framework 4.61";
#elif NET47
            return ".net framework 4.7";
#elif NETCOREAPP1_0
            return ".net core 1.0";
#elif NETCOREAPP1_1
            return ".net core 1.1";
#elif NETCOREAPP2_0
            return ".net core 2.0";
#elif NETCOREAPP2_1
            return ".net core 2.1";
#elif NETCOREAPP3_0
            return ".net core 3.0";
#elif NETCOREAPP3_1
            return ".net core 3.1";
#elif NETFRAMEWORK
            return ".net framework";
#elif NETCOREAPP
            return ".net core";
#else
            return "unknown .net runtime";
#endif
        }

        private static string GetRuntimeEnvironment()
        {
            var envNetlify = Environment.GetEnvironmentVariable("NETLIFY_IMAGES_CDN_DOMAIN");
            if (envNetlify != null)
            {
                return "Netlify";
            }

            var envVercel = Environment.GetEnvironmentVariable("VERCEL");
            if (envVercel != null)
            {
                return "Vercel";
            }

            var envPath = Environment.GetEnvironmentVariable("PATH");
            if (envPath != null && envPath.Contains("heroku"))
            {
                return "Heroku";
            }

            var envAwsLambda = Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_VERSION");
            if (envAwsLambda != null)
            {
                return "AWS Lambda";
            }

            var envGcpFunctions = Environment.GetEnvironmentVariable("_");
            if (envGcpFunctions != null && envGcpFunctions.Contains("google"))
            {
                return "GCP Cloud Functions";
            }

            var envGoogleCloud = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
            if (envGoogleCloud != null)
            {
                return "GCP Compute Instances";
            }

            var envOryx = Environment.GetEnvironmentVariable("ORYX_ENV_TYPE");
            var envWebsiteInstance = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            if (envOryx != null && envWebsiteInstance != null && envOryx.Contains("AppService"))
            {
                return "Azure Compute";
            }

            return "Unknown";
        }

        public static string Construct()
        {
            if (instance == null)
            {
                instance = new RuntimeEnvironmentHeader();
                instance.GatherEnvironmentInfo();
            }

            return
                $"driver=csharp-{instance.driverVersion}; runtime={instance.runtime}; env={instance.environment}; os={instance.operatingSystem}";
        }
    }
}
