using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FaunaDB.Values;

namespace FaunaDB.Query {
    public struct Language
    {
        Expr Value { get; }

        Language(Expr value)
        {
            Value = value;
        }

        /// <summary>
        /// Treat any Value as a query.
        /// If the Value is not a valid query (e.g. an ObjectV not wrapped by <see cref="QueryObject"/>), this will fail at run time.
        /// </summary>
        public static explicit operator Language(Value v) =>
            new Language(v);

        /// <summary>
        /// Extract the value out of a Query.
        /// For example, <c>Query.Add(1, 2).Value</c> is <c>new ObjectV("add", new ArrayV(1, 2))</c>.
        /// </summary>
        public static explicit operator Expr(Language q) =>
            q.Value;

        static readonly Language Null = new Language(NullV.Instance);

        #region implicit
        public static implicit operator Language(NullV n) =>
            Null;

        public static implicit operator Language(bool b) =>
            new Language(b);

        public static implicit operator Language(int i) =>
            new Language(i);

        public static implicit operator Language(long i) =>
            new Language(i);

        public static implicit operator Language(float f) =>
            new Language(f);

        public static implicit operator Language(double d) =>
            new Language(d);

        public static implicit operator Language(string s) =>
            new Language(s);

        public static implicit operator Language(ArrayV a) =>
            new Language(a);

        public static implicit operator Language(Ref r) =>
            new Language(r);

        public static implicit operator Language(EventType e) =>
            e.Name();
        #endregion

        public static Language QueryArray(params Language[] values) =>
        new Language(ArrayV.FromEnumerable(from v in values select (Value) v));

        #region Operators
        public static Language operator !(Language a) =>
            Not(a);

        public static Language operator +(Language a, Language b) =>
            Add(a, b);

        public static Language operator -(Language a, Language b) =>
            Subtract(a, b);

        public static Language operator *(Language a, Language b) =>
            Multiply(a, b);

        public static Language operator /(Language a, Language b) =>
            Divide(a, b);

        public static Language operator %(Language a, Language b) =>
            Modulo(a, b);

        public static Language operator &(Language a, Language b) =>
            And(a, b);

        public static Language operator |(Language a, Language b) =>
            Or(a, b);

        public static Language operator <(Language a, Language b) =>
            Less(a, b);

        public static Language operator <=(Language a, Language b) =>
            LessOrEqual(a, b);

        public static Language operator >(Language a, Language b) =>
            Greater(a, b);

        public static Language operator >=(Language a, Language b) =>
            GreaterOrEqual(a, b);

        #endregion

        #region Basic forms

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static Language Let(ObjectV vars, Language @in) =>
            Q("let", new Language(vars), "in", @in);

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Let(1, a => a)</c> is equivalent to <c>Query.Let(new ObjectV("auto0", 1), Query.Var("auto0"))</c>
        /// </example>
        public static Language Let(Language value, Func<Language, Language> @in)
        {
            using (var v = new AutoVar())
                return Let(new ObjectV(v.Name, value.Value), @in(v.Query));
        }

        public static Language Let(Language value1, Language value2, Func<Language, Language, Language> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Let(
                    new ObjectV(v1.Name, value1.Value, v2.Name, value2.Value),
                    @in(v1.Query, v2.Query));               
        }

        public static Language Let(Language value1, Language value2, Language value3, Func<Language, Language, Language, Language> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Let(
                    new ObjectV(v1.Name, value1.Value, v2.Name, value2.Value, v3.Name, value3.Value),
                    @in(v1.Query, v2.Query, v3.Query));                  
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Language Var(string varName) =>
            Q("var", varName);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Language If(Language condition, Language then, Language @else) =>
            Q("if", condition, "then", then, "else", @else);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Language Do(params Language[] expressions) =>
            Q("do", Varargs(expressions));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        // todo: rename?
        public static Language QueryObject(ObjectV fields) =>
            Q("object", new Language(fields));

        public static Language QueryObject(string key1, Language value1) =>
            QueryObject(new ObjectV(key1, value1.Value));

        public static Language QueryObject(string key1, Language value1, string key2, Language value2) =>
            QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value));

