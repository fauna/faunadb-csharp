using System;
using System.Collections.Generic;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents any scalar or non-scalar value in the FaunaDB query language. FaunaDB value types consist of
    /// all of the JSON value types, as well as the FaunaDB-specific types, {@link RefV} and {@link SetRefV}.
    /// <para>
    /// Scalar values are <see cref="LongV"/>, <see cref="StringV"/>, <see cref="DoubleV"/>, <see cref="BooleanV"/>, <see cref="NullV"/>,
    /// <see cref="RefV"/>, and <see cref="SetRefV"/>.
    /// </para>
    /// <para>
    /// Non-scalar values are <see cref="ObjectV"/> and <see cref="ArrayV"/>.
    /// </para>
    /// <para>
    /// This interface itself does not have any directly accessible data. It must first be coerced into a type before
    /// its data can be accessed.
    /// </para>
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values">FaunaDB Value Types</see>
    /// </para>
    /// </summary>
    /// <example>
    /// Consider the <see cref="Value"/> node modeling the root of the tree:
    /// <code>
    /// {
    ///   "ref": { "@ref": { "id": "classes" } },
    ///   "data": { "someKey": "string1", "someKey2": 123 }
    /// }
    /// </code>
    /// <para>
    /// The result tree can be accessed using:
    /// <code>
    ///   Field&lt;RefV&gt; ref = Field.At("ref").To&lt;RefV&gt;();
    ///   Field&lt;string&gt; someKey = Field.At("data", "someKey").To&lt;string&gt;();
    ///   Field&lt;string&gt; nonExistingKey = Field.At("non-existing-key").To&lt;long&gt;();
    ///
    ///   node.Get(ref); // new RefV("classes")
    ///   node.Get(someKey); // "string1"
    ///   node.GetOption(nonExistingKey) // Option.None&lt;string&gt;()
    /// </code>
    /// </para>
    /// <para>
    /// The interface also has helpers to transverse values without <see cref="Field"/> references:
    /// <code>
    ///   node.At("ref").To&lt;RefV&gt;().Get(); // new RefV("classes")
    ///   node.At("data", "someKey").To&lt;string&gt;().Get() // "string1"
    ///   node.At("non-existing-key").To&lt;long&gt;().GetOption() // Option.None&lt;long&gt;()
    /// </code>
    /// </para>
    /// <para>
    /// See <see cref="Field"/>
    /// </para>
    /// </example>
    [JsonConverter(typeof(ValueJsonConverter))]
    public abstract class Value : Expr
    {
        /// <summary>
        /// Navigate through object's keys, assuming value is an instance of <see cref="ObjectV"/>.
        /// </summary>
        /// <param name="keys">keys path to navigate to</param>
        /// <returns><see cref="Value"/> under the path or <see cref="NullV"/></returns>
        public Value At(params string[] keys) =>
            Field.At(keys).Get(this).Match(
                Success: value => value,
                Failure: reason => NullV.Instance
            );

        /// <summary>
        /// Navigate through array's indexes, assuming value is an instance of <see cref="ArrayV"/>.
        /// </summary>
        /// <param name="indexes">indexes path to navigate to</param>
        /// <returns><see cref="Value"/> under the path or <see cref="NullV"/></returns>
        public Value At(params int[] indexes) =>
            Field.At(indexes).Get(this).Match(
                Success: value => value,
                Failure: reason => NullV.Instance
            );

        /// <summary>
        /// Attempts to coerce this value to given type T specified.
        /// </summary>
        /// <typeparam name="T">The type name in which this value shoulbe be decoded</typeparam>
        public IResult<T> To<T>() =>
            Field.Decode<T>(this);

        /// <summary>
        /// Loop through this node collecting the {@link Field} passed, assuming the node is an instance of {@link ArrayV}
        /// <para>
        /// See <see cref="Field"/>
        /// </para>
        /// </summary>
        /// <example>
        /// Consider the <see cref="Value"/> node modeling the root of the tree:
        /// <code>
        /// {
        ///   "data": {
        ///     "arrayOfStrings": ["Jhon", "Bill"],
        ///     "arrayOfObjects": [ {"name": "Jhon"}, {"name": "Bill"} ]
        ///    }
        /// }
        /// </code>
        /// <para>
        /// The result tree can be accessed using:
        /// <code>
        ///   node.Get("arrayOfStrings").Collect(Field.To&lt;string&gt;()); // ["Jhon", "Bill"]
        ///
        ///   Field&lt;string&gt; name = Field.At("name").To&lt;string&gt;();
        ///   node.Get("arrayOfObjects").Collect(name); // ["Jhon", "Bill"]
        /// }
        /// </code>
        /// </para>
        /// </example>
        /// <param name="field">field to extract from each array value</param>
        /// <returns>a <see cref="IReadOnlyList{T}"/> with the collected <see cref="Field"/></returns>
        public IReadOnlyList<T> Collect<T>(Field<T> field) =>
            Field.Root.Collect(field).Get(this).Value;

        /// <summary>
        /// Extract a <see cref="Field"/> from this node
        /// <para>
        /// See <see cref="Field"/>
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        /// <param name="field">field to extract</param>
        /// <returns>the resulting value of extracting the <see cref="Field"/> from this node</returns>
        public T Get<T>(Field<T> field) =>
            field.Get(this).Value;

        /// <summary>
        /// Attempts to extact a <see cref="Field"/> from this node
        /// <para>
        /// See <see cref="Field"/>
        /// </para>
        /// </summary>
        /// <param name="field">field to extract</param>
        /// <returns>An <see cref="IOption{T}"/> with the resulting value if the field's extraction was successful</returns>
        public IOption<T> GetOption<T>(Field<T> field) =>
            field.Get(this).ToOption;

        #region implicit conversions
        public static implicit operator Value(bool b) =>
            BooleanV.Of(b);

        public static implicit operator Value(double d) =>
            new DoubleV(d);

        public static implicit operator Value(long l) =>
            new LongV(l);

        public static implicit operator Value(int i) =>
            new LongV(i);

        public static implicit operator Value(string s) =>
            s == null ? NullV.Instance : new StringV(s);

        public static implicit operator Value(DateTime dt) =>
            FromDateTime(dt);

        public static implicit operator Value(DateTimeOffset dt) =>
            FromDateTimeOffset(dt);

        public static implicit operator Value(byte[] bytes) =>
            new BytesV(bytes);

        internal static Value FromDateTime(DateTime dt, Type forceType = null)
        {
            if (forceType == typeof(DateV))
            {
                return NewMethod(dt);
            }

            if (forceType == typeof(TimeV) || dt.Ticks % (24 * 60 * 60 * 10000) > 0)
            {
                return new TimeV(dt);
            }

            return new DateV(dt.Date);
        }

        private static Value NewMethod(DateTime dt)
        {
            return new DateV(dt.Date);
        }

        internal static Value FromDateTimeOffset(DateTimeOffset dt, Type forceType = null)
        {
            if (forceType == typeof(DateV))
            {
                return new DateV(dt.UtcDateTime.Date);
            }

            if (forceType == typeof(TimeV) || dt.UtcTicks % (24 * 60 * 60 * 10000) > 0)
            {
                return new TimeV(dt.UtcDateTime);
            }

            return new DateV(dt.UtcDateTime.Date);
        }
        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator bool(Value v) =>
            ((BooleanV)v).Value;

        public static explicit operator double(Value v) =>
            ((DoubleV)v).Value;

        public static explicit operator long(Value v) =>
            ((LongV)v).Value;

        public static explicit operator string(Value v) =>
            ((StringV)v).Value;

        public static explicit operator DateTime(Value v) =>
            Expr.ToDateTime(v);

        public static explicit operator DateTimeOffset(Value v) =>
            Expr.ToDateTimeOffset(v);
        #endregion
    }
}
