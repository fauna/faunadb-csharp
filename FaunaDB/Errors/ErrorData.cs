using FaunaDB.Query;
using FaunaDB.Types;
using FaunaDB.Utils;
using System;
using System.Linq;

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
            Expr pos;
            if (o.Value.TryGetValue("position", out pos))
                position = (ArrayV) pos;

            return code == "validation failed" ?
                new ValidationFailed(
                    description, position,
                    (from _ in (ArrayV) o["failures"] select (Failure) _).ToList()) :
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

        #region boilerplate
        public override bool Equals(object obj) =>
            Equals(obj as ErrorData);

        public virtual bool Equals(ErrorData e) =>
            e != null && e.Code == Code && e.Description == Description && e.Position == Position;

        public static bool operator ==(ErrorData a, ErrorData b) =>
            object.Equals(a, b);

        public static bool operator !=(ErrorData a, ErrorData b) =>
            !object.Equals(a, b);

        public override int GetHashCode() =>
            HashUtil.Hash(Position);

        public override string ToString() =>
            $"ErrorData({Code}, {Description}, {(Position == null ? "null" : Position.ToString())})";
        #endregion
    }
}

