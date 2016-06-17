using FaunaDB.Types;
using FaunaDB.Utils;
using System;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        #region Basic Forms
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static Expr Let(Expr vars, Expr @in) =>
            UnescapedObject.With("let", vars, "in", @in);

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>.
        /// </summary>
        /// <example>
        /// <code>
        /// <c>Language.Let(1, a => a)</c> is equivalent to <c>Language.Let(new ObjectV("a", 1), Language.Var("a"))</c>
        /// </code>
        /// </example>
        public static Expr Let(Expr v0, Func<Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;

            return Let(
                UnescapedObject.With(p0, v0),
                In(Var(p0)));
        }

        public static Expr Let(Expr v0, Expr v1, Func<Expr, Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1),
                In(Var(p0), Var(p1)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Func<Expr, Expr, Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2),
                In(Var(p0), Var(p1), Var(p2)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Func<Expr, Expr, Expr, Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3),
                In(Var(p0), Var(p1), Var(p2), Var(p3)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Expr v4, Func<Expr, Expr, Expr, Expr, Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3, p4, v4),
                In(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4)));
        }

        public static Expr Let(Expr v0, Expr v1, Expr v2, Expr v3, Expr v4, Expr v5, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> In)
        {
            ParameterInfo[] info = In.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;
            string p3 = info[3].Name;
            string p4 = info[4].Name;
            string p5 = info[5].Name;

            return Let(
                UnescapedObject.With(p0, v0, p1, v1, p2, v2, p3, v3, p4, v4, p5, v5),
                In(Var(p0), Var(p1), Var(p2), Var(p3), Var(p4), Var(p5)));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr Var(string varName) =>
            UnescapedObject.With("var", varName);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr If(Expr @if, Expr @then, Expr @else) =>
            UnescapedObject.With("if", @if, "then", @then, "else", @else);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr Do(params Expr[] expressions) =>
            UnescapedObject.With("do", Varargs(expressions));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static Expr Lambda(Expr vars, Expr expr) =>
            UnescapedObject.With("lambda", vars, "expr", expr);

        #endregion

        #region Collection Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Map(Expr collection, Expr lambda) =>
            UnescapedObject.With("map", lambda, "collection", collection);

        public static Expr Map(Expr collection, Func<Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        public static Expr Map(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        public static Expr Map(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        public static Expr Map(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        public static Expr Map(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        public static Expr Map(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Foreach(Expr collection, Expr lambda) =>
            UnescapedObject.With("foreach", lambda, "collection", collection);

        public static Expr Foreach(Expr collection, Func<Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        public static Expr Foreach(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Filter(Expr collection, Expr lambda) =>
            UnescapedObject.With("filter", lambda, "collection", collection);

        public static Expr Filter(Expr collection, Func<Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        public static Expr Filter(Expr collection, Func<Expr, Expr, Expr, Expr, Expr, Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Take(Expr number, Expr collection) =>
            UnescapedObject.With("take", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Drop(Expr number, Expr collection) =>
            UnescapedObject.With("drop", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Prepend(Expr elements, Expr collection) =>
            UnescapedObject.With("prepend", elements, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Append(Expr elements, Expr collection) =>
            UnescapedObject.With("append", elements, "collection", collection);
        #endregion

        #region Read Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Get(Expr @ref, Expr ts = null) =>
            ts == null ? UnescapedObject.With("get", @ref) : UnescapedObject.With("get", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Paginate(
            Expr set,
            Expr ts = null,
            Expr after = null,
            Expr before = null,
            Expr size = null,
            Expr events = null,
            Expr sources = null) =>
                UnescapedObject.With(
                    "paginate", set,
                    "ts", ts,
                    "after", after,
                    "before", before,
                    "size", size,
                    "events", events,
                    "sources", sources);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Exists(Expr @ref, Expr ts = null) =>
            ts == null ? UnescapedObject.With("exists", @ref) : UnescapedObject.With("exists", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Count(Expr set, Expr events = null) =>
            events == null ? UnescapedObject.With("count", set) : UnescapedObject.With("count", set, "events", events);
        #endregion

        #region Write Functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Create(Expr classRef, Expr @params) =>
            UnescapedObject.With("create", classRef, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Update(Expr @ref, Expr @params) =>
            UnescapedObject.With("update", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Replace(Expr @ref, Expr @params) =>
            UnescapedObject.With("replace", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Delete(Expr @ref) =>
            UnescapedObject.With("delete", @ref);

        public enum Action
        {
            CREATE,
            DELETE
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, Expr action, Expr @params) =>
            UnescapedObject.With("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// <see cref="Remove"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, Expr action) =>
            UnescapedObject.With("remove", @ref, "ts", ts, "action", action);

        #endregion

        #region Sets
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Match(Expr index, params Expr[] terms) =>
            UnescapedObject.With("match", index, "terms", terms.Length == 0 ? null : Varargs(terms));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Union(Expr head, params Expr[] tail) =>
            UnescapedObject.With("union", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Intersection(Expr head, params Expr[] tail) =>
            UnescapedObject.With("intersection", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Difference(Expr head, params Expr[] tail) =>
            UnescapedObject.With("difference", Varargs(head, tail));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Distinct(Expr set) =>
            UnescapedObject.With("distinct", set);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Join(Expr source, Expr target) =>
            UnescapedObject.With("join", source, "with", target);

        public static Expr Join(Expr source, Func<Expr, Expr> target) =>
            Join(source, Lambda(target));
        #endregion

        #region Authentication
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Login(Expr @ref, Expr @params) =>
            UnescapedObject.With("login", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Logout(Expr deleteTokens) =>
            UnescapedObject.With("logout", deleteTokens);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Identify(Expr @ref, Expr password) =>
            UnescapedObject.With("identify", @ref, "password", password);
        #endregion

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

        #region Time and Date

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Time(Expr time) =>
            UnescapedObject.With("time", time);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Epoch(Expr number, Expr unit) =>
            UnescapedObject.With("epoch", number, "unit", unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Date(Expr date) =>
            UnescapedObject.With("date", date);
        #endregion

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
