using FaunaDB.Collections;
using FaunaDB.Query;
using FaunaDB.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FaunaDB.Types
{
    /// <summary>
    /// Corresponds to a JSON array.
    /// </summary>
    public sealed class ArrayV : Value, IEnumerable<Value>
    {
        public static readonly ArrayV Empty = new ArrayV(ArrayList<Value>.Empty);

        public IReadOnlyList<Value> Value { get; }

        public static ArrayV Of(params Value[] values) =>
            new ArrayV(values);

        internal ArrayV(params Value[] value)
        {
            Value = new ArrayList<Value>(value);

            if (Value == null)
                throw new NullReferenceException();
        }

        internal ArrayV(IReadOnlyList<Value> value)
        {
            Value = value;

            if (Value == null)
                throw new NullReferenceException();
        }

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add</c> for each element to be in the new ArrayV.
        /// </param>
        public ArrayV(Action<Action<Value>> builder)
        {
            var value = new ArrayList<Value>();
            builder(value.Add);
            Value = value;
        }

        /// <summary>
        /// Get the nth value.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"/>
        public Value this[int n] { get { return Value[n]; } }

        public int Length { get { return Value.Count; } }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteArray(Value);
        }

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable) Value).GetEnumerator();

        IEnumerator<Value> IEnumerable<Value>.GetEnumerator() =>
            ((IEnumerable<Value>) Value).GetEnumerator();
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
            $"Arr({string.Join(", ", Value.GetEnumerator())})";
        #endregion
    }
}
