using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Threading;

namespace FaunaDB.Types
{
    using Converter = Func<EncoderImpl, object, Value>;

    /// <summary>
    /// FaunaDB object to <see cref="Value"/> encoder.
    /// </summary>
    public static class Encoder
    {
        /// <summary>
        /// Encode the specified object into a corresponding FaunaDB value.
        /// </summary>
        /// <example>
        /// Encode(10) => LongV.Of(10)
        /// Encode(3.14) => DoubleV.Of(3.14)
        /// Encode(true) => BooleanV.True
        /// Encode(null) => NullV.Instance
        /// Encode("a string") => StringV.Of("a string")
        /// Encode(new int[] {1, 2}) => ArrayV.Of(1, 2)
        /// Encode(new List&lt;int&gt; {1, 2}) => ArrayV.Of(1, 2)
        /// Encode(new byte[] {1, 2}) => new ByteV(1, 2)
        /// Encode(DateTime.Now) => new TimeV("2000-01-01T01:10:30.123Z")
        /// Encode(DateTime.Today) => new DateV("2001-01-01")
        /// Encode(user) => ObjectV.With("user_name", "john", "password", "s3cr3t")
        /// </example>
        /// <returns>A FaunaDB <see cref="Value"/> corresponding to the given argument</returns>
        /// <param name="obj">Any instance of user defined classes, primitive values or any
        /// generic collection like <see cref="System.Collections.Generic.IList{T}"/> or <see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/></param>
        public static Value Encode(object obj) =>
            new EncoderImpl().Encode(obj);
    }

    class EncoderImpl
    {
        static readonly Dictionary<Type, Converter> converters = new Dictionary<Type, Converter>();
        static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        Stack<object> stack = new Stack<object>();

        public Value Encode(object obj)
        {
            if (obj == null)
                return NullV.Instance;

            if (typeof(Value).IsInstanceOfType(obj))
                return (Value)obj;

            if (stack.Contains(obj, ReferenceComparer.Default))
                throw new InvalidOperationException($"Self referencing loop detected for object `{obj}`");

            try
            {
                stack.Push(obj);

                return EncodeIntern(obj);
            }
            finally
            {
                stack.Pop();
            }
        }

        static readonly object dbNullValue = GetDBNullValue();

        static object GetDBNullValue()
        {
#if !(NETSTANDARD1_5)
            return DBNull.Value;
#else
            var dbNullType = Type.GetType("System.DBNull");
            return dbNullType.GetField("Value").GetValue(null);
#endif
        }

        Value EncodeIntern(object obj)
        {
            var type = obj.GetType();

            if (dbNullValue.Equals(obj))
                return NullV.Instance;

            if (type.GetTypeInfo().IsEnum)
                return WrapEnum(obj, type);

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return StringV.Of((string)obj);

                case TypeCode.Boolean:
                    return BooleanV.Of((bool)obj);

                case TypeCode.DateTime:
                    return WrapDateTime((DateTime)obj);

                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return LongV.Of(Convert.ToInt64(obj));

                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return LongV.Of(Convert.ToInt64(obj));

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return DoubleV.Of(Convert.ToDouble(obj));

#if !(NETSTANDARD1_5)
                case TypeCode.DBNull:
                    return NullV.Instance;
#endif

                case TypeCode.Empty:
                    return NullV.Instance;

                case TypeCode.Object:
                    if (typeof(byte[]).IsInstanceOfType(obj))
                        return new BytesV((byte[])obj);

                    if (typeof(IDictionary).IsInstanceOfType(obj))
                        return WrapDictionary((IDictionary)obj);

                    if (typeof(IEnumerable).IsInstanceOfType(obj))
                        return WrapEnumerable((IEnumerable)obj);

                    return WrapObj(obj);
            }

            throw new InvalidOperationException($"Unsupported type {type} at `{obj}`");
        }

        Value WrapDateTime(DateTime date)
        {
            if (date.Ticks % (24 * 60 * 60 * 10000) > 0)
                return new TimeV(date);

            return new DateV(date);
        }

        Value WrapEnumerable(IEnumerable array)
        {
            var ret = new List<Value>();

            foreach (var value in array)
                ret.Add(Encode(value));

            return new ArrayV(ret);
        }

        Value WrapDictionary(IDictionary dic)
        {
            var ret = new Dictionary<string, Value>();

            foreach (DictionaryEntry entry in dic)
                ret.Add(entry.Key.ToString(), Encode(entry.Value));

            return new ObjectV(ret);
        }

