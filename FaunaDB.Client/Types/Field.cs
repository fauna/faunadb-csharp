using System.Collections.Generic;
using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// A field extractor for a FaunaDB <see cref="Value"/>
    /// <para>
    /// See <see cref="Value"/>
    /// </para>
    /// </summary>
    public sealed class Field<T>
    {
        internal readonly Path path;
        internal readonly Func<Value, IResult<T>> codec;

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
        /// Creates a field extractor that coerces its value using the type specified
        /// </summary>
        /// <returns>a new field that coerces its value using the type specified</returns>
        public Field<U> To<U>() =>
            new Field<U>(path, Field.Decode<U>);

        /// <summary>
        /// Creates a field extractor that collects each inner value of an array using the nested field passed,
        /// assuming the root value is an instance of <see cref="ArrayV"/>
        /// </summary>
        /// <param name="field">field to be extracted from each array's element</param>
        /// <returns>a new field that collects each inner value using the field passed</returns>
        public Field<IReadOnlyList<U>> Collect<U>(Field<U> field) =>
            new Field<IReadOnlyList<U>>(path, Field.ToCollection(path, field));

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
    /// See <see cref="Value"/>
    /// </para>
    /// </summary>
    public static class Field
    {
        internal static Func<Value, IResult<IReadOnlyList<V>>> ToCollection<V>(Path path, Field<V> field)
        {
            return input => input.To<Value[]>().FlatMap(values =>
            {
                var success = new List<V>();
                var failures = new List<string>();

                for (int i = 0; i < values.Length; i++)
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
                    return Result.Fail<IReadOnlyList<V>>($"Failed to collect values: {string.Join(", ", failures)}");

                return Result.Success<IReadOnlyList<V>>(success);
            });
        }

        internal static IResult<T> Decode<T>(Value value)
        {
            try
            {
                if (value == NullV.Instance)
                    return Result.Fail<T>("Value is null");

                return Result.Success(Decoder.Decode<T>(value));
            }
            catch (Exception ex)
            {
                return Result.Fail<T>(ex.Message);
            }
        }

        internal static readonly Field<Value> Root =
            new Field<Value>(Path.Empty, Result.Success);

        /// <summary>
        /// Creates a field that extracts its value from a object path, assuming the value
        /// is an instance of <see cref="ObjectV"/>.
        /// </summary>
        /// <param name="keys">path to the field</param>
        /// <returns>the field extractor</returns>
        public static Field<Value> At(params string[] keys) =>
            new Field<Value>(Path.From(keys), Result.Success);

        /// <summary>
        /// Creates a field that extracts its value from a array index, assuming the value
        /// is an instance of <see cref="ArrayV"/>.
        /// </summary>
        /// <param name="indexes">indexes path to the value</param>
        /// <returns>the field extractor</returns>
        public static Field<Value> At(params int[] indexes) =>
            new Field<Value>(Path.From(indexes), Result.Success);

        /// <summary>
        /// Creates a field that coerces its value using the type specified.
        /// </summary>
        /// <returns>the field extractor</returns>
        public static Field<T> To<T>() =>
            new Field<T>(Path.Empty, Decode<T>);
    }
}
