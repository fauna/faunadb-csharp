using System;
using System.Collections.Immutable;
using System.Linq;

using FaunaDB.Values;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Data for one error returned by the server.
    /// </summary>
    public class ErrorData : IEquatable<ErrorData>
    {
        /// <summary>
        /// Get an ErrorData from a conforming Value.
        /// Only call this if you know that the Value represents error data.
        /// </summary>
        public static explicit operator ErrorData(Value v)
        {
            var o = (ObjectV) v;

            var code = (string) o["code"];
            var description = (string) o["description"];

            ArrayV position = null;
            Value pos;
            if (o.Val.TryGetValue("position", out pos))
                position = (ArrayV) pos;

            return code == "validation failed" ?
                new ValidationFailed(
                    description, position,
                    (from _ in (ArrayV) o["failures"] select (Failure) _).ToImmutableArray()) :
                new ErrorData(code, description, position);
        }

        /// <summary>
        /// Error code. See the many error codes <see href="https://faunadb.com/documentation#errors">here</see>. 
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Error description.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Position of the error in the query.
        /// Elements are ObjectV / ArrayV keys: meaning, StringV and LongV values.
        /// May be null.
        /// </summary>
        public ArrayV Position { get; }

        public ErrorData(string code, string description, ArrayV position)
        {
            Code = code;
            Description = description;
            Position = position;
        }

        #region Boilerplate
        public override bool Equals(object obj)
        {
            return Equals(obj as ErrorData);
        }

        public virtual bool Equals(ErrorData e)
        {
            return e != null && e.Code == Code && e.Description == Description && e.Position == Position;
        }

        public static bool operator ==(ErrorData a, ErrorData b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(ErrorData a, ErrorData b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return HashUtil.Hash(Position);
        }

        public override string ToString()
        {
            return string.Format("ErrorData({0}, {1}, {2})", Code, Description, Position == null ? "null" : Position.ToString());
        }
        #endregion
    }
}

