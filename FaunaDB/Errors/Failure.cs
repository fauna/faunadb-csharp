using System;
using FaunaDB.Values;

namespace FaunaDB.Errors
{
    /// <summary>
    /// Part of a <see cref="ValidationFailed"/>.
    /// See the <c>Invalid Data</c> section of the <see href="https://faunadb.com/documentation#errors">docs</see>. 
    /// </summary>
    public class Failure : IEquatable<Failure>
    {
        public string Code { get; }
        public string Description { get; }
        /// <summary>
        /// Field of the failure in the instance.
        /// Like <see cref="ErrorData.Position"/>, elements are ObjectV / ArrayV keys: meaning, StringV and LongV values.
        /// </summary>
        public ArrayV Field { get; }

        public static explicit operator Failure(Value v)
        {
            var o = (ObjectV) v;
            return new Failure((string) o["code"], (string) o["description"], (ArrayV) o["field"]);
        }

        public Failure(string code, string description, ArrayV field)
        {
            Code = code;
            Description = description;
            Field = field;
        }

        #region Boilerplate
        public override bool Equals(object obj)
        {
            return Equals(obj as Failure);
        }

        public bool Equals(Failure f)
        {
            return f != null && f.Code == Code && f.Description == Description && f.Field == Field;
        }

        public static bool operator ==(Failure a, Failure b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Failure a, Failure b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return HashUtil.Hash(Field);
        }

        public override string ToString()
        {
            return string.Format("Failure({0}, {1}, {2})", Code, Description, Field);
        }
        #endregion
    }
}

