using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        public static Expr Foreach(Expr collection, Func<Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));
    }
}
