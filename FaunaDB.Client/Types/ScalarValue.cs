﻿using FaunaDB.Query;
using FaunaDB.Errors;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using FaunaDB.Collections;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents a scalar value at the FaunaDB query language.
    /// </summary>
    public abstract class ScalarValue<TWrapped> : Value
    {
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
    /// Represents a Boolean value in the FaunaDB query language.
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
    /// Represents a Double value in the FaunaDB query language.
    /// </summary>
    public sealed class DoubleV : ScalarValue<double>
    {
        internal DoubleV(double value) : base(value) {}

        public static DoubleV Of(double v) =>
            new DoubleV(v);
    }

    /// <summary>
    /// Represents a Long value in the FaunaDB query language.
    /// </summary>
    public sealed class LongV : ScalarValue<long>
    {
        internal LongV(long value) : base(value) {}

        public static LongV Of(long v) =>
            new LongV(v);
    }

    /// <summary>
    /// Represents a String value in the FaunaDB query language.
    /// </summary>
    public sealed class StringV : ScalarValue<string>
    {
        internal StringV(string value) : base(value)
        {
            value.AssertNotNull(nameof(value));
        }

        public static StringV Of(string v) =>
            new StringV(v);
    }

    /// <summary>
    /// A FaunaDB set literal.
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>
    /// </summary>
    public sealed class SetRefV : ScalarValue<IReadOnlyDictionary<string, Value>>
    {
        internal SetRefV(IReadOnlyDictionary<string, Value> value) : base(value)
        {
            value.AssertNotNull(nameof(value));
        }

        public SetRefV Of(IReadOnlyDictionary<string, Value> value) =>
            new SetRefV(value);

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
    /// A FaunaDB Expression wrapped as a Value
    /// </summary>
    public class ExprV : ScalarValue<Expr>
    {
        internal ExprV(Expr value) : base(value)
        {
            value.AssertNotNull(nameof(value));
        }

        protected internal override void WriteJson(JsonWriter writer) =>
            Value.WriteJson(writer);

        internal static ExprV Of(Expr e) =>
            new ExprV(e);
        
    }
       
    /// <summary>
    /// Represents a Timestamp value in the FaunaDB query language.
    /// <para>
    /// See the <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public sealed class TimeV : ScalarValue<DateTime>
    {
        /// <summary>
        /// Construct a TimeV from an iso8601 time string.
        /// It must use the 'Z' time zone.
        /// </summary>
        internal TimeV(string iso8601Time) : base(DateTimeUtil.FromIsoTime(iso8601Time, TimeFormat)) { }

        /// <summary>
        /// Construct a TimeV from a <see cref="DateTime"/> object.
        /// </summary>
        internal TimeV(DateTime dateTime) : base(dateTime) { }

        public static TimeV Of(string isoTime) =>
            new TimeV(isoTime);

        public static TimeV Of(DateTime dateTime) =>
            new TimeV(dateTime);

        public DateTimeOffset DateTimeOffset
        {
            get
            {
                return new DateTimeOffset(Value);
            }
        }

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
            $"FaunaTime({Value.ToIso(TimeFormat)})";
        #endregion
    }

    /// <summary>
    /// Represents a Date value in the FaunaDB query language.
    /// <para>
    /// See the <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public sealed class DateV : ScalarValue<DateTime>
    {
        /// <summary>
        /// Construct a DateV from an iso8601 date string.
        /// </summary>
        internal DateV(string iso8601Date) : base(DateTimeUtil.FromIsoDate(iso8601Date, DateFormat)) { }

        /// <summary>
        /// Construct a DateV from a <see cref="DateTime"/> object.
        /// </summary>
        internal DateV(DateTime dateDate) : base(dateDate) { }

        public static DateV Of(string isoDate) =>
            new DateV(isoDate);

        public static DateV Of(DateTime dateTime) =>
            new DateV(dateTime);

        public DateTimeOffset DateTimeOffset
        {
            get
            {
                return new DateTimeOffset(Value);
            }
        }

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
            return DateTime.ParseExact(dateTruncated, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        /// <summary>
        /// Given the response of the server use timestamps with high resolution it can represent
        /// timestamps with resolution of 1ns, like for example: 1970-01-01T00:00:00.000000001Z.
        /// However C# has a resolution of 100ns, so it cannot handle the last two digits of response.
        /// </summary>
        static string TruncateLastTwoDigits(string iso)
        {
            var index = iso.LastIndexOf(".", StringComparison.CurrentCulture);

            if (index >= 0)
            {
                iso = iso.Substring(0, Math.Min(iso.Length, index + 8));

                if (!iso.EndsWith("Z", StringComparison.CurrentCulture))
                    iso += "Z";
            }
            else
            {
                if (iso.EndsWith("Z", StringComparison.CurrentCulture))
                    iso = iso.Substring(0, iso.Length - 1);

                iso += ".0000000Z";
            }

            return iso;
        }

        public static DateTime FromIsoDate(string iso, string format)
        {
            return DateTime.ParseExact(iso, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

    }
}
