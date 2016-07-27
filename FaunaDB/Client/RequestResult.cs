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
        /// Query data. This is only valid for <see cref="FaunaClient.Get"/> (which is also called by <see cref="FaunaClient.Ping"/> .
        /// </summary>
        public IReadOnlyDictionary<string, string> Query { get; }

        /// <summary>
        /// Request data. This will be null for <see cref="FaunaClient.Get"/>s. 
        /// </summary>
        public string RequestContent { get; }

        /// <summary>
        /// Value returned by the response. Includes "resource" wrapper object, or may be an "errors" object instead.
        /// </summary>
        public string ResponseContent { get; }

        public int StatusCode { get; }

        public IReadOnlyDictionary<string, IEnumerable<string>> ResponseHeaders { get; }

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

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

        public TimeSpan TimeTaken { get { return EndTime - StartTime; } }
    }
}

