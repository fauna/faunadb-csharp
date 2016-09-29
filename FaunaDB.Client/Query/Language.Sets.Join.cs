using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
       public static Expr Join(Expr source, Func<Expr, Expr> target) =>
            Join(source, Lambda(target));

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Join(Expr source, Func<Expr, Expr, Expr> target) =>
            Join(source, Lambda(target));

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Join(Expr source, Func<Expr, Expr, Expr, Expr> target) =>
            Join(source, Lambda(target));

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Join(Expr source, Func<Expr, Expr, Expr, Expr, Expr> target) =>
            Join(source, Lambda(target));

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Join(Expr source, Func<Expr, Expr, Expr, Expr, Expr, Expr> target) =>
            Join(source, Lambda(target));

        /// <summary>
        /// Creates a new Join expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#sets">FaunaDB Set Functions</see>
        /// </para>
        /// </summary>
        public static Expr Join(Expr source, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> target) =>
            Join(source, Lambda(target));
    }
}
