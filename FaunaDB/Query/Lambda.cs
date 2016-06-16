using FaunaDB.Values;
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
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static Expr Lambda(Expr vars, Expr expr) =>
            Q("lambda", vars, "expr", expr);

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
                Array(p0, p1),
                lambda(Var(p0), Var(p1)));
        }

        public static Expr Lambda(Func<Expr, Expr, Expr, Expr> lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Lambda(
                Array(p0, p1, p2),
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
                Array(p0, p1, p2, p3),
                lambda(Var(p0), Var(p1), Var(p2), Var(p3)));
        }

        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);
        public static Expr Lambda(Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            LambdaDelegate(lambda);

        private static Expr LambdaDelegate(Delegate lambda)
        {
            ParameterInfo[] info = lambda.Method.GetParameters();

            Expr vars = ArrayV.FromEnumerable(from v in info select new StringV(v.Name));
            Expr expr = (Expr)lambda.DynamicInvoke((from v in info select Var(v.Name)).ToArray());

            return Lambda(vars, expr);
        }
    }
}
