using FaunaDB.Collections;

namespace FaunaDB.Query
{
    public partial struct Language
    {
        /// <summary>
        /// Creates a new empty Object value.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj() =>
            Obj(ImmutableDictionary.Empty<string, Expr>());

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
       public static Expr Obj(string key1, Expr value1) =>
            Obj(ImmutableDictionary.Of(key1, value1));

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2) =>
            Obj(ImmutableDictionary.Of(key1, value1, key2, value2));

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3) =>
            Obj(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3));

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4) =>
            Obj(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4));

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
        public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5) =>
            Obj(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5));

        /// <summary>
        /// Creates a new Object value with the provided entries.
        /// <para>
        /// See the <see href="https://faunadb.com/documentation/queries#values">FaunaDB Values</see>
        /// </para>
        /// </summary>
       public static Expr Obj(string key1, Expr value1, string key2, Expr value2, string key3, Expr value3, string key4, Expr value4, string key5, Expr value5, string key6, Expr value6) =>
            Obj(ImmutableDictionary.Of(key1, value1, key2, value2, key3, value3, key4, value4, key5, value5, key6, value6));
    }
}
