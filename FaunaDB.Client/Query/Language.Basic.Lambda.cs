using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a lambda expression that receives one argument.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda(a => a);
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda("a", Var("a"));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;

            return Lambda(p0, lambda(Var(p0)));
        }

        /// <summary>
        /// Creates a lambda expression that receives two arguments.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda((a, b) => Add(a, b));
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda(Arr("a", "b"), Add(Var("a"), Var("b")));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Lambda(
                Arr(p0, p1),
                lambda(Var(p0), Var(p1)));
        }

        /// <summary>
        /// Creates a lambda expression that receives three arguments.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda((a, b, c) => Add(a, b, c));
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda(Arr("a", "b", "c"), Add(Var("a"), Var("b"), Var("c")));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Lambda(
                Arr(p0, p1, p2),
                lambda(Var(p0), Var(p1), Var(p2)));
        }

        /// <summary>
        /// Creates a lambda expression that receives four arguments.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda((a, b, c, d) => Add(a, b, c, d));
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda(Arr("a", "b", "c", "d"), Add(Var("a"), Var("b"), Var("c"), Var("d")));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;

            return Lambda(
                Arr(p0, p1, p2, p3),
                lambda(Var(p0), Var(p1), Var(p2), Var(p3)));
        }

        /// <summary>
        /// Creates a lambda expression that receives five arguments.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda((a, b, c, d, e) => Add(a, b, c, d, e));
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda(Arr("a", "b", "c", "d", "e"), Add(Var("a"), Var("b"), Var("c"), Var("d"), Var("e")));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;

            return Lambda(
                Arr(p0, p1, p2, p3, p4),
                lambda(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4)));
        }

        /// <summary>
        /// Creates a lambda expression that receives six arguments.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#basic_forms">FaunaDB Basic Forms</see>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// var lambda = Lambda((a, b, c, d, e, f) => Add(a, b, c, d, e, f));
        /// </code>
        /// is equivalent to
        /// <code>
        /// var lambda = Lambda(Arr("a", "b", "c", "d", "f"), Add(Var("a"), Var("b"), Var("c"), Var("d"), Var("f")));
        /// </code>
        /// </example>
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;
            string p5 = info[5].Name;

            return Lambda(
                Arr(p0, p1, p2, p3, p4, p5),
                lambda(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4), Var(p5)));
        }
    }
}
