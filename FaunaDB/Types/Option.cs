using System;

namespace FaunaDB.Types
{
    public interface IOption<T>
    {
        T Value { get; }

        IOption<U> Map<U>(Func<T, U> func);

        IOption<U> FlatMap<U>(Func<T, IOption<U>> func);

        U Match<U>(Func<T, U> Some, Func<U> None);

        void Match(Action<T> Some, Action None);
    }

    public class Some<T> : IOption<T>
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

    public interface INone { }

    public class None<T> : IOption<T>, INone
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

    public static class Option
    {
        public static IOption<T> Some<T>(T value) =>
            new Some<T>(value);

        public static IOption<T> None<T>() =>
            new None<T>();

        public static IOption<T> Of<T>(T value)
        {
            if (value != null)
                return Some(value);

            return None<T>();
        }
    }
}
