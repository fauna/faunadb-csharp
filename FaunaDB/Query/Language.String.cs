namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region String Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr Concat(Expr strings) =>
            UnescapedObject.With("concat", strings);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr Concat(Expr strings, Expr separator) =>
            UnescapedObject.With("concat", strings, "separator", separator);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr CaseFold(Expr @string) =>
            UnescapedObject.With("casefold", @string);
        #endregion
    }
}
