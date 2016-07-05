using FaunaDB.Collections;
using FaunaDB.Query;
using FaunaDB.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Types
{
    /// <summary>
    /// Corresponds to a JSON object.
    /// </summary>
    public sealed class ObjectV : Value
    {
        #region Construction
        public static readonly ObjectV Empty = new ObjectV(new OrderedDictionary<string, Value>());

        public IReadOnlyDictionary<string, Value> Value { get; }

        public ObjectV() : this(Empty.Value) { }

        public ObjectV(IReadOnlyDictionary<string, Value> value)
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
        public ObjectV(Action<Action<string, Value>> builder)
        {
            var dic = new OrderedDictionary<string, Value>();
            builder((k, v) => dic.Add(new KeyValuePair<string, Value>(k, v)));
            Value = dic;
        }

        #endregion

        /// <exception cref="KeyNotFoundException"/>
        public Value this[string key] { get { return Value[key]; } }

        /// <summary>
        /// If there is no value at the given key, just return <c>null</c>.
        /// Otherwise, return the value.
        /// </summary>
        public Value GetOrNull(string key)
        {
            Value value;
            Value.TryGetValue(key, out value);
            return value;
        }

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("object");
            writer.WriteObject(Value);
            writer.WriteEndObject();
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
            var props = string.Join(",", from kv in Value select $"{kv.Key}: {kv.Value}");
            return $"ObjectV({props})";
        }
        #endregion

        public static ObjectV With() =>
            new ObjectV();

        public static ObjectV With(string key1, Value value1) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1));

        public static ObjectV With(string key1, Value value1, string key2, Value value2) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2));

        public static ObjectV With(string key1, Value value1, string key2, Value value2, string key3, Value value3) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3));

        public static ObjectV With(string key1, Value value1, string key2, Value value2, string key3, Value value3, string key4, Value value4) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4));

        public static ObjectV With(string key1, Value value1, string key2, Value value2, string key3, Value value3, string key4, Value value4, string key5, Value value5) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        public static ObjectV With(string key1, Value value1, string key2, Value value2, string key3, Value value3, string key4, Value value4, string key5, Value value5, string key6, Value value6) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));

        public static ObjectV With(string key1, Value value1, string key2, Value value2, string key3, Value value3, string key4, Value value4, string key5, Value value5, string key6, Value value6, string key7, Value value7) =>
            new ObjectV(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6, key7, value7));
    }

}

