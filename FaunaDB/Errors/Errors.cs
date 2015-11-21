using System;

namespace FaunaDB.Errors {
    /// <summary>
    /// Thrown when a query is malformed.
    /// </summary>
    public class InvalidQueryException : Exception
    {
        public InvalidQueryException(string message) : base(message) {}
    }

    // todo: replace with something else?
    /// <summary>
    /// Thrown when a value can not be accepted.
    /// </summary>
    public class InvalidValueException : Exception
    {
        public InvalidValueException(string message) : base(message) {}
    }

    /// <summary>
    /// Thrown when the response from the server is unusable.
    /// </summary>
    //todo: throw this for failed Value downcasts?
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException(string message) : base(message) {}
    }
}
