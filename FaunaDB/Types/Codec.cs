using FaunaDB.Collections;
using System;

namespace FaunaDB.Types
{
    public struct Codec
    {
        public static readonly Func<Value, Result<Value>> VALUE = value =>
        {
            if (value == NullV.Instance)
                return Result.Fail<Value>("Value is null");

            return Result.Success(value);
        };

        public static readonly Func<Value, Result<Ref>> REF =
            input => Cast.DoCast<Ref>(input);

        public static readonly Func<Value, Result<SetRef>> SETREF =
            input => Cast.DoCast<SetRef>(input);

        public static readonly Func<Value, Result<long>> LONG =
            Cast.MapTo(Cast.ScalarValue<LongV, long>());

        public static readonly Func<Value, Result<string>> STRING =
            Cast.MapTo(Cast.ScalarValue<StringV, string>());

        public static readonly Func<Value, Result<bool>> BOOLEAN =
            Cast.MapTo(Cast.ScalarValue<BooleanV, bool>());

        public static readonly Func<Value, Result<double>> DOUBLE =
            Cast.MapTo(Cast.ScalarValue<DoubleV, double>());

        public static readonly Func<Value, Result<DateTime>> DATE =
            Cast.MapTo(Cast.ScalarValue<DateV, DateTime>());

        public static readonly Func<Value, Result<DateTime>> TS =
            Cast.MapTo(Cast.ScalarValue<TsV, DateTime>());

        public static readonly Func<Value, Result<ArrayList<Value>>> ARRAY =
            Cast.MapTo<ArrayV, ArrayList<Value>>(input => input.Value);

        public static readonly Func<Value, Result<OrderedDictionary<string, Value>>> OBJECT =
            Cast.MapTo<ObjectV, OrderedDictionary<string, Value>>(input => input.Value);
    }

    struct Cast
    {
        public static Func<Value, Result<O>> MapTo<I, O>(Func<I, O> func) where I : Value
        {
            return input => DoCast<I>(input).Map(func);
        }

        public static Result<O> DoCast<O>(Value input) where O : Value
        {
            if (typeof(O).IsAssignableFrom(input.GetType()))
                return Result.Success((O) input);

            return Result.Fail<O>($"Cannot convert {input.GetType().Name} to {typeof(O).Name}");
        }

        public static Func<T, R> ScalarValue<T, R>() where T : ScalarValue<T, R>
        {
            return input => input.Value;
        }
    }
}
