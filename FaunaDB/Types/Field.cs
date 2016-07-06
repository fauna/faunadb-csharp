using FaunaDB.Collections;
using System.Collections.Generic;
using System;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    public sealed class Field<T>
    {
        static Func<Value, IResult<IReadOnlyList<V>>> ToCollection<V>(Path path, Field<V> field)
        {
            return input => input.To(Codec.ARRAY).FlatMap(ToList(path, field));
        }

        static Func<IReadOnlyList<Value>, IResult<IReadOnlyList<V>>> ToList<V>(Path path, Field<V> field)
        {
            return values =>
            {
                var success = new ArrayList<V>();
                var failures = new ArrayList<string>();

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
                    return Fail<IReadOnlyList<V>>($"Failed to collect values: {string.Join(", ", failures)}");

                return Success<IReadOnlyList<V>>(success);
            };
        }

        Path path;
        Func<Value, IResult<T>> codec;

        internal Field(Path path, Func<Value, IResult<T>> codec)
        {
            this.path = path;
            this.codec = codec;
        }

        public Field<U> At<U>(Field<U> other) =>
            new Field<U>(path.SubPath(other.path), other.codec);

        public Field<U> To<U>(Func<Value, IResult<U>> codec) =>
            new Field<U>(path, codec);

        public Field<IReadOnlyList<U>> Collect<U>(Field<U> field) =>
            new Field<IReadOnlyList<U>>(path, ToCollection(path, field));

        internal IResult<T> Get(Value root) =>
            path.Get(root).FlatMap(codec);

        public override bool Equals(object obj)
        {
            var other = obj as Field<T>;
            return other != null && path.Equals(other.path);
        }

        public override int GetHashCode() =>
            path.GetHashCode();

        public override string ToString() =>
            path.ToString();
    }

    public static class Field
    {
        public static readonly Field<Value> Root =
            new Field<Value>(Path.Empty, Codec.VALUE);

        public static Field<Value> At(params string[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<Value> At(params int[] values) =>
            new Field<Value>(Path.From(values), Codec.VALUE);

        public static Field<T> To<T>(Func<Value, IResult<T>> codec) =>
            new Field<T>(Path.Empty, codec);
    }
}
