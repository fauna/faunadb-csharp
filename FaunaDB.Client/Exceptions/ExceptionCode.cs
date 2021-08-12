using System.ComponentModel;

namespace FaunaDB.Client.Exceptions
{
    public static class ExceptionCodes
    {
        public const string InvalidExpression = "invalid expression";
        public const string InvalidUrlParameter = "invalid url parameter";
        public const string SchemaNotFound = "schema not found";
        public const string TransactionAborted = "transaction aborted";
        public const string InvalidWriteTime = "invalid write time";
        public const string InvalidArgument = "invalid argument";//+
        public const string InvaliRef = "invalid ref"; //+
        public const string MissingIdentity = "missing identity";
        public const string InvalidScope = "invalid scope";
        public const string InvalidToken = "invalid token";
        public const string CallError = "call error";
        public const string StackOverflow = "stack overflow";
        public const string PermissionDenied = "permission denied";
        public const string AuthenticationFailed = "authentication failed";//+
        public const string ValueNotFound = "value not found";//+
        public const string InstanceNotFound = "instance not found";//+
        public const string InstanceAlreadyExists = "instance already exists"; //+
        public const string ValidationFailed = "validation failed"; //+
        public const string InstanceNotUnique = "instance not unique";//+
        public const string InvalidObjectInContainer = "invalid object in container";
        public const string MoveDatabaseError = "move database error";
        public const string RecoveryFailed = "recovery failed";
        public const string FeatureNotAvailable = "feature not available";
        public const string UnknownCode = "unknown code";
    }
}
