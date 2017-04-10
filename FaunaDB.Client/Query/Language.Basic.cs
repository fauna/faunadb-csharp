using System.Collections.Generic;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Call expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Call(Expr @ref, params Expr[] arguments) =>
            UnescapedObject.With("call", @ref, "arguments", Varargs(arguments));

        /// <summary>
        /// Creates a new Query expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Query(Expr lambda) =>
            UnescapedObject.With("query", lambda);

        /// <summary>
        /// Creates a new At expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr At(Expr timestamp, Expr expr) =>
            UnescapedObject.With("at", timestamp, "expr", expr);

        /// <summary>
        /// Creates a new Let expression with the provided bindings.
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Let(Expr vars, Expr @in) =>
            UnescapedObject.With("let", vars, "in", @in);

        /// <summary>
        /// Creates a new Let expression wrapping the provided map of bindings.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Let(IReadOnlyDictionary<string, Expr> vars, Expr @in) =>
            Let(new UnescapedObject(vars), @in);

        /// <summary>
        /// Creates a new Var expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Var(string varName) =>
            UnescapedObject.With("var", varName);

        /// <summary>
        /// Creates a new If expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr If(Expr @if, Expr @then, Expr @else) =>
            UnescapedObject.With("if", @if, "then", @then, "else", @else);

        /// <summary>
        /// Creates a new Do expression containing the provided expressions.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        public static Expr Do(params Expr[] expressions) =>
            UnescapedObject.With("do", Varargs(expressions));

        /// <summary>
        /// Creates a new Lambda expression.
        /// <para>
        /// This is the raw version. Usually it's easier to use the overload.
        /// </para>
        /// <para>
        /// See <see cref="Lambda(System.Func{Expr, Expr})"/>,
        /// <see cref="Lambda(System.Func{Expr, Expr, Expr})"/>,
        /// <see cref="Lambda(System.Func{Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Lambda(System.Func{Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Lambda(System.Func{Expr, Expr, Expr, Expr, Expr, Expr})"/>,
        /// <see cref="Lambda(System.Func{Expr, Expr, Expr, Expr, Expr, Expr, Expr})"/>
        /// </para>
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <param name="vars">Variable names. Can be a single string or an array of strings</param>
        /// <param name="expr">Any composed expression created by <see cref="Language"/></param>
        /// <example>
        /// <code>
        /// var lambda1 = Lambda("a", Add(Var("a"), 1);
        /// var lambda2 = Lambda(Arr("a", "b"), Add(Var("a"), Var("b"));
        /// </code>
        /// </example>
        public static Expr Lambda(Expr vars, Expr expr) =>
            UnescapedObject.With("lambda", vars, "expr", expr);
    }
}
