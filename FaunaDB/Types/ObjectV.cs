using FaunaDB.Query;
using FaunaDB.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FaunaDB.Types
{
    /// <summary>
    /// Corresponds to a JSON object.
    /// </summary>
    public sealed class ObjectV : Value
    {
        #region Construction
        public static readonly ObjectV Empty = new ObjectV(new OrderedDictionary<string, Expr>());

        public OrderedDictionary<string, Expr> Value { get; }

        public ObjectV() : this(Empty.Value) { }

        public ObjectV(OrderedDictionary<string, Expr> value)
        {
            Value = value;

            if (Value == null)
                throw new NullReferenceException();
        }

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add(key, value)</c> for each pair to be in the new ObjectV.
        /// </param>
        public ObjectV(Action<Action<string, Expr>> builder)
        {
            var d = new OrderedDictionary<string, Expr>();
            builder((k, v) => d.Add(new KeyValuePair<string, Expr>(k, v)));
            Value = d.ToImmutable();
        }

        #endregion

        /// <exception cref="KeyNotFoundException"/>
        public Expr this[string key] { get { return Value[key]; } }

        /// <summary>
        /// If there is no value at the given key, just return <c>null</c>.
        /// Otherwise, return the value.
        /// </summary>
        public Expr GetOrNull(string key)
        {
            Expr value;
            Value.TryGetValue(key, out value);
            return value;
        }

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject(Value);
        }

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var obj = v as ObjectV;
            return obj != null && Value.Equals(obj.Value);
        }

        protected override int HashCode() =>
            HashUtil.Hash(Value.Values);

        public override string ToString()
        {
            return $"ObjectV({Value.ToString()})";
        }
        #endregion

        public static ObjectV Of() =>
            new ObjectV();

        public static ObjectV Of(string key1, Expr value1) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1));

        public static ObjectV Of(string key1, Expr value1, string key2, Expr value2) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2));

        public static ObjectV Of(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3));

        public static ObjectV Of(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4));

        public static ObjectV Of(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        public static ObjectV Of(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));
    }
}

