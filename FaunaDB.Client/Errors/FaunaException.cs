using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Error returned by the FaunaDB server.
    /// For documentation of error types, see the <see href="https://fauna.com/documentation#errors">docs</see>.
    /// </summary>
    public class FaunaException : Exception
    {
        QueryErrorResponse queryErrorResponse;

        /// <summary>
        /// List of all errors sent by the server.
        /// </summary>
        public IReadOnlyList<QueryError> Errors =>
            queryErrorResponse.Errors;

        public int StatusCode =>
            queryErrorResponse.StatusCode;

        protected FaunaException(QueryErrorResponse response) : base(CreateMessage(response.Errors))
        {
            queryErrorResponse = response;
        }

        protected FaunaException(string message) : base(message) { }

        static string CreateMessage(IReadOnlyList<QueryError> errors) =>
            string.Join(", ", from error in errors select $"{error.Code}: {error.Description}");
   }

    /// <summary>
    /// HTTP 400 error.
    /// An exception thrown if FaunaDB cannot evaluate a query.
    /// <para>HTTP 400 error.</para>
    /// </summary>
    public class BadRequest : FaunaException
    {
        internal BadRequest(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 401 error.
    /// An exception thrown if FaunaDB responds with an HTTP 401 (Unauthorized).
    /// <para>HTTP 401 error.</para>
    /// </summary>
    public class Unauthorized : FaunaException
    {
        internal Unauthorized(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 404 error.
    /// An exception thrown if a HTTP 404 (Not Found) is returned from FaunaDB.
    /// <para>HTTP 404 error.</para>
    /// </summary>
    public class NotFound : FaunaException
    {
        public NotFound(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 500 error.
    /// An exception thrown if a HTTP 500 (Internal Server Error) occurs when making a request to FaunaDB. Such
    /// errors represent an internal failure within the database.
    /// <para>HTTP 500 error.</para>
    /// </summary>
    public class InternalError : FaunaException
    {
        internal InternalError(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 503 error.
    /// An exception thrown if a FaunaDB host is unavailable for any reason. For example, if the client cannot connect
    /// to the host, or if the host does not respond.
    /// <para>HTTP 503 error.</para>
    /// </summary>
    public class UnavailableError : FaunaException
    {
        internal UnavailableError(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// An exception thrown if a FaunaDB response is unknown or unparseable by the client.
    /// </summary>
    public class UnknowException : FaunaException
    {
        internal UnknowException(QueryErrorResponse response) : base(response) {}
        internal UnknowException(string message) : base(message) { }
    }
}

