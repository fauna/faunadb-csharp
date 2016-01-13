using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FaunaDB.Values;

namespace FaunaDB {
    public struct Query
    {
        Value Value { get; }

        Query(Value value)
        {
            Value = value;
        }

        /// <summary>
        /// Treat any Value as a query.
        /// If the Value is not a valid query (e.g. an ObjectV not wrapped by <see cref="QueryObject"/>), this will fail at run time.
        /// </summary>
        public static explicit operator Query(Value v) =>
            new Query(v);

        /// <summary>
        /// Extract the value out of a Query.
        /// For example, <c>Query.Add(1, 2).Value</c> is <c>new ObjectV("add", new ArrayV(1, 2))</c>.
        /// </summary>
        public static explicit operator Value(Query q) =>
            q.Value;

        static readonly Query Null = new Query(Value.Null);

        #region implicit
        public static implicit operator Query(NullV n) =>
            Null;

        public static implicit operator Query(bool b) =>
            new Query(b);

        public static implicit operator Query(int i) =>
            new Query(i);

        public static implicit operator Query(long i) =>
            new Query(i);

        public static implicit operator Query(float f) =>
            new Query(f);

        public static implicit operator Query(double d) =>
            new Query(d);

        public static implicit operator Query(string s) =>
            new Query(s);

        public static implicit operator Query(ArrayV a) =>
            new Query(a);

        public static implicit operator Query(Ref r) =>
            new Query(r);

        public static implicit operator Query(EventType e) =>
            e.Name();
        #endregion

        public static Query QueryArray(params Query[] values) =>
        new Query(ArrayV.FromEnumerable(from v in values select (Value) v));

        #region Operators
        public static Query operator !(Query a) =>
            Not(a);

        public static Query operator +(Query a, Query b) =>
            Add(a, b);

        public static Query operator -(Query a, Query b) =>
            Subtract(a, b);

        public static Query operator *(Query a, Query b) =>
            Multiply(a, b);

        public static Query operator /(Query a, Query b) =>
            Divide(a, b);

        public static Query operator %(Query a, Query b) =>
            Modulo(a, b);

        public static Query operator &(Query a, Query b) =>
            And(a, b);

        public static Query operator |(Query a, Query b) =>
            Or(a, b);

        public static Query operator <(Query a, Query b) =>
            Less(a, b);

        public static Query operator <=(Query a, Query b) =>
            LessOrEqual(a, b);

        public static Query operator >(Query a, Query b) =>
            Greater(a, b);

        public static Query operator >=(Query a, Query b) =>
            GreaterOrEqual(a, b);

        #endregion

        #region Basic forms

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static Query Let(ObjectV vars, Query @in) =>
            Q("let", new Query(vars), "in", @in);

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Let(1, a => a)</c> is equivalent to <c>Query.Let(new ObjectV("auto0", 1), Query.Var("auto0"))</c>
        /// </example>
        public static Query Let(Query value, Func<Query, Query> @in)
        {
            using (var v = new AutoVar())
                return Let(new ObjectV(v.Name, value.Value), @in(v.Query));
        }

        public static Query Let(Query value1, Query value2, Func<Query, Query, Query> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Let(
                    new ObjectV(v1.Name, value1.Value, v2.Name, value2.Value),
                    @in(v1.Query, v2.Query));               
        }

        public static Query Let(Query value1, Query value2, Query value3, Func<Query, Query, Query, Query> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Let(
                    new ObjectV(v1.Name, value1.Value, v2.Name, value2.Value, v3.Name, value3.Value),
                    @in(v1.Query, v2.Query, v3.Query));                  
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Query Var(string varName) =>
            Q("var", varName);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Query If(Query condition, Query then, Query @else) =>
            Q("if", condition, "then", then, "else", @else);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Query Do(params Query[] expressions) =>
            Q("do", Varargs(expressions));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        // todo: rename?
        public static Query QueryObject(ObjectV fields) =>
            Q("object", new Query(fields));

        public static Query QueryObject(string key1, Query value1) =>
            QueryObject(new ObjectV(key1, value1.Value));

        public static Query QueryObject(string key1, Query value1, string key2, Query value2) =>
            QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value));

