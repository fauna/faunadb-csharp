using Newtonsoft.Json;
using System;

namespace FaunaDB.Query
{
    [JsonConverter(typeof(ExprJsonConverter))]
    public abstract partial class Expr : IEquatable<Expr>
    {
        internal abstract void WriteJson(JsonWriter writer);

        /// <summary>
        /// Convert to a JSON string.
        /// </summary>
        /// <param name="pretty">If true, output with helpful whitespace.</param>
        public string ToJson(bool pretty = false) =>
            JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None);

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

    internal class ExprJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            typeof(Expr).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            ((Expr)value).WriteJson(writer);
    }
}
