using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public static Expr Filter(Expr collection, Func<Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Var, Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Var, Var, Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Var, Var, Var, Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Var, Var, Var, Var, Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Var, Var, Var, Var, Var, Var, Expr> lambda) =>
            Filter(collection, Lambda(lambda));
    }
}
