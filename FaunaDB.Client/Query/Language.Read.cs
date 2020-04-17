using System;
using System.Collections.Generic;
using FaunaDB.Types;

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
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/paginate">FaunaDB Paginate Function</see>
        /// </para>
        /// </summary>
        public static Expr Paginate(
            Expr set,
            Expr ts = null,
            Expr after = null,
            Expr before = null,
            Expr size = null,
            Expr events = null,
            Expr sources = null,
            Cursor cursor = null) =>
            UnescapedObject.With(new Dictionary<string, Expr>
            {
                { "paginate", set ?? NullV.Instance },
                { "ts", ts },
                { "size", size },
                { "events", events },
                { "sources", sources },
                { "after", after },
                { "before", before },
                { "cursor", cursor?.Source }
            });
        

        /// <summary>
        /// Creates a new Cursor to be used with a Paginate expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/paginate">FaunaDB Paginate Function</see>
        /// </para>
        /// </summary>
        public class Cursor
        {

            public Expr Source { get; }

            internal Cursor(Expr expr)
            {
                Source = expr;
            }

        }

        public static Cursor RawCursor(Expr expr) =>
                new Cursor(expr);

        public static Cursor After(Expr expr) =>
                new Cursor(Obj("after", expr));

        public static Cursor Before(Expr expr) =>
                new Cursor(Obj("before", expr));


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
