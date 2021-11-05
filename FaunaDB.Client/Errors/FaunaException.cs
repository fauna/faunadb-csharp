using System;
using System.Collections.Generic;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Error returned by the FaunaDB server.
    /// For documentation of error types, see the <see href="https://fauna.com/documentation#errors">docs</see>.
    /// </summary>
    public class FaunaException : Exception
    {
        /// <summary>
        /// Server http error code
        /// </summary>
        public int HttpStatusCode { get; private set; }

        /// <summary>
        /// Server error code
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Array of errors sent by the server.
        /// </summary>
        public IReadOnlyList<string> Position { get; private set; }

        public FaunaException(int httpStatusCode, string code, string message, string[] position) : base(message)
        {
            Code = code;
            HttpStatusCode = httpStatusCode;
            Position = position;
        }
    }

    public class InvalidExpressionException : FaunaException
    {
        public InvalidExpressionException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidExpression, message, position) { }
    }

    public class InvalidRefException : FaunaException
    {
        public InvalidRefException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidRef, message, position) { }
    }

    public class InvalidUrlParameterException : FaunaException
    {
        public InvalidUrlParameterException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidRef, message, position) { }
    }

    public class InstanceAlreadyExistsException : FaunaException
    {
        public InstanceAlreadyExistsException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidRef, message, position) { }
    }

    public class ValidationFailedException : FaunaException
    {
        public IReadOnlyList<string> Failures { get; private set; }

        public ValidationFailedException(int httpStatusCode, string message, string[] position, IReadOnlyList<string> failures) : base(httpStatusCode, ExceptionCodes.ValidationFailed, message, position)
        {
            this.Failures = failures;
        }
    }

    public class InstanceNotUniqueException : FaunaException
    {
        public InstanceNotUniqueException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InstanceNotUnique, message, position) { }
    }

    public class FeatureNotAvailableException : FaunaException
    {
        public FeatureNotAvailableException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.FeatureNotAvailable, message, position) { }
    }

    public class ValueNotFoundException : FaunaException
    {
        public ValueNotFoundException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.ValueNotFound, message, position) { }
    }

    public class InstanceNotFoundException : FaunaException
    {
        public InstanceNotFoundException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InstanceNotFound, message, position) { }
    }

    public class AuthenticationFailedException : FaunaException
    {
        public AuthenticationFailedException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.AuthenticationFailed, message, position) { }
    }

    public class InvalidArgumentException : FaunaException
    {
        public InvalidArgumentException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidArgument, message, position) { }
    }

    public class TransactionAbortedException : FaunaException
    {
        public TransactionAbortedException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.TransactionAborted, message, position) { }
    }

    public class InvalidWriteTimeException : FaunaException
    {
        public InvalidWriteTimeException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidWriteTime, message, position) { }
    }

    public class MissingIdentityException : FaunaException
    {
        public MissingIdentityException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.MissingIdentity, message, position) { }
    }

    public class InvalidTokenException : FaunaException
    {
        public InvalidTokenException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidToken, message, position) { }
    }

    public class FunctionCallException : FaunaException
    {
        public IReadOnlyList<FaunaException> Exceptions { get; private set; }

        public FunctionCallException(int httpStatusCode, string message, string[] position, IReadOnlyList<FaunaException> exceptions) : base(httpStatusCode, ExceptionCodes.CallError, message, position)
        {
            this.Exceptions = exceptions;
        }
    }

    public class StackOverflowException : FaunaException
    {
        public StackOverflowException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.StackOverflow, message, position) { }
    }

    public class PermissionDeniedException : FaunaException
    {
        public PermissionDeniedException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.PermissionDenied, message, position) { }
    }

    public class UnknownException : FaunaException
    {
        public UnknownException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.UnknownCode, message, position) { }

        public UnknownException(string message) : base(-1, ExceptionCodes.UnknownCode, message, new string[] { string.Empty }) { }
    }

    public class StreamingException : FaunaException
    {
        public StreamingException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidRef, message, position) { }
    }

    public class UnauthorizedException : FaunaException
    {
        public UnauthorizedException(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.Unauthorized, message, position) { }
    }
}
