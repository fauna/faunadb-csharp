using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace FaunaDB.Client
{
    public static class ClientLogger
    {
        /// <summary>
        /// Event handler that sends request descriptions to <c>logger</c>.
        /// </summary>
        /// <example>
        /// myClient.OnResponse += ClientLogger.Logger(Console.WriteLine);
        /// </example>
        public static EventHandler<RequestResult> Logger(Action<string> logger)
        {
            return (client, info) => logger(ShowRequest(info));;
        }

        /// <summary>
        /// String describing a single request.
        /// </summary>
        public static string ShowRequest(RequestResult rr)
        {
            var logged = new StringBuilder();
            Action<string> log = str => logged.Append(str);

            log(string.Format("Fauna {0} /{1}{2}\n", rr.Method.Name(), rr.Path, rr.Query == null ? "" : Client.QueryString(rr.Query)));
            log(string.Format("  Credentials: user: {0}, pass: {1}\n", rr.Client.User, rr.Client.Password));
            if (rr.RequestContent != null)
                log(string.Format("  Request JSON: {0}\n", Indent(rr.RequestContent.ToJson(pretty: true))));

            log(string.Format("  Response headers:\n    {0}\n", Indent(rr.ResponseHeaders.ToString().TrimEnd('\r', '\n', ' '))));
            log(string.Format("  Response JSON:\n    {0}\n", Indent(rr.ResponseContent.ToJson(pretty: true))));
            log(string.Format("  Response ({0}): API processing {1}ms, network latency {2}ms\n",
                rr.StatusCode, ProcessingTime(rr.ResponseHeaders), rr.TimeTaken.Milliseconds));

            return logged.ToString();
        }

        static string Indent(string s)
        {
            return s.Replace("\n", "\n    ");
        }

        static string ProcessingTime(HttpResponseHeaders headers)
        {
            IEnumerable<string> time;
            return headers.TryGetValues("X-HTTP-Request-Processing-Time", out time) ? string.Join("", time) : "N/A";
        }
    }
    /*
                    log(string.Format("Fauna {0} /{1}{2}\n", action, path, query));
                    log(string.Format("  Credentials: {0}\n", credentials_str));
                    if (data != null)
                        log(string.Format("  Request JSON: {0}\n", Indent(data.ToJson(pretty: true))));

                    var responseAndTime = await Time(() => ExecuteWithoutLogging(action, path, data, query));
                    var response = responseAndTime.Item1;
                    var responseHttp = response.Item1;
                    var responseContent = response.Item2;
                    var time = responseAndTime.Item2;

                    log(string.Format("  Response headers:\n    {0}\n", Indent(responseHttp.Headers.ToString().TrimEnd('\r', '\n', ' '))));
                    log(string.Format("  Response JSON:\n    {0}\n", Indent(responseContent.ToJson(pretty: true))));
                    log(
                        string.Format("  Response ({0}): API processing {1}ms, network latency {2}ms\n",
                        responseHttp.StatusCode, ProcessingTime(responseHttp.Headers), time.Milliseconds));

                    return HandleResponse(response);

        async Task<T> Logging<T>(Func<Action<string>, Task<T>> func)
        {
            var logged = new StringBuilder();
            var res = await func(str => logged.Append(str));
            Log(logged.ToString());
            return res;
        }
    */
}

