using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Sets
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Match(Expr index, params Expr[] terms) =>
            UnescapedObject.With("match", index, "terms", terms.Length == 0 ? null : Varargs(terms));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Union(Expr head, params Expr[] tail) =>
            UnescapedObject.With("union", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Intersection(Expr head, params Expr[] tail) =>
            UnescapedObject.With("intersection", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Difference(Expr head, params Expr[] tail) =>
            UnescapedObject.With("difference", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Distinct(Expr set) =>
            UnescapedObject.With("distinct", set);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Join(Expr source, Expr target) =>
            UnescapedObject.With("join", source, "with", target);

        public static Expr Join(Expr source, Func<Expr, Expr> target) =>
            Join(source, Lambda(target));
        #endregion
    }
}
