namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Login expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#auth_functions">docs</see>.
        /// </para>
        /// </summary>
        public static Expr Login(Expr @ref, Expr @params) =>
            UnescapedObject.With("login", @ref, "params", @params);

        /// <summary>
        /// Creates a new Logout expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#auth_functions">docs</see>.
        /// </para>
        /// </summary>
        /// <param name="deleteTokens"><see cref="Expr"/> object where, True will delete all tokens associated with the current session. False will delete only the token used in this request</param>
        public static Expr Logout(Expr deleteTokens) =>
            UnescapedObject.With("logout", deleteTokens);

        /// <summary>
        /// Creates a new Logout expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#auth_functions">docs</see>.
        /// </para>
        /// </summary>
        /// <param name="deleteTokens">True will delete all tokens associated with the current session. False will delete only the token used in this request</param>
        public static Expr Logout(bool deleteTokens) =>
            UnescapedObject.With("logout", deleteTokens);

        /// <summary>
        /// Creates a new Identify expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#auth_functions">docs</see>.
        /// </para>
        /// </summary>
        /// <param name="ref">Reference to the object</param>
        /// <param name="password">Password to be validated</param>
        public static Expr Identify(Expr @ref, Expr password) =>
            UnescapedObject.With("identify", @ref, "password", password);
    }
}
