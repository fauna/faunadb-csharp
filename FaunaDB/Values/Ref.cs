using Newtonsoft.Json;
using System;
using System.Linq;

using FaunaDB.Errors;

namespace FaunaDB.Values
{
    /// <summary>
    /// FaunaDB ref. See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class Ref : ValueWrap<Ref, string>
    {
        /// <summary>
        /// Create a Ref from a string, such as <c>new Ref("databases/prydain")</c>.
        /// </summary>
        /// <param name="value">Value.</param>
        public Ref(string value) : base(value) {}

        /// <summary>
        /// Create a Ref from parts.
        /// </summary>
        /// <example>
        /// <c>new Ref("databases", "prydain")</c>
        /// </example>
        /// <example>
        /// <c>new Ref(new Ref("databases"), "prydain")</c>.
        /// </example>
        public Ref(params string[] parts) : base(string.Join("/", parts)) {}

        /// <summary>
        /// Gets the class part ouf of the Ref.
        /// </summary>
        /// <remarks>
        /// This is done by removing the ID.
        /// So, <c>new Ref("a", "b/c").Class</c> will be <c>new Ref("a/b")</c>.
        /// </remarks>
        public Ref Class
        {
            get
            {
                var parts = Val.Split('/');
                var count = parts.Count();
                return count == 1 ? this : new Ref(parts.Take(count - 1).ToArray());
            }
        }

        /// <summary>
        /// Removes the class part of the Ref, leaving only the ID.
        /// </summary>
        /// <remarks>
        /// This is everything after the last <c>/</c>.
        /// </remarks>
        /// <exception cref="InvalidValue">Thrown if the Ref does not have an ID portion, as in <c>new Ref("classes")</c>.</exception>
        public string Id
        {
            get
            {
                var parts = Val.Split('/');
                if (parts.Count() == 1)
                    throw new InvalidValueException("The Ref does not have an ID.");
                return parts.Last();
            }
        }

        public static implicit operator string(Ref r) =>
            r.Val;

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ref", Val);
        }
    }
}
