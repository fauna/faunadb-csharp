using FaunaDB.Types;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using FaunaDB.Collections;

namespace FaunaDB.Query
{
    public class UnescapedObject : Expr
    {
        public static readonly UnescapedObject Empty = new UnescapedObject(new OrderedDictionary<string, Expr>());

        private IReadOnlyDictionary<string, Expr> Values;

        public UnescapedObject() : this(new OrderedDictionary<string, Expr>()) { }

        public UnescapedObject(IReadOnlyDictionary<string, Expr> values)
        {
            Values = values;
        }

        public override bool Equals(Expr v)
        {
            var w = v as UnescapedObject;
            return w != null && w.Values.Equals(Values);
        }

        protected override int HashCode() =>
            Values.GetHashCode();

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject(Values);
        }

        public override string ToString()
        {
            var props = string.Join(",", from kv in Values select $"{kv.Key}: {kv.Value}");
            return $"UObject({props})";
        }

        public static UnescapedObject With() =>
            new UnescapedObject();

        public static UnescapedObject With(string key1, Expr value1) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6, string key7, Expr value7) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6, key7, value7));
    }

    public class UnescapedArray : Expr
    {
        public static readonly UnescapedArray Empty = new UnescapedArray(new ArrayList<Expr>());

        public ArrayList<Expr> Value { get; }

        public static UnescapedArray Of(IEnumerable<Expr> values) =>
            new UnescapedArray(new ArrayList<Expr>(values));

        public UnescapedArray(ArrayList<Expr> value)
        {
            Value = value;

            if (Value == null)
                throw new NullReferenceException();
        }

        public override bool Equals(Expr v)
        {
            var other = v as UnescapedArray;
            return other != null && Value.SequenceEqual(other.Value);
        }

        protected override int HashCode() =>
            Value.GetHashCode();

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteArray(Value);
        }

        public override string ToString() =>
            $"UArr({string.Join(", ", Value)})";

    }
}
