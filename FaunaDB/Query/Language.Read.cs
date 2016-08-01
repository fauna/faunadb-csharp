﻿namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Get expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Get(Expr @ref, Expr ts = null) =>
            UnescapedObject.With("get", @ref, "ts", ts);

        /// <summary>
        /// Creates a new Paginate expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Paginate(
            Expr set,
            Expr ts = null,
            Expr after = null,
            Expr before = null,
            Expr size = null,
            Expr events = null,
            Expr sources = null) =>
                UnescapedObject.With(
                    "paginate", set,
                    "ts", ts,
                    "after", after,
                    "before", before,
                    "size", size,
                    "events", events,
                    "sources", sources);

        /// <summary>
        /// Creates a new Exists expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Exists(Expr @ref, Expr ts = null) =>
            UnescapedObject.With("exists", @ref, "ts", ts);

        /// <summary>
        /// Creates a new Count expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Count(Expr set, Expr events = null) =>
            UnescapedObject.With("count", set, "events", events);
    }
}
