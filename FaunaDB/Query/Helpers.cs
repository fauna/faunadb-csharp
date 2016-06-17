using FaunaDB.Types;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Arr(params Expr[] values) =>
            ArrayV.FromEnumerable(values);

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(ObjectV fields) =>
            Q("object", fields);

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

                System.Array array = (System.Array)obj;
                Expr[] exprs = new Expr[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    exprs[i] = Obj(array.GetValue(i));
                }

                return ArrayV.FromEnumerable(exprs);
            }
            else
            {
                var attributes = ImmutableDictionary.CreateBuilder<string, Expr>();

                foreach (var property in obj.GetType().GetProperties())
                {
                    var value = property.GetValue(obj);

                    attributes.Add(property.Name, Obj(value));
                }

                return Obj(new ObjectV(attributes.ToImmutable()));
            }
        }

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1) =>
            Obj(new ObjectV(key1, value1));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2) =>
            Obj(new ObjectV(key1, value1, key2, value2));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3,
                string key4, Expr value4, string key5, Expr value5) =>
            Obj(new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        /// <summary>
        /// See the <see cref="https://faunadb.com/documentation/queries#values">docs</see>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3,
                string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            Obj(new ObjectV(
                key1, value1, key2, value2, key3, value3,
                key4, value4, key5, value5, key6, value6));

        #region Helpers
        static Expr Varargs(params Expr[] values) =>
            values.Length == 1 ? values[0] : ArrayV.FromEnumerable(values);

        static Expr Q(string key1, Expr value1) =>
            new ObjectV(key1, value1);

        static Expr Q(string key1, Expr value1, string key2, Expr value2) =>
            new ObjectV(key1, value1, key2, value2);

        static Expr Q(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            new ObjectV(key1, value1, key2, value2, key3, value3);

        static Expr Q(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            new ObjectV(key1, value1, key2, value2, key3, value3, key4, value4);
        #endregion

    }
}
