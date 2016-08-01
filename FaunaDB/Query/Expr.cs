using Newtonsoft.Json;
using System;

namespace FaunaDB.Query
{
    /// <summary>
    /// A query language expression. Constructors for this class are at the <see cref="Language"/> class.
    /// </summary>
    [JsonConverter(typeof(ExprJsonConverter))]
    public abstract partial class Expr : IEquatable<Expr>
    {
        protected internal abstract void WriteJson(JsonWriter writer);

        #region boilerplate
        public override bool Equals(object obj)
        {
            var v = obj as Expr;
            return v != null && Equals(v);
        }

        public abstract bool Equals(Expr v);

        public static bool operator ==(Expr a, Expr b) =>
            Equals(a, b);

        public static bool operator !=(Expr a, Expr b) =>
            !Equals(a, b);

        public override int GetHashCode() =>
            HashCode();

        // Force subclasses to implement hash code.
        protected abstract int HashCode();
        #endregion
    }

    class ExprJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            typeof(Expr).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException($"You are not allowed to deserialize objects of type {objectType.Name}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            ((Expr)value).WriteJson(writer);
    }
}
