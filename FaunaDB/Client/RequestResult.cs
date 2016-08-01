using System;
using System.Collections.Generic;

namespace FaunaDB.Client
{
    /// <summary>
    /// Stores information about a single request and response.
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// HTTP method that was used.
        /// </summary>
        public HttpMethodKind Method { get; }

        /// <summary>
        /// Path that was queried. Relative to client's domain.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The query parameters submitted on the request.
        /// </summary>
        public IReadOnlyDictionary<string, string> Query { get; }

        /// <summary>
        /// String data submited for the server.
        /// </summary>
        public string RequestContent { get; }

        /// <summary>
        /// String returned by the server.
        /// </summary>
        public string ResponseContent { get; }

        /// <summary>
        /// Http status code result of the request.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Response headers returned by the FaunaDB server.
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<string>> ResponseHeaders { get; }

        /// <summary>
        /// <see cref="DateTime"/> when the query was issued.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// <see cref="DateTime"/> when the query finished.
        /// </summary>
        public DateTime EndTime { get; }

        /// <summary>
        /// Indicates how long the query took to execute.
        /// </summary>
        public TimeSpan TimeTaken { get { return EndTime - StartTime; } }

        public RequestResult(
            HttpMethodKind method,
            string path,
            IReadOnlyDictionary<string, string> query,
            string requestContent,
            string responseContent,
            int statusCode,
            IReadOnlyDictionary<string, IEnumerable<string>> responseHeaders,
            DateTime startTime,
            DateTime endTime)
        {
            Method = method;
            Path = path;
            Query = query;
            RequestContent = requestContent;
            ResponseContent = responseContent;
            StatusCode = statusCode;
            ResponseHeaders = responseHeaders;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}

