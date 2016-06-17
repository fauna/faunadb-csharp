using FaunaDB.Types;
using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct LetBinding<TFunc>
        {
            private ObjectV vars;
            private Func<TFunc, Expr> invoker;

            public LetBinding(ObjectV vars, Func<TFunc, Expr> invoker)
            {
                this.vars = vars;
                this.invoker = invoker;
            }

            public Expr In(Expr @in) =>
                Let(vars, @in);

            public Expr In(TFunc @in) =>
                Let(vars, invoker(@in));
        }

        public static LetBinding<Func<Expr, Expr>> Let(string k0, Expr v0) =>
            new LetBinding<Func<Expr, Expr>>(Q(k0, v0), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);

                return fn(a0);
            });

        public static LetBinding<Func<Expr, Expr, Expr>> Let(string k0, Expr v0, string k1, Expr v1) =>
            new LetBinding<Func<Expr, Expr, Expr>>(Q(k0, v0, k1, v1), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);

                return fn(a0, a1);
            });

        public static LetBinding<Func<Expr, Expr, Expr, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2) =>
            new LetBinding<Func<Expr, Expr, Expr, Expr>>(Q(k0, v0, k1, v1, k2, v2), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);

                return fn(a0, a1, a2);
            });

        public static LetBinding<Func<Expr, Expr, Expr, Expr, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3) =>
            new LetBinding<Func<Expr, Expr, Expr, Expr, Expr>>(Q(k0, v0, k1, v1, k2, v2, k3, v3), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);

                return fn(a0, a1, a2, a3);
            });

        public static LetBinding<Func<Expr, Expr, Expr, Expr, Expr, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4) =>
            new LetBinding<Func<Expr, Expr, Expr, Expr, Expr, Expr>>(Q(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);
                var a4 = Var(info[4].Name);

                return fn(a0, a1, a2, a3, a4);
            });

        public static LetBinding<Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr>> Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5) =>
            new LetBinding<Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr>>(Q(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4, k5, v5), fn => {
                ParameterInfo[] info = fn.Method.GetParameters();

                var a0 = Var(info[0].Name);
                var a1 = Var(info[1].Name);
                var a2 = Var(info[2].Name);
                var a3 = Var(info[3].Name);
                var a4 = Var(info[4].Name);
                var a5 = Var(info[5].Name);

                return fn(a0, a1, a2, a3, a4, a5);
            });
    }
}
