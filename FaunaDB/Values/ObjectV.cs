using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FaunaDB.Values
{
    /// <summary>
    /// Corresponds to a JSON object.
    /// </summary>
    public sealed class ObjectV : ValueWrap<ObjectV, ImmutableDictionary<string, Value>>
    {
        #region Construction
        public static readonly ObjectV Empty = new ObjectV(ImmutableDictionary<string, Value>.Empty);

        public ObjectV(ImmutableDictionary<string, Value> value) : base(value)
        {
            if (value == null)
                throw new NullReferenceException();
        }

        #region Pairs
        public static KeyValuePair<string, Value> Pair(string key, Value value) =>
            new KeyValuePair<string, Value>(key, value);

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1) =>
            new[] { Pair(k1, v1) };

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1, string k2, Value v2) =>
            new[] { Pair(k1, v1), Pair(k2, v2) };

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1, string k2, Value v2, string k3, Value v3) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3) };

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3), Pair(k4, v4) };

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4, string k5, Value v5) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3), Pair(k4, v4), Pair(k5, v5) };

        public static KeyValuePair<string, Value>[] Pairs(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4, string k5, Value v5, string k6, Value v6) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3), Pair(k4, v4), Pair(k5, v5), Pair(k6, v6) };
        #endregion

        #region Terse constructors

        public ObjectV(string k1, Value v1)
            : base(ImmutableUtil.Create(Pairs(k1, v1))) {}

        public ObjectV(string k1, Value v1, string k2, Value v2)
            : base(ImmutableUtil.Create(Pairs(k1, v1, k2, v2))) {}

        public ObjectV(string k1, Value v1, string k2, Value v2, string k3, Value v3)
            : base(ImmutableUtil.Create(Pairs(k1, v1, k2, v2, k3, v3))) {}

        public ObjectV(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4)
            : base(ImmutableUtil.Create(Pairs(k1, v1, k2, v2, k3, v3, k4, v4))) {}

        public ObjectV(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4, string k5, Value v5)
            : base(ImmutableUtil.Create(Pairs(k1, v1, k2, v2, k3, v3, k4, v4, k5, v5))) {}

        public ObjectV(string k1, Value v1, string k2, Value v2, string k3, Value v3, string k4, Value v4, string k5, Value v5, string k6, Value v6)
            : base(ImmutableUtil.Create(Pairs(k1, v1, k2, v2, k3, v3, k4, v4, k5, v5, k6, v6))) {}
        
        #endregion

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add(key, value)</c> for each pair to be in the new ObjectV.
        /// </param>
        public ObjectV(Action<Action<string, Value>> builder) : this(ImmutableUtil.BuildDict(builder)) {}

        /// <summary>
        /// Blindly uses all entries from <c>notNullPairs</c>, but ignores <c>nullablePairs</c> where values are null.
        /// Use <see cref="Pairs"/> to simplify calling this.
        /// </summary>
        public static ObjectV WithoutNullValues(
            IEnumerable<KeyValuePair<string, Value>> notNullPairs,
            IEnumerable<KeyValuePair<string, Value>> nullablePairs) =>
            new ObjectV(ImmutableUtil.DictWithoutNullValues(notNullPairs, nullablePairs));

        /// <summary>
        /// Create an ObjectV while removing any null values.
        /// Use <see cref="Pairs"/> to simplify calling this. 
        /// </summary>
        public static ObjectV WithoutNullValues(params KeyValuePair<string, Value>[] nullablePairs) =>
            new ObjectV(ImmutableUtil.DictWithoutNullValues(nullablePairs));
        #endregion

        /// <exception cref="KeyNotFoundException"/>
        public Value this[string key] { get { return Val[key]; } }

        /// <summary>
        /// If there is no value at the given key, just return <c>null</c>.
        /// Otherwise, return the value.
        /// </summary>
        public Value GetOrNull(string key)
        {
            Value value;
            Val.TryGetValue(key, out value);
            return value;
        }

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject(Val);
        }

        #region boilerplate
        public override bool Equals(Value v)
        {
            var obj = v as ObjectV;
            return obj != null && ImmutableUtil.DictEquals(Val, obj.Val);
        }

        protected override int HashCode() =>
            HashUtil.Hash(Val.Values);

        public override string ToString()
        {
            var props = string.Join(", ", from kv in Val select $"{kv.Key}: {kv.Value}");
            return $"ObjectV({props})";
        }
        #endregion
    }
}

