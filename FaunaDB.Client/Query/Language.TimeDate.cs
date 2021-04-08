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
            Day,
            HalfDay,
            Hour,
            Minute,
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
        
        /// <summary>
        /// Returns a new time or date with the offset in terms of the unit added.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/timeadd">TimeAdd</see>
        /// </para>
        /// </summary>
        public static Expr TimeAdd(Expr value, Expr offset, Expr unit) =>
            UnescapedObject.With("time_add", value, "offset", offset, "unit", unit);

        /// <summary>
        /// Returns a new time or date with the offset in terms of the unit subtracted.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/timesubtract">TimeSubtract</see>
        /// </para>
        /// </summary>
        public static Expr TimeSubtract(Expr value, Expr offset, Expr unit) =>
            UnescapedObject.With("time_subtract", value, "offset", offset, "unit", unit);

        /// <summary>
        /// Returns the number of intervals in terms of the unit between
        /// two times or dates. Both start and finish must be of the same type.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/timediff">TimeDiff</see>
        /// </para>
        /// </summary>
        public static Expr TimeDiff(Expr start, Expr finish, Expr unit) =>
            UnescapedObject.With("time_diff", start, "other", finish, "unit", unit);
    }
}
