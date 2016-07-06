using System.Collections.Generic;
using FaunaDB.Collections;

using static FaunaDB.Types.Result;

namespace FaunaDB.Types
{
    sealed class Path
    {
        public static readonly Path Empty = new Path(ArrayList<Segment>.Empty);

        internal static Path From(params string[] values)
        {
            var segments = new ArrayList<Segment>();
            foreach (var field in values)
                segments.Add(new ObjectKey(field));
            return new Path(segments);
        }

        internal static Path From(params int[] values)
        {
            var segments = new ArrayList<Segment>();
            foreach (var index in values)
                segments.Add(new ArrayIndex(index));
            return new Path(segments);
        }

        IReadOnlyList<Segment> segments;

        Path(IReadOnlyList<Segment> segments)
        {
            this.segments = segments;
        }

        internal Path SubPath(Path other)
        {
            var list = new ArrayList<Segment>();
            foreach (var s in segments) list.Add(s);
            foreach (var s in other.segments) list.Add(s);
            return new Path(list);
        }

        internal IResult<Value> Get(Value root)
        {
            IResult<Value> result = Success(root);

            foreach (var s in segments)
                result = result.FlatMap(value => s.Get(value));

            return result.Match(
                Success: value => Success(value),
                Failure: reason => Fail<Value>($"Cannot find path \"{this}\". {reason}"));
        }

        public override bool Equals(object obj)
        {
            Path other = obj as Path;
            return other != null && segments.Equals(other.segments);
        }

        public override int GetHashCode() =>
            segments.GetHashCode();

        public override string ToString() =>
            string.Join("/", segments);

        interface Segment
        {
            IResult<Value> Get(Value root);
        }

        class ObjectKey : Segment
        {
            private string field;

            public ObjectKey(string field)
            {
                this.field = field;
            }

            public IResult<Value> Get(Value root)
            {
                return root.To(Codec.OBJECT).FlatMap(obj => {
                    Value value;
                    if (obj.TryGetValue(field, out value))
                        return Success(value);

                    return Fail<Value>($"Object key \"{field}\" not found");
                });
            }

            public override bool Equals(object obj)
            {
                ObjectKey other = obj as ObjectKey;
                return other != null && field == other.field;
            }

            public override int GetHashCode() =>
                field.GetHashCode();

            public override string ToString() =>
                field.ToString();
        }

        class ArrayIndex : Segment
        {
            private int index;

            public ArrayIndex(int index)
            {
                this.index = index;
            }

            public IResult<Value> Get(Value root)
            {
                return root.To(Codec.ARRAY).FlatMap(array => {
                    if (index >= 0 && index < array.Count)
                        return Success(array[index]);
                    
                    return Fail<Value>($"Array index \"{index}\" not found");
                });
            }

            public override bool Equals(object obj)
            {
                ArrayIndex other = obj as ArrayIndex;
                return other != null && index == other.index;
            }

            public override int GetHashCode() =>
                index.GetHashCode();

            public override string ToString() =>
                index.ToString();
        }
    }
}
