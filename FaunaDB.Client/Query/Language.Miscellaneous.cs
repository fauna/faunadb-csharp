using FaunaDB.Types;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new NextId expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr NextId() =>
            UnescapedObject.With("next_id", NullV.Instance);

        /// <summary>
        /// Creates a new Database expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Database(Expr db_name, Expr scope = null) =>
            UnescapedObject.With("database", db_name, "scope", scope);

        /// <summary>
        /// Creates a new Index expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Index(Expr index_name, Expr scope = null) =>
            UnescapedObject.With("index", index_name, "scope", scope);

        /// <summary>
        /// Creates a new Class expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Class(Expr class_name, Expr scope = null) =>
            UnescapedObject.With("class", class_name, "scope", scope);

        /// <summary>
        /// Creates a new Function expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Function(Expr function_name, Expr scope = null) =>
            UnescapedObject.With("function", function_name, "scope", scope);

        /// <summary>
        /// Returns an internal reference to classes object. Useful to paginate over all classes of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Classes(Expr scope = null) =>
            UnescapedObject.With("classes", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to indexes object. Useful to paginate over all indexes of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Indexes(Expr scope = null) =>
            UnescapedObject.With("indexes", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to databases object. Useful to paginate over all databases of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Databases(Expr scope = null) =>
            UnescapedObject.With("databases", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to functions object. Useful to paginate over all functions of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Functions(Expr scope = null) =>
            UnescapedObject.With("functions", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to keys object. Useful to paginate over all keys of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Keys(Expr scope = null) =>
            UnescapedObject.With("keys", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to tokens object. Useful to paginate over all tokens of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Tokens(Expr scope = null) =>
            UnescapedObject.With("tokens", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to credentials object. Useful to paginate over all credentials of a given scope database.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Credentials(Expr scope = null) =>
            UnescapedObject.With("credentials", scope ?? Null());

        /// <summary>
        /// Creates a new Equals expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr EqualsFn(params Expr[] values) =>
            UnescapedObject.With("equals", Varargs(values));

        /// <summary>
        /// Creates a new Contains expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Contains(Expr path, Expr @in) =>
            UnescapedObject.With("contains", path, "in", @in);

        /// <summary>
        /// Creates a new Contains expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Contains(PathSelector path, Expr @in) =>
            UnescapedObject.With("contains", path.Segments, "in", @in);

        /// <summary>
        /// Creates a new Select expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr Select(Expr path, Expr @from, Expr @default = null) =>
            UnescapedObject.With("select", path, "from", @from, "default", @default);

        /// <summary>
        /// Creates a new Select expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Select(PathSelector path, Expr @from, Expr @default = null) =>
            UnescapedObject.With("select", path.Segments, "from", @from, "default", @default);

        /// <summary>
        /// Creates a new Add expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Add(params Expr[] values) =>
            UnescapedObject.With("add", Varargs(values));

        /// <summary>
        /// Creates a new Multiply expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Multiply(params Expr[] values) =>
            UnescapedObject.With("multiply", Varargs(values));

        /// <summary>
        /// Creates a new Subtract expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Subtract(params Expr[] values) =>
            UnescapedObject.With("subtract", Varargs(values));

        /// <summary>
        /// Creates a new Divide expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Divide(params Expr[] values) =>
            UnescapedObject.With("divide", Varargs(values));

        /// <summary>
        /// Creates a new Modulo expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Modulo(params Expr[] values) =>
            UnescapedObject.With("modulo", Varargs(values));

        /// <summary>
        /// Creates a new LT expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr LT(params Expr[] values) =>
            UnescapedObject.With("lt", Varargs(values));

        /// <summary>
        /// Creates a new LTE expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr LTE(params Expr[] values) =>
            UnescapedObject.With("lte", Varargs(values));

        /// <summary>
        /// Creates a new GT expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr GT(params Expr[] values) =>
            UnescapedObject.With("gt", Varargs(values));

        /// <summary>
        /// Creates a new GTE expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr GTE(params Expr[] values) =>
            UnescapedObject.With("gte", Varargs(values));

        /// <summary>
        /// Creates a new And expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr And(params Expr[] values) =>
            UnescapedObject.With("and", Varargs(values));

        /// <summary>
        /// Creates a new Or expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Or(params Expr[] values) =>
            UnescapedObject.With("or", Varargs(values));

        /// <summary>
        /// Creates a new Not expression.
        /// <para>
        /// See the <see href="https://fauna.com/documentation/queries#misc_functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Not(Expr boolean) =>
            UnescapedObject.With("not", boolean);
    }
}
