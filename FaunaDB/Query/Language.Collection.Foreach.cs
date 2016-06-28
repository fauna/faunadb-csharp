using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public static Expr Foreach(Expr collection, Func<Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Var, Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Var, Var, Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Var, Var, Var, Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Var, Var, Var, Var, Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Var, Var, Var, Var, Var, Var, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));
    }
}
