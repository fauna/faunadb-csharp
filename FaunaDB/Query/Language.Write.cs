namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Create expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Create(Expr classRef, Expr @params) =>
            UnescapedObject.With("create", classRef, "params", @params);

        /// <summary>
        /// Creates a new Update expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Update(Expr @ref, Expr @params) =>
            UnescapedObject.With("update", @ref, "params", @params);

        /// <summary>
        /// Creates a new Replace expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Replace(Expr @ref, Expr @params) =>
            UnescapedObject.With("replace", @ref, "params", @params);

        /// <summary>
        /// Creates a new Delete expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Delete(Expr @ref) =>
            UnescapedObject.With("delete", @ref);

        /// <summary>
        /// Possible actions for functions <see cref="Insert(Expr, Expr, ActionType, Expr)"/> and <see cref="Remove(Expr, Expr, ActionType)"/>.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/architecture#temporal">Temporal Data Model</see>.
        /// </para>
        /// </summary>
        public enum ActionType
        {
            Create,
            Delete
        }

        /// <summary>
        /// Creates a new Insert expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, ActionType action, Expr @params) =>
            Insert(@ref, ts, (Expr)action, @params);

        /// <summary>
        /// Creates a new Insert expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, Expr action, Expr @params) =>
            UnescapedObject.With("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// Creates a new Remove expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, ActionType action) =>
            Remove(@ref, ts, (Expr)action);

        /// <summary>
        /// Creates a new Remove expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, Expr action) =>
            UnescapedObject.With("remove", @ref, "ts", ts, "action", action);
    }
}
