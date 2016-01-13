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
        public static EventHandler<RequestResult> Logger(Action<string> logger) =>
            (client, info) => logger(ShowRequest(info));

        /// <summary>
        /// String describing a single request.
        /// </summary>
        public static string ShowRequest(RequestResult rr)
        {
            var logged = new StringBuilder();
            Action<string> log = str => logged.Append(str);

            log($"Fauna {rr.Method.Name()} /{rr.Path}{rr.Query == null ? "" : Client.QueryString(rr.Query)}\n");
            log($"  Credentials: user: {rr.Client.User}, pass: {rr.Client.Password}\n");
            if (rr.RequestContent != null)
                log($"  Request JSON: {Indent(rr.RequestContent.ToJson(pretty: true))}\n");

            log($"  Response headers:\n    {Indent(rr.ResponseHeaders.ToString().TrimEnd('\r', '\n', ' '))}\n");
            log($"  Response JSON:\n    {Indent(rr.ResponseContent.ToJson(pretty: true))}\n");
            log($"  Response ({rr.StatusCode}): API processing {ProcessingTime(rr.ResponseHeaders)}ms, network latency {rr.TimeTaken.Milliseconds}ms\n");

            return logged.ToString();
        }

        static string Indent(string s) =>
            s.Replace("\n", "\n    ");

        static string ProcessingTime(HttpResponseHeaders headers)
        {
            IEnumerable<string> time;
            return headers.TryGetValues("X-HTTP-Request-Processing-Time", out time) ? string.Join("", time) : "N/A";
        }
    }
}

