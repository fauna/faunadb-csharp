using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Use a lambda expression to tersely define a Lambda query.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Lambda(a => a)</c> is equivalent to <c>Query.Lambda("auto0", Query.Var("auto0"))</c>.
        /// </example>
        public static Expr Lambda(Func<Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;

            return Lambda(p0, lambda(Var(p0)));
        }

        public static Expr Lambda(Func<Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Lambda(
                Arr(p0, p1),
                lambda(Var(p0), Var(p1)));
        }

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
        #endregion
    }
}
