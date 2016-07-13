using System.Collections.Generic;
using System;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    public struct Codec
    {
        public static IResult<Value> VALUE(Value value)
        {
            if (value == NullV.Instance)
                return Fail<Value>("Value is null");

            return Success(value);
        }

        public static IResult<RefV> REF(Value input) =>
            Cast.DoCast<RefV>(input);

        public static IResult<SetRef> SETREF(Value input) =>
            Cast.DoCast<SetRef>(input);

        public static IResult<long> LONG(Value input) =>
            Cast.MapTo<LongV, long>(input, Cast.ScalarValue);

        public static IResult<string> STRING(Value input) =>
            Cast.MapTo<StringV, string>(input, Cast.ScalarValue);

        public static IResult<bool> BOOLEAN(Value input) =>
            Cast.MapTo<BooleanV, bool>(input, Cast.ScalarValue);

        public static IResult<double> DOUBLE(Value input) =>
            Cast.MapTo<DoubleV, double>(input, Cast.ScalarValue);

        public static IResult<DateTime> DATE(Value input) =>
            Cast.MapTo<DateV, DateTime>(input, Cast.ScalarValue);

        public static IResult<DateTime> TIME(Value input) =>
            Cast.MapTo<TimeV, DateTime>(input, Cast.ScalarValue);

        public static IResult<IReadOnlyList<Value>> ARRAY(Value input) =>
            Cast.MapTo<ArrayV, IReadOnlyList<Value>>(input, x => x.Value);

        public static IResult<IReadOnlyDictionary<string, Value>> OBJECT(Value input) =>
            Cast.MapTo<ObjectV, IReadOnlyDictionary<string, Value>>(input, x => x.Value);
    }

    struct Cast
    {
        public static IResult<O> MapTo<I, O>(Value input, Func<I, O> func) where I : Value =>
            DoCast<I>(input).Map(func);

        public static IResult<O> DoCast<O>(Value input) where O : Value
        {
            if (typeof(O).IsAssignableFrom(input.GetType()))
                return Success((O) input);

            return Fail<O>($"Cannot convert {input.GetType().Name} to {typeof(O).Name}");
        }

        public static R ScalarValue<R>(ScalarValue<R> input) =>
            input.Value;
    }
}
