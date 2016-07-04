using System;

namespace FaunaDB.Types
{
    public interface IResult<T>
    {
        IResult<U> Map<U>(Func<T, U> func);

        IResult<U> FlatMap<U>(Func<T, IResult<U>> func);

        U Match<U>(Func<T, U> Success, Func<string, U> Failure);

        void Match(Action<T> Success, Action<string> Failure);

        T Value { get; }

        IOption<T> ValueOption { get; }
    }

    class Success<T> : IResult<T>
    {
        T value;

        internal Success(T value)
        {
            this.value = value;
        }

        public IResult<U> Map<U>(Func<T, U> func) =>
            new Success<U>(func(value));

        public IResult<U> FlatMap<U>(Func<T, IResult<U>> func) =>
            func(value);

        public U Match<U>(Func<T, U> Success, Func<string, U> Failure) =>
            Success(value);

        public void Match(Action<T> Success, Action<string> Failure) =>
            Success(value);

        public T Value { get { return value; } }

        public IOption<T> ValueOption { get { return Option.Some(value); } }

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

    class Failure<T> : IResult<T>
    {
        string reason;

        internal Failure(string reason)
        {
            this.reason = reason;
        }

        public IResult<U> Map<U>(Func<T, U> func) =>
            new Failure<U>(reason);

        public IResult<U> FlatMap<U>(Func<T, IResult<U>> func) =>
            new Failure<U>(reason);

        public U Match<U>(Func<T, U> Success, Func<string, U> Failure) =>
            Failure(reason);

        public void Match(Action<T> Success, Action<string> Failure) =>
            Failure(reason);

        public T Value { get { throw new InvalidOperationException(reason); } }

        public IOption<T> ValueOption { get { return Option.None<T>(); } }

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

    public class Result
    {
        public static IResult<T> Success<T>(T value) =>
            new Success<T>(value);

        public static IResult<T> Fail<T>(string reason) =>
            new Failure<T>(reason);
    }
}
