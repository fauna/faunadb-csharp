using FaunaDB.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Errors
{
    /// <summary>
    /// An ErrorData that also stores Failure information.
    /// </summary>
    public class ValidationFailed : ErrorData, IEquatable<ValidationFailed>
    {
        /// <summary>
        /// List of all <see cref="Failure"/> objects returned by the server. 
        /// </summary>
        public List<Failure> Failures { get; }

        public ValidationFailed(string description, ArrayV position, List<Failure> failures)
            : base("validation failed", description, position)
        {
            Failures = failures;
        }

        #region boilerplate
        public override bool Equals(object obj) =>
            Equals(obj as ValidationFailed);

        public override bool Equals(ErrorData e) =>
            Equals(e as ValidationFailed);

        public bool Equals(ValidationFailed v) =>
            v != null &&
                v.Code == Code &&
                v.Description == Description &&
                v.Position == Position &&
                v.Failures.SequenceEqual(Failures);

        public static bool operator ==(ValidationFailed a, ValidationFailed b) =>
            object.Equals(a, b);

        public static bool operator !=(ValidationFailed a, ValidationFailed b) =>
            !object.Equals(a, b);

        public override int GetHashCode() =>
            base.GetHashCode();

        public override string ToString() =>
            $"ValidationFailed({Description}, {Position}, [{string.Join(", ", Failures)}])";
        #endregion
    }
}

