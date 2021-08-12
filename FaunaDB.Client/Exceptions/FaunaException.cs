using System;

namespace FaunaDB.Client.Exceptions
{
    public class FaunaException : Exception
    {
        public int HttpStatusCode { get; set; }

        public string Code { get; set; }

        public string[] Position { get; set; }

        public FaunaException(int httpStatusCode, string code, string message, string[] position) : base(message)
        {
            Code = code;
            HttpStatusCode = httpStatusCode;
            Position = position;
        }
    }

    public class InvalidExpression : FaunaException
    {
        public InvalidExpression(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidExpression, message, position) { }
    }

    public class InvalidRef : FaunaException
    {
        public InvalidRef(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvaliRef, message, position) { }
    }

    public class InvalidUrlParameter : FaunaException
    {
        public InvalidUrlParameter(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvaliRef, message, position) { }
    }

    public class SchemaNotFound : FaunaException
    {
        public SchemaNotFound(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvaliRef, message, position) { }
    }

    public class InstanceAlreadyExists : FaunaException
    {
        public InstanceAlreadyExists(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvaliRef, message, position) { }
    }

    public class ValidationFailed : FaunaException
    {
        public ValidationFailed(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.ValidationFailed, message, position) { }
    }

    public class InstanceNotUnique : FaunaException
    {
        public InstanceNotUnique(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InstanceNotUnique, message, position) { }
    }

    public class FeatureNotAvailable : FaunaException
    {
        public FeatureNotAvailable(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.FeatureNotAvailable, message, position) { }
    }

    public class ValueNotFound : FaunaException
    {
        public ValueNotFound(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.ValueNotFound, message, position) { }
    }

    public class InstanceNotFound : FaunaException
    {
        public InstanceNotFound(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InstanceNotFound, message, position) { }
    }

    public class AuthenticationFailed : FaunaException
    {
        public AuthenticationFailed(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.AuthenticationFailed, message, position) { }
    }

    public class InvalidArgument : FaunaException
    {
        public InvalidArgument(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidArgument, message, position) { }
    }

    public class TransactionAborted : FaunaException
    {
        public TransactionAborted(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.TransactionAborted, message, position) { }
    }

    public class InvalidWriteTime : FaunaException
    {
        public InvalidWriteTime(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidWriteTime, message, position) { }
    }

    public class MissingIdentity : FaunaException
    {
        public MissingIdentity(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.MissingIdentity, message, position) { }
    }

    public class InvalidScope : FaunaException
    {
        public InvalidScope(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidScope, message, position) { }
    }

    public class InvalidToken : FaunaException
    {
        public InvalidToken(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidToken, message, position) { }
    }

    public class CallError : FaunaException
    {
        public CallError(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.CallError, message, position) { }
    }

    public class StackOverflow : FaunaException
    {
        public StackOverflow(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.StackOverflow, message, position) { }
    }

    public class PermissionDenied2 : FaunaException
    {
        public PermissionDenied2(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.PermissionDenied, message, position) { }
    }

    public class InvalidObjectInContainer : FaunaException
    {
        public InvalidObjectInContainer(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvalidObjectInContainer, message, position) { }
    }

    public class MoveDatabaseError : FaunaException
    {
        public MoveDatabaseError(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.MoveDatabaseError, message, position) { }
    }

    public class RecoveryFailed : FaunaException
    {
        public RecoveryFailed(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.RecoveryFailed, message, position) { }
    }

    public class UnknownCode : FaunaException
    {
        public UnknownCode(int httpStatusCode, string message, string[] position) : base(httpStatusCode, ExceptionCodes.InvaliRef, message, position) { }
    }

}
