namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Authentication
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Login(Expr @ref, Expr @params) =>
            UnescapedObject.With("login", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Logout(Expr deleteTokens) =>
            UnescapedObject.With("logout", deleteTokens);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Identify(Expr @ref, Expr password) =>
            UnescapedObject.With("identify", @ref, "password", password);
        #endregion
    }
}
