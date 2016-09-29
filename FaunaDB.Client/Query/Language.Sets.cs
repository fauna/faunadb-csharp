using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Match expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Match(Expr index, params Expr[] terms) =>
            UnescapedObject.With("match", index, "terms", terms.Length == 0 ? null : Varargs(terms));

        /// <summary>
        /// Creates a new Union expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Union(params Expr[] values) =>
            UnescapedObject.With("union", Varargs(values));

        /// <summary>
        /// Creates a new Intersection expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
       public static Expr Intersection(params Expr[] values) =>
            UnescapedObject.With("intersection", Varargs(values));

        /// <summary>
        /// Creates a new Difference expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Difference(params Expr[] values) =>
            UnescapedObject.With("difference", Varargs(values));

        /// <summary>
        /// Creates a new Distinct expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Distinct(Expr set) =>
            UnescapedObject.With("distinct", set);

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// </summary>
        /// <param name="source">A set resulting from one of the Set Functions</param>
        /// <param name="target">Can be either an index reference or a lambda function</param>
       public static Expr Join(Expr source, Expr target) =>
            UnescapedObject.With("join", source, "with", target);

    }
}
