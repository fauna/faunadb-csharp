namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new Concat expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr Concat(Expr strings, Expr separator = null) =>
            UnescapedObject.With("concat", strings, "separator", separator);

        /// <summary>
        /// Possible normalizer values accepted by <see cref="Casefold(Expr, Normalizer)"/>.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#time_functions">FaunaDB Time and Date Functions</see>
        /// </para>
        /// </summary>
        public enum Normalizer
        {
            NFD,
            NFC,
            NFKD,
            NFKC,
            NFKCCaseFold
        }

        /// <summary>
        /// Creates a new Casefold expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr Casefold(Expr @string, Normalizer normalizer) =>
            Casefold(@string, (Expr)normalizer);

        /// <summary>
        /// Creates a new Casefold expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr Casefold(Expr @string, Expr normalizer = null) =>
            UnescapedObject.With("casefold", @string, "normalizer", normalizer);

        /// <summary>
        /// Creates a new NGram expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#string_functions">FaunaDB String Functions</see>
        /// </para>
        /// </summary>
        public static Expr NGram(Expr terms, Expr min = null, Expr max = null) =>
            UnescapedObject.With("ngram", terms, "min", min, "max", max);
    }
}
