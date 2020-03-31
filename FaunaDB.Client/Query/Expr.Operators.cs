using System;
using System.Collections.Generic;
using FaunaDB.Types;

using static FaunaDB.Query.Language;

namespace FaunaDB.Query
{
    public abstract partial class Expr
    {
        #region implicit conversions
        public static implicit operator Expr(bool b) =>
            BooleanV.Of(b);

        public static implicit operator Expr(double d) =>
            DoubleV.Of(d);

        public static implicit operator Expr(long l) =>
            LongV.Of(l);

        public static implicit operator Expr(int i) =>
            LongV.Of(i);

        public static implicit operator Expr(string s) =>
            s == null ? NullV.Instance : StringV.Of(s);

        public static implicit operator Expr(DateTime dt) =>
            Value.FromDateTime(dt);

        public static implicit operator Expr(DateTimeOffset dt) =>
            Value.FromDateTimeOffset(dt);

        public static implicit operator Expr(Dictionary<string, Expr> dict) =>
            Encoder.Encode(dict);

        public static implicit operator Expr(ActionType action)
        {
            switch (action)
            {
                case ActionType.Create:
                    return StringV.Of("create");

                case ActionType.Delete:
                    return StringV.Of("delete");
            }

            throw new ArgumentException("Invalid action value");
        }

        public static implicit operator Expr(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Microsecond:
                    return "microsecond";

                case TimeUnit.Millisecond:
                    return "millisecond";

                case TimeUnit.Nanosecond:
                    return "nanosecond";

                case TimeUnit.Second:
                    return "second";
            }

            throw new ArgumentException("Invalid time unit value");
        }

        public static implicit operator Expr(Normalizer normalizer)
        {
            switch (normalizer)
            {
                case Normalizer.NFD:
                    return "NFD";

                case Normalizer.NFC:
                    return "NFC";

                case Normalizer.NFKD:
                    return "NFKD";

                case Normalizer.NFKC:
                    return "NFKC";

                case Normalizer.NFKCCaseFold:
                    return "NFKCCaseFold";
            }

            throw new ArgumentException("Invalid normalizer value");
        }

        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator bool(Expr v) =>
            ((BooleanV)v).Value;

        public static explicit operator double(Expr v) =>
            ((DoubleV)v).Value;

        public static explicit operator long(Expr v) =>
            ((LongV)v).Value;

        public static explicit operator string(Expr v) =>
            v == NullV.Instance ? null : ((StringV)v).Value;

        public static explicit operator DateTime(Expr v) =>
            ToDateTime(v);

        public static explicit operator DateTimeOffset(Expr v) =>
            ToDateTimeOffset(v);

        internal static DateTime ToDateTime(Expr v)
        {
            var date = v as DateV;
            if (date != null)
                return date.Value;

            var time = v as TimeV;
            if (time != null)
                return time.Value;

            throw new ArgumentException($"Cannot convert {v} to DateTime");
        }

        internal static DateTimeOffset ToDateTimeOffset(Expr v)
        {
            var date = v as DateV;
            if (date != null)
                return date.DateTimeOffset;

            var time = v as TimeV;
            if (time != null)
                return time.DateTimeOffset;

            throw new ArgumentException($"Cannot convert {v} to DateTimeOffset");
        }

        public static explicit operator ActionType(Expr v)
        {
            switch (((StringV)v).Value)
            {
                case "create":
                    return ActionType.Create;

                case "delete":
                    return ActionType.Delete;
            }

            throw new ArgumentException("Invalid string value. Should be \"create\" or \"delete\"");
        }

        public static explicit operator TimeUnit(Expr unit)
        {
            switch (((StringV)unit).Value)
            {
                case "microsecond":
                    return TimeUnit.Microsecond;

                case "millisecond":
                    return TimeUnit.Millisecond;

                case "nanosecond":
                    return TimeUnit.Nanosecond;

                case "second":
                    return TimeUnit.Second;
            }

            throw new ArgumentException("Invalid string value. Should be \"second\", \"millisecond\", \"microsecond\" or \"nanosecond\"");
        }

        public static explicit operator Normalizer(Expr normalizer)
        {
            switch (((StringV)normalizer).Value)
            {
                case "NFD":
                    return Normalizer.NFD;

                case "NFC":
                    return Normalizer.NFC;

                case "NFKD":
                    return Normalizer.NFKD;

                case "NFKC":
                    return Normalizer.NFKC;

                case "NFKCCaseFold":
                    return Normalizer.NFKCCaseFold;
            }

            throw new ArgumentException("Invalid string value. Should be \"NFD\", \"NFC\", \"NFKD\", \"NFKC\", \"NFKCCaseFold\"");
        }
        #endregion
    }
}
