using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public static Expr Filter(Expr collection, Func<Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));
    }
}
