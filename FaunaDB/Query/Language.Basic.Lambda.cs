using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Use a lambda expression to tersely define a Lambda query.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Lambda(a => a)</c> is equivalent to <c>Query.Lambda("auto0", Query.Var("auto0"))</c>.
        /// </example>
        public static Expr Lambda(Func<Var, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;

            return Lambda(p0, lambda(Var(p0)));
        }

        public static Expr Lambda(Func<Var, Var, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Lambda(
                Arr(p0, p1),
                lambda(Var(p0), Var(p1)));
        }

        public static Expr Lambda(Func<Var, Var, Var, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Lambda(
                Arr(p0, p1, p2),
                lambda(Var(p0), Var(p1), Var(p2)));
        }

        public static Expr Lambda(Func<Var, Var, Var, Var, Expr> lambda)
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

        public static Expr Lambda(Func<Var, Var, Var, Var, Var, Expr> lambda)
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

        public static Expr Lambda(Func<Var, Var, Var, Var, Var, Var, Expr> lambda)
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
