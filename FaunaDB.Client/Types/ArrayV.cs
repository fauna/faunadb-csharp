using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Utils;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents an array value in the FaunaDB query language. Arrays are polymorphic ordered lists of other values.
    /// </summary>
    public sealed class ArrayV : Value, IEnumerable<Value>
    {
        public static readonly ArrayV Empty = new ArrayV(new List<Value>());

        public IReadOnlyList<Value> Value { get; }

        public static ArrayV Of(params Value[] values) =>
            new ArrayV(values);

        public static ArrayV Of(IEnumerable<Value> values) =>
            new ArrayV(values);

        internal ArrayV(params Value[] values) : this((IEnumerable<Value>)values)
        { }

        internal ArrayV(IEnumerable<Value> values)
        {
            values.AssertNotNull(nameof(values));
            Value = new List<Value>(values);
        }

        internal ArrayV(IReadOnlyList<Value> value)
        {
            value.AssertNotNull(nameof(value));
            Value = value;
        }

        internal ArrayV(Action<Action<Value>> builder)
        {
            var value = new List<Value>();
            builder(value.Add);
            Value = value;
        }

        /// <summary>
        /// Get the nth value.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"/>
        public Value this[int n] { get { return Value[n]; } }

        public int Length { get { return Value.Count; } }

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteArray(Value);
        }

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)Value).GetEnumerator();

        IEnumerator<Value> IEnumerable<Value>.GetEnumerator() =>
            Value.GetEnumerator();
        #endregion

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var a = v as ArrayV;
            return a != null && Value.SequenceEqual(a.Value);
        }

        protected override int HashCode() =>
            HashUtil.Hash(Value);

        public override string ToString() =>
            $"Arr({string.Join(", ", Value)})";
        #endregion
    }
}
