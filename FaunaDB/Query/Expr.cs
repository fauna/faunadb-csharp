using FaunaDB.Errors;
using FaunaDB.Types;
using Newtonsoft.Json;
using System;

namespace FaunaDB.Query
{
    [JsonConverter(typeof(ValueJsonConverter))]
    public abstract partial class Expr : IEquatable<Expr>
    {
        internal abstract void WriteJson(JsonWriter writer);

        /// <summary>
        /// Convert to a JSON string.
        /// </summary>
        /// <param name="pretty">If true, output with helpful whitespace.</param>
        public string ToJson(bool pretty = false) =>
            JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None);

        /// <summary>
        /// Read a Value from JSON.
        /// </summary>
        /// <exception cref="Errors.InvalidResponseException"/>
        //todo: Should we convert invalid Value downcasts and missing field exceptions to InvalidResponseException?
        public static Expr FromJson(string json)
        {
            // We handle dates ourselves. Don't want them automatically parsed.
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            try
            {
                return JsonConvert.DeserializeObject<Expr>(json, settings);
            }
            catch (JsonReaderException j)
            {
                throw new InvalidResponseException($"Bad JSON: {j}");
            }
        }


        #region boilerplate
        public override bool Equals(object obj)
        {
            var v = obj as Expr;
            return v != null && Equals(v);
        }

        public abstract bool Equals(Expr v);

        public static bool operator ==(Expr a, Expr b) =>
            object.Equals(a, b);

        public static bool operator !=(Expr a, Expr b) =>
            !object.Equals(a, b);

        public override int GetHashCode() =>
            HashCode();

        // Force subclasses to implement hash code.
        protected abstract int HashCode();
        #endregion

    }
}
