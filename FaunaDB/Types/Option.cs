using System;

namespace FaunaDB.Types
{
    public abstract class IOption
    {
        internal bool IsSome { get; set; }
    }

    public class Option<T> : IOption
    {
        private T value;
        public T Value
        {
            private set
            {
                this.value = value;
            }
            get
            {
                if (!IsSome)
                    throw new InvalidOperationException("Trying to get value from None()");

                return value;
            }
        }

        public Option<U> Map<U>(Func<T, U> func) =>
            IsSome ? new Option<U>(func(value)) : new Option<U>();

        public Option<U> FlatMap<U>(Func<T, Option<U>> func) =>
            IsSome ? func(value) : new Option<U>();

        public U Match<U>(Func<T, U> Some, Func<U> None) =>
            IsSome ? Some(value) : None();

        public void Match(Action<T> Some, Action None)
        {
            if (IsSome)
                Some(value);
            else
                None();
        }

        internal Option(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Value = value;
            IsSome = true;
        }

        internal Option()
        {
            Value = default(T);
            IsSome = false;
        }

        public override bool Equals(object obj)
        {
            IOption other0 = obj as IOption;
            if (!IsSome && !other0.IsSome)
                return true;

            Option<T> other1 = obj as Option<T>;
            return other1 != null && IsSome == other1.IsSome && Equals(Value, other1.Value);
        }

        public override int GetHashCode() =>
            IsSome ? Value.GetHashCode() : 0;

        public override string ToString() =>
            IsSome ? $"Some({Value.ToString()})" : "None()";
    }

    public class Option
    {
        public static Option<T> Some<T>(T value) =>
            new Option<T>(value);

        public static Option<T> None<T>() =>
            new Option<T>();
    }
}
