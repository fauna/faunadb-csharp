using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using FaunaDB.Errors;
using FaunaDB.Values;

namespace FaunaDB {
    public static class Query
    {
        #region Basic forms

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use the overload.
        /// </summary>
        public static ObjectV Let(ObjectV vars, Value @in)
        {
            return new ObjectV("let", vars, "in", @in);
        }

        /// <summary>
        /// Use a lambda expression to conveniently define let expressions.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Let(1, a => a)</c> is equivalent to <c>Query.Let(new ObjectV("auto0", 1), Query.Var("auto0"))</c>
        /// </example>
        public static ObjectV Let(Value value, Func<Value, Value> @in)
        {
            using (var v = new AutoVar())
                return Let(new ObjectV(v.Name, value), @in(v.Query));
        }

        public static ObjectV Let(Value value1, Value value2, Func<Value, Value, Value> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Let(new ObjectV(v1.Name, value1, v2.Name, value2), @in(v1.Query, v2.Query));               
        }

        public static ObjectV Let(Value value1, Value value2, Value value3, Func<Value, Value, Value, Value> @in)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Let(new ObjectV(v1.Name, value1, v2.Name, value2, v3.Name, value3), @in(v1.Query, v2.Query, v3.Query));                  
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static ObjectV Var(string varName)
        {
            return new ObjectV("var", varName);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static ObjectV If(Value condition, Value then, Value @else)
        {
            return new ObjectV("if", condition, "then", then, "else", @else);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static ObjectV Do(params Value[] expressions)
        {
            return new ObjectV("do", varargs(expressions));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static ObjectV QueryObject(ObjectV fields)
        {
            return new ObjectV("object", fields);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        public static ObjectV Quote(Value expr)
        {
            return new ObjectV("quote", expr);
        }

        #region Lambda

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// This is the raw version. Usually it's easier to use an overload.
        /// </summary>
        public static ObjectV Lambda(Value pattern, Value expr)
        {
            return new ObjectV("lambda", pattern, "expr", expr);
        }

        /// <summary>
        /// Use a lambda expression to tersely define a Lambda query.
        /// See the <see href="https://faunadb.com/documentation/queries#basic_forms">docs</see>. 
        /// </summary>
        /// <example>
        /// <c>Query.Lambda(a => a)</c> is equivalent to <c>Query.Lambda("auto0", Query.Var("auto0"))</c>.
        /// </example>
        public static ObjectV Lambda(Func<Value, Value> lambda)
        {
            using (var v = new AutoVar())
                return Lambda(v.Name, lambda(v.Query));
        }

        public static ObjectV Lambda(Func<Value, Value, Value> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name), lambda(v1.Query, v2.Query));
        }

        public static ObjectV Lambda(Func<Value, Value, Value, Value> lambda)
        {
            using (AutoVar v1 = new AutoVar(), v2 = new AutoVar(), v3 = new AutoVar())
                return Lambda(new ArrayV(v1.Name, v2.Name, v3.Name), lambda(v1.Query, v2.Query, v3.Query));
        }

        public static ObjectV Lambda(Func<Value, Value, Value, Value, Value> lambda)
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
        public static ObjectV Map(Value collection, Value lambda)
        {
            return new ObjectV("map", lambda, "collection", collection);
        }

        public static ObjectV Map(Value collection, Func<Value, Value> lambda)
        {
            return Map(collection, Lambda(lambda));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Foreach(Value collection, Value lambda)
        {
            return new ObjectV("foreach", lambda, "collection", collection);
        }

        public static ObjectV Foreach(Value collection, Func<Value, Value> lambda)
        {
            return Foreach(collection, Lambda(lambda));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Filter(Value collection, Value lambda)
        {
            return new ObjectV("filter", lambda, "collection", collection);
        }

        public static ObjectV Filter(Value collection, Func<Value, Value> lambda)
        {
            return Filter(collection, Lambda(lambda));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Take(Value number, Value collection)
        {
            return new ObjectV("take", number, "collection", collection);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Drop(Value number, Value collection)
        {
            return new ObjectV("drop", number, "collection", collection);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Prepend(Value elements, Value collection)
        {
            return new ObjectV("prepend", elements, "collection", collection);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#collection_functions">docs</see>. 
        /// </summary>
        public static ObjectV Append(Value elements, Value collection)
        {
            return new ObjectV("append", elements, "collection", collection);
        }

        #endregion

        #region Read functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static ObjectV Get(Value @ref, Value ts = null)
        {
            return ObjectV.WithoutNullValues(ObjectV.Pairs("get", @ref), ObjectV.Pairs("ts", ts));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static ObjectV Paginate(
            Value set,
            Value size = null,
            Value ts = null,
            Value after = null,
            Value before = null,
            Value events = null,
            Value sources = null)
        {
            return ObjectV.WithoutNullValues(
                ObjectV.Pairs("paginate", set),
                ObjectV.Pairs(
                    "size", size,
                    "ts", ts,
                    "after", after,
                    "before", before,
                    "events", events,
                    "sources", sources));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static ObjectV Exists(Value @ref, Value ts = null)
        {
            return ObjectV.WithoutNullValues(ObjectV.Pairs("exists", @ref), ObjectV.Pairs("ts", ts));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#read_functions">docs</see>. 
        /// </summary>
        public static ObjectV Count(Value set, Value events = null)
        {
            return ObjectV.WithoutNullValues(ObjectV.Pairs("count", set), ObjectV.Pairs("events", events));
        }

        #endregion

        #region Write functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Create(Value classRef, Value @params)
        {
            return new ObjectV("create", classRef, "params", @params);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Update(Value @ref, Value @params)
        {
            return new ObjectV("update", @ref, "params", @params);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Replace(Value @ref, Value @params)
        {
            return new ObjectV("replace", @ref, "params", @params);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Delete(Value @ref)
        {
            return new ObjectV("delete", @ref);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Insert(Value @ref, Value ts, Value action, Value @params)
        {
            return new ObjectV("insert", @ref, "ts", ts, "action", action, "params", @params);
        }

        /// <summary>
        /// <see cref="Insert"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static ObjectV Insert(Event e, Value @params)
        {
            return Insert(e.Resource, e.Ts, e.Action, @params);
        }

        /// <summary>
        /// <see cref="Remove"/> that takes an <see cref="Event"/> object instead of separate parameters.
        /// </summary>
        public static ObjectV Remove(Value @ref, Value ts, Value action)
        {
            return new ObjectV("remove", @ref, "ts", ts, "action", action);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#write_functions">docs</see>. 
        /// </summary>
        public static ObjectV Remove(Event e)
        {
            return Remove(e.Resource, e.Ts, e.Action);
        }

        #endregion

        #region Sets

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static ObjectV Match(Value terms, Value index)
        {
            return new ObjectV("match", index, "terms", terms);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static ObjectV Union(params Value[] sets)
        {
            return new ObjectV("union", varargs(sets));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static ObjectV Intersection(params Value[] sets)
        {
            return new ObjectV("intersection", varargs(sets));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static ObjectV Difference(params Value[] sets)
        {
            return new ObjectV("difference", varargs(sets));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#sets">docs</see>. 
        /// </summary>
        public static ObjectV Join(Value source, Value target)
        {
            return new ObjectV("join", source, "with", target);
        }

        public static ObjectV Join(Value source, Func<Value, Value> target)
        {
            return Join(source, Lambda(target));
        }

        #endregion

        #region Authentication

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static ObjectV Login(Value @ref, Value @params)
        {
            return new ObjectV("login", @ref, "params", @params);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static ObjectV LoginWithPassword(Value @ref, Value password)
        {
            return Login(@ref, QueryObject(new ObjectV("password", password)));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static ObjectV Logout(bool deleteTokens)
        {
            return new ObjectV("logout", deleteTokens);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#auth_functions">docs</see>. 
        /// </summary>
        public static ObjectV Identify(Value @ref, Value password)
        {
            return new ObjectV("identify", @ref, "password", password);
        }

        #endregion

        #region String functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static ObjectV Concat(Value strings, Value separator = null)
        {
            return ObjectV.WithoutNullValues(
                ObjectV.Pairs("concat", strings),
                ObjectV.Pairs("separator", separator));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#string_functions">docs</see>. 
        /// </summary>
        public static ObjectV CaseFold(Value @string)
        {
            return new ObjectV("casefold", @string);
        }

        #endregion

        #region Time and date functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static ObjectV Time(Value timeString)
        {
            return new ObjectV("time", timeString);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static ObjectV Epoch(Value number, Value unit)
        {
            return new ObjectV("epoch", number, "unit", unit);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#time_functions">docs</see>. 
        /// </summary>
        public static ObjectV Date(Value dateString)
        {
            return new ObjectV("date", dateString);
        }

                          #endregion

        #region Miscellaneous functions

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV EqualsExpr(params Value[] values)
        {
            return new ObjectV("equals", varargs(values));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Contains(Value path, Value @in)
        {
            return new ObjectV("contains", path, "in", @in);
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Select(Value path, Value @from, Value @default = null)
        {
            return ObjectV.WithoutNullValues(ObjectV.Pairs("select", path), ObjectV.Pairs("from", @from, "default", @default));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Add(params Value[] numbers)
        {
            return new ObjectV("add", varargs(numbers));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Multiply(params Value[] numbers)
        {
            return new ObjectV("multiply", varargs(numbers));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Subtract(params Value[] numbers)
        {
            return new ObjectV("subtract", varargs(numbers));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Divide(params Value[] numbers)
        {
            return new ObjectV("divide", varargs(numbers));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Modulo(params Value[] numbers)
        {
            return new ObjectV("modulo", varargs(numbers));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV And(params Value[] booleans)
        {
            return new ObjectV("and", varargs(booleans));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Or(params Value[] booleans)
        {
            return new ObjectV("or", varargs(booleans));
        }

        /// <summary>
        /// See the <see href="https://faunadb.com/documentation/queries#misc_functions">docs</see>. 
        /// </summary>
        public static ObjectV Not(Value boolean)
        {
            return new ObjectV("not", boolean);
        }
        #endregion

        #region Helpers

        static ThreadLocal<int> autoVarNumber = new ThreadLocal<int>(() => 0);

        class AutoVar : IDisposable
        {
            public string Name { get; }
            public ObjectV Query { get { return Var(Name); } }

            public AutoVar()
            {
                Name = string.Format("auto{0}", autoVarNumber.Value);
                autoVarNumber.Value++;
            }

            public void Dispose()
            {
                autoVarNumber.Value--;
            }
        }

        static Value varargs(params Value[] values)
        {
            return values.Count() == 1 ? values[0] : new ArrayV(values);
        }

        #endregion
    }
}