        public static Language QueryObject(string key1, Language value1, string key2, Language value2, string key3, Language value3) =>
            QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value));

        public static Language QueryObject(string key1, Language value1, string key2, Language value2, string key3, Language value3, string key4, Language value4) =>
        QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value));

        public static Language QueryObject(string key1, Language value1, string key2, Language value2, string key3, Language value3,
                string key4, Language value4, string key5, Language value5) =>
        QueryObject(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value, key5, value5.Value));

        public static Language QueryObject(string key1, Language value1, string key2, Language value2, string key3, Language value3,
                string key4, Language value4, string key5, Language value5, string key6, Language value6) =>
            QueryObject(new ObjectV(
                key1, value1.Value, key2, value2.Value, key3, value3.Value,
                key4, value4.Value, key5, value5.Value, key6, value6.Value));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static Language Quote(Expr expr) =>
            Q("quote", new Language(expr));

        #region Lambda
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static Language Lambda(string varName, Language expr) =>
            Q("lambda", varName, "expr", expr);

        public static Language Lambda(ArrayV varNames, Language expr) =>
            Q("lambda", varNames, "expr", expr);

        /// <summary>
        /// Use a lambda expression to tersely define a Lambda query.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Lambda(a => a)</c> is equivalent to <c>Query.Lambda("auto0", Query.Var("auto0"))</c>.
        /// </example>
        public static Language Lambda(Func<Language, Language> lambda)
        {
            using (var v = new AutoVar())
                return Lambda(v.Name, lambda(v.Query));
        }

        public static Language Lambda(Func<Language, Language, Language> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name), lambda(v1.Query, v2.Query));
        }

        public static Language Lambda(Func<Language, Language, Language, Language> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name, v3.Name), lambda(v1.Query, v2.Query, v3.Query));
        }

        public static Language Lambda(Func<Language, Language, Language, Language, Language> lambda)
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
        public static Language Map(Language collection, Language lambda) =>
            Q("map", lambda, "collection", collection);

        public static Language Map(Language collection, Func<Language, Language> lambda) =>
            Map(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Foreach(Language collection, Language lambda) =>
            Q("foreach", lambda, "collection", collection);

        public static Language Foreach(Language collection, Func<Language, Language> lambda) =>
            Foreach(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Filter(Language collection, Language lambda) =>
            Q("filter", lambda, "collection", collection);

        public static Language Filter(Language collection, Func<Language, Language> lambda) =>
            Filter(collection, Lambda(lambda));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Take(Language number, Language collection) =>
            Q("take", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Drop(Language number, Language collection) =>
            Q("drop", number, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Prepend(Language elements, Language collection) =>
            Q("prepend", elements, "collection", collection);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static Language Append(Language elements, Language collection) =>
            Q("append", elements, "collection", collection);
        #endregion

        #region Read functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Language Get(Language @ref, Language? ts = null) =>
            // todo: helper?
            ts == null ? Q("get", @ref) : Q("get", @ref, "ts", ts.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Language Paginate(
            Language set,
            Language? size = null,
            Language? ts = null,
            Language? after = null,
            Language? before = null,
            Language? events = null,
            Language? sources = null) =>
            //todo: helper?
            new Language(ObjectV.WithoutNullValues(
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
        public static Language Exists(Language @ref, Language? ts = null) =>
            //todo: helper?
            ts == null ? Q("exists", @ref) : Q("exists", @ref, "ts", ts.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static Language Count(Language set, Language? events = null) =>
            //todo: helper?
            events == null ? Q("count", set) : Q("count", set, "events", events.Value);
        #endregion

        #region Write functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Create(Language classRef, Language @params) =>
            Q("create", classRef, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Update(Language @ref, Language @params) =>
            Q("update", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Replace(Language @ref, Language @params) =>
            Q("replace", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Delete(Language @ref) =>
            Q("delete", @ref);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Insert(Language @ref, Language ts, Language action, Language @params) =>
            Q("insert", @ref, "ts", ts, "action", action, "params", @params);

        /// <summary>
        /// <see cref="Insert"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Language Insert(Event e, Language @params) =>
            Insert(e.Resource, e.Ts, e.Action, @params);

        /// <summary>
        /// <see cref="Remove"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static Language Remove(Language @ref, Language ts, Language action) =>
            Q("remove", @ref, "ts", ts, "action", action);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static Language Remove(Event e) =>
            Remove(e.Resource, e.Ts, e.Action);
        #endregion

        #region Sets
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Language Match(Language index, params Language[] terms) =>
            Q("match", index, "terms", Varargs(terms));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Language Union(params Language[] sets) =>
            Q("union", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Language Intersection(params Language[] sets) =>
            Q("intersection", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Language Difference(params Language[] sets) =>
            Q("difference", Varargs(sets));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static Language Join(Language source, Language target) =>
            Q("join", source, "with", target);

        public static Language Join(Language source, Func<Language, Language> target) =>
            Join(source, Lambda(target));
        #endregion

        #region Authentication
        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Language Login(Language @ref, Language @params) =>
            Q("login", @ref, "params", @params);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Language Logout(bool deleteTokens) =>
            Q("logout", deleteTokens);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static Language Identify(Language @ref, Language password) =>
            Q("identify", @ref, "password", password);
        #endregion

        #region String functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Language Concat(Language strings, Language? separator = null) =>
            // todo: helper?
            separator == null ? Q("concat", strings) : Q("concat", strings, "separator", separator.Value);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static Language CaseFold(Language @string) =>
            Q("casefold", @string);
        #endregion

        #region Time and date functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Language Time(Language timeString) =>
            Q("time", timeString);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Language Epoch(Language number, Language unit) =>
            Q("epoch", number, "unit", unit);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static Language Date(Language dateString) =>
            Q("date", dateString);
        #endregion

        #region Miscellaneous functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language EqualsExpr(params Language[] values) =>
            Q("equals", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Contains(Language path, Language @in) =>
            Q("contains", path, "in", @in);

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Select(Language path, Language @from, Language? @default = null) =>
            new Language(ObjectV.WithoutNullValues(
                ObjectV.Pairs("select", path.Value, "from", @from.Value),
                ObjectV.Pairs("default", @default?.Value)));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Add(params Language[] numbers) =>
            Q("add", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Multiply(params Language[] numbers) =>
            Q("multiply", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Subtract(params Language[] numbers) =>
            Q("subtract", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Divide(params Language[] numbers) =>
            Q("divide", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Modulo(params Language[] numbers) =>
            Q("modulo", Varargs(numbers));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Language Less(params Language[] values) =>
            Q("lt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Language LessOrEqual(params Language[] values) =>
            Q("lte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Language Greater(params Language[] values) =>
            Q("gt", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>.
        /// </summary>
        public static Language GreaterOrEqual(params Language[] values) =>
            Q("gte", Varargs(values));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language And(params Language[] booleans) =>
            Q("and", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Or(params Language[] booleans) =>
            Q("or", Varargs(booleans));

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static Language Not(Language boolean) =>
            Q("not", boolean);
        #endregion

        #region Helpers
        static ThreadLocal<int> autoVarNumber = new ThreadLocal<int>(() => 0);

        class AutoVar : IDisposable
        {
            public string Name { get; }
            public Language Query { get { return Var(Name); } }

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

        static Language Varargs(params Language[] values) =>
            values.Count() == 1 ? values[0] : ArrayV.FromEnumerable(from q in values select q.Value);

        #region Q
        static Language Q(string key1, Language value1) =>
        new Language(new ObjectV(key1, value1.Value));

        static Language Q(string key1, Language value1, string key2, Language value2) =>
        new Language(new ObjectV(key1, value1.Value, key2, value2.Value));

        static Language Q(string key1, Language value1, string key2, Language value2, string key3, Language value3) =>
        new Language(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value));

        static Language Q(string key1, Language value1, string key2, Language value2, string key3, Language value3, string key4, Language value4) =>
        new Language(new ObjectV(key1, value1.Value, key2, value2.Value, key3, value3.Value, key4, value4.Value));
        #endregion
        #endregion
    }
}
