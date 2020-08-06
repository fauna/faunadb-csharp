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
    /// Represents an Object value in the FaunaDB query language. Objects are polymorphic dictionaries.
    /// </summary>
    public sealed class ObjectV : Value
    {
        #region Construction
        public static readonly ObjectV Empty =
            new ObjectV(ImmutableDictionary.Empty<string, Value>());

        public IReadOnlyDictionary<string, Value> Value { get; }

        internal ObjectV() : this(Empty.Value) { }

        internal ObjectV(IReadOnlyDictionary<string, Value> value)
        {
            Value = value;

            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add(key, value)</c> for each pair to be in the new ObjectV.
        /// </param>
        internal ObjectV(Action<Action<string, Value>> builder)
        {
            var values = new Dictionary<string, Value>();
            builder((k, v) => values.Add(k, v));
            Value = values;
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
            Value value = null;
            Value.TryGetValue(key, out value);
            return value;
        }

        protected internal override void WriteJson(JsonWriter writer)
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
            return obj != null && Value.DictEquals(obj.Value);
        }

        protected override int HashCode() =>
            HashUtil.Hash(Value.Values);

        public override string ToString()
        {
            var props = Value.Debug();
            return $"{{{props}}}";
        }
        #endregion

        public static ObjectV With() =>
            new ObjectV();

        public static ObjectV With(IReadOnlyDictionary<string, Value> values) =>
            new ObjectV(values);

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

