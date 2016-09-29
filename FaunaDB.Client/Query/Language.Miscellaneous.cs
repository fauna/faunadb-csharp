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
