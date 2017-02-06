using System.Collections.Generic;
using System;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    /// <summary>
    /// A field extractor for a FaunaDB <see cref="Value"/>
    /// <para>
    /// See <see cref="Value"/> and <see cref="Codec"/>
    /// </para>
    /// </summary>
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
                var success = new List<V>();
                var failures = new List<string>();

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

        /// <summary>
        /// Creates a field extractor composed with another nested field
        /// </summary>
        /// <param name="other">nested field to compose with</param>
        /// <returns>a new field extractor with the nested field</returns>
        public Field<U> At<U>(Field<U> other) =>
            new Field<U>(path.SubPath(other.path), other.codec);

        /// <summary>
        /// Creates a field extractor that coerces its value using the codec passed
        /// </summary>
        /// <param name="codec">codec to be used to coerce the field's value</param>
        /// <returns>a new field that coerces its value using the codec passed</returns>
        public Field<U> To<U>(Func<Value, IResult<U>> codec) =>
            new Field<U>(path, codec);

        /// <summary>
        /// Creates a field extractor that collects each inner value of an array using the nested field passed,
        /// assuming the root value is an instance of <see cref="ArrayV"/>
        /// </summary>
        /// <param name="field">field to be extracted from each array's element</param>
        /// <returns>a new field that collects each inner value using the field passed</returns>
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

    /// <summary>
    /// A field extractor for a FaunaDB <see cref="Value"/>
    /// <para>
    /// See <see cref="Value"/> and <see cref="Codec"/>
    /// </para>
    /// </summary>
    public static class Field
    {
        internal static readonly Field<Value> Root =
            new Field<Value>(Path.Empty, Codec.VALUE);

        /// <summary>
        /// Creates a field that extracts its value from a object path, assuming the value
        /// is an instance of <see cref="ObjectV"/>.
        /// </summary>
        /// <param name="keys">path to the field</param>
        /// <returns>the field extractor</returns>
        public static Field<Value> At(params string[] keys) =>
            new Field<Value>(Path.From(keys), Codec.VALUE);

        /// <summary>
        /// Creates a field that extracts its value from a array index, assuming the value
        /// is an instance of <see cref="ArrayV"/>.
        /// </summary>
        /// <param name="indexes">indexes path to the value</param>
        /// <returns>the field extractor</returns>
        public static Field<Value> At(params int[] indexes) =>
            new Field<Value>(Path.From(indexes), Codec.VALUE);

        /// <summary>
        /// Creates a field that coerces its value using the codec passed
        /// </summary>
        /// <param name="codec">codec used to coerce the field's value</param>
        /// <returns>the field extractor</returns>
        public static Field<T> To<T>(Func<Value, IResult<T>> codec) =>
            new Field<T>(Path.Empty, codec);
    }
}
