using FaunaDB.Collections;
using System;

namespace FaunaDB.Types
{
    public interface Result<T>
    {
        bool IsSuccess { get; }

        bool IsFailure { get; }

        Result<U> Map<U>(Func<T, U> func);

        Result<U> FlatMap<U>(Func<T, Result<U>> func);
    }

    public class Result
    {
        public static Result<T> Success<T>(T value) =>
            new Success<T>(value);

        public static Result<T> Fail<T>(string reason) =>
            new Failure<T>(reason);
    }

    internal class Success<T> : Result<T>
    {
        private T value;

        internal Success(T value)
        {
            this.value = value;
        }

        public bool IsSuccess { get { return true; } }

        public bool IsFailure { get { return false; } }

        public Result<U> Map<U>(Func<T, U> func) =>
            new Success<U>(func(value));

        public Result<U> FlatMap<U>(Func<T, Result<U>> func) =>
            func(value);

        public override bool Equals(object obj)
        {
            Success<T> other = obj as Success<T>;
            return other != null && object.Equals(value, other.value);
        }

        public override int GetHashCode() =>
            value.GetHashCode();

        public override string ToString() =>
            value.ToString();
    }

    internal class Failure<T> : Result<T>
    {
        private string reason;

        internal Failure(string reason)
        {
            this.reason = reason;
        }

        public bool IsSuccess { get { return false; } }

        public bool IsFailure { get { return true; } }

        public Result<U> Map<U>(Func<T, U> func) =>
            new Failure<U>(reason);

        public Result<U> FlatMap<U>(Func<T, Result<U>> func) =>
            new Failure<U>(reason);

        public override bool Equals(object obj)
        {
            Failure<T> other = obj as Failure<T>;
            return other != null && reason.Equals(other.reason);
        }

        public override int GetHashCode() =>
            reason.GetHashCode();

        public override string ToString() =>
            reason;
    }

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
