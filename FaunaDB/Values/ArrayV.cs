using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FaunaDB.Values
{
    /// <summary>
    /// Corresponds to a JSON array.
    /// </summary>
    public sealed class ArrayV : ValueWrap<ArrayV, ImmutableArray<Value>>, IEnumerable<Value>
    {
        #region Construction
        public static readonly ArrayV Empty = new ArrayV(ImmutableArray<Value>.Empty);

        public ArrayV(ImmutableArray<Value> value) : base(value)
        {
            if (value == null)
                throw new NullReferenceException();
        }

        /// <summary>
        /// Create from values.
        /// </summary>
        public ArrayV(params Value[] values) : this(ImmutableArray.Create(values)) {}

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add</c> for each element to be in the new ArrayV.
        /// </param>
        public ArrayV(Action<Action<Value>> builder) : this(ImmutableUtil.BuildArray(builder)) {}
        #endregion

        /// <summary>
        /// Get the nth value.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"/>
        public Value this[int n] { get { return Val[n]; } }

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteArray(Val);
        }

        #region IEnumerable
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable) Val).GetEnumerator();
        }

        public IEnumerator<Value> GetEnumerator()
        {
            return ((IEnumerable<Value>) Val).GetEnumerator();
        }
        #endregion

        #region boilerplate
        public override bool Equals(Value v)
        {
            var a = v as ArrayV;
            return a != null && Val.SequenceEqual(a.Val);
        }

        protected override int HashCode()
        {
            return HashUtil.Hash(Val);
        }

        public override string ToString()
        {
            return string.Format("ArrayV({0})", string.Join(", ", this));
        }
        #endregion
    }
}
