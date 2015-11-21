using Newtonsoft.Json;
using System;
using System.Globalization;

using FaunaDB.Errors;

namespace FaunaDB.Values
{
    /// <summary>
    /// FaunaDB timestamp.
    /// See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class FaunaTime : Value
    {
        public string Iso8601Time { get; }

        /// <summary>
        /// Construct from an iso8601 time string.
        /// It must use the 'Z' time zone.
        /// </summary>
        public FaunaTime(string iso8601Time)
        {
            if (!iso8601Time.EndsWith("Z"))
                throw new InvalidValueException(string.Format("Only allowed timezone is 'Z', got: {0}", iso8601Time));
            Iso8601Time = iso8601Time;
        }

        /// <summary>
        /// Convert from a DateTime by rendering as iso8601.
        /// </summary>
        public static explicit operator FaunaTime(DateTime value)
        {
            return new FaunaTime(value.ToIso(TimeFormat));
        }

        /// <summary>
        /// Convert to DateTime.
        /// Since DateTime has millisecond precision, this is lossy.
        /// </summary>
        public static implicit operator DateTime(FaunaTime ft)
        {
            return DateTimeUtil.FromIso(ft.Iso8601Time, TimeFormat);
        }

        const string TimeFormat = "o";

        internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@ts", Iso8601Time);
        }

        #region boilerplate
        public override bool Equals(Value v)
        {
            var t = v as FaunaTime;
            return t != null && t.Iso8601Time == Iso8601Time;
        }

        protected override int HashCode()
        {
            return Iso8601Time.GetHashCode();
        }

        override public string ToString()
        {
            return string.Format("FaunaDate({0})", Iso8601Time);
        }
        #endregion
    }

    /// <summary>
    /// FaunaDB date.
    /// See the <see href="https://faunadb.com/documentation/queries#values-special_types">docs</see>. 
    /// </summary>
    public sealed class FaunaDate : Value
    {
        public string Iso8601Date { get; }

        /// <summary>
        /// Construct from an iso8601 date string.
        /// </summary>
        public FaunaDate(string iso8601Date)
        {
            Iso8601Date = iso8601Date;
        }

        /// <summary>
        /// Convert from a DateTime by rendering as iso8601.
        /// This throws out time-of-day data.
        /// </summary>
        public static explicit operator FaunaDate(DateTime value)
        {
            return new FaunaDate(value.ToIso(DateFormat));
        }

        public static implicit operator DateTime(FaunaDate ft)
        {
            return DateTimeUtil.FromIso(ft.Iso8601Date, DateFormat);
        }

        const string DateFormat = "yyyy-MM-dd";

        internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@date", Iso8601Date);
        }

        #region boilerplate
        public override bool Equals(Value v)
        {
            var d = v as FaunaDate;
            return d != null && d.Iso8601Date == Iso8601Date;
        }

        protected override int HashCode()
        {
            return Iso8601Date.GetHashCode();
        }

        override public string ToString()
        {
            return string.Format("FaunaDate({0})", Iso8601Date);
        }
        #endregion
    }

    static class DateTimeUtil
    {
        public static string ToIso(this DateTime dt, string format)
        {
            return dt.ToString(format, CultureInfo.InvariantCulture);
        }

        public static DateTime FromIso(string iso, string format)
        {
            var dt = DateTime.ParseExact(iso, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            //todo: On Mono, dt will not be in UTC even though we specified AssumeUniversal. Test again on windows.
            return dt.ToUniversalTime();
        }
    }
}
