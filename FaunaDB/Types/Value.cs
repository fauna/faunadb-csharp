using FaunaDB.Collections;
using FaunaDB.Query;
using System;

namespace FaunaDB.Types
{
    public abstract class Value : Expr
    {
        public Value At(params string[] values) =>
            Field.At(values).Get(this).Match(
                Success: value => value,
                Failure: reason => NullV.Instance
                );

        public Value At(params int[] values) =>
            Field.At(values).Get(this).Match(
                Success: value => value,
                Failure: reason => NullV.Instance
                );

        public Result<T> To<T>(Func<Value, Result<T>> codec) =>
            codec(this);

        public ArrayList<T> Collect<T>(Field<T> field) =>
            Field.Root.Collect(field).Get(this).Value;

        public T Get<T>(Field<T> field) =>
            field.Get(this).Value;

        public Option<T> GetOption<T>(Field<T> field) =>
            field.Get(this).ValueOption;

        #region implicit conversions
        public static implicit operator Value(ArrayList<Value> values) =>
            values == null ? NullV.Instance : ArrayV.Of(values);

        public static implicit operator Value(OrderedDictionary<string, Value> values) =>
            values == null ? NullV.Instance : new ObjectV(values);

        public static implicit operator Value(bool b) =>
            BooleanV.Of(b);

        public static implicit operator Value(double d) =>
            new DoubleV(d);

        public static implicit operator Value(long l) =>
            new LongV(l);

        public static implicit operator Value(int i) =>
            new LongV(i);

        public static implicit operator Value(string s) =>
            s == null ? NullV.Instance : new StringV(s);

        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator ArrayList<Value>(Value v) =>
            ((ArrayV)v).Value;

        public static explicit operator OrderedDictionary<string, Value>(Value v) =>
            ((ObjectV)v).Value;

        public static explicit operator bool(Value v) =>
            ((BooleanV)v).Value;

        public static explicit operator double(Value v) =>
            ((DoubleV)v).Value;

        public static explicit operator long(Value v) =>
            ((LongV)v).Value;

        public static explicit operator string(Value v) =>
            ((StringV)v).Value;
        #endregion
    }

}
