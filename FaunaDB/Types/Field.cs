using FaunaDB.Collections;
using System;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    public sealed class Field<T>
    {
        private static Func<Value, IResult<ArrayList<V>>> ToCollection<V>(Path path, Field<V> field)
        {
            return input => input.To(Codec.ARRAY).FlatMap(ToList(path, field));
        }

        private static Func<ArrayList<Value>, IResult<ArrayList<V>>> ToList<V>(Path path, Field<V> field)
        {
            return values =>
            {
                ArrayList<V> success = new ArrayList<V>();
                ArrayList<string> failures = new ArrayList<string>();

                for (int i = 0; i < values.Count; i++)
                {
                    IResult<V> result = field.Get(values[i]);

                    result.Match(
                        Success: x => success.Add(x),
                        Failure: reason => {
                            Path subPath = path.SubPath(Path.From(i)).SubPath(field.path);
                            failures.Add($"\"{subPath}\" {reason}");
                        });
                }

                if (failures.Count > 0)
                    return Fail<ArrayList<V>>($"Failed to collect values: {string.Join(", ", failures)}");

                return Success(success.ToImmutable());
            };
        }

        private Path path;
        private Func<Value, IResult<T>> codec;

        internal Field(Path path, Func<Value, IResult<T>> codec)
        {
            this.path = path;
            this.codec = codec;
        }

        public Field<U> At<U>(Field<U> other) =>
            new Field<U>(path.SubPath(other.path), other.codec);

        public Field<U> To<U>(Func<Value, IResult<U>> codec) =>
            new Field<U>(path, codec);

        public Field<ArrayList<U>> Collect<U>(Field<U> field) =>
            new Field<ArrayList<U>>(path, ToCollection(path, field));

        internal IResult<T> Get(Value root) =>
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
        public static readonly Field<Value> Root =
            new Field<Value>(Path.Empty, Codec.VALUE);

        public static Field<Value> At(params string[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<Value> At(params int[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<T> As<T>(Func<Value, IResult<T>> codec) =>
            new Field<T>(Path.Empty, codec);
    }
}
