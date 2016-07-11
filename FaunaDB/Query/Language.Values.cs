using FaunaDB.Types;
using System;
using System.Collections.Generic;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct PathSelector
        {
            IReadOnlyList<Expr> segments;

            internal Expr Segments { get { return Arr(segments); } }

            internal PathSelector(IReadOnlyList<Expr> segments)
            {
                this.segments = segments;
            }

            internal PathSelector(params string[] segments)
            {
                var segs = new List<Expr>();
                foreach (var s in segments)
                    segs.Add(s);
                this.segments = segs;
            }

            internal PathSelector(params int[] segments)
            {
                var segs = new List<Expr>();
                foreach (var s in segments)
                    segs.Add(s);
                this.segments = segs;
            }

            public PathSelector At(params string[] others)
            {
                var all = new List<Expr>(segments);
                foreach (var s in others)
                    all.Add(s);
                return new PathSelector(all);
            }

            public PathSelector At(params int[] others)
            {
                var all = new List<Expr>(segments);
                foreach (var s in others)
                    all.Add(s);
                return new PathSelector(all);
            }
        }

        public static PathSelector Path(params string[] segments) =>
            new PathSelector(segments);

        public static PathSelector Path(params int[] segments) =>
            new PathSelector(segments);

        public static Expr Null() =>
            NullV.Instance;

        public static Expr Ref(string id) =>
            new RefV(id);

        public static Expr Ref(Expr classRef, Expr id) =>
            UnescapedObject.With("ref", classRef, "id", id);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Arr(params Expr[] values) =>
            UnescapedArray.Of(values);

        public static Expr Arr(IEnumerable<Expr> values) =>
            UnescapedArray.Of(values);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(IReadOnlyDictionary<string, Expr> fields) =>
            UnescapedObject.With("object", new UnescapedObject(fields));

        static Expr Varargs(Expr[] values) =>
            values.Length == 1 ? values[0] : UnescapedArray.Of(values);
    }
}
