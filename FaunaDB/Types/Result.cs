using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FaunaDB.Collections;

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

        bool isSuccess { get; }

        bool isFailure { get; }
    }

    class Success<T> : IResult<T>
    {
        readonly T value;
        readonly IEqualityComparer comparer;

        internal Success(T value, IEqualityComparer comparer)
        {
            this.value = value;
            this.comparer = comparer;
        }

        public IResult<U> Map<U>(Func<T, U> func) =>
            Result.Success(func(value));

        public IResult<U> FlatMap<U>(Func<T, IResult<U>> func) =>
            func(value);

        public U Match<U>(Func<T, U> Success, Func<string, U> Failure) =>
            Success(value);

        public void Match(Action<T> Success, Action<string> Failure) =>
            Success(value);

        public T Value { get { return value; } }

        public IOption<T> ValueOption { get { return Option.Some(value); } }

        public bool isSuccess => true;

        public bool isFailure => false;

        public override bool Equals(object obj)
        {
            var other = obj as Success<T>;
            return other != null && comparer.Equals(value, other.value);
        }

        public override int GetHashCode() =>
            value.GetHashCode();

        public override string ToString() =>
            value.ToString();
    }

    class Failure<T> : IResult<T>
    {
        readonly string reason;

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

        public bool isSuccess => false;

        public bool isFailure => true;

        public override bool Equals(object obj)
        {
            var other = obj as Failure<T>;
            return other != null && Equals(reason, other.reason);
        }

        public override int GetHashCode() =>
            reason.GetHashCode();

        public override string ToString() =>
            reason;
    }

    public static class Result
    {
        public static IResult<T> Success<T>(T value) =>
            new Success<T>(value, EqualityComparer<T>.Default);

        public static IResult<IReadOnlyDictionary<Key, Value>> Success<Key, Value>(IReadOnlyDictionary<Key, Value> value) =>
            new Success<IReadOnlyDictionary<Key, Value>>(value, DictionaryComparer<Key, Value>.Default);

        public static IResult<IReadOnlyList<T>> Success<T>(IReadOnlyList<T> value) =>
            new Success<IReadOnlyList<T>>(value, ListComparer<T>.Default);

        public static IResult<T> Fail<T>(string reason) =>
            new Failure<T>(reason);
    }

    class DictionaryComparer<Key, Value> : AbstractComparer<IReadOnlyDictionary<Key, Value>>
    {
        public static readonly DictionaryComparer<Key, Value> Default =
            new DictionaryComparer<Key, Value>();

        public override bool Equals(IReadOnlyDictionary<Key, Value> x, IReadOnlyDictionary<Key, Value> y) =>
            x.DictEquals(y);
    }

    class ListComparer<T> : AbstractComparer<IReadOnlyList<T>>
    {
        public static readonly ListComparer<T> Default =
            new ListComparer<T>();

        public override bool Equals(IReadOnlyList<T> x, IReadOnlyList<T> y) =>
            x.SequenceEqual(y);
    }

    abstract class AbstractComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        public abstract bool Equals(T x, T y);

        public new bool Equals(object x, object y) => Equals((T)x, (T)y);

        public int GetHashCode(T obj) => 0;

        public int GetHashCode(object obj) => 0;
    }
}