        Value WrapEnum(object obj, Type type)
        {
            Converter converter;

            try
            {
                locker.EnterUpgradeableReadLock();
                if (!converters.TryGetValue(type, out converter))
                {
                    try
                    {
                        locker.EnterWriteLock();
                        converter = CreateEnumConverter(type);
                        converters.Add(type, converter);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }

            return converter.Invoke(this, obj);
        }

        Value WrapObj(object obj)
        {
            var type = obj.GetType();
            Converter converter;

            try
            {
                locker.EnterUpgradeableReadLock();
                if (!converters.TryGetValue(type, out converter))
                {
                    try
                    {
                        locker.EnterWriteLock();
                        converter = CreateConverter(type);
                        converters.Add(type, converter);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }

            return converter.Invoke(this, obj);
        }

        /*
         * Func<EncoderImpl, object, Value> lambda = (encoder, obj) => {
         *    var dic = new Dictionary<string, Expr> {
         *       { "Field1", encoder.Encode(obj.Field1) },
         *       { "Field2", encoder.Encode(obj.Field2) },
         *       { "Field3", encoder.Encode(obj.Field3) },
         *    };
         *    return new ObjectV(dic);
         * };
         */
        Converter CreateConverter(Type srcType)
        {
            var dictType = typeof(Dictionary<string, Value>);
            var addMethod = dictType.GetMethod("Add", new Type[] { typeof(string), typeof(Value) });
            var objectVConstructor = typeof(ObjectV).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(IReadOnlyDictionary<string, Value>) }, null);

            var encoderExpr = Expression.Parameter(typeof(EncoderImpl), "encoder");
            var objExpr = Expression.Parameter(typeof(object), "obj");

            var fields = srcType.GetAllFields()
                .Where(field => !field.Has<CompilerGeneratedAttribute>() && !field.Has<FaunaIgnoreAttribute>())
                .Select(field => AddElementToDic(encoderExpr, srcType, field, addMethod, objExpr));

            var properties = srcType
                .GetProperties()
                .Where(parameter => parameter.CanRead && !parameter.Has<FaunaIgnoreAttribute>())
                .Select(parameter => AddElementToDic(encoderExpr, srcType, parameter, addMethod, objExpr));

            var newDictExpr = Expression.ListInit(Expression.New(dictType), fields.Concat(properties));
            var lambda = Expression.Lambda<Converter>(
                Expression.New(objectVConstructor, newDictExpr),
                encoderExpr,
                objExpr
            );

            return lambda.Compile();
        }

        Converter CreateEnumConverter(Type srcType)
        {
            var encoderExpr = Expression.Parameter(typeof(EncoderImpl), "encoder");
            var objExpr = Expression.Parameter(typeof(object), "obj");

            var defaultValue = Expression.Constant(null, typeof(Value));
            var switchValue = Expression.Convert(objExpr, srcType);

            var cases = srcType.GetTypeInfo()
                .GetEnumValues()
                .OfType<Enum>()
                .Select(CreateSwitchCase)
                .ToArray();

            return Expression.Lambda<Converter>(
                Expression.Switch(switchValue, defaultValue, cases),
                encoderExpr,
                objExpr
            ).Compile();
        }

        SwitchCase CreateSwitchCase(Enum enumValue)
        {
            var dstType = enumValue.GetType();
            var enumName = Enum.GetName(dstType, enumValue);
            var member = dstType.GetMember(enumName)[0];
            var faunaEnum = member.GetCustomAttribute<FaunaEnum>();

            var body = Expression.Constant(
                StringV.Of(faunaEnum != null ? faunaEnum.Alias : enumName),
                typeof(Value)
            );

            return Expression.SwitchCase(body, Expression.Constant(enumValue));
        }

        /*
         * (object) ((SrcType)objExpr).member
         */
        Expression AccessMember(Type srcType, MemberInfo member, Expression objExpr)
        {
            return Expression.Convert(
                Expression.MakeMemberAccess(Expression.Convert(objExpr, srcType), member),
                typeof(object)
            );
        }

        /*
         * encodeExpr.Encode( argExpression )
         */
        Expression CallEncode(Expression encoderExpr, Expression argExpression)
        {
            var encodeMethod = typeof(EncoderImpl).GetMethod("Encode", new Type[] { typeof(object) });

            return Expression.Call(
                encoderExpr,
                encodeMethod,
                argExpression
            );
        }

        /*
         * Add(member.Name, Encode( objExpr.member ))
         */
        ElementInit AddElementToDic(Expression encoderExpr, Type srcType, MemberInfo member, MethodInfo addMethod, Expression objExpr)
        {
            var keyExpr = Expression.Constant(member.GetName());
            var valueExpr = CallEncode(encoderExpr, AccessMember(srcType, member, objExpr));

            return Expression.ElementInit(
                addMethod, keyExpr, valueExpr
            );
        }

        class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Default = new ReferenceComparer();

            public new bool Equals(object x, object y) =>
                object.ReferenceEquals(x, y);

            public int GetHashCode(object obj) =>
                RuntimeHelpers.GetHashCode(obj);
        }
    }
}
