namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>.
        /// </summary>
        public static Expr Time(Expr time) =>
            UnescapedObject.With("time", time);

        public enum TimeUnit
        {
            Second,
            Millisecond,
            Microsecond,
            Nanosecond
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>.
        /// </summary>
        public static Expr Epoch(Expr number, TimeUnit unit) =>
            Epoch(number, (Expr)unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>.
        /// </summary>
        public static Expr Epoch(Expr number, Expr unit) =>
            UnescapedObject.With("epoch", number, "unit", unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>.
        /// </summary>
        public static Expr Date(Expr date) =>
            UnescapedObject.With("date", date);
    }
}
