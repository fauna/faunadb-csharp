namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Time and Date

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Time(Expr time) =>
            UnescapedObject.With("time", time);

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
        #endregion
    }
}
