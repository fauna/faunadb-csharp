﻿using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives one argument.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives one argument</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives two arguments.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives two arguments</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives three arguments.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives three arguments</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives four arguments.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives four arguments</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives five arguments.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives five arguments</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// Creates a new Foreach expression with a lambda function that receives six arguments.
        /// <para>
        /// <see href="https://faunadb.com/documentation/queries#collection_functions">FaunaDB Collection Functions</see>
        /// </para>
        /// </summary>
        /// <param name="collection">A collection expression</param>
        /// <param name="lambda">A lambda function that receives six arguments</param>
        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));
    }
}
