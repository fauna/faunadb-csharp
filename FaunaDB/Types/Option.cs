using System;

namespace FaunaDB.Types
{
    public interface IOption
    {
        bool HasValue { get; }
    }

    public class Option<T> : IOption
    {
        private T _value;
        public T Value
        {
            private set
            {
                _value = value;
            }
            get
            {
                if (!HasValue)
                    throw new ArgumentNullException("");

                return _value;
            }
        }

        public bool HasValue { get; private set; }

        internal Option(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Value = value;
            HasValue = true;
        }

        internal Option()
        {
            Value = default(T);
            HasValue = false;
        }

        public override bool Equals(object obj)
        {
            IOption other0 = obj as IOption;
            if (!HasValue && !other0.HasValue)
                return true;

            Option<T> other1 = obj as Option<T>;
            return other1 != null && HasValue == other1.HasValue && Equals(Value, other1.Value);
        }

        public override int GetHashCode() =>
            HasValue ? Value.GetHashCode() : 0;

        public override string ToString() =>
            HasValue ? $"Some({Value.ToString()})" : "None()";
    }

    public class Option
    {
        public static Option<T> Some<T>(T value) =>
            new Option<T>(value);

        public static Option<T> None<T>() =>
            new Option<T>();
    }
}
