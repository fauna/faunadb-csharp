using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    public interface IOption<T>
    {
        T Value { get; }

        /// <summary>
        /// Apply the function passed on the optional value.
        /// </summary>
        /// <param name="func">the map function to be applied</param>
        /// <returns>
        /// If this is a some value, return a new optional with the function applied to it.
        /// If this is a none value, returns none and ignore the function.
        /// </returns>
        IOption<U> Map<U>(Func<T, U> func);

        /// <summary>
        /// Apply the function passed on the optional value.
        /// </summary>
        /// <param name="func">the map function to be applied</param>
        /// <returns>
        /// If this is a some value, return a new optional with the function applied to it.
        /// If this is a none value, returns none and ignore the function.
        /// </returns>
        IOption<U> FlatMap<U>(Func<T, IOption<U>> func);

        /// <summary>
        /// Matches the current instance. Case it contains some value it will execute the first argument.
        /// Case it contains none value it will execute the second argument.
        /// </summary>
        /// <example>
        /// IOption&lt;string&gt; optional = ...
        ///
        /// int parsed = result.Match(
        ///   Some: value => int.Parse(value),
        ///   None: () => ReturnDefaultValue()
        /// );
        /// </example>
        /// <param name="Some">Function to be executed case this instance contains some value</param>
        /// <param name="None">Function to be executed case this instance contains none value</param>
        U Match<U>(Func<T, U> Some, Func<U> None);

        /// <summary>
        /// Matches the current instance. Case it contains some value it will execute the first argument.
        /// Case it contains none value it will execute the second argument.
        /// </summary>
        /// <example>
        /// IOption&lt;string&gt; optional = ...
        ///
        /// result.Match(
        ///   Some: value => DoSomething(value),
        ///   None: () => DoSomethingElse()
        /// );
        /// </example>
        /// <param name="Some">Function to be executed case this instance contains some value</param>
        /// <param name="None">Function to be executed case this instance contains none value</param>
        void Match(Action<T> Some, Action None);
    }

    class Some<T> : IOption<T>
    {
        T value;

        public Some(T value)
        {
            this.value = value;
        }

        public T Value { get { return value; } }

        public IOption<U> Map<U>(Func<T, U> func) =>
            new Some<U>(func(value));

        public IOption<U> FlatMap<U>(Func<T, IOption<U>> func) =>
            func(value);

        public U Match<U>(Func<T, U> Some, Func<U> None) =>
            Some(value);

        public void Match(Action<T> Some, Action None) =>
            Some(value);

        public override bool Equals(object obj)
        {
            var other = obj as Some<T>;
            return other != null && Equals(value, other.value);
        }

        public override int GetHashCode() =>
            value.GetHashCode();

        public override string ToString() =>
            $"Some({value})";
    }

    interface INone { }

    class None<T> : IOption<T>, INone
    {
        public T Value { get { throw new InvalidOperationException(); } }

        public IOption<U> Map<U>(Func<T, U> func) =>
            new None<U>();

        public IOption<U> FlatMap<U>(Func<T, IOption<U>> func) =>
            new None<U>();

        public U Match<U>(Func<T, U> Some, Func<U> None) =>
            None();

        public void Match(Action<T> Some, Action None) =>
            None();

        public override bool Equals(object obj)
        {
            var other = obj as INone;

            return other != null;
        }

        public override int GetHashCode() =>
            0;

        public override string ToString() =>
            $"None()";
    }

    /// <summary>
    /// Represents an optional value.
    /// </summary>
    public static class Option
    {
        public static IOption<T> Some<T>(T value) =>
            new Some<T>(value);

        public static IOption<T> None<T>() =>
            new None<T>();

        public static IOption<object> None() =>
            new None<object>();

        public static IOption<T> Of<T>(T value)
        {
            if (value != null)
                return Some(value);

            return None<T>();
        }
    }
}
