using System;

namespace FaunaDB.Errors {
    /// <summary>
    /// Thrown when the response from the server is unusable.
    /// </summary>
    //todo: throw this for failed Value downcasts?
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException(string message) : base(message) {}
    }
}
