using System;
using System.Collections.Generic;
using System.Linq;
using FaunaDB.Types;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Builder for path selectors.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public struct PathSelector
        {
            private IReadOnlyList<Expr> segments;

            internal Expr Segments { get { return Arr(segments); } }

            internal PathSelector(IReadOnlyList<Expr> segments)
            {
                this.segments = segments;
            }

            internal PathSelector(params string[] segments)
            {
                var segs = new List<Expr>();
                foreach (var s in segments)
                {
                    segs.Add(s);
                }

                this.segments = segs;
            }

            internal PathSelector(params int[] segments)
            {
                var segs = new List<Expr>();
                foreach (var s in segments)
                {
                    segs.Add(s);
                }

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
                {
                    all.Add(s);
                }

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
                {
                    all.Add(s);
                }

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
        /// Helper for constructing a <see cref="PathSelector"/> with the given expression.
        /// <para>
        /// See <see cref="PathSelector"/>
        /// </para>
        /// </summary>
        public static PathSelector Path(params Expr[] expr) =>
            new PathSelector(expr);

        /// <summary>
        /// Creates a null value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Null() =>
            NullV.Instance;

        /// <summary>
        /// Creates a ref value from a string.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Ref(string @ref) =>
            UnescapedObject.With("@ref", @ref);

        /// <summary>
        /// Calls ref function to create a ref value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Ref(Expr classRef, Expr id) =>
            UnescapedObject.With("ref", classRef, "id", id);

        /// <summary>
        /// Creates a new Timestamp value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Ts(DateTime dateTime) =>
            new TimeV(dateTime);

        /// <summary>
        /// Creates a new Timestamp value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Ts(string iso8601Time) =>
            new TimeV(iso8601Time);

        /// <summary>
        /// Creates a new Date value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Dt(DateTime dateTime) =>
            new DateV(dateTime);

        /// <summary>
        /// Creates a new Date value.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Dt(string iso8601Date) =>
            new DateV(iso8601Date);

        /// <summary>
        /// Creates a new Array value containing the provided entries.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Arr(params Expr[] values) =>
            new UnescapedArray(values);

        /// <summary>
        /// Creates a new Array value containing the provided enumerable of values.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Arr(IEnumerable<Expr> values) =>
            new UnescapedArray(values.ToList());

        /// <summary>
        /// Creates a new Object value wrapping the provided map.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#simple-type">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(IReadOnlyDictionary<string, Expr> fields) =>
            UnescapedObject.With("object", new UnescapedObject(fields));

        /// <summary>
        /// Creates a new Bytes value
        /// </summary>
        public static Expr Bytes(params byte[] bytes) =>
            BytesV.Of(bytes);

        private static Expr Varargs(Expr[] values)
        {
            if (values == null)
            {
                return Null();
            }

            return values.Length == 1 ? values[0] : new UnescapedArray(values);
        }
    }
}
