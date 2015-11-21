using System;
using System.Collections.Immutable;
using System.Linq;

using FaunaDB.Values;

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
        public ImmutableArray<Failure> Failures { get; }

        public ValidationFailed(string description, ArrayV position, ImmutableArray<Failure> failures)
            : base("validation failed", description, position)
        {
            Failures = failures;
        }

        #region boilerplate
        public override bool Equals(object obj)
        {
            return Equals(obj as ValidationFailed);
        }

        public override bool Equals(ErrorData e)
        {
            return Equals(e as ValidationFailed);
        }

        public bool Equals(ValidationFailed v)
        {
            return v != null &&
                v.Code == Code &&
                v.Description == Description &&
                v.Position == Position &&
                v.Failures.SequenceEqual(Failures);
        }

        public static bool operator ==(ValidationFailed a, ValidationFailed b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(ValidationFailed a, ValidationFailed b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("ValidationFailed({0}, {1}, [{2}])", Description, Position, string.Join(", ", Failures));
        }
        #endregion
    }
}

