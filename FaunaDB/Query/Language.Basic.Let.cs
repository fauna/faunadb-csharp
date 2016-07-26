using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public struct LetBinding
        {
            UnescapedObject vars;

            internal LetBinding(UnescapedObject vars)
            {
                this.vars = vars;
            }

            public Expr In(Expr @in) =>
                Let(vars, @in);
        }

        public static LetBinding Let(string k0, Expr v0) =>
            new LetBinding(UnescapedObject.With(k0, v0));

        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1) =>
            new LetBinding(UnescapedObject.With(k0, v0, k1, v1));

        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2) =>
            new LetBinding(UnescapedObject.With(k0, v0, k1, v1, k2, v2));

        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3) =>
            new LetBinding(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3));

        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4) =>
            new LetBinding(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4));

        public static LetBinding Let(string k0, Expr v0, string k1, Expr v1, string k2, Expr v2, string k3, Expr v3, string k4, Expr v4, string k5, Expr v5) =>
            new LetBinding(UnescapedObject.With(k0, v0, k1, v1, k2, v2, k3, v3, k4, v4, k5, v5));
    }
}
