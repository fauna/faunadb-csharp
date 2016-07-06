using System;
using FaunaDB.Errors;
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
        #endregion

        #region Operators
        public static Expr operator !(Expr a) =>
            Language.Not(a);

        public static Expr operator +(Expr a, Expr b) =>
            Language.Add(a, b);

        public static Expr operator -(Expr a, Expr b) =>
            Language.Subtract(a, b);

        public static Expr operator *(Expr a, Expr b) =>
            Language.Multiply(a, b);

        public static Expr operator /(Expr a, Expr b) =>
            Language.Divide(a, b);

        public static Expr operator %(Expr a, Expr b) =>
            Language.Modulo(a, b);

        public static Expr operator &(Expr a, Expr b) =>
            Language.And(a, b);

        public static Expr operator |(Expr a, Expr b) =>
            Language.Or(a, b);

        public static Expr operator <(Expr a, Expr b) =>
            Language.LT(a, b);

        public static Expr operator <=(Expr a, Expr b) =>
            Language.LTE(a, b);

        public static Expr operator >(Expr a, Expr b) =>
            Language.GT(a, b);

        public static Expr operator >=(Expr a, Expr b) =>
            Language.GTE(a, b);

        #endregion
    }
}
