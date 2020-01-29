using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Get expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Get(Expr @ref, Expr ts = null) =>
            UnescapedObject.With("get", @ref, "ts", ts);

        /// <summary>
        /// Creates a new KeyFromSecret expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr KeyFromSecret(Expr secret) =>
            UnescapedObject.With("key_from_secret", secret);

        /// <summary>
        /// Creates a new Paginate expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
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
        /// See the <see href="https://fauna.com/documentation/queries#read_functions">FaunaDB Read Functions</see>
        /// </para>
        /// </summary>
        public static Expr Exists(Expr @ref, Expr ts = null) =>
            UnescapedObject.With("exists", @ref, "ts", ts);

        /// <summary>
        /// Creates a new Exists expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/reduce">FaunaDB Reduce Function</see>
        /// </para>
        /// </summary>
        public static Expr Reduce(Expr lambda, Expr initial, Expr collection) =>
            UnescapedObject.With("reduce", lambda, "initial", initial, "collection", collection);

        /// <summary>
        /// Creates a new Exists expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/reduce">FaunaDB Reduce Function</see>
        /// </para>
        /// </summary>
        public static Expr Reduce(Func<Expr, Expr, Expr> lambda, Expr initial, Expr collection) =>
            Reduce(Lambda(lambda), initial, collection);

        /// <summary>
        /// Returns the number of items that exist in the array or set
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/count">FaunaDB Count Function</see>
        /// </para>
        /// </summary>
        public static Expr Count(Expr expr) =>
            UnescapedObject.With("count", expr);

        /// <summary>
        /// Returns the sum of all items.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/sum">FaunaDB Sum Function</see>
        /// </para>
        /// </summary>
        public static Expr Sum(Expr expr) =>
            UnescapedObject.With("sum", expr);

        /// <summary>
        /// Returns the average value of the items.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/mean">FaunaDB Mean Function</see>
        /// </para>
        /// </summary>
        public static Expr Mean(Expr expr) =>
           UnescapedObject.With("mean", expr);

    }
}
