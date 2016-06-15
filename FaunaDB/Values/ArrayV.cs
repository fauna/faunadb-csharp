using FaunaDB.Query;
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
    public sealed class ArrayV : Value, IEnumerable<Expr>
    {
        #region Construction
        public static readonly ArrayV Empty = new ArrayV(ImmutableArray<Expr>.Empty);

        public ImmutableArray<Expr> Value { get;  }

        public static ArrayV FromEnumerable(IEnumerable<Expr> values) =>
            new ArrayV(values.ToImmutableArray());

        public ArrayV(ImmutableArray<Expr> value)
        {
            Value = value;

            if (Value == null)
                throw new NullReferenceException();
        }

        /// <summary>
        /// Create from values.
        /// </summary>
        public ArrayV(params Expr[] values) : this(ImmutableArray.Create(values)) {}

        /// <summary>
        /// Create from a builder expression.
        /// </summary>
        /// <param name="builder">
        /// A lambda <c>(add) => { ... }</c> that calls <c>add</c> for each element to be in the new ArrayV.
        /// </param>
        public ArrayV(Action<Action<Expr>> builder) : this(ImmutableUtil.BuildArray(builder)) {}
        #endregion

        /// <summary>
        /// Get the nth value.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"/>
        public Expr this[int n] { get { return Value[n]; } }

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteArray(Value);
        }

        #region IEnumerable
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            ((System.Collections.IEnumerable) Value).GetEnumerator();

        public IEnumerator<Expr> GetEnumerator() =>
            ((IEnumerable<Expr>) Value).GetEnumerator();
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
            $"ArrayV({string.Join(", ", this)})";
        #endregion
    }
}
