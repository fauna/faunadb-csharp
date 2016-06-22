using FaunaDB.Collections;
using System;

namespace FaunaDB.Types
{
    public sealed class Path
    {
        public static readonly Path Empty = new Path(new ArrayList<Segment>());

        public static Path From(params string[] values)
        {
            ArrayList<Segment> segments = new ArrayList<Segment>();
            foreach (var field in values)
                segments.Add(new ObjectKey(field));
            return new Path(segments);
        }

        public static Path From(params int[] values)
        {
            ArrayList<Segment> segments = new ArrayList<Segment>();
            foreach (var index in values)
                segments.Add(new ArrayIndex(index));
            return new Path(segments);
        }

        private ArrayList<Segment> segments;

        private Path(ArrayList<Segment> segments)
        {
            this.segments = segments;
        }

        public Path SubPath(Path other)
        {
            ArrayList<Segment> list = new ArrayList<Segment>();
            foreach (var s in segments) list.Add(s);
            foreach (var s in other.segments) list.Add(s);
            return new Path(list);
        }

        public Result<Value> Get(Value root)
        {
            Result<Value> result = Result.Success(root);

            foreach (var s in segments)
            {
                result = result.FlatMap(value => s.Get(value));
                if (result.IsFailure)
                    return Result.Fail<Value>($"Cannot find path \"{this}\". {result}");
            }

            return result;
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
            Result<Value> Get(Value root);
        }

        class ObjectKey : Segment
        {
            private string field;

            public ObjectKey(string field)
            {
                this.field = field;
            }

            public Result<Value> Get(Value root)
            {
                return root.To(Codec.OBJECT).FlatMap(obj => {
                    Value value;
                    if (obj.TryGetValue(field, out value))
                        return Result.Success(value);

                    return Result.Fail<Value>($"Object key \"{field}\" not found");
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

            public Result<Value> Get(Value root)
            {
                return root.To(Codec.ARRAY).FlatMap(array => {
                    if (index >= 0 && index < array.Count)
                        return Result.Success(array[index]);
                    
                    return Result.Fail<Value>($"Array index \"{index}\" not found");
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
