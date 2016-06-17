using FaunaDB.Client;
using FaunaDB.Types;
using FaunaDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Error returned by the FaunaDB server.
    /// For documentation of error types, see the <see href="https://faunadb.com/documentation#errors">docs</see>.
    /// </summary>
    public class FaunaException : Exception, IEquatable<FaunaException>
    {
        /// <summary>
        /// List of all errors sent by the server.
        /// </summary>
        public List<ErrorData> Errors { get; }

        public RequestResult RequestResult { get; }

        internal static void RaiseForStatusCode(RequestResult rr)
        {
            var code = rr.StatusCode;

            if (200 <= ((int) code) && ((int) code) <= 299)
                return;
            
            var errors = (from _ in ((ArrayV) ((ObjectV) rr.ResponseContent)["errors"]) select (ErrorData) _).ToList();

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    throw new BadRequest(rr, errors);
                case HttpStatusCode.Unauthorized:
                    throw new Unauthorized(rr, errors);
                case HttpStatusCode.Forbidden:
                    throw new PermissionDenied(rr, errors);
                case HttpStatusCode.NotFound:
                    throw new NotFound(rr, errors);
                case HttpStatusCode.MethodNotAllowed:
                    throw new MethodNotAllowed(rr, errors);
                case HttpStatusCode.InternalServerError:
                    throw new InternalError(rr, errors);
                case HttpStatusCode.ServiceUnavailable:
                    throw new UnavailableError(rr, errors);
                default:
                    throw new FaunaException(rr, errors);
            }
        }

        protected FaunaException(RequestResult rr, List<ErrorData> errors) : base(errors.FirstOrDefault()?.Description)
        {
            RequestResult = rr;
            Errors = errors;
        }

        #region boilerplate
        public override bool Equals(object obj) =>
            Equals(obj as FaunaException);

        public bool Equals(FaunaException e) =>
            e != null && e.Errors.SequenceEqual(Errors);

        public static bool operator ==(FaunaException a, FaunaException b) =>
            object.Equals(a, b);

        public static bool operator !=(FaunaException a, FaunaException b) =>
            !object.Equals(a, b);

        public override int GetHashCode() =>
            HashUtil.Hash(Errors);
        #endregion
    }

    /// <summary>
    /// HTTP 400 error.
    /// </summary>
    public class BadRequest : FaunaException
    {
        internal BadRequest(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 401 error.
    /// </summary>
    public class Unauthorized : FaunaException
    {
        internal Unauthorized(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 403 error.
    /// </summary>
    public class PermissionDenied : FaunaException
    {
        internal PermissionDenied(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 404 error.
    /// </summary>
    public class NotFound : FaunaException
    {
        public NotFound(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 405 error.
    /// </summary>
    public class MethodNotAllowed : FaunaException
    {
        internal MethodNotAllowed(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 500 error.
    /// </summary>
    public class InternalError : FaunaException
    {
        internal InternalError(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }

    /// <summary>
    /// HTTP 503 error.
    /// </summary>
    public class UnavailableError : FaunaException
    {
        internal UnavailableError(RequestResult rr, List<ErrorData> errors) : base(rr, errors) {}
    }
}

