using System;

namespace FaunaDB.Types
{
    public interface Result<T>
    {
        Result<U> Map<U>(Func<T, U> func);

        Result<U> FlatMap<U>(Func<T, Result<U>> func);

        U Match<U>(Func<T, U> Success, Func<string, U> Failure);

        void Match(Action<T> Success, Action<string> Failure);

        T Value { get; }

        Option<T> ValueOption { get; }
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

        public U Match<U>(Func<T, U> Success, Func<string, U> Failure) =>
            Success(value);

        public void Match(Action<T> Success, Action<string> Failure) =>
            Success(value);

        public T Value { get { return value; } }

        public Option<T> ValueOption { get { return Option.Some(value); } }
    }

    internal class Failure<T> : Result<T>
    {
        private string reason;

        internal Failure(string reason)
        {
            this.reason = reason;
        }

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

        public U Match<U>(Func<T, U> Success, Func<string, U> Failure) =>
            Failure(reason);

        public void Match(Action<T> Success, Action<string> Failure) =>
            Failure(reason);

        public T Value { get { throw new InvalidOperationException(reason); } }

        public Option<T> ValueOption { get { return Option.None<T>(); } }
    }
}
