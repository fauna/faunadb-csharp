using FaunaDB.Types;
using System;
using System.Collections.Generic;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Builder for path selectors.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
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

            /// <summary>
            /// Narrow to a specific path on a object node.
            /// </summary>
            /// <param name="others">A list of nested fields</param>
            /// <returns>A new narrowed path</returns>
            public PathSelector At(params string[] others)
            {
                var all = new List<Expr>(segments);
                foreach (var s in others)
                    all.Add(s);
                return new PathSelector(all);
            }

            /// <summary>
            /// Narrow to a specific element index on a array node.
            /// </summary>
            /// <param name="others">A list of nested indexes</param>
            /// <returns>A new narrowed path</returns>
            public PathSelector At(params int[] others)
            {
                var all = new List<Expr>(segments);
                foreach (var s in others)
                    all.Add(s);
                return new PathSelector(all);
            }
        }

        /// <summary>
        /// Helper for constructing a <see cref="PathSelector"/> with the given path terms.
        /// <para>
        /// See <see cref="PathSelector"/>
        /// </para>
        /// </summary>
        public static PathSelector Path(params string[] segments) =>
            new PathSelector(segments);

        /// <summary>
        /// Helper for constructing a <see cref="PathSelector"/> with the given path terms.
        /// <para>
        /// See <see cref="PathSelector"/>
        /// </para>
        /// </summary>
        public static PathSelector Path(params int[] segments) =>
            new PathSelector(segments);

        /// <summary>
        /// Creates a null value.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Null() =>
            NullV.Instance;

        /// <summary>
        /// Creates a <see cref="RefV"/> value.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Ref(string id) =>
            new RefV(id);

        /// <summary>
        /// Calls ref function to create a ref value.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
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
        /// Creates a new Object value wrapping the provided map.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(IReadOnlyDictionary<string, Expr> fields) =>
            UnescapedObject.With("object", new UnescapedObject(fields));

        static Expr Varargs(Expr[] values) =>
            values.Length == 1 ? values[0] : UnescapedArray.Of(values);
    }
}
