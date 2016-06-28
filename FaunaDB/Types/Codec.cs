using FaunaDB.Collections;
using System;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    public struct Codec
    {
        public static Result<Value> VALUE(Value value)
        {
            if (value == NullV.Instance)
                return Fail<Value>("Value is null");

            return Success(value);
        }

        public static Result<Ref> REF(Value input) =>
            Cast.DoCast<Ref>(input);

        public static Result<SetRef> SETREF(Value input) =>
            Cast.DoCast<SetRef>(input);

        public static Result<long> LONG(Value input) =>
            Cast.MapTo<LongV, long>(input, Cast.ScalarValue);

        public static Result<string> STRING(Value input) =>
            Cast.MapTo<StringV, string>(input, Cast.ScalarValue);

        public static Result<bool> BOOLEAN(Value input) =>
            Cast.MapTo<BooleanV, bool>(input, Cast.ScalarValue);

        public static Result<double> DOUBLE(Value input) =>
            Cast.MapTo<DoubleV, double>(input, Cast.ScalarValue);

        public static Result<DateTime> DATE(Value input) =>
            Cast.MapTo<DateV, DateTime>(input, Cast.ScalarValue);

        public static Result<DateTime> TS(Value input) =>
            Cast.MapTo<TsV, DateTime>(input, Cast.ScalarValue);

        public static Result<ArrayList<Value>> ARRAY(Value input) =>
            Cast.MapTo<ArrayV, ArrayList<Value>>(input, x => x.Value);

        public static Result<OrderedDictionary<string, Value>> OBJECT(Value input) =>
            Cast.MapTo<ObjectV, OrderedDictionary<string, Value>>(input, x => x.Value);
    }

    struct Cast
    {
        public static Result<O> MapTo<I, O>(Value input, Func<I, O> func) where I : Value =>
            DoCast<I>(input).Map(func);

        public static Result<O> DoCast<O>(Value input) where O : Value
        {
            if (typeof(O).IsAssignableFrom(input.GetType()))
                return Success((O) input);

            return Fail<O>($"Cannot convert {input.GetType().Name} to {typeof(O).Name}");
        }

        public static R ScalarValue<R>(ScalarValue<R> input) =>
            input.Value;
    }
}
