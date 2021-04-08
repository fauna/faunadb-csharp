namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Check if the expression is an array.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isarray">IsArray</see>.
        /// </para>
        /// </summary>
        public static Expr IsArray(Expr expr) =>
            UnescapedObject.With("is_array", expr);
        
        /// <summary>
        /// Check if the expression is a boolean.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isboolean">IsBoolean</see>.
        /// </para>
        /// </summary>
        public static Expr IsBoolean(Expr expr) =>
            UnescapedObject.With("is_boolean", expr);
        
        /// <summary>
        /// Check if the expression is a byte array.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isbytes">IsBytes</see>.
        /// </para>
        /// </summary>
        public static Expr IsBytes(Expr expr) =>
            UnescapedObject.With("is_bytes", expr);
        
        /// <summary>
        /// Check if the expression is a collection.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/iscollection">IsCollection</see>.
        /// </para>
        /// </summary>
        public static Expr IsCollection(Expr expr) =>
            UnescapedObject.With("is_collection", expr);
        
        /// <summary>
        /// Check if the expression is a credentials.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/iscredentials">IsCredentials</see>.
        /// </para>
        /// </summary>
        public static Expr IsCredentials(Expr expr) =>
            UnescapedObject.With("is_credentials", expr);
        
        /// <summary>
        /// Check if the expression is a database.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isdatabase">IsDatabase</see>.
        /// </para>
        /// </summary>
        public static Expr IsDatabase(Expr expr) =>
            UnescapedObject.With("is_database", expr);
        
        /// <summary>
        /// Check if the expression is a date.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isdate">IsDate</see>.
        /// </para>
        /// </summary>
        public static Expr IsDate(Expr expr) =>
            UnescapedObject.With("is_date", expr);
        
        /// <summary>
        /// Check if the expression is a document (either a reference or an instance).
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isdoc">IsDoc</see>.
        /// </para>
        /// </summary>
        public static Expr IsDoc(Expr expr) =>
            UnescapedObject.With("is_doc", expr);
        
        /// <summary>
        /// Check if the expression is a double.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isdouble">IsDouble</see>.
        /// </para>
        /// </summary>
        public static Expr IsDouble(Expr expr) =>
            UnescapedObject.With("is_double", expr);
        
        /// <summary>
        /// Check if the expression is a function.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isfunction">IsFunction</see>.
        /// </para>
        /// </summary>
        public static Expr IsFunction(Expr expr) =>
            UnescapedObject.With("is_function", expr);
        
        /// <summary>
        /// Check if the expression is an index.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isindex">IsIndex</see>.
        /// </para>
        /// </summary>
        public static Expr IsIndex(Expr expr) =>
            UnescapedObject.With("is_index", expr);
        
        /// <summary>
        /// Check if the expression is an integer.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isinteger">IsInteger</see>.
        /// </para>
        /// </summary>
        public static Expr IsInteger(Expr expr) =>
            UnescapedObject.With("is_integer", expr);
        
        /// <summary>
        /// Check if the expression is a key.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/iskey">IsKey</see>.
        /// </para>
        /// </summary>
        public static Expr IsKey(Expr expr) =>
            UnescapedObject.With("is_key", expr);
        
        /// <summary>
        /// Check if the expression is a lambda.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/islambda">IsLambda</see>.
        /// </para>
        /// </summary>
        public static Expr IsLambda(Expr expr) =>
            UnescapedObject.With("is_lambda", expr);
        
        /// <summary>
        /// Check if the expression is null.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isnull">IsNull</see>.
        /// </para>
        /// </summary>
        public static Expr IsNull(Expr expr) =>
            UnescapedObject.With("is_null", expr);
        
        /// <summary>
        /// Check if the expression is a number.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isnumber">IsNumber</see>.
        /// </para>
        /// </summary>
        public static Expr IsNumber(Expr expr) =>
            UnescapedObject.With("is_number", expr);
        
        /// <summary>
        /// Check if the expression is an object.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isobject">IsObject</see>.
        /// </para>
        /// </summary>
        public static Expr IsObject(Expr expr) =>
            UnescapedObject.With("is_object", expr);
        
        /// <summary>
        /// Check if the expression is a reference.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isref">IsRef</see>.
        /// </para>
        /// </summary>
        public static Expr IsRef(Expr expr) =>
            UnescapedObject.With("is_ref", expr);
        
        /// <summary>
        /// Check if the expression is a role.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isrole">IsRole</see>.
        /// </para>
        /// </summary>
        public static Expr IsRole(Expr expr) =>
            UnescapedObject.With("is_role", expr);
        
        /// <summary>
        /// Check if the expression is a set.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isset">IsSet</see>.
        /// </para>
        /// </summary>
        public static Expr IsSet(Expr expr) =>
            UnescapedObject.With("is_set", expr);
        
        /// <summary>
        /// Check if the expression is a string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/isstring">IsString</see>.
        /// </para>
        /// </summary>
        public static Expr IsString(Expr expr) =>
            UnescapedObject.With("is_string", expr);
        
        /// <summary>
        /// Check if the expression is a timestamp.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/istimestamp">IsTimestamp</see>.
        /// </para>
        /// </summary>
        public static Expr IsTimestamp(Expr expr) =>
            UnescapedObject.With("is_timestamp", expr);
        
        /// <summary>
        /// Check if the expression is a token.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/istoken">IsToken</see>.
        /// </para>
        /// </summary>
        public static Expr IsToken(Expr expr) =>
            UnescapedObject.With("is_token", expr);
    }
}
