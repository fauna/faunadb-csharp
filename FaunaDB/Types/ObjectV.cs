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

        #region Pairs
        public static KeyValuePair<string, Expr> Pair(string key, Expr value) =>
            new KeyValuePair<string, Expr>(key, value);

        public static KeyValuePair<string, Expr>[] Pairs(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3) };

        public static KeyValuePair<string, Expr>[] Pairs(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5, string k6, Expr v6) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3), Pair(k4, v4), Pair(k5, v5), Pair(k6, v6) };

        public static KeyValuePair<string, Expr>[] Pairs(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5, string k6, Expr v6, string k7, Expr v7) =>
            new[] { Pair(k1, v1), Pair(k2, v2), Pair(k3, v3), Pair(k4, v4), Pair(k5, v5), Pair(k6, v6), Pair(k7, v7) };
        #endregion

        #region Terse constructors

        public ObjectV(string k1, Expr v1)
            : this(ImmutableDictionary.Of(k1, v1)) {}

        public ObjectV(string k1, Expr v1, string k2, Expr v2)
            : this(ImmutableDictionary.Of(k1, v1, k2, v2)) {}

        public ObjectV(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3)
            : this(ImmutableDictionary.Of(k1, v1, k2, v2, k3, v3)) {}

        public ObjectV(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4)
            : this(ImmutableDictionary.Of(k1, v1, k2, v2, k3, v3, k4, v4)) {}

        public ObjectV(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5)
            : this(ImmutableDictionary.Of(k1, v1, k2, v2, k3, v3, k4, v4, k5, v5)) {}

        public ObjectV(string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5, string k6, Expr v6)
            : this(ImmutableDictionary.Of(k1, v1, k2, v2, k3, v3, k4, v4, k5, v5, k6, v6)) {}
        
        #endregion

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
    }
}

