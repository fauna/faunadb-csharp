using FaunaDB.Types;
using FaunaDB.Utils;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace FaunaDB.Query
{

    public partial struct Language
    {
        public static Expr Ref(string id)
        {
            return new Ref(id);
        }

        public static Expr Ref(Expr classRef, Expr id)
        {
            return UnescapedObject.With("ref", classRef, "id", id);
        }

        public static Expr SetRef(Expr set)
        {
            return new SetRef(set);
        }

        public static TsV Ts(DateTime dateTime)
        {
            return new TsV(dateTime);
        }

        public static TsV Ts(string iso8601Time)
        {
            return new TsV(iso8601Time);
        }

        public static DateV Dt(DateTime dateTime)
        {
            return new DateV(dateTime);
        }

        public static DateV Dt(string iso8601Date)
        {
            return new DateV(iso8601Date);
        }

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static ArrayV Arr(params Expr[] values) =>
            ArrayV.FromEnumerable(values);

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(Expr fields) =>
            UnescapedObject.With("object", fields);

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(object obj)
        {
            if (obj == null)
                return NullV.Instance;

            Type type = obj.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                MethodInfo methodInfo = typeof(Expr).GetMethod("op_Implicit", new Type[] { type });

                return (Expr)methodInfo.Invoke(null, new object[] { obj });
            }
            else if (type.IsArray)
            {
                Type arrayType = type.GetElementType();

                Array array = (Array)obj;
                Expr[] exprs = new Expr[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    exprs[i] = Obj(array.GetValue(i));
                }

                return ArrayV.FromEnumerable(exprs);
            }
            else
            {
                var attributes = new OrderedDictionary<string, Expr>();

                foreach (var property in obj.GetType().GetProperties())
                {
                    var value = property.GetValue(obj);

                    attributes.Add(property.Name, Obj(value));
                }

                return Obj(new UnescapedObject(attributes.ToImmutable()));
            }
        }

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj() =>
            Obj(UnescapedObject.With());

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1) =>
            Obj(UnescapedObject.With(key1, value1));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2) =>
            Obj(UnescapedObject.With(key1, value1, key2, value2));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            Obj(UnescapedObject.With(key1, value1, key2, value2, key3, value3));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            Obj(UnescapedObject.With(key1, value1, key2, value2, key3, value3, key4, value4));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            Obj(UnescapedObject.With(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            Obj(UnescapedObject.With(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));

        #region Helpers
        static Expr Varargs(Expr head, Expr[] tail)
        {
            if (tail.Length == 0)
                return head;

            Expr[] values = new Expr[tail.Length + 1];
            values[0] = head;
            tail.CopyTo(values, 1);

            return ArrayV.FromEnumerable(values);
        }

        static Expr Varargs(Expr[] values) =>
            values.Length == 1 ? values[0] : ArrayV.FromEnumerable(values);
        #endregion

    }
}
