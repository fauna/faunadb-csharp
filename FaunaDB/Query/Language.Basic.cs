using System.Collections.Generic;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Basic Forms
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static Expr Let(Expr vars, Expr @in) =>
            UnescapedObject.With("let", vars, "in", @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        public static Expr Let(IReadOnlyDictionary<string, Expr> vars, Expr @in) =>
            Let(new UnescapedObject(vars), @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        public static Expr Var(string varName) =>
            UnescapedObject.With("var", varName);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        public static Expr If(Expr @if, Expr @then, Expr @else) =>
            UnescapedObject.With("if", @if, "then", @then, "else", @else);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        public static Expr Do(params Expr[] expressions) =>
            UnescapedObject.With("do", Varargs(expressions));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static Expr Lambda(Expr vars, Expr expr) =>
            UnescapedObject.With("lambda", vars, "expr", expr);
        #endregion
    }
}
