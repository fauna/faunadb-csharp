using FaunaDB.Types;
using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct LetBinding<TFunc>
        {
            private UnescapedObject vars;
            private Func<TFunc, Expr> invoker;

            public LetBinding(UnescapedObject vars, Func<TFunc, Expr> invoker)
            {
                this.vars = vars;
                this.invoker = invoker;
            }

            public Expr In(Expr @in) =>
                Let(vars, @in);

            public Expr In(TFunc @in) =>
                Let(vars, invoker(@in));
        }

        public static LetBinding<Func<Var, Expr>> Let(string k0, Expr v0) =>
            new LetBinding<Func<Var, Expr>>(UnescapedObject.With(k0, v0), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);

                return fn(a0);
            });

        public static LetBinding<Func<Var, Var, Expr>> Let(string k0, Expr v0, string k1, Expr v1) =>
            new LetBinding<Func<Var, Var, Expr>>(UnescapedObject.With(k0, v0, k1, v1), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);

                return fn(a0, a1);
            });

        public static LetBinding<Func<Var, Var, Var, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2) =>
            new LetBinding<Func<Var, Var, Var, Expr>>(UnescapedObject.With(k0, v0, k1, v1, k2, v2), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);

                return fn(a0, a1, a2);
            });

        public static LetBinding<Func<Var, Var, Var, Var, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3) =>
            new LetBinding<Func<Var, Var, Var, Var, Expr>>(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);

                return fn(a0, a1, a2, a3);
            });

        public static LetBinding<Func<Var, Var, Var, Var, Var, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4) =>
            new LetBinding<Func<Var, Var, Var, Var, Var, Expr>>(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);
                var a4 = Var(info[4].Name);

                return fn(a0, a1, a2, a3, a4);
            });

        public static LetBinding<Func<Var, Var, Var, Var, Var, Var, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5) =>
            new LetBinding<Func<Var, Var, Var, Var, Var, Var, Expr>>(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4, k5, v5), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);
                var a4 = Var(info[4].Name);
                var a5 = Var(info[5].Name);

                return fn(a0, a1, a2, a3, a4, a5);
            });

        #region Inline functions

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        /// <example>
        /// <code>
        /// <c>Language.Let(1, a => a)</c> is equivalent to <c>Language.Let(new ObjectV("a", 1), Language.Var("a"))</c>
        /// </code>
        /// </example>
        public static Expr Let(Expr v0, Func<Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;

            return Let(
                UnescapedObject.With(p0, v0),
                In(Var(p0)));
        }

        public static Expr Let(Expr v0, Expr v1, Func<Var, Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1),
                In(Var(p0), Var(p1)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Func<Var, Var, Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2),
                In(Var(p0), Var(p1), Var(p2)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Func<Var, Var, Var, Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3),
                In(Var(p0), Var(p1), Var(p2), Var(p3)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Expr v4, Func<Var, Var, Var, Var, Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3, p4, v4),
                In(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Expr v4, Expr v5, Func<Var, Var, Var, Var, Var, Var, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;
            string p5 = info[5].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3, p4, v4, p5, v5),
                In(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4), Var(p5)));
        }
        #endregion
    }
}
