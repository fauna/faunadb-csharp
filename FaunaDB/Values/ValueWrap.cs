using Newtonsoft.Json;
using System;

namespace FaunaDB.Values
{
    /// <summary>
    /// Base class for Values that simply wrap something, such as <see cref="BoolV"/>. 
    /// </summary>
    public abstract class ValueWrap<TThis, TWrapped> : Value where TThis : ValueWrap<TThis, TWrapped>
    {
        /// <summary>
        /// Wrapped value.
        /// </summary>
        /// <remarks>
        /// For simple wrappings (bool, double, long, string, array, object),
        /// there should exist an implicit conversion from this to Value
        /// and an explicit (unsafe because a Value might not be a BoolV) conversion from Value to this.
        /// </remarks>
        public TWrapped Val { get; }

        protected ValueWrap(TWrapped value)
        {
            Val = value;
        }

        internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteValue(Val);
        }

        #region boilerplate
        public override bool Equals(Value v)
        {
            var w = v as TThis;
            return w != null && w.Val.Equals(Val);
        }

        protected override int HashCode() =>
            Val.GetHashCode();

        public override string ToString() =>
            $"{GetType().Name}({Val})";
        #endregion
    }

    /// <summary>
    /// Wrapped boolean.
    /// </summary>
    public class BoolV : ValueWrap<BoolV, bool>
    {
        BoolV(bool value) : base(value) {}

        public static BoolV Of(bool b) =>
            b ? True : False;

        public static readonly BoolV True = new BoolV(true);
        public static readonly BoolV False = new BoolV(false);
    }

    /// <summary>
    /// Wrapped double.
    /// </summary>
    public class DoubleV : ValueWrap<DoubleV, double>
    {
        internal DoubleV(double value) : base(value) {}
    }

    /// <summary>
    /// Wrapped long. This is any JSON number with no fractional part.
    /// </summary>
    public class LongV : ValueWrap<LongV, long>
    {
        internal LongV(long value) : base(value) {}
    }

    /// <summary>
    /// Wrapped string.
    /// </summary>
    public class StringV : ValueWrap<StringV, string>
    {
        internal StringV(string value) : base(value)
        {
            if (value == null)
                throw new NullReferenceException();
        }
    }
}
