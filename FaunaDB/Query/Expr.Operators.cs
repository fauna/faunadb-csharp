using FaunaDB.Errors;
using FaunaDB.Types;
using FaunaDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaunaDB.Query
{
    public abstract partial class Expr
    {
        #region implicit conversions
        public static implicit operator Expr(List<Value> values) =>
            values == null ? NullV.Instance : ArrayV.FromEnumerable(values);

        public static implicit operator Expr(OrderedDictionary<string, Value> values) =>
            values == null ? NullV.Instance : new ObjectV(values);

        public static implicit operator Expr(bool b) =>
            BooleanV.Of(b);

        public static implicit operator Expr(double d) =>
            new DoubleV(d);

        public static implicit operator Expr(long l) =>
            new LongV(l);

        public static implicit operator Expr(int i) =>
            new LongV(i);

        public static implicit operator Expr(string s) =>
            s == null ? NullV.Instance : new StringV(s);

        public static implicit operator Expr(Language.Action action)
        {
            switch (action)
            {
                case Language.Action.CREATE:
                    return new StringV("create");

                case Language.Action.DELETE:
                    return new StringV("delete");
            }

            throw new InvalidValueException("Invalid action value");
        }
        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator List<Value>(Expr v) =>
            ((ArrayV)v).Value;

        public static explicit operator OrderedDictionary<string, Value>(Expr v) =>
            ((ObjectV)v).Value;

        public static explicit operator bool(Expr v) =>
            ((BooleanV)v).Value;

        public static explicit operator double(Expr v) =>
            ((DoubleV)v).Value;

        public static explicit operator long(Expr v) =>
            ((LongV)v).Value;

        public static explicit operator string(Expr v) =>
            ((StringV)v).Value;

        public static explicit operator Language.Action(Expr v)
        {
            switch (((StringV)v).Value)
            {
                case "create":
                    return Language.Action.CREATE;

                case "delete":
                    return Language.Action.DELETE;
            }

            throw new InvalidValueException("Invalid string value. Should be \"create\" or \"delete\"");
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
