namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Map expression.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
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
    }
}
