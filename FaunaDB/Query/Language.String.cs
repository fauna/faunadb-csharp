namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Concat expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr Concat(Expr strings, Expr separator = null) =>
            UnescapedObject.With("concat", strings, "separator", separator);

        /// <summary>
        /// Creates a new CaseFold expression.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr Casefold(Expr @string) =>
            UnescapedObject.With("casefold", @string);
    }
}
