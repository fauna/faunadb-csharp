using FaunaDB.Collections;
using FaunaDB.Types;
using System;
using System.Collections.Generic;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct PathSelector
        {
            private ArrayList<Expr> segments;

            public Expr Segments { get { return Arr(segments); } }

            internal PathSelector(ArrayList<Expr> segments)
            {
                this.segments = segments;
            }

            internal PathSelector(params string[] segments)
            {
                this.segments = new ArrayList<Expr>();
                foreach (var s in segments)
                    this.segments.Add(s);
            }

            internal PathSelector(params int[] segments)
            {
                this.segments = new ArrayList<Expr>();
                foreach (var s in segments)
                    this.segments.Add(s);
            }

            public PathSelector At(params string[] others)
            {
                ArrayList<Expr> all = new ArrayList<Expr>(segments);
                foreach (var s in others)
                    all.Add(s);
                return new PathSelector(all);
            }

            public PathSelector At(params int[] others)
            {
                ArrayList<Expr> all = new ArrayList<Expr>(segments);
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
            new Ref(id);

        public static Expr Ref(Expr classRef, Expr id) =>
            UnescapedObject.With("ref", classRef, "id", id);

        public static Expr Ts(DateTime dateTime) =>
            new TsV(dateTime);

        public static Expr Ts(string iso8601Time) =>
            new TsV(iso8601Time);

        public static Expr Dt(DateTime dateTime) =>
            new DateV(dateTime);

        public static Expr Dt(string iso8601Date) =>
            new DateV(iso8601Date);

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Arr(params Expr[] values) =>
            UnescapedArray.Of(values);

        public static Expr Arr(IEnumerable<Expr> values) =>
            UnescapedArray.Of(values);

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(Expr fields) =>
            UnescapedObject.With("object", fields);

        #region Helpers
        static Expr Varargs(Expr head, Expr[] tail)
        {
            if (tail.Length == 0)
                return head;

            Expr[] values = new Expr[tail.Length + 1];
            values[0] = head;
            tail.CopyTo(values, 1);

            return UnescapedArray.Of(values);
        }

        static Expr Varargs(Expr[] values) =>
            values.Length == 1 ? values[0] : UnescapedArray.Of(values);
        #endregion

    }
}
