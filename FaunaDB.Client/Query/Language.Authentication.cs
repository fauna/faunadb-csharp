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
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/identify">docs</see>.
        /// </para>
        /// </summary>
        /// <param name="ref">Reference to the object</param>
        /// <param name="password">Password to be validated</param>
        public static Expr Identify(Expr @ref, Expr password) =>
            UnescapedObject.With("identify", @ref, "password", password);

        /// <summary>
        /// Creates a new Identity expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/identity">docs</see>.
        /// </para>
        /// </summary>
        public static Expr Identity() =>
            UnescapedObject.With("identity", Null());
        
        /// <summary>
        /// Creates a new CurrentIdentity expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/currentidentity">docs</see>.
        /// </para>
        /// </summary>
        public static Expr CurrentIdentity() =>
            UnescapedObject.With("current_identity", Null());

        /// <summary>
        /// Creates a new HasIdentity expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/hasidentity">docs</see>.
        /// </para>
        /// </summary>
        public static Expr HasIdentity() =>
            UnescapedObject.With("has_identity", Null());
        
        /// <summary>
        /// Creates a new HasCurrentIdentity expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/hascurrentidentity">docs</see>.
        /// </para>
        /// </summary>
        public static Expr HasCurrentIdentity() =>
            UnescapedObject.With("has_current_identity", Null());
        
        /// <summary>
        /// Creates a new CurrentToken expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/currenttoken">docs</see>.
        /// </para>
        /// </summary>
        public static Expr CurrentToken() =>
            UnescapedObject.With("current_token", Null());
        
        /// <summary>
        /// Creates a new HasCurrentToken expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/hascurrenttoken">docs</see>.
        /// </para>
        /// </summary>
        public static Expr HasCurrentToken() =>
            UnescapedObject.With("has_current_token", Null());
        
        /// <summary>
        /// Creates a new AccessProviders expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/accessprovider">docs</see>.
        /// </para>
        /// </summary>
        public static Expr AccessProviders(Expr scope = null) =>
            UnescapedObject.With("access_providers", scope ?? Null());
        
        /// <summary>
        /// Creates a new AccessProvider expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/accessprovider">docs</see>.
        /// </para>
        /// </summary>
        public static Expr AccessProvider(Expr name) =>
            UnescapedObject.With("access_provider", name);
        
        /// <summary>
        /// Creates a new AccessProvider expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/accessprovider">docs</see>.
        /// </para>
        /// </summary>
        public static Expr AccessProvider(Expr name, Expr scope) =>
            UnescapedObject.With("access_provider", name, "scope", scope);
        
        /// <summary>
        /// Creates a new CreateAccessProvider expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#write-functions">docs</see>.
        /// </para>
        /// </summary>
        public static Expr CreateAccessProvider(Expr expr) =>
            UnescapedObject.With("create_access_provider", expr);
    }
}