        public static Query QueryObject(string key1, Query value1, string key2, Query value2, string key3, Query value3) =>
            QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value));

        public static Query QueryObject(string key1, Query value1, string key2, Query value2, string key3, Query value3, string key4, Query value4) =>
        QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value));

        public static Query QueryObject(string key1, Query value1, string key2, Query value2, string key3, Query value3,
                string key4, Query value4, string key5, Query value5) =>
        QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value, key5, value5.Value));

        public static Query QueryObject(string key1, Query value1, string key2, Query value2, string key3, Query value3,
                string key4, Query value4, string key5, Query value5, string key6, Query value6) =>
            QueryObject(new ObjectV(
                key1, value1.Value, key2, value2.Value, key3, value3.Value,
                key4, value4.Value, key5, value5.Value, key6, value6.Value));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Query Quote(Value expr) =>
            Q("quote", new Query(expr));

        #region Lambda
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static Query Lambda(string varName, Query expr) =>
            Q("lambda", varName, "expr", expr);

        public static Query Lambda(ArrayV varNames, Query expr) =>
            Q("lambda", varNames, "expr", expr);

        /// <summary>
        /// Use a lambda expression to tersely define a Lambda query.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Lambda(a => a)</c> is equivalent to <c>Query.Lambda("auto0", Query.Var("auto0"))</c>.
        /// </example>
        public static Query Lambda(Func<Query, Query> lambda)
        {
            using (var v = new AutoVar())
                return Lambda(v.Name, lambda(v.Query));
        }

        public static Query Lambda(Func<Query, Query, Query> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name), lambda(v1.Query, v2.Query));
        }

        public static Query Lambda(Func<Query, Query, Query, Query> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name, v3.Name), lambda(v1.Query, v2.Query, v3.Query));
        }

        public static Query Lambda(Func<Query, Query, Query, Query, Query> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar(), v4 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name, v3.Name, v4.Name), lambda(v1.Query, v2.Query, v3.Query, v4.Query));
        }
        #endregion
        #endregion

        #region Collection functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Map(Query collection, Query lambda) =>
            Q("map", lambda, "collection", collection);

        public static Query Map(Query collection, Func<Query, Query> lambda) =>
            Map(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Foreach(Query collection, Query lambda) =>
            Q("foreach", lambda, "collection", collection);

        public static Query Foreach(Query collection, Func<Query, Query> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Filter(Query collection, Query lambda) =>
            Q("filter", lambda, "collection", collection);

        public static Query Filter(Query collection, Func<Query, Query> lambda) =>
            Filter(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Take(Query number, Query collection) =>
            Q("take", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Drop(Query number, Query collection) =>
            Q("drop", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Prepend(Query elements, Query collection) =>
            Q("prepend", elements, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Query Append(Query elements, Query collection) =>
            Q("append", elements, "collection", collection);
        #endregion

        #region Read functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Query Get(Query @ref, Query? ts = null) =>
            // todo: helper?
            ts == null ? Q("get", @ref) : Q("get", @ref, "ts", ts.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Query Paginate(
            Query set,
            Query? size = null,
            Query? ts = null,
            Query? after = null,
            Query? before = null,
            Query? events = null,
            Query? sources = null) =>
            //todo: helper?
            new Query(ObjectV.WithoutNullValues(
                ObjectV.Pairs("paginate", set.Value),
                ObjectV.Pairs(
                    "size", size?.Value,
                    "ts", ts?.Value,
                    "after", after?.Value,
                    "before", before?.Value,
                    "events", events?.Value,
                    "sources", sources?.Value)));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Query Exists(Query @ref, Query? ts = null) =>
            //todo: helper?
            ts == null ? Q("exists", @ref) : Q("exists", @ref, "ts", ts.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Query Count(Query set, Query? events = null) =>
            //todo: helper?
            events == null ? Q("count", set) : Q("count", set, "events", events.Value);
        #endregion

        #region Write functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Create(Query classRef, Query @params) =>
            Q("create", classRef, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Update(Query @ref, Query @params) =>
            Q("update", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Replace(Query @ref, Query @params) =>
            Q("replace", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Delete(Query @ref) =>
            Q("delete", @ref);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Insert(Query @ref, Query ts, Query action, Query @params) =>
            Q("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// <see cref="Insert"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Query Insert(Event e, Query @params) =>
            Insert(e.Resource, e.Ts, e.Action, @params);

        /// <summary>
        /// <see cref="Remove"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Query Remove(Query @ref, Query ts, Query action) =>
            Q("remove", @ref, "ts", ts, "action", action);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Query Remove(Event e) =>
            Remove(e.Resource, e.Ts, e.Action);
        #endregion

        #region Sets
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Query Match(Query index, params Query[] terms) =>
            Q("match", index, "terms", Varargs(terms));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Query Union(params Query[] sets) =>
            Q("union", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Query Intersection(params Query[] sets) =>
            Q("intersection", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Query Difference(params Query[] sets) =>
            Q("difference", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Query Join(Query source, Query target) =>
            Q("join", source, "with", target);

        public static Query Join(Query source, Func<Query, Query> target) =>
            Join(source, Lambda(target));
        #endregion

        #region Authentication
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Query Login(Query @ref, Query @params) =>
            Q("login", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Query Logout(bool deleteTokens) =>
            Q("logout", deleteTokens);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Query Identify(Query @ref, Query password) =>
            Q("identify", @ref, "password", password);
        #endregion

        #region String functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Query Concat(Query strings, Query? separator = null) =>
            // todo: helper?
            separator == null ? Q("concat", strings) : Q("concat", strings, "separator", separator.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Query CaseFold(Query @string) =>
            Q("casefold", @string);
        #endregion

        #region Time and date functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Query Time(Query timeString) =>
            Q("time", timeString);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Query Epoch(Query number, Query unit) =>
            Q("epoch", number, "unit", unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Query Date(Query dateString) =>
            Q("date", dateString);
        #endregion

        #region Miscellaneous functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query EqualsExpr(params Query[] values) =>
            Q("equals", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Contains(Query path, Query @in) =>
            Q("contains", path, "in", @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Select(Query path, Query @from, Query? @default = null) =>
            new Query(ObjectV.WithoutNullValues(
                ObjectV.Pairs("select", path.Value, "from", @from.Value),
                ObjectV.Pairs("default", @default?.Value)));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Add(params Query[] numbers) =>
            Q("add", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Multiply(params Query[] numbers) =>
            Q("multiply", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Subtract(params Query[] numbers) =>
            Q("subtract", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Divide(params Query[] numbers) =>
            Q("divide", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Modulo(params Query[] numbers) =>
            Q("modulo", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Query Less(params Query[] values) =>
            Q("lt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Query LessOrEqual(params Query[] values) =>
            Q("lte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Query Greater(params Query[] values) =>
            Q("gt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Query GreaterOrEqual(params Query[] values) =>
            Q("gte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query And(params Query[] booleans) =>
            Q("and", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Or(params Query[] booleans) =>
            Q("or", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Query Not(Query boolean) =>
            Q("not", boolean);
        #endregion

        #region Helpers
        static ThreadLocal<int> autoVarNumber = new ThreadLocal<int>(() => 0);

        class AutoVar : IDisposable
        {
            public string Name { get; }
            public Query Query { get { return Var(Name); } }

            public AutoVar()
            {
                Name = AutoName(autoVarNumber.Value);
                autoVarNumber.Value++;
            }

            static List<string> autoVarNames = new List<string>();

            static string AutoName(int number)
            {
                if (number < autoVarNames.Count)
                    return autoVarNames[number];
                else
                {
                    lock (autoVarNames)
                    {
                        for (var i = autoVarNames.Count; i <= number; i++)
                            autoVarNames.Add($"auto{autoVarNumber.Value}");
                    }
                    return autoVarNames[number];
                }
            }

            public void Dispose()
            {
                autoVarNumber.Value--;
            }
        }

        static Query Varargs(params Query[] values) =>
            values.Count() == 1 ? values[0] : ArrayV.FromEnumerable(from q in values select q.Value);

        #region Q
        static Query Q(string key1, Query value1) =>
        new Query(new ObjectV(key1, value1.Value));

        static Query Q(string key1, Query value1, string key2, Query value2) =>
        new Query(new ObjectV(key1, value1.Value, key2, value2.Value));

        static Query Q(string key1, Query value1, string key2, Query value2, string key3, Query value3) =>
        new Query(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value));

        static Query Q(string key1, Query value1, string key2, Query value2, string key3, Query value3, string key4, Query value4) =>
        new Query(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value));
        #endregion
        #endregion
    }
}
