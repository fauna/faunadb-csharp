using FaunaDB.Collections;
using FaunaDB.Query;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace FaunaDB.Types
{
    /// <summary>
    /// Base class for Values that simply wrap something, such as <see cref="BooleanV"/>. 
    /// </summary>
    public abstract class ScalarValue<TWrapped> : Value
    {
        /// <summary>
        /// Wrapped value.
        /// </summary>
        /// <remarks>
        /// For simple wrappings (bool, double, long, string, array, object),
        /// there should exist an implicit conversion from this to Value
        /// and an explicit (unsafe because a Value might not be a BoolV) conversion from Value to this.
        /// </remarks>
        public TWrapped Value { get; }

        protected ScalarValue(TWrapped value)
        {
            Value = value;
        }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteValue(Value);
        }

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var w = v as ScalarValue<TWrapped>;
            return w != null && w.Value.Equals(Value);
        }

        protected override int HashCode() =>
            Value.GetHashCode();

        public override string ToString() =>
            $"{GetType().Name}({Value})";
        #endregion
    }

    /// <summary>
    /// Wrapped boolean.
    /// </summary>
    public class BooleanV : ScalarValue<bool>
    {
        internal BooleanV(bool value) : base(value) {}

        public static BooleanV Of(bool b) =>
            b ? True : False;

        public static readonly BooleanV True = new BooleanV(true);
        public static readonly BooleanV False = new BooleanV(false);
    }

    /// <summary>
    /// Wrapped double.
    /// </summary>
    public class DoubleV : ScalarValue<double>
    {
        internal DoubleV(double value) : base(value) {}

        public static DoubleV Of(double v) =>
            new DoubleV(v);
    }

    /// <summary>
    /// Wrapped long. This is any JSON number with no fractional part.
    /// </summary>
    public class LongV : ScalarValue<long>
    {
        internal LongV(long value) : base(value) {}

        public static LongV Of(long v) =>
            new LongV(v);
    }

    /// <summary>
    /// Wrapped string.
    /// </summary>
    public class StringV : ScalarValue<string>
    {
        internal StringV(string value) : base(value)
        {
            if (value == null)
                throw new NullReferenceException();
        }

        public static StringV Of(string v) =>
            new StringV(v);
    }

    /// <summary>
    /// FaunaDB ref. See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class Ref : ScalarValue<string>
    {
        /// <summary>
        /// Create a Ref from a string, such as <c>new Ref("databases/prydain")</c>.
        /// </summary>
        /// <param name="value">Value.</param>
        public Ref(string value) : base(value) { }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ref", Value);
        }

        public static implicit operator string(Ref r) =>
            r.Value;
    }

    /// <summary>
    /// FaunaDB Set.
    /// </summary>
    /// <remarks>
    /// This represents a set returned as part of a response. This looks like <c>{"@set": set_query}</c>.
    /// For query sets see <see cref="Language"/>.
    /// </remarks>
    public sealed class SetRef : ScalarValue<IReadOnlyDictionary<string, Value>>
    {
        public SetRef(IReadOnlyDictionary<string, Value> q) : base(q) { }

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("@set");
            writer.WriteObject(Value);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// FaunaDB timestamp.
    /// See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class TsV : ScalarValue<DateTime>
    {
        /// <summary>
        /// Construct from an iso8601 time string.
        /// It must use the 'Z' time zone.
        /// </summary>
        public TsV(string iso8601Time) : base(DateTimeUtil.FromIsoTime(iso8601Time, TimeFormat)) { }

        public TsV(DateTime dateTime) : base(dateTime) { }

        /// <summary>
        /// Convert from a DateTime by rendering as iso8601.
        /// </summary>
        public static explicit operator TsV(DateTime value) =>
            new TsV(value);

        /// <summary>
        /// Convert to DateTime.
        /// Since DateTime has millisecond precision, this is lossy.
        /// </summary>
        public static implicit operator DateTime(TsV ft) =>
            ft.Value;

        const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFZ";

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ts", Value.ToIso(TimeFormat));
        }

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var t = v as TsV;
            return t != null && t.Value == Value;
        }

        protected override int HashCode() =>
            Value.GetHashCode();

        override public string ToString() =>
            $"FaunaDate({Value})";
        #endregion
    }

    /// <summary>
    /// FaunaDB date.
    /// See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class DateV : ScalarValue<DateTime>
    {
        /// <summary>
        /// Construct from an iso8601 date string.
        /// </summary>
        public DateV(string iso8601Date) : base(DateTimeUtil.FromIsoDate(iso8601Date, DateFormat)) { }

        public DateV(DateTime dateDate) : base(dateDate) { }

        /// <summary>
        /// Convert from a DateTime by rendering as iso8601.
        /// This throws out time-of-day data.
        /// </summary>
        public static explicit operator DateV(DateTime value) =>
            new DateV(value);

        public static implicit operator DateTime(DateV ft) =>
            ft.Value;

        const string DateFormat = "yyyy-MM-dd";

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@date", Value.ToIso(DateFormat));
        }

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var d = v as DateV;
            return d != null && d.Value == Value;
        }

        protected override int HashCode() =>
            Value.GetHashCode();

        override public string ToString() =>
            $"FaunaDate({Value})";
        #endregion
    }

    static class DateTimeUtil
    {
        public static string ToIso(this DateTime dt, string format) =>
            dt.ToString(format, CultureInfo.InvariantCulture);

        public static DateTime FromIsoTime(string iso, string format)
        {
            //TODO Remove this workaround

            var index = iso.LastIndexOf(".");

            if (index >= 0)
            {
                iso = iso.Substring(0, Math.Min(iso.Length, index + 8));

                if (!iso.EndsWith("Z"))
                    iso = iso + "Z";
            }
            else
            {
                if (iso.EndsWith("Z"))
                    iso = iso.Substring(0, iso.Length - 1);

                iso += ".0000000Z";
            }

            var dt = DateTime.ParseExact(iso, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            //todo: On Mono, dt will not be in UTC even though we specified AssumeUniversal. Test again on windows.
            return dt.ToUniversalTime();
        }

        public static DateTime FromIsoDate(string iso, string format)
        {
            var dt = DateTime.ParseExact(iso, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            //todo: On Mono, dt will not be in UTC even though we specified AssumeUniversal. Test again on windows.
            return dt.ToUniversalTime();
        }

    }
}
