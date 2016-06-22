using System;

namespace FaunaDB.Types
{
    public sealed class Field<T>
    {
        private Path path;
        private Func<Value, Result<T>> codec;

        internal Field(Path path, Func<Value, Result<T>> codec)
        {
            this.path = path;
            this.codec = codec;
        }

        public Field<U> At<U>(Field<U> other) =>
            new Field<U>(path.SubPath(other.path), other.codec);

        public Field<U> To<U>(Func<Value, Result<U>> codec) =>
            new Field<U>(path, codec);

        //public Field<ArrayList<U>> Collect<U>() =>
        //    new Field<ArrayList<U>>();

        internal Result<T> Get(Value root) =>
            path.Get(root).FlatMap(codec);

        public override bool Equals(object obj)
        {
            Field<T> other = obj as Field<T>;
            return other != null && path.Equals(other.path);
        }

        public override int GetHashCode() =>
            path.GetHashCode();

        public override string ToString() =>
            path.ToString();

    }

    public sealed class Field
    {
        public static Field<Value> At(params string[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<Value> At(params int[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<T> At<T>(Func<Value, Result<T>> codec) =>
            new Field<T>(Path.Empty, codec);
    }
}
