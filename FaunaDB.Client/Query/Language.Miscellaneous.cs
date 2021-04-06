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
        [Obsolete("Contains is deprecated, please use ContainsPath instead.")]
        public static Expr Contains(Expr path, Expr @in) =>
            UnescapedObject.With("contains", path, "in", @in);

        /// <summary>
        /// Creates a new ContainsField expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containsfield">FaunaDB ConstainsField Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsField(Expr path, Expr @in) =>
            UnescapedObject.With("contains_field", path, "in", @in);

        /// <summary>
        /// Creates a new ContainsValue expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containsvalue">FaunaDB ConstainsValue Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsValue(Expr value, Expr @in) =>
            UnescapedObject.With("contains_value", value, "in", @in); 

        /// <summary>
        /// Creates a new Contains expression.
        /// <para>
        /// See the <see href="https://app.fauna.com/documentation/reference/queryapi#miscellaneous-functions">FaunaDB Miscellaneous Functions</see>
        /// </para>
        /// </summary>
        [Obsolete("Contains is deprecated, please use ContainsPath instead.")]
        public static Expr Contains(PathSelector path, Expr @in) =>
            UnescapedObject.With("contains", path.Segments, "in", @in);

        /// <summary>
        /// Creates a new ContainsPath expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containspath">FaunaDB ConstainsPath Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsPath(PathSelector path, Expr @in) =>
            UnescapedObject.With("contains_path", path.Segments, "in", @in);

        /// <summary>
        /// Creates a new ContainsPath expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containspath">FaunaDB ConstainsPath Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsPath(Expr expr, Expr @in) =>
            UnescapedObject.With("contains_path", expr, "in", @in);

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

        /// <summary>
        ///   Merges two or more objects
        /// </summary>
        public static Expr Merge(Expr merge, Expr with) =>
            UnescapedObject.With("merge", merge, "with", with);

        /// <summary>
        ///   Merges two or more objects
        /// </summary>
        public static Expr Merge(Expr merge, Expr with, Expr lambda) =>
            UnescapedObject.With("merge", merge, "with", with, "lambda", lambda);

        /// <summary>
        ///   Merges two or more objects
        /// </summary>
        public static Expr Merge(Expr merge, Expr with, Func<Expr, Expr, Expr, Expr> lambda) =>
            UnescapedObject.With("merge", merge, "with", with, "lambda", Lambda(lambda));

        /// <summary>
        /// Creates a new Abs expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/abs">FaunaDB Abs Function</see>
        /// </para>
        /// </summary>
        public static Expr Abs(Expr value) =>
            UnescapedObject.With("abs", value);

        /// <summary>
        /// Creates a new Acos expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/acos">FaunaDB Acos Function</see>
        /// </para>
        /// </summary>
        public static Expr Acos(Expr value) =>
            UnescapedObject.With("acos", value);

        /// <summary>
        /// Creates a new Asin expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/asin">FaunaDB Asin Function</see>
        /// </para>
        /// </summary>
        public static Expr Asin(Expr expr) =>
          UnescapedObject.With("asin", expr);

        /// <summary>
        /// Creates a new Atan expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/atan">FaunaDB Atan Function</see>
        /// </para>
        /// </summary>
        public static Expr Atan(Expr expr) =>
          UnescapedObject.With("atan", expr);

        /// <summary>
        /// Creates a new BitAnd expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/bitand">FaunaDB BitAnd Function</see>
        /// </para>
        /// </summary>
        public static Expr BitAnd(params Expr[] terms) =>
          UnescapedObject.With("bitand", Varargs(terms));

        /// <summary>
        /// Creates a new BitNot expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/bitnot">FaunaDB BitNot Function</see>
        /// </para>
        /// </summary>
        public static Expr BitNot(Expr expr) =>
          UnescapedObject.With("bitnot", expr);

        /// <summary>
        /// Creates a new BitOr expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/bitor">FaunaDB BitOr Function</see>
        /// </para>
        /// </summary>
        public static Expr BitOr(params Expr[] terms) =>
          UnescapedObject.With("bitor", Varargs(terms));

        /// <summary>
        /// Creates a new BitXor expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/bitxor">FaunaDB BitXor Function</see>
        /// </para>
        /// </summary>
        public static Expr BitXor(params Expr[] terms) =>
          UnescapedObject.With("bitxor", Varargs(terms));

        /// <summary>
        /// Creates a new Ceil expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/ceil">FaunaDB Ceil Function</see>
        /// </para>
        /// </summary>
        public static Expr Ceil(Expr expr) =>
          UnescapedObject.With("ceil", expr);

        /// <summary>
        /// Creates a new Cos expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/cos">FaunaDB Cos Function</see>
        /// </para>
        /// </summary>
        public static Expr Cos(Expr expr) =>
          UnescapedObject.With("cos", expr);

        /// <summary>
        /// Creates a new Cosh expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/cosh">FaunaDB Cosh Function</see>
        /// </para>
        /// </summary>
        public static Expr Cosh(Expr expr) =>
          UnescapedObject.With("cosh", expr);

        /// <summary>
        /// Creates a new Degrees expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/degrees">FaunaDB Degrees Function</see>
        /// </para>
        /// </summary>
        public static Expr Degrees(Expr expr) =>
          UnescapedObject.With("degrees", expr);

        /// <summary>
        /// Creates a new Exp expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/exp">FaunaDB Exp Function</see>
        /// </para>
        /// </summary>
        public static Expr Exp(Expr expr) =>
          UnescapedObject.With("exp", expr);

        /// <summary>
        /// Creates a new Floor expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/floor">FaunaDB Floor Function</see>
        /// </para>
        /// </summary>
        public static Expr Floor(Expr expr) =>
          UnescapedObject.With("floor", expr);

        /// <summary>
        /// Creates a new Hypot expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/hypot">FaunaDB Hypot Function</see>
        /// </para>
        /// </summary>
        public static Expr Hypot(Expr value, Expr exp = null) =>
          UnescapedObject.With("hypot", value, "b", exp);

        /// <summary>
        /// Creates a new Ln expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/ln">FaunaDB Ln Function</see>
        /// </para>
        /// </summary>
        public static Expr Ln(Expr expr) =>
          UnescapedObject.With("ln", expr);

        /// <summary>
        /// Creates a new Log expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/log">FaunaDB Log Function</see>
        /// </para>
        /// </summary>
        public static Expr Log(Expr expr) =>
          UnescapedObject.With("log", expr);

        /// <summary>
        /// Creates a new Max expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/max">FaunaDB Max Function</see>
        /// </para>
        /// </summary>
        public static Expr Max(params Expr[] terms) =>
          UnescapedObject.With("max", Varargs(terms));

        /// <summary>
        /// Creates a new Min expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/min">FaunaDB Min Function</see>
        /// </para>
        /// </summary>
        public static Expr Min(params Expr[] terms) =>
          UnescapedObject.With("min", Varargs(terms));

        /// <summary>
        /// Creates a new Pow expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/pow">FaunaDB Pow Function</see>
        /// </para>
        /// </summary>
        public static Expr Pow(Expr value, Expr exp = null) =>
          UnescapedObject.With("pow", value, "exp", exp);

        /// <summary>
        /// Creates a new Radians expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/radians">FaunaDB Radians Function</see>
        /// </para>
        /// </summary>
        public static Expr Radians(Expr expr) =>
          UnescapedObject.With("radians", expr);

        /// <summary>
        /// Creates a new Round expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/round">FaunaDB Round Function</see>
        /// </para>
        /// </summary>
        public static Expr Round(Expr expr, Expr precision = null) =>
          UnescapedObject.With("round", expr, "precision", precision);

        /// <summary>
        /// Creates a new Sign expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/sign">FaunaDB Sign Function</see>
        /// </para>
        /// </summary>
        public static Expr Sign(Expr expr) =>
          UnescapedObject.With("sign", expr);

        /// <summary>
        /// Creates a new Sin expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/sin">FaunaDB Sin Function</see>
        /// </para>
        /// </summary>
        public static Expr Sin(Expr expr) =>
          UnescapedObject.With("sin", expr);

        /// <summary>
        /// Creates a new Sinh expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/sinh">FaunaDB Sinh Function</see>
        /// </para>
        /// </summary>
        public static Expr Sinh(Expr expr) =>
          UnescapedObject.With("sinh", expr);

        /// <summary>
        /// Creates a new Sqrt expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/sqrt">FaunaDB Sqrt Function</see>
        /// </para>
        /// </summary>
        public static Expr Sqrt(Expr expr) =>
          UnescapedObject.With("sqrt", expr);

        /// <summary>
        /// Creates a new Tan expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/tan">FaunaDB Tan Function</see>
        /// </para>
        /// </summary>
        public static Expr Tan(Expr expr) =>
          UnescapedObject.With("tan", expr);

        /// <summary>
        /// Creates a new Tanh expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/tanh">FaunaDB Tanh Function</see>
        /// </para>
        /// </summary>
        public static Expr Tanh(Expr expr) =>
          UnescapedObject.With("tanh", expr);

        /// <summary>
        /// Creates a new Trunc expression.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/trunc">FaunaDB Trunc Function</see>
        /// </para>
        /// </summary>
        public static Expr Trunc(Expr expr, Expr precision = null) =>
          UnescapedObject.With("trunc", expr, "precision", precision);


        /// <summary>
        ///   Returns the offset position of a string in another string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/findstr">FaunaDB FindStr Function</see>
        /// </para>
        /// </summary>
        public static Expr FindStr(Expr expr, Expr find, Expr start = null) =>
            UnescapedObject.With("findstr", expr, "find", find, "start", start);

        /// <summary>
        ///   Returns an array of up to 1024 objects describing where the pattern is found in the search string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/findstrregex">FaunaDB FindStrRegex Function</see>
        /// </para>
        /// </summary>
        public static Expr FindStrRegex(Expr expr, Expr pattern, Expr start = null, Expr numResults = null) =>
            UnescapedObject.With("findstrregex", expr, "pattern", pattern, "start", start, "num_results", numResults);

        /// <summary>
        ///   Returns the number of code points in the string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/length">FaunaDB Length Function</see>
        /// </para>
        /// </summary>
        public static Expr Length(Expr expr) =>
            UnescapedObject.With("length", expr);

        /// <summary>
        ///   Returns a string in which all uppercase characters have been replaced by their corresponding lowercase characters.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/lowercase">FaunaDB LowerCase Function</see>
        /// </para>
        /// </summary>
        public static Expr LowerCase(Expr expr) =>
            UnescapedObject.With("lowercase", expr);

        /// <summary>
        ///   Removes all white spaces, tabs, and new lines from the beginning of a string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/ltrim">FaunaDB Ltrim Function</see>
        /// </para>
        /// </summary>
        public static Expr LTrim(Expr expr) =>
            UnescapedObject.With("ltrim", expr);

        /// <summary>
        ///   Returns a string consisting of the value string repeated number times.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/repeat">FaunaDB Repeat Function</see>
        /// </para>
        /// </summary>
        public static Expr Repeat(Expr expr) =>
            UnescapedObject.With("repeat", expr);

        /// <summary>
        ///   Returns a string consisting of the value string repeated number times.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/repeat">FaunaDB Repeat Function</see>
        /// </para>
        /// </summary>
        public static Expr Repeat(Expr expr, Expr number) =>
            UnescapedObject.With("repeat", expr, "number", number);

        /// <summary>
        ///   Replaces a string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/replacestr">FaunaDB ReplaceStr Function</see>
        /// </para>
        /// </summary>
        public static Expr ReplaceStr(Expr expr, Expr find, Expr replace) =>
            UnescapedObject.With("replacestr", expr, "find", find, "replace", replace);

        /// <summary>
        ///   Replaces all the occurrences (or the first one) of find pattern substituted with replace string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/replacestrregex">FaunaDB Length Function</see>
        /// </para>
        /// </summary>
        public static Expr ReplaceStrRegex(Expr expr, Expr pattern, Expr replace, Expr first = null) =>
            UnescapedObject.With("replacestrregex", expr, "pattern", pattern, "replace", replace, "first", first);

        /// <summary>
        ///   Replaces a string inside.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/rtrim">FaunaDB RTrim Function</see>
        /// </para>
        /// </summary>
        public static Expr RTrim(Expr expr) =>
            UnescapedObject.With("rtrim", expr);

        /// <summary>
        ///   Replaces a string inside.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/space">FaunaDB Space Function</see>
        /// </para>
        /// </summary>
        public static Expr Space(Expr count) =>
            UnescapedObject.With("space", count);


        /// <summary>
        ///   Returns a portion of the string.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/substring">FaunaDB SubString Function</see>
        /// </para>
        /// </summary>
        public static Expr SubString(Expr expr, Expr start = null, Expr length = null) =>
            UnescapedObject.With("substring", expr, "start", start, "length", length);

        /// <summary>
        ///   Returns a string which has the first letter of each word capitalized.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/titlecase">FaunaDB TitleCase Function</see>
        /// </para>
        /// </summary>
        public static Expr TitleCase(Expr expr) =>
            UnescapedObject.With("titlecase", expr);

        /// <summary>
        ///   Returns a string which has both the leading and trailing white spaces, tabs, and new lines removed.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/trim">FaunaDB Trim Function</see>
        /// </para>
        /// </summary>
        public static Expr Trim(Expr expr) =>
            UnescapedObject.With("trim", expr);

        /// <summary>
        ///   Returns a string which has all lowercase characters in the string replaced by their corresponding uppercase characters.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/uppercase">FaunaDB UpperCase Function</see>
        /// </para>
        /// </summary>
        public static Expr UpperCase(Expr expr) =>
            UnescapedObject.With("uppercase", expr);

        /// <summary>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/startswith">FaunaDB StartsWith Function</see>
        /// </summary>
        public static Expr StartsWith(Expr value, Expr search) =>
            UnescapedObject.With("startswith", value, "search", search);

        /// <summary>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/endswith">FaunaDB EndsWith Function</see>
        /// </summary>
        public static Expr EndsWith(Expr value, Expr search) =>
            UnescapedObject.With("endswith", value, "search", search);

        /// <summary>
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containsstr">FaunaDB ContainsStr Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsStr(Expr value, Expr search) =>
            UnescapedObject.With("containsstr", value, "search", search);

        /// <summary>
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/containsstrregex">FaunaDB ContainsStrRegex Function</see>
        /// </para>
        /// </summary>
        public static Expr ContainsStrRegex(Expr value, Expr pattern) =>
            UnescapedObject.With("containsstrregex", value, "pattern", pattern);

        /// <summary>
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/regexescape">FaunaDB RegexEscape Function</see>
        /// </para>
        /// </summary>
        public static Expr RegexEscape(Expr value) =>
            UnescapedObject.With("regexescape", value);


        /// <summary>
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/documents">FaunaDB Documents Function</see>
        /// </para>
        /// </summary>
        public static Expr Documents(Expr collection) =>
            UnescapedObject.With("documents", collection);
        
        /// <summary>
        /// Try to convert an object into an array of (field, value).
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/toarray">ToArray</see>
        /// </para>
        /// </summary>
        public static Expr ToArray(Expr expr) =>
            UnescapedObject.With("to_array", expr);
        
        /// <summary>
        /// Try to convert an array of (field, value) into an object.
        /// <para>
        /// See the <see href="https://docs.fauna.com/fauna/current/api/fql/functions/toobject">ToObject</see>
        /// </para>
        /// </summary>
        public static Expr ToObject(Expr fields) =>
            UnescapedObject.With("to_object", fields);
    }
}
