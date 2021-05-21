using FaunaDB.Types;
using FaunaDB.Errors;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using FaunaDB.Collections;

namespace FaunaDB.Query
{
    class UnescapedObject : Expr
    {
        public static readonly UnescapedObject Empty =
            new UnescapedObject(ImmutableDictionary.Empty<string, Expr>());

        IReadOnlyDictionary<string, Expr> Values { get; }

        public UnescapedObject(IReadOnlyDictionary<string, Expr> values)
        {
            values.AssertNotNull(nameof(values));
            Values = values;
        }

        public override bool Equals(Expr v)
        {
            var w = v as UnescapedObject;
            return w != null && w.Values.DictEquals(Values);
        }

        protected override int HashCode() =>
            Values.GetHashCode();

        protected internal override void WriteJson(JsonWriter writer) =>
            writer.WriteObject(Values);

        public override string ToString()
        {
            var props = string.Join(",", from kv in Values select $"{kv.Key}: {kv.Value}");
            return $"UObject({props})";
        }

        public static UnescapedObject With(Dictionary<string, Expr> exprs) =>
            new UnescapedObject(ImmutableDictionary.Of(exprs));

        public static UnescapedObject With(string key1, Expr value1) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2, key3, value3));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2, key3, value3, key4, value4));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2, key3, value3, key4, value4, key5, value5));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));

        public static UnescapedObject With(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6, string key7, Expr value7) =>
            new UnescapedObject(ImmutableDictionary.Of(key1, value1 ?? NullV.Instance, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6, key7, value7));
    }

    class UnescapedArray : Expr
    {
        public static readonly UnescapedArray Empty =
            new UnescapedArray(new List<Expr>());

        IReadOnlyList<Expr> Value { get; }

        public UnescapedArray(IReadOnlyList<Expr> value)
        {
            value.AssertNotNull(nameof(value));
            Value = value;
        }

        public UnescapedArray(params Expr[] value)
        {
            value.AssertNotNull(nameof(value));
            Value = value;
        }

        public override bool Equals(Expr v)
        {
            var other = v as UnescapedArray;
            return other != null && Value.SequenceEqual(other.Value);
        }

        protected override int HashCode() =>
            Value.GetHashCode();

        protected internal override void WriteJson(JsonWriter writer) =>
            writer.WriteArray(Value);

        public override string ToString() =>
            $"UArr({string.Join(", ", Value)})";
    }
}
