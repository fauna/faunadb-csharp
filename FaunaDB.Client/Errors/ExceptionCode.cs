using System.ComponentModel;

namespace FaunaDB.Errors
{
    public class ExceptionCodes
    {
        public const string InvalidExpression = "invalid expression";
        public const string InvalidUrlParameter = "invalid url parameter";
        public const string TransactionAborted = "transaction aborted";
        public const string InvalidWriteTime = "invalid write time";
        public const string InvalidArgument = "invalid argument";
        public const string InvalidRef = "invalid ref";
        public const string MissingIdentity = "missing identity";
        public const string InvalidToken = "invalid token";
        public const string CallError = "call error";
        public const string StackOverflow = "stack overflow";
        public const string PermissionDenied = "permission denied";
        public const string AuthenticationFailed = "authentication failed";
        public const string ValueNotFound = "value not found";
        public const string InstanceNotFound = "instance not found";
        public const string InstanceAlreadyExists = "instance already exists";
        public const string ValidationFailed = "validation failed";
        public const string InstanceNotUnique = "instance not unique";
        public const string FeatureNotAvailable = "feature not available";
        public const string UnknownCode = "unknown code";
        public const string Unauthorized = "unauthorized";
    }
}
