namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Read Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Get(Expr @ref, Expr ts = null) =>
            ts == null ? UnescapedObject.With("get", @ref) : UnescapedObject.With("get", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Paginate(
            Expr set,
            Expr ts = null,
            Expr after = null,
            Expr before = null,
            Expr size = null,
            Expr events = null,
            Expr sources = null) =>
                UnescapedObject.With(
                    "paginate", set,
                    "ts", ts,
                    "after", after,
                    "before", before,
                    "size", size,
                    "events", events,
                    "sources", sources);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Exists(Expr @ref, Expr ts = null) =>
            ts == null ? UnescapedObject.With("exists", @ref) : UnescapedObject.With("exists", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Count(Expr set, Expr events = null) =>
            events == null ? UnescapedObject.With("count", set) : UnescapedObject.With("count", set, "events", events);
        #endregion
    }
}
