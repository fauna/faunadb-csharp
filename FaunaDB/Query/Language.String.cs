namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>.
        /// </summary>
        public static Expr Concat(Expr strings, Expr separator = null) =>
            UnescapedObject.With("concat", strings, "separator", separator);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>.
        /// </summary>
        public static Expr Casefold(Expr @string) =>
            UnescapedObject.With("casefold", @string);
    }
}
