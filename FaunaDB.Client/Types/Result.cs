﻿using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents the result of an operation. Usually a coercion operation.
    /// </summary>
    public interface IResult<T>
    {
        /// <summary>
        /// Apply the function passed on the result value.
        /// </summary>
        /// <param name="func">the map function to be applied</param>
        /// <returns>
        /// If this is a successful result, return a new successful result with the map function result.
        /// If this is a failure, returns a new failure with the same error message.
        /// </returns>
        IResult<U> Map<U>(Func<T, U> func);

        /// <summary>
        /// Apply the function passed on the result value.
        /// </summary>
        /// <param name="func">the map function to be applied</param>
        /// <returns>
        /// If this is a successful result, returns the map function result.
        /// If this is a failure, returns a new failure with the same error message.
        /// </returns>
        IResult<U> FlatMap<U>(Func<T, IResult<U>> func);

        /// <summary>
        /// Matches the current instance. Case it represents a successful result it will execute the first argument.
        /// Case it represents a failure object it will execute the second argument.
        /// </summary>
        /// <example>
        /// IResult&lt;string&gt; result = ...
        /// int parsed = result.Match(
        ///   Success: value => int.Parse(value),
        ///   Failure: reason => ReturnDefaultValue()
        /// );
        /// </example>
        /// <param name="Success">Function to be executed case this instance represents a successful result</param>
        /// <param name="Failure">Function to be executed case this instance represents a failure result</param>
        U Match<U>(Func<T, U> Success, Func<string, U> Failure);

        /// <summary>
        /// Matches the current instance. Case it represents a successful result it will execute the first argument.
        /// Case it represents a failure object it will execute the second argument.
        /// </summary>
        /// <example>
        /// IResult&lt;string&gt; result = ...
        ///
        /// result.Match(
        ///   Success: value => DoSomething(),
        ///   Failure: reason => DoSomethingElse(reason)
        /// );
        /// </example>
        /// <param name="Success">Action to be executed case this instance represents a successful result</param>
        /// <param name="Failure">Action to be executed case this instance represents a failure result</param>
        void Match(Action<T> Success, Action<string> Failure);

        /// <summary>
        /// Extracts the resulting value or throw an exception if the operation has failed.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        T Value { get; }

        /// <summary>
        /// Gets an <see cref="IOption{T}"/> type containing the result value if the operation was successful,
        /// or <see cref="Option.None{T}()"/> if it was a failure
        /// </summary>
        IOption<T> ToOption { get; }

        /// <summary>
        /// Return true if the operation was successful
        /// </summary>
        bool isSuccess { get; }

        /// <summary>
        /// Return true if the operation has failed
        /// </summary>
        bool isFailure { get; }
    }

    internal class Success<T> : IResult<T>
    {
        private readonly T value;

        internal Success(T value)
        {
            this.value = value;
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

        public IOption<T> ToOption { get { return Option.Some(value); } }

        public bool isSuccess => true;

        public bool isFailure => false;

        public override bool Equals(object obj)
        {
            var other = obj as Success<T>;
            return other != null && Equals(value, other.value);
        }

        public override int GetHashCode() =>
            value.GetHashCode();

        public override string ToString() =>
            value.ToString();
    }

    internal class Failure<T> : IResult<T>
    {
        private readonly string reason;

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

        public IOption<T> ToOption { get { return Option.None<T>(); } }

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
            $"Failure({reason})";
    }

    /// <summary>
    /// Represents the result of an operation. Usually a coercion operation.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates a successful result
        /// </summary>
        /// <param name="value">result's value</param>
        /// <returns>a successful result</returns>
        public static IResult<T> Success<T>(T value) =>
            new Success<T>(value);

        /// <summary>
        /// Creates failure result
        /// </summary>
        /// <param name="reason">the reason for the failure</param>
        /// <returns>a failure result</returns>
        public static IResult<T> Fail<T>(string reason) =>
            new Failure<T>(reason);

        /// <summary>
        /// Creates failure result. Specialization for object type.
        /// </summary>
        /// <param name="reason">the reason for the failure</param>
        /// <returns>a failure result</returns>
        public static IResult<object> Fail(string reason) =>
            new Failure<object>(reason);
    }
}
