using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

using FaunaDB.Values;
using FaunaDB.Query;

namespace FaunaDB.Client
{
    /// <summary>
    /// Stores information about a single request and response.
    /// </summary>
    public class RequestResult
    {
        /// <summary>
        /// The client.
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// HTTP method that was used.
        /// </summary>
        public HttpMethodKind Method { get; }

        /// <summary>
        /// Path that was queried. Relative to client's domain.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Query data. This is only valid for <see cref="Client.Get"/> (which is also called by <see cref="Client.Ping"/> .
        /// </summary>
        public IReadOnlyDictionary<string, string> Query { get; }

        /// <summary>
        /// Request data. This will be null for <see cref="Client.Get"/>s. 
        /// </summary>
        public Expr RequestContent { get; }

        /// <summary>
        /// Value returned by the response. Includes "resource" wrapper object, or may be an "errors" object instead.
        /// </summary>
        public Expr ResponseContent { get; }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders ResponseHeaders { get; }

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        public RequestResult(
            Client client,
            HttpMethodKind method,
            string path,
            IReadOnlyDictionary<string, string> query,
            Expr requestContent,
            Expr responseContent,
            HttpStatusCode statusCode,
            HttpResponseHeaders responseHeaders,
            DateTime startTime,
            DateTime endTime)
        {
            Client = client;
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

        public Tuple<string, string> Auth { get { return new Tuple<string, string>(Client.User, Client.Password); } }
    }

    /// <summary>
    /// A method used by the <see cref="Client"/>.
    /// </summary>
    public enum HttpMethodKind
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }

    public static class HttpMethodUtil
    {
        /// <summary>
        /// All-caps name of the method.
        /// </summary>
        public static string Name(this HttpMethodKind method)
        {
            switch (method)
            {
                case HttpMethodKind.Get:
                    return "GET";
                case HttpMethodKind.Post:
                    return "POST";
                case HttpMethodKind.Put:
                    return "PUT";
                case HttpMethodKind.Patch:
                    return "PATCH";
                case HttpMethodKind.Delete:
                    return "DELETE";
                default:
                    throw new Exception($"Bad value: {method}");
            }
        }
    }
}

