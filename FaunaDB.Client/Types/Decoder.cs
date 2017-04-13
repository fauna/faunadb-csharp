using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FaunaDB.Types
{
    using Converter = Func<Value, object>;

    /// <summary>
    /// FaunaDB <see cref="Value"/> to object decoder.
    /// </summary>
    public static class Decoder
    {
        /// <summary>
        /// Decode the FaunaDB value into the specified type T.
        /// </summary>
        /// <example>
        /// Decode&lt;int&gt;(LongV.Of(10)) => 10
        /// Decode&lt;double&gt;(DoubleV.Of(3.14)) => 3.14
        /// Decode&lt;bool&gt;(BooleanV.True) => true
        /// Decode&lt;object&gt;(NullV.Instance) => null
        /// Decode&lt;string&gt;(StringV.Of("a string")) => "a string"
        /// Decode&lt;int[]&gt;(ArrayV.Of(1, 2)) => new int[] { 1, 2 }
        /// Decode&lt;List&lt;int&gt;&gt;(ArrayV.Of(1, 2)) => new List&lt;int&gt;&gt; { 1, 2 }
        /// Decode&lt;byte[]&gt;(new ByteV(1, 2)) => new byte[] { 1, 2 }
        /// Decode&lt;DateTime&gt;(new TimeV("2000-01-01T01:10:30.123Z")) => new DateTime(2000, 1, 1, 1, 10, 30, 123)
        /// Decode&lt;DateTime&gt;(new DateV("2001-01-01")) => new DateTime(2001, 1, 1)
        /// Decode&lt;User&gt;(ObjectV.With("user_name", "john", "password", "s3cr3t")) => new User("john", "s3cr3t")
        /// </example>
        /// <returns>An instance of type T</returns>
        /// <param name="value">The FaunaDB value to be decoded</param>
        /// <typeparam name="T">The type name in which the value should be decoded</typeparam>
        public static T Decode<T>(Value value) =>
            (T)DecoderImpl.Decode(value, typeof(T));

        /// <summary>
        /// Decode the FaunaDB value into the type specified in the argument.
        /// </summary>
        /// <example>
        /// Decode(LongV.Of(10), typeof(int)) => 10
        /// Decode(DoubleV.Of(3.14), typeof(double)) => 3.14
        /// Decode(BooleanV.True, typeof(bool)) => true
        /// Decode(NullV.Instance, typeof(object)) => null
        /// Decode(StringV.Of("a string"), typeof(string)) => "a string"
        /// Decode(ArrayV.Of(1, 2), typeof(int[])) => new int[] { 1, 2 }
        /// Decode(ArrayV.Of(1, 2), typeof(List&lt;int&gt;&gt;)) => new List&lt;int&gt;&gt; { 1, 2 }
        /// Decode(new ByteV(1, 2), typeof(byte[])) => new byte[] { 1, 2 }
        /// Decode(new TimeV("2000-01-01T01:10:30.123Z"), typeof(DateTime)) => new DateTime(2000, 1, 1, 1, 10, 30, 123)
        /// Decode(new DateV("2001-01-01"), typeof(DateTime)) => new DateTime(2001, 1, 1)
        /// Decode(ObjectV.With("user_name", "john", "password", "s3cr3t"), typeof(User)) => new User("john", "s3cr3t")
        /// </example>
        /// <returns>An instance of type specified in the argument</returns>
        /// <param name="value">The FaunaDB value to be decoded</param>
        /// <param name="dstType">The type in which the value should be decoded</param>
        public static object Decode(Value value, Type dstType) =>
            DecoderImpl.Decode(value, dstType);
    }

    class DecoderImpl
    {
        static readonly Dictionary<Type, Converter> converters = new Dictionary<Type, Func<Value, object>>();
        static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public static object Decode(Value value, Type dstType)
        {
            if (value == null || value == NullV.Instance)
                return null;

            return DecodeIntern(value, dstType);
        }

        static object DecodeIntern(Value value, Type dstType)
        {
            if (typeof(Value).IsAssignableFrom(dstType))
            {
                if (dstType.IsAssignableFrom(value.GetType()))
                    return value;

                throw new InvalidOperationException($"Cannot cast {value.GetType()} to {dstType}");
            }

            switch (Type.GetTypeCode(dstType))
            {
                case TypeCode.String:
                    return ToString(value);

                case TypeCode.Boolean:
                    return ToBoolean(value);

                case TypeCode.DateTime:
                    return ToDateTime(value);

                case TypeCode.SByte:
                    return Convert.ToSByte(ToLong<sbyte>(value));
                case TypeCode.Int16:
                    return Convert.ToInt16(ToLong<short>(value));
                case TypeCode.Int32:
                    return Convert.ToInt32(ToLong<int>(value));
                case TypeCode.Int64:
                    return ToLong<long>(value);

                case TypeCode.Char:
                    return Convert.ToChar(ToLong<char>(value));
                case TypeCode.Byte:
                    return Convert.ToByte(ToLong<byte>(value));
                case TypeCode.UInt16:
                    return Convert.ToUInt16(ToLong<ushort>(value));
                case TypeCode.UInt32:
                    return Convert.ToUInt32(ToLong<uint>(value));
                case TypeCode.UInt64:
                    return Convert.ToUInt64(ToLong<ulong>(value));

                case TypeCode.Single:
                    return Convert.ToSingle(ToDouble<float>(value));
                case TypeCode.Double:
                    return ToDouble<double>(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(ToDouble<decimal>(value));

                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return null;

                case TypeCode.Object:
                    if (typeof(byte[]) == dstType && value is BytesV)
                        return ToByteArray(value);

                    if (dstType.IsArray && dstType.HasElementType)
                        return FromArray(value, dstType);

                    if (typeof(IDictionary).IsAssignableFrom(dstType) || (dstType.IsGenericType && dstType.Is(typeof(IDictionary<,>))))
                        return FromDictionary(value, dstType);

                    if (typeof(IList).IsAssignableFrom(dstType) || (dstType.IsGenericType && dstType.Is(typeof(IEnumerable<>))))
                        return FromEnumerable(value, dstType);

                    return FromObject(value, dstType);
            }

            throw new InvalidOperationException($"Cannot convert {value} to {dstType}");
        }

        static IList FromEnumerable(Value value, Type type)
        {
            if (!(value is ArrayV))
                throw new InvalidOperationException($"Cannot convert `{value}` to {type}");

            if (!type.IsGenericType)
                throw new InvalidOperationException($"The type {type} is not generic");

            var valueType = type.GenericTypeArguments[0];

            var createType = type.IsAbstract
                                 ? typeof(List<>).MakeGenericType(valueType)
                                 : type;

            var ret = (IList)Activator.CreateInstance(createType);

            foreach (var v in ((ArrayV)value).Value)
                ret.Add(Decode(v, valueType));

            return ret;
        }

        static IDictionary FromDictionary(Value value, Type type)
        {
            if (!(value is ObjectV))
                throw new InvalidOperationException($"Cannot convert `{value}` to {type}");

            if (!type.IsGenericType)
                throw new InvalidOperationException($"The type {type} is not generic");

            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];

            var createType = type.IsAbstract
                                 ? typeof(Dictionary<,>).MakeGenericType(keyType, valueType)
                                 : type;

            var ret = (IDictionary)Activator.CreateInstance(createType);

            foreach (var kv in ((ObjectV)value).Value)
                ret.Add(kv.Key, Decode(kv.Value, valueType));

            return ret;
        }

        static Array FromArray(Value value, Type type)
        {
            var elementType = type.GetElementType();

            if (!(value is ArrayV))
                throw new InvalidOperationException($"Cannot convert `{value}` to an array of type {elementType}");

            var array = ((ArrayV)value).Value;

            var ret = Array.CreateInstance(elementType, array.Count);

            for (var i = 0; i < array.Count; i++)
                ret.SetValue(Decode(array[i], elementType), i);

            return ret;
        }

        static long ToLong<R>(Value value) =>
            FromScalar<long>(value, typeof(R));

        static double ToDouble<R>(Value value) =>
            FromScalar<double>(value, typeof(R));

        static string ToString(Value value) =>
            FromScalar<string>(value, typeof(string));

        static bool ToBoolean(Value value) =>
            FromScalar<bool>(value, typeof(bool));

        static DateTime ToDateTime(Value value) =>
            FromScalar<DateTime>(value, typeof(DateTime));

        static byte[] ToByteArray(Value value) =>
            FromScalar<byte[]>(value, typeof(byte[]));

        static T FromScalar<T>(Value value, Type type)
        {
            var scalar = value as ScalarValue<T>;

            if ((object)scalar == null)
                throw new InvalidOperationException($"Cannot convert `{value}` to {type}");

            return scalar.Value;
        }

        static object FromObject(Value value, Type dstType)
        {
            Converter converter;

            try
            {
                locker.EnterUpgradeableReadLock();
                if (!converters.TryGetValue(dstType, out converter))
                {
                    try
                    {
                        locker.EnterWriteLock();
                        converter = CreateConverter(dstType);
                        converters.Add(dstType, converter);
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

            return converter.Invoke(value);
        }

        static Converter CreateConverter(Type dstType)
        {
            var creator = GetCreator(dstType);

            if (creator != null)
                return FromMethodCreator(creator, dstType);

            var constructor = GetConstructor(dstType);
            if (constructor == null)
                throw new InvalidOperationException($"No default constructor or constructor/static method annotated with attribute [FaunaConstructor] found on type `{dstType}`");

            return FromConstructor(constructor, dstType);
        }

        static MethodInfo GetCreator(Type dstType)
        {
            var methods = dstType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Has<FaunaConstructorAttribute>());

            if (methods.Count() > 1)
                throw new InvalidOperationException($"More than one static method creator found on type `{dstType}`");

            if (methods.Count() == 1)
                return methods.First();

            return null;
        }

        static ConstructorInfo GetConstructor(Type dstType)
        {
            var constructors = dstType
                .GetConstructors()
                .Where(c => c.Has<FaunaConstructorAttribute>());

            if (constructors.Count() > 1)
                throw new InvalidOperationException($"More than one constructor found on type `{dstType}`");

            if (constructors.Count() == 1)
                return constructors.First();

            return dstType.GetConstructor(Type.EmptyTypes);
        }

        static Expression AssignProperties(Expression objExpr, Expression varExpr, Type dstType, IEnumerable<string> ignoreProperties)
        {
            var fields = dstType
                .GetAllFields()
                .Where(f => !ignoreProperties.Contains(f.GetName()))
                .Where(f => !f.Has<CompilerGeneratedAttribute>() && !f.Has<FaunaIgnoreAttribute>())
                .Select(f => Expression.Assign(Expression.MakeMemberAccess(varExpr, f), CallDecode(CallGetValue(objExpr, f), f.FieldType)));

            var properties = dstType
                .GetProperties()
                .Where(p => !ignoreProperties.Contains(p.GetName()))
                .Where(p => p.CanWrite && !p.Has<FaunaIgnoreAttribute>())
                .Select(p => Expression.Assign(Expression.MakeMemberAccess(varExpr, p), CallDecode(CallGetValue(objExpr, p), p.PropertyType)));

            return properties.Count() > 0
                             ? Expression.Block(fields.Concat(properties))
                             : (Expression)Expression.Empty();
        }

        static Converter Create(Func<IEnumerable<Expression>, Expression> creatorExpr, ParameterInfo[] parameters, Type dstType)
        {
            var objExpr = Expression.Parameter(typeof(Value), "obj");
            var varExpr = Expression.Variable(dstType, "output");
            var target = Expression.Label(dstType);

            var parametersExpr = parameters
                .Select(p => CallDecode(CallGetValue(objExpr, p), p.ParameterType));

            var ignoreProperties = parameters.Select(p => p.GetName());

            var expr = Expression.Lambda<Converter>(
                Expression.Block(
                    new ParameterExpression[] { varExpr },
                    Expression.Assign(varExpr, creatorExpr(parametersExpr)),
                    AssignProperties(objExpr, varExpr, dstType, ignoreProperties),
                    Expression.Label(target, varExpr)
                ),
                objExpr
            );

            return expr.Compile();
        }

        /*
         * Func<Value, object> = value =>
         * {
         *      T1 arg1 = (T1)Decode( GetValue(value, "attrib1", defaultValue1), typeof(T1) );
         *      T2 arg2 = (T2)Decode( GetValue(value, "attrib2", defaultValue2), typeof(T2) );
         *
         *      DstType output = new DstType(arg1, arg2);
         *
         *      output.Field3 = (T3)Decode( GetValue(value, "attrib3", defaultValue3), typeof(T3) );
         *      output.Field4 = (T4)Decode( GetValue(value, "attrib4", defaultValue4), typeof(T4) );
         *
         *      return output;
         * }
         */
        static Converter FromConstructor(ConstructorInfo constructor, Type dstType)
        {
            return Create(
                p => Expression.New(constructor, p),
                constructor.GetParameters(),
                dstType
            );
        }

        /*
         * Func<Value, object> = value =>
         * {
         *      T1 arg1 = (T1)Decode( GetValue(value, "attrib1", defaultValue1), typeof(T1) );
         *      T2 arg2 = (T2)Decode( GetValue(value, "attrib2", defaultValue2), typeof(T2) );
         *
         *      DstType output = DstType.Creator(arg1, arg2);
         *
         *      output.Field3 = (T3)Decode( GetValue(value, "attrib3", defaultValue3), typeof(T3) );
         *      output.Field4 = (T4)Decode( GetValue(value, "attrib4", defaultValue4), typeof(T4) );
         *
         *      return output;
         * }
         */
        static Converter FromMethodCreator(MethodInfo creator, Type dstType)
        {
            return Create(
                p => Expression.Call(creator, p),
                creator.GetParameters(),
                dstType
            );
        }

        /*
         * Decode( objExpr, type )
         */
        static Expression CallDecode(Expression objExpr, Type type)
        {
            var decodeMethod = typeof(DecoderImpl).GetMethod(
                "Decode", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Value), typeof(Type) }, null
            );

            return Expression.Convert(Expression.Call(
                decodeMethod,
                objExpr,
                Expression.Constant(type)
            ), type);
        }

        static Expression CallGetValue(Expression objExpr, MemberInfo member)
        {
            var field = member.GetCustomAttribute<FaunaFieldAttribute>();
            var hasDefaultValue = field != null && field.DefaultValue != null;
            var defaultValue = field != null ? Encoder.Encode(field.DefaultValue) : null;

            return CallGetValue(objExpr, member.GetName(), hasDefaultValue, defaultValue);
        }

        static Expression CallGetValue(Expression objExpr, ParameterInfo parameter)
        {
            var ignore = parameter.GetCustomAttribute<FaunaIgnoreAttribute>();
            var field = parameter.GetCustomAttribute<FaunaFieldAttribute>();
            var hasDefaultValue = field != null && field.DefaultValue != null;

            var defaultValue = hasDefaultValue
                ? Encoder.Encode(field.DefaultValue)
                : Encoder.Encode(parameter.DefaultValue);

            if (ignore != null)
                return Expression.Constant(defaultValue, typeof(Value));

            return CallGetValue(objExpr, parameter.GetName(), hasDefaultValue || parameter.HasDefaultValue, defaultValue);
        }

        /*
         * if (hasDefaultValue)
         *     GetValue( objExpr, property, defaultValue )
         * else
         *     GetValue( objExpr, property )
         */
        static Expression CallGetValue(Expression objExpr, string property, bool hasDefaultValue, Value defaultValue)
        {
            if (hasDefaultValue)
            {
                var getValueMethod = typeof(DecoderImpl).GetMethod(
                    "GetValue", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(ObjectV), typeof(string), typeof(Value) }, null
                );

                return Expression.Call(
                    getValueMethod,
                    Expression.Convert(objExpr, typeof(ObjectV)),
                    Expression.Constant(property),
                    Expression.Convert(Expression.Constant(defaultValue), typeof(Value))
                );
            }
            else
            {
                var getValueMethod = typeof(DecoderImpl).GetMethod(
                    "GetValue", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(ObjectV), typeof(string) }, null
                );

                return Expression.Call(
                    getValueMethod,
                    Expression.Convert(objExpr, typeof(ObjectV)),
                    Expression.Constant(property)
                );
            }
        }

        static Value GetValue(ObjectV obj, string property)
        {
            var values = obj.Value;

            Value value;
            if (values.TryGetValue(property, out value))
                return value;

            throw new InvalidOperationException($"Missing required property: `{property}`");
        }

        static Value GetValue(ObjectV obj, string property, Value defaultValue)
        {
            var values = obj.Value;

            Value value;
            if (values.TryGetValue(property, out value))
                return value;

            return defaultValue;
        }
    }
}
