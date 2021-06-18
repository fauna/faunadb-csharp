﻿namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Map expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// <para>
        /// See <see cref="Map(Expr, System.Func{Expr, Expr})"/>,
        /// <see cref="Map(Expr, System.Func{Expr, Expr, Expr})"/>,
        /// <see cref="Map(Expr, System.Func{Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Map(Expr, System.Func{Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Map(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Map(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">Lambda expression created by <see cref="Lambda(Expr, Expr)"/></param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Arr(1, 2, 3), Lambda("i", Multiply(Var("i"), 2)));
        ///
        /// Assert.AreEqual(Arr(2, 4, 6), result);
        /// </code>
        /// </example>
        public static Expr Map(Expr collection, Expr lambda) =>
            UnescapedObject.With("map", lambda, "collection", collection);

        /// <summary>
        /// Creates a new Foreach expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// <para>
        /// See <see cref="Foreach(Expr, System.Func{Expr, Expr})"/>,
        /// <see cref="Foreach(Expr, System.Func{Expr, Expr, Expr})"/>,
        /// <see cref="Foreach(Expr, System.Func{Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Foreach(Expr, System.Func{Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Foreach(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Foreach(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </para>
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">Lambda expression created by <see cref="Lambda(Expr, Expr)"/></param>
        public static Expr Foreach(Expr collection, Expr lambda) =>
            UnescapedObject.With("foreach", lambda, "collection", collection);

        /// <summary>
        /// Creates a new Filter expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// <para>
        /// See <see cref="Filter(Expr, System.Func{Expr, Expr})"/>,
        /// <see cref="Filter(Expr, System.Func{Expr, Expr, Expr})"/>,
        /// <see cref="Filter(Expr, System.Func{Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Filter(Expr, System.Func{Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Filter(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Filter(Expr, System.Func{Expr, Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">Lambda expression created by <see cref="Lambda(Expr, Expr)"/>. It must return a boolean value.</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Filter(Arr(1, 2, 3, 4), Lambda("i", GT(Var("i"), 2))));
        ///
        /// Assert.AreEqual(Arr(3, 4), result);
        /// </code>
        /// </example>
        public static Expr Filter(Expr collection, Expr lambda) =>
            UnescapedObject.With("filter", lambda, "collection", collection);

        /// <summary>
        /// Creates a new Take expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="number">Number of elements to take from the head of collection</param>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Take(2, Arr(1, 2, 3)));
        ///
        /// Assert.AreEqual(Arr(1, 2), result);
        /// </code>
        /// </example>
        public static Expr Take(Expr number, Expr collection) =>
            UnescapedObject.With("take", number, "collection", collection);

        /// <summary>
        /// Creates a new Drop expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="number">Number of elements to drop from the head of collection</param>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Drop(2, Arr(1, 2, 3)));
        ///
        /// Assert.AreEqual(Arr(3), result);
        /// </code>
        /// </example>
        public static Expr Drop(Expr number, Expr collection) =>
            UnescapedObject.With("drop", number, "collection", collection);

        /// <summary>
        /// Creates a new Prepend expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="elements">Elements to be prepended into the collection</param>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Prepend(Arr(1, 2, 3), Arr(4, 5, 6)));
        ///
        /// Assert.AreEqual(Arr(1, 2, 3, 4, 5, 6), result);
        /// </code>
        /// </example>
        public static Expr Prepend(Expr elements, Expr collection) =>
            UnescapedObject.With("prepend", elements, "collection", collection);

        /// <summary>
        /// Creates a new Append expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="elements">Elements to be appended into the collection</param>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Append(Arr(4, 5, 6), Arr(1, 2, 3)));
        ///
        /// Assert.AreEqual(Arr(1, 2, 3, 4, 5, 6), result);
        /// </code>
        /// </example>
        public static Expr Append(Expr elements, Expr collection) =>
            UnescapedObject.With("append", elements, "collection", collection);

        /// <summary>
        /// Creates a new IsEmpty expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(IsEmpty(Arr(4, 5, 6)));
        ///
        /// Assert.AreEqual(false, result.To<bool>().Value);
        /// </code>
        /// </example>
        public static Expr IsEmpty(Expr collection) =>
            UnescapedObject.With("is_empty", collection);

        /// <summary>
        /// Creates a new IsNonEmpty expression.
        /// <para>
        /// <see href="https://fauna.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(IsNonEmpty(Arr(4, 5, 6)));
        ///
        /// Assert.AreEqual(true, result.To<bool>().Value);
        /// </code>
        /// </example>
        public static Expr IsNonEmpty(Expr collection) =>
            UnescapedObject.With("is_nonempty", collection);

        /// <summary>
        /// Creates a new Reverse expression.
        /// <para>
        /// <see href="https://docs.fauna.com/fauna/current/api/fql/functions/reverse">FaunaDB Reverse Function</see>
        /// </para>
        /// </summary>
        /// <param name="expr">An expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Reverse(Arr(0, 1, 2, 3)));
        ///
        /// Assert.AreEqual(new int[] { 3, 2, 1, 0 }, result.To<int[]>().Value);
        /// </code>
        /// </example>
        public static Expr Reverse(Expr expr) =>
            UnescapedObject.With("reverse", expr);

        /// <summary>
        /// Evaluates to true if all elements of the collection is true.
        /// <para>
        /// <see href="https://docs.fauna.com/fauna/current/api/fql/functions/all">All function</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(All(Arr(true, true, true)));
        ///
        /// Assert.IsTrue(true, result.To<bool>().Value);
        /// </code>
        /// </example>
        public static Expr All(Expr collection) =>
            UnescapedObject.With("all", collection);

        /// <summary>
        /// Evaluates to true if any element of the collection is true.
        /// <para>
        /// <see href="https://docs.fauna.com/fauna/current/api/fql/functions/any">Any function</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <example>
        /// <code>
        /// var result = await client.Query(Any(Arr(true, false, false)));
        ///
        /// Assert.IsTrue(true, result.To<bool>().Value);
        /// </code>
        /// </example>
        public static Expr Any(Expr collection) =>
            UnescapedObject.With("any", collection);
    }
}
