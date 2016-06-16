using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FaunaDB.Values;
using System.Reflection;
using System.Collections.Immutable;

namespace FaunaDB.Query {
    public partial struct Language
    {
        public static Expr Array(params Expr[] values) =>
            ArrayV.FromEnumerable(values);

        #region Basic forms

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static Expr Let(Expr vars, Expr @in) =>
            Q("let", vars, "in", @in);

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Let(1, a => a)</c> is equivalent to <c>Query.Let(new ObjectV("auto0", 1), Query.Var("auto0"))</c>
        /// </example>
        public static Expr Let(Expr value, Func<Expr, Expr> @in)
        {
            ParameterInfo[] info = @in.Method.GetParameters();
            string p0 = info[0].Name;

            return Let(
                Q(p0, value),
                @in(Var(p0)));
        }

        public static Expr Let(Expr value1, Expr value2, Func<Expr, Expr, Expr> @in)
        {
            ParameterInfo[] info = @in.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;

            return Let(
                Q(p0, value1, p1, value2),
                @in(Var(p0), Var(p1)));
        }

        public static Expr Let(Expr value1, Expr value2, Expr value3, Func<Expr, Expr, Expr, Expr> @in)
        {
            ParameterInfo[] info = @in.Method.GetParameters();
            string p0 = info[0].Name;
            string p1 = info[1].Name;
            string p2 = info[2].Name;

            return Let(
                Q(p0, value1, p1, value2, p2, value3),
                @in(Var(p0), Var(p1), Var(p2)));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr Var(string varName) =>
            Q("var", varName);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr If(Expr @if, Expr @then, Expr @else) =>
            Q("if", @if, "then", @then, "else", @else);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Expr Do(params Expr[] expressions) =>
            Q("do", Varargs(expressions));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        // todo: rename?
        public static Expr Obj(ObjectV fields) =>
            Q("object", fields);

        public static Expr Obj(string key1, Expr value1) =>
            Obj(new ObjectV(key1, value1));

        public static Expr Obj(string key1, Expr value1, string key2, Expr value2) =>
            Obj(new ObjectV(key1, value1, key2, value2));

        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3));

        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4));

        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3,
                string key4, Expr value4, string key5, Expr value5) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3,
                string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            Obj(new ObjectV(
                key1, value1, key2, value2, key3, value3,
                key4, value4, key5, value5, key6, value6));

        #endregion

        #region Collection functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Map(Expr collection, Expr lambda) =>
            Q("map", lambda, "collection", collection);

        public static Expr Map(Expr collection, Func<Expr, Expr> lambda) =>
            Map(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Foreach(Expr collection, Expr lambda) =>
            Q("foreach", lambda, "collection", collection);

        public static Expr Foreach(Expr collection, Func<Expr, Expr> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Filter(Expr collection, Expr lambda) =>
            Q("filter", lambda, "collection", collection);

        public static Expr Filter(Expr collection, Func<Expr, Expr> lambda) =>
            Filter(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Take(Expr number, Expr collection) =>
            Q("take", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Drop(Expr number, Expr collection) =>
            Q("drop", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Prepend(Expr elements, Expr collection) =>
            Q("prepend", elements, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Expr Append(Expr elements, Expr collection) =>
            Q("append", elements, "collection", collection);
        #endregion

        #region Read functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Get(Expr @ref, Expr ts = null) =>
            ts == null ? Q("get", @ref) : Q("get", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Paginate(
            Expr set,
            Expr size = null,
            Expr ts = null,
            Expr after = null,
            Expr before = null,
            Expr events = null,
            Expr sources = null) =>
            ObjectV.WithoutNullValues(
                ObjectV.Pairs("paginate", set),
                ObjectV.Pairs(
                    "size", size,
                    "ts", ts,
                    "after", after,
                    "before", before,
                    "events", events,
                    "sources", sources));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Exists(Expr @ref, Expr ts = null) =>
            ts == null ? Q("exists", @ref) : Q("exists", @ref, "ts", ts);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Expr Count(Expr set, Expr events = null) =>
            events == null ? Q("count", set) : Q("count", set, "events", events);
        #endregion

        #region Write functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Create(Expr classRef, Expr @params) =>
            Q("create", classRef, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Update(Expr @ref, Expr @params) =>
            Q("update", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Replace(Expr @ref, Expr @params) =>
            Q("replace", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Delete(Expr @ref) =>
            Q("delete", @ref);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Insert(Expr @ref, Expr ts, Expr action, Expr @params) =>
            Q("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// <see cref="Remove"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Expr Remove(Expr @ref, Expr ts, Expr action) =>
            Q("remove", @ref, "ts", ts, "action", action);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Expr Remove(Event e) =>
            Remove(e.Resource, e.Ts, e.Action.Name());

        /// <summary>
        /// <see cref="Insert"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Expr Insert(Event e, Expr @params) =>
            Insert(e.Resource, e.Ts, e.Action.Name(), @params);

        #endregion

        #region Sets
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Match(Expr index, params Expr[] terms) =>
            Q("match", index, "terms", Varargs(terms));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Union(params Expr[] sets) =>
            Q("union", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Intersection(params Expr[] sets) =>
            Q("intersection", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Difference(params Expr[] sets) =>
            Q("difference", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Expr Join(Expr source, Expr target) =>
            Q("join", source, "with", target);

        public static Expr Join(Expr source, Func<Expr, Expr> target) =>
            Join(source, Lambda(target));
        #endregion

        #region Authentication
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Login(Expr @ref, Expr @params) =>
            Q("login", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Logout(Expr deleteTokens) =>
            Q("logout", deleteTokens);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Expr Identify(Expr @ref, Expr password) =>
            Q("identify", @ref, "password", password);
        #endregion

        #region String functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr Concat(Expr strings) =>
            // todo: helper?
            Q("concat", strings);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr Concat(Expr strings, Expr separator) =>
            // todo: helper?
            Q("concat", strings, "separator", separator);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Expr CaseFold(Expr @string) =>
            Q("casefold", @string);
        #endregion

        #region Time and date functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Time(Expr time) =>
            Q("time", time);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Epoch(Expr number, Expr unit) =>
            Q("epoch", number, "unit", unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Expr Date(Expr date) =>
            Q("date", date);
        #endregion

        #region Miscellaneous functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr EqualsExpr(params Expr[] values) =>
            Q("equals", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Contains(Expr path, Expr @in) =>
            Q("contains", path, "in", @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Select(Expr path, Expr @from, Expr @default = null) =>
            ObjectV.WithoutNullValues(
                ObjectV.Pairs("select", path, "from", @from),
                ObjectV.Pairs("default", @default));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Add(params Expr[] numbers) =>
            Q("add", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Multiply(params Expr[] numbers) =>
            Q("multiply", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Subtract(params Expr[] numbers) =>
            Q("subtract", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Divide(params Expr[] numbers) =>
            Q("divide", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Modulo(params Expr[] numbers) =>
            Q("modulo", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr Less(params Expr[] values) =>
            Q("lt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr LessOrEqual(params Expr[] values) =>
            Q("lte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr Greater(params Expr[] values) =>
            Q("gt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Expr GreaterOrEqual(params Expr[] values) =>
            Q("gte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr And(params Expr[] booleans) =>
            Q("and", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Or(params Expr[] booleans) =>
            Q("or", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Expr Not(Expr boolean) =>
            Q("not", boolean);
        #endregion

        #region Helpers

        static Expr Varargs(params Expr[] values) =>
            values.Count() == 1 ? values[0] : ArrayV.FromEnumerable(values);

        #region Q
        static Expr Q(string key1, Expr value1) =>
            new ObjectV(key1, value1);

        static Expr Q(string key1, Expr value1, string key2, Expr value2) =>
            new ObjectV(key1, value1, key2, value2);

        static Expr Q(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new ObjectV(key1, value1, key2, value2, key3, value3);

        static Expr Q(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4);
        #endregion
        #endregion
    }
}
