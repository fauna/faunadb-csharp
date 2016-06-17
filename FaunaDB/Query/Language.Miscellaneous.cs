using FaunaDB.Types;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Miscellaneous Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr NextId() =>
            UnescapedObject.With("next_id", NullV.Instance);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr EqualsFn(Expr first, params Expr[] tail) =>
            UnescapedObject.With("equals", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Contains(Expr path, Expr @in) =>
            UnescapedObject.With("contains", path, "in", @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Select(Expr path, Expr @from, Expr @default = null) =>
            UnescapedObject.With("select", path, "from", @from, "default", @default);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Add(Expr first, params Expr[] tail) =>
            UnescapedObject.With("add", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Multiply(Expr first, params Expr[] tail) =>
            UnescapedObject.With("multiply", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Subtract(Expr first, params Expr[] tail) =>
            UnescapedObject.With("subtract", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Divide(Expr first, params Expr[] tail) =>
            UnescapedObject.With("divide", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Modulo(Expr first, params Expr[] tail) =>
            UnescapedObject.With("modulo", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr LT(Expr first, params Expr[] tail) =>
            UnescapedObject.With("lt", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr LTE(Expr first, params Expr[] tail) =>
            UnescapedObject.With("lte", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr GT(Expr first, params Expr[] tail) =>
            UnescapedObject.With("gt", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr GTE(Expr first, params Expr[] tail) =>
            UnescapedObject.With("gte", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr And(Expr first, params Expr[] tail) =>
            UnescapedObject.With("and", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Or(Expr first, params Expr[] tail) =>
            UnescapedObject.With("or", Varargs(first, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Not(Expr boolean) =>
            UnescapedObject.With("not", boolean);
        #endregion
    }
}
