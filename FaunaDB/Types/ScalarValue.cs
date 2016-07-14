using FaunaDB.Query;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using FaunaDB.Collections;

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

        protected internal override void WriteJson(JsonWriter writer)
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
    public sealed class BooleanV : ScalarValue<bool>
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
    public sealed class DoubleV : ScalarValue<double>
    {
        internal DoubleV(double value) : base(value) {}

        public static DoubleV Of(double v) =>
            new DoubleV(v);
    }

    /// <summary>
    /// Wrapped long. This is any JSON number with no fractional part.
    /// </summary>
    public sealed class LongV : ScalarValue<long>
    {
        internal LongV(long value) : base(value) {}

        public static LongV Of(long v) =>
            new LongV(v);
    }

    /// <summary>
    /// Wrapped string.
    /// </summary>
    public sealed class StringV : ScalarValue<string>
    {
        internal StringV(string value) : base(value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }

        public static StringV Of(string v) =>
            new StringV(v);
    }

    /// <summary>
    /// FaunaDB ref. See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class RefV : ScalarValue<string>
    {
        /// <summary>
        /// Create a Ref from a string, such as <c>new Ref("databases/prydain")</c>.
        /// </summary>
        /// <param name="value">Value.</param>
        public RefV(string value) : base(value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ref", Value);
        }

        public static implicit operator string(RefV r) =>
            r.Value;
    }

    /// <summary>
    /// FaunaDB Set.
    /// </summary>
    /// <remarks>
    /// This represents a set returned as part of a response. This looks like <c>{"@set": set_query}</c>.
    /// For query sets see <see cref="Language"/>.
    /// </remarks>
    public sealed class SetRefV : ScalarValue<IReadOnlyDictionary<string, Value>>
    {
        public SetRefV(IReadOnlyDictionary<string, Value> name) : base(name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("@set");
            writer.WriteObject(Value);
            writer.WriteEndObject();
        }

        public override bool Equals(Expr v)
        {
            var other = v as SetRefV;
            return other != null && Value.DictEquals(other.Value);
        }

        public override int GetHashCode() =>
            Value.GetHashCode();
    }

    /// <summary>
    /// FaunaDB timestamp.
    /// See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class TimeV : ScalarValue<DateTime>
    {
        /// <summary>
        /// Construct from an iso8601 time string.
        /// It must use the 'Z' time zone.
        /// </summary>
        public TimeV(string iso8601Time) : base(DateTimeUtil.FromIsoTime(iso8601Time, TimeFormat)) { }

        public TimeV(DateTime dateTime) : base(dateTime) { }

        /// <summary>
        /// Convert from a DateTime by rendering as iso8601.
        /// </summary>
        public static explicit operator TimeV(DateTime value) =>
            new TimeV(value);

        /// <summary>
        /// Convert to DateTime.
        /// Since DateTime has millisecond precision, this is lossy.
        /// </summary>
        public static implicit operator DateTime(TimeV ft) =>
            ft.Value;

        const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFZ";

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ts", Value.ToIso(TimeFormat));
        }

        #region boilerplate
        public override bool Equals(Expr v)
        {
            var t = v as TimeV;
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

        protected internal override void WriteJson(JsonWriter writer)
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

        public static DateTime FromIsoTime(string dateString, string format)
        {
            var dateTruncated = TruncateLastTwoDigits(dateString);
            var dateParsed = DateTime.ParseExact(dateTruncated, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return dateParsed.ToUniversalTime();
        }

        /// <summary>
        /// Given the response of the server use timestamps with high resolution it can represent
        /// timestamps with resolution of 1ns, like for example: 1970-01-01T00:00:00.000000001Z.
        /// However C# has a resolution of 100ns, so it cannot handle the last two digits of response.
        /// </summary>
        static string TruncateLastTwoDigits(string iso)
        {
            var index = iso.LastIndexOf(".");

            if (index >= 0)
            {
                iso = iso.Substring(0, Math.Min(iso.Length, index + 8));

                if (!iso.EndsWith("Z"))
                    iso += "Z";
            }
            else
            {
                if (iso.EndsWith("Z"))
                    iso = iso.Substring(0, iso.Length - 1);

                iso += ".0000000Z";
            }

            return iso;
        }

        public static DateTime FromIsoDate(string iso, string format)
        {
            var dt = DateTime.ParseExact(iso, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return dt.ToUniversalTime();
        }

    }
}
