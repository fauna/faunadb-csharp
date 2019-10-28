using System;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Create expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Create(Expr classRef, Expr @params) =>
            UnescapedObject.With("create", classRef, "params", @params);

        /// <summary>
        /// Creates a new Update expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Update(Expr @ref, Expr @params) =>
            UnescapedObject.With("update", @ref, "params", @params);

        /// <summary>
        /// Creates a new Replace expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Replace(Expr @ref, Expr @params) =>
            UnescapedObject.With("replace", @ref, "params", @params);

        /// <summary>
        /// Creates a new Delete expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Delete(Expr @ref) =>
            UnescapedObject.With("delete", @ref);

        /// <summary>
        /// Possible actions for functions <see cref="Insert(Expr, Expr, ActionType, Expr)"/> and <see cref="Remove(Expr, Expr, ActionType)"/>.
        /// </summary>
        public enum ActionType
        {
            Create,
            Delete
        }

        /// <summary>
        /// Creates a new Insert expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, ActionType action, Expr @params) =>
            Insert(@ref, ts, (Expr)action, @params);

        /// <summary>
        /// Creates a new Insert expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, Expr action, Expr @params) =>
            UnescapedObject.With("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// Creates a new Remove expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, ActionType action) =>
            Remove(@ref, ts, (Expr)action);

        /// <summary>
        /// Creates a new Remove expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, Expr action) =>
            UnescapedObject.With("remove", @ref, "ts", ts, "action", action);

        /// <summary>
        /// Creates a new CreateClass expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        [Obsolete("CreateClass is deprecated, please use CreateCollection instead.")]
        public static Expr CreateClass(Expr class_params) =>
            UnescapedObject.With("create_class", class_params);

        /// <summary>
        /// Creates a new CreateCollection expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/createcollection">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateCollection(Expr coll_params) =>
            UnescapedObject.With("create_collection", coll_params);

        /// <summary>
        /// Creates a new CreateDatabase expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateDatabase(Expr db_params) =>
            UnescapedObject.With("create_database", db_params);

        /// <summary>
        /// Creates a new CreateIndex expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateIndex(Expr index_params) =>
            UnescapedObject.With("create_index", index_params);

        /// <summary>
        /// Creates a new CreateKey expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateKey(Expr key_params) =>
            UnescapedObject.With("create_key", key_params);

        /// <summary>
        /// Creates a new CreateFunction expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateFunction(Expr function_params) =>
            UnescapedObject.With("create_function", function_params);

        /// <summary>
        /// Creates a new CreateRole expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">FaunaDB Write Functions</see>.
        /// </para>
        /// </summary>
        public static Expr CreateRole(Expr role_params) =>
            UnescapedObject.With("create_role", role_params);

        /// <summary>
        /// Moves a database to a new hierarchy.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/movedatabase">FaunaDB MoveDatabase Function</see>.
        /// </para>
        /// </summary>
        public static Expr MoveDatabase(Expr from, Expr to) =>
            UnescapedObject.With("move_database", from, "to", to);
    }
}
