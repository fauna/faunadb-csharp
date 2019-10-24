using System;
using FaunaDB.Types;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new NextId expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        [Obsolete("Use NewId() instead")]
        public static Expr NextId() =>
            UnescapedObject.With("next_id", NullV.Instance);

        /// <summary>
        /// Creates a new NewId expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr NewId() =>
            UnescapedObject.With("new_id", NullV.Instance);

        /// <summary>
        /// Creates a new Database expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Database(Expr db_name, Expr scope = null) =>
            UnescapedObject.With("database", db_name, "scope", scope);

        /// <summary>
        /// Creates a new Index expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Index(Expr index_name, Expr scope = null) =>
            UnescapedObject.With("index", index_name, "scope", scope);

        /// <summary>
        /// Creates a new Collection expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/collection">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Collection(Expr coll_name, Expr scope = null) =>
            UnescapedObject.With("collection", coll_name, "scope", scope);

        /// <summary>
        /// Creates a new Class expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        [Obsolete("Class is deprecated, please use Collection instead.")]
        public static Expr Class(Expr class_name, Expr scope = null) =>
            UnescapedObject.With("class", class_name, "scope", scope);

        /// <summary>
        /// Creates a new Function expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Function(Expr function_name, Expr scope = null) =>
            UnescapedObject.With("function", function_name, "scope", scope);

        /// <summary>
        /// Creates a new Role expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Role(Expr role_name, Expr scope = null) =>
            UnescapedObject.With("role", role_name, "scope", scope);

        /// <summary>
        /// Returns an internal reference to collections object. Useful to paginate over all classes of a given scope database.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/collections">FaunaDB Collections</see>
        /// </para>
        /// </summary>
        public static Expr Collections(Expr scope = null) =>
            UnescapedObject.With("collections", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to classes object. Useful to paginate over all classes of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        [Obsolete("Classes is deprecated, please use Collections instead.")]
        public static Expr Classes(Expr scope = null) =>
            UnescapedObject.With("classes", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to indexes object. Useful to paginate over all indexes of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Indexes(Expr scope = null) =>
            UnescapedObject.With("indexes", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to databases object. Useful to paginate over all databases of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Databases(Expr scope = null) =>
            UnescapedObject.With("databases", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to functions object. Useful to paginate over all functions of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Functions(Expr scope = null) =>
            UnescapedObject.With("functions", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to roles object. Useful to paginate over all roles of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Roles(Expr scope = null) =>
            UnescapedObject.With("roles", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to keys object. Useful to paginate over all keys of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Keys(Expr scope = null) =>
            UnescapedObject.With("keys", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to tokens object. Useful to paginate over all tokens of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Tokens(Expr scope = null) =>
            UnescapedObject.With("tokens", scope ?? Null());

        /// <summary>
        /// Returns an internal reference to credentials object. Useful to paginate over all credentials of a given scope database.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Credentials(Expr scope = null) =>
            UnescapedObject.With("credentials", scope ?? Null());

        /// <summary>
        /// Creates a new Equals expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr EqualsFn(params Expr[] values) =>
            UnescapedObject.With("equals", Varargs(values));

        /// <summary>
        /// Creates a new Contains expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Contains(Expr path, Expr @in) =>
            UnescapedObject.With("contains", path, "in", @in);

        /// <summary>
        /// Creates a new Contains expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Contains(PathSelector path, Expr @in) =>
            UnescapedObject.With("contains", path.Segments, "in", @in);

        /// <summary>
        /// Creates a new Select expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr Select(Expr path, Expr @from, Expr @default = null) =>
            UnescapedObject.With("select", path, "from", @from, "default", @default);

        /// <summary>
        /// Creates a new Select expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Select(PathSelector path, Expr @from, Expr @default = null) =>
            UnescapedObject.With("select", path.Segments, "from", @from, "default", @default);

        /// <summary>
        /// Creates a new SelectAll expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr SelectAll(Expr path, Expr @from) =>
             UnescapedObject.With("select_all", path, "from", @from);

        /// <summary>
        /// Creates a new SelectAll expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr SelectAll(PathSelector path, Expr @from) =>
            UnescapedObject.With("select_all", path.Segments, "from", @from);

        /// <summary>
        /// Creates a new Add expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Add(params Expr[] values) =>
            UnescapedObject.With("add", Varargs(values));

        /// <summary>
        /// Creates a new Multiply expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Multiply(params Expr[] values) =>
            UnescapedObject.With("multiply", Varargs(values));

        /// <summary>
        /// Creates a new Subtract expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Subtract(params Expr[] values) =>
            UnescapedObject.With("subtract", Varargs(values));

        /// <summary>
        /// Creates a new Divide expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Divide(params Expr[] values) =>
            UnescapedObject.With("divide", Varargs(values));

        /// <summary>
        /// Creates a new Modulo expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Modulo(params Expr[] values) =>
            UnescapedObject.With("modulo", Varargs(values));

        /// <summary>
        /// Creates a new LT expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr LT(params Expr[] values) =>
            UnescapedObject.With("lt", Varargs(values));

        /// <summary>
        /// Creates a new LTE expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr LTE(params Expr[] values) =>
            UnescapedObject.With("lte", Varargs(values));

        /// <summary>
        /// Creates a new GT expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr GT(params Expr[] values) =>
            UnescapedObject.With("gt", Varargs(values));

        /// <summary>
        /// Creates a new GTE expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
       public static Expr GTE(params Expr[] values) =>
            UnescapedObject.With("gte", Varargs(values));

        /// <summary>
        /// Creates a new And expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr And(params Expr[] values) =>
            UnescapedObject.With("and", Varargs(values));

        /// <summary>
        /// Creates a new Or expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Or(params Expr[] values) =>
            UnescapedObject.With("or", Varargs(values));

        /// <summary>
        /// Creates a new Not expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        public static Expr Not(Expr boolean) =>
            UnescapedObject.With("not", boolean);

        /// <summary>
        ///   Converts an expression to a string literal.
        /// </summary>
        public static Expr ToStringExpr(Expr expr) =>
            UnescapedObject.With("to_string", expr);

        /// <summary>
        ///   Converts an expression to a number literal.
        /// </summary>
        public static Expr ToNumber(Expr expr) =>
            UnescapedObject.With("to_number", expr);

        /// <summary>
        ///   Converts an expresion to a time literal.
        /// </summary>
        public static Expr ToTime(Expr expr) =>
            UnescapedObject.With("to_time", expr);

        /// <summary>
        ///  Converts a time expression to seconds since the UNIX epoch.
        /// </summary>
        public static Expr ToSeconds(Expr expr) =>
            UnescapedObject.With("to_seconds", expr);

        /// <summary>
        ///  Converts a time expression to milliseconds since the UNIX epoch.
        /// </summary>
        public static Expr ToMillis(Expr expr) =>
            UnescapedObject.With("to_millis", expr);

        /// <summary>
        ///  Converts a time expression to microseconds since the UNIX epoch.
        /// </summary>
        public static Expr ToMicros(Expr expr) =>
            UnescapedObject.With("to_micros", expr);

        /// <summary>
        ///  Returns a time expression's day of the year, from 1 to 365, or 366 in a leap year.
        /// </summary>
        public static Expr DayOfYear(Expr expr) =>
            UnescapedObject.With("day_of_year", expr);

        /// <summary>
        ///  Returns a time expression's day of the month, from 1 to 31.
        /// </summary>
        public static Expr DayOfMonth(Expr expr) =>
            UnescapedObject.With("day_of_month", expr);

        /// <summary>
        ///  Returns a time expression's day of the week following ISO-8601 convention, from 1 (Monday) to 7 (Sunday).
        /// </summary>
        public static Expr DayOfWeek(Expr expr) =>
            UnescapedObject.With("day_of_week", expr);

        /// <summary>
        ///  Returns the time expression's year, following the ISO-8601 standard.
        /// </summary>
        public static Expr Year(Expr expr) =>
            UnescapedObject.With("year", expr);

        /// <summary>
        ///  Returns a time expression's month of the year, from 1 to 12.
        /// </summary>
        public static Expr Month(Expr expr) =>
            UnescapedObject.With("month", expr);

        /// <summary>
        ///  Returns a time expression's hour of the day, from 0 to 23.
        /// </summary>
        public static Expr Hour(Expr expr) =>
            UnescapedObject.With("hour", expr);

        /// <summary>
        ///  Returns a time expression's minute of the hour, from 0 to 59.
        /// </summary>
        public static Expr Minute(Expr expr) =>
            UnescapedObject.With("minute", expr);

        /// <summary>
        ///  Returns a time expression's second of the minute, from 0 to 59.
        /// </summary>
        public static Expr Second(Expr expr) =>
            UnescapedObject.With("second", expr);

        /// <summary>
        ///   Converts an expression to a date literal.
        /// </summary>
        public static Expr ToDate(Expr expr) =>
            UnescapedObject.With("to_date", expr);
    }
}
