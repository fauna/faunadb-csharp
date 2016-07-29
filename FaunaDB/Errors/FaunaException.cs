using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Error returned by the FaunaDB server.
    /// For documentation of error types, see the <see href="https://faunadb.com/documentation#errors">docs</see>.
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
    /// </summary>
    public class BadRequest : FaunaException
    {
        internal BadRequest(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 401 error.
    /// </summary>
    public class Unauthorized : FaunaException
    {
        internal Unauthorized(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 404 error.
    /// </summary>
    public class NotFound : FaunaException
    {
        public NotFound(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 500 error.
    /// </summary>
    public class InternalError : FaunaException
    {
        internal InternalError(QueryErrorResponse response) : base(response) {}
    }

    /// <summary>
    /// HTTP 503 error.
    /// </summary>
    public class UnavailableError : FaunaException
    {
        internal UnavailableError(QueryErrorResponse response) : base(response) {}
    }

    public class UnknowException : FaunaException
    {
        internal UnknowException(QueryErrorResponse response) : base(response) {}
        internal UnknowException(string message) : base(message) { }
    }
}

