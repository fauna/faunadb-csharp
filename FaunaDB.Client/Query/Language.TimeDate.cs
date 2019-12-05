namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Time expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public static Expr Time(Expr time) =>
            UnescapedObject.With("time", time);

        /// <summary>
        /// Possible time units accepted by <see cref="Epoch(Expr, TimeUnit)"/>.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public enum TimeUnit
        {
            Second,
            Millisecond,
            Microsecond,
            Nanosecond
        }

        /// <summary>
        /// Creates a new Epoch expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public static Expr Epoch(Expr number, TimeUnit unit) =>
            Epoch(number, (Expr)unit);

        /// <summary>
        /// Creates a new Epoch expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public static Expr Epoch(Expr number, Expr unit) =>
            UnescapedObject.With("epoch", number, "unit", unit);

        /// <summary>
        /// Creates a new Date expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public static Expr Date(Expr date) =>
            UnescapedObject.With("date", date);

        /// <summary>
        /// Constructs a Timestamp representing the transaction’s start time.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/now">FaunaDB Now Functions</see>
        /// </para>
        /// </summary>
        public static Expr Now() =>
            UnescapedObject.With("now", Null());
    }
}
