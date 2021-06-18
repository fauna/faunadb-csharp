using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Converter = System.Func<FaunaDB.Types.Value, object>;

namespace FaunaDB.Types
{
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

    internal class DecoderImpl
    {
        private static readonly Dictionary<Type, Converter> converters = new Dictionary<Type, Func<Value, object>>();
        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        private static InvalidCastException invalidCast(object value, Type to)
        {
            var from = value?.GetType();
            return new InvalidCastException($"Invalid cast from '{from}' to '{to}'.");
        }

        public static object Decode(Value value, Type dstType, Type forceType = null)
        {
            if (value == NullV.Instance && dstType == typeof(Value))
            {
                return value;
            }

            if (value == null || value == NullV.Instance)
            {
                return dstType.GetTypeInfo().IsValueType ? Activator.CreateInstance(dstType) : null;
            }

            return DecodeIntern(value, dstType, forceType);
        }

        private static object DecodeIntern(Value value, Type dstType, Type forceType = null)
        {
            if (typeof(Value).IsAssignableFrom(dstType))
            {
                if (dstType.IsAssignableFrom(value.GetType()))
                {
                    return value;
                }

                throw invalidCast(value, dstType);
            }

            if (dstType.GetTypeInfo().IsEnum)
            {
                return ToEnum(value, dstType);
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
                    return Convert.ToSByte(ToNumber<sbyte>(value));
                case TypeCode.Int16:
                    return Convert.ToInt16(ToNumber<short>(value));
                case TypeCode.Int32:
                    return Convert.ToInt32(ToNumber<int>(value));
                case TypeCode.Int64:
                    return Convert.ToInt64(ToNumber<long>(value));

                case TypeCode.Char:
                    return Convert.ToChar(ToNumber<char>(value));
                case TypeCode.Byte:
                    return Convert.ToByte(ToNumber<byte>(value));
                case TypeCode.UInt16:
                    return Convert.ToUInt16(ToNumber<ushort>(value));
                case TypeCode.UInt32:
                    return Convert.ToUInt32(ToNumber<uint>(value));
                case TypeCode.UInt64:
                    return Convert.ToUInt64(ToNumber<ulong>(value));

                case TypeCode.Single:
                    return Convert.ToSingle(ToNumber<float>(value));
                case TypeCode.Double:
                    return Convert.ToDouble(ToNumber<double>(value));
                case TypeCode.Decimal:
                    return Convert.ToDecimal(ToNumber<decimal>(value));

#if !NETSTANDARD1_5
                case TypeCode.DBNull:
                    return null;
#endif
                case TypeCode.Empty:
                    return null;

                case TypeCode.Object:
                    if (typeof(DateTimeOffset) == dstType)
                    {
                        if (forceType == typeof(StringV) && value.GetType() == typeof(StringV))
                        {
                            if (value != null && value != NullV.Instance)
                            {
                                return new DateTimeOffset(Convert.ToDateTime(ToString(value)));
                            }
                            else if (dstType.Is(typeof(Nullable<>)))
                            {
                                return null;
                            }

                            return default(DateTimeOffset);
                        }

                        return new DateTimeOffset(ToDateTime(value));
                    }

                    if (typeof(byte[]) == dstType && value is BytesV)
                    {
                        return ToByteArray(value);
                    }

                    if (dstType.IsArray && dstType.HasElementType)
                    {
                        return FromArray(value, dstType);
                    }

                    if (typeof(IDictionary).IsAssignableFrom(dstType) || (dstType.GetTypeInfo().IsGenericType && dstType.Is(typeof(IDictionary<,>))))
                    {
                        return FromDictionary(value, dstType);
                    }

                    if (dstType.GetTypeInfo().IsGenericType && dstType.Is(typeof(ISet<>)))
                    {
                        return FromSet(value, dstType);
                    }

                    if (typeof(IList).IsAssignableFrom(dstType) || (dstType.GetTypeInfo().IsGenericType && dstType.Is(typeof(IEnumerable<>))))
                    {
                        return FromEnumerable(value, dstType);
                    }

                    // All other objects with forcetype StringV
                    if (forceType == typeof(StringV) && value.GetType() == typeof(StringV))
                    {
                        if (value != null && value != NullV.Instance)
                        {
                            return Activator.CreateInstance(dstType, ToString(value));
                        }

                        return null;
                    }

                    if (dstType.GetTypeInfo().IsGenericType && dstType.Is(typeof(Nullable<>)))
                    {
                        return DecodeIntern(value, Nullable.GetUnderlyingType(dstType));
                    }

                    return FromObject(value, dstType);
            }

            throw invalidCast(value, dstType);
        }

        private static IEnumerable FromSet(Value value, Type type)
        {
            if (!(value is ArrayV))
            {
                throw invalidCast(value, type);
            }

            var valueType = type.GenericTypeArguments[0];

            var createType = type.GetTypeInfo().IsAbstract
                                 ? typeof(HashSet<>).MakeGenericType(valueType)
                                 : type;

            var set = (IEnumerable)Activator.CreateInstance(createType);

            var adder = createType.GetMethod("Add", new Type[] { valueType });
            var payload = new object[1];

            foreach (var v in ((ArrayV)value).Value)
            {
                payload[0] = Decode(v, valueType);
                adder.Invoke(set, payload);
            }

            return set;
        }

        private static IList FromEnumerable(Value value, Type type)
        {
            if (!(value is ArrayV))
            {
                throw invalidCast(value, type);
            }

            if (!type.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException($"The type {type} is not generic");
            }

            var valueType = type.GenericTypeArguments[0];

            var createType = type.GetTypeInfo().IsAbstract
                                 ? typeof(List<>).MakeGenericType(valueType)
                                 : type;

            var ret = (IList)Activator.CreateInstance(createType);

            foreach (var v in ((ArrayV)value).Value)
            {
                ret.Add(Decode(v, valueType));
            }

            return ret;
        }

        private static IDictionary FromDictionary(Value value, Type type)
        {
            if (!(value is ObjectV))
            {
                throw invalidCast(value, type);
            }

            if (!type.GetTypeInfo().IsGenericType)
            {
                throw new InvalidOperationException($"The type {type} is not generic");
            }

            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];

            var createType = type.GetTypeInfo().IsAbstract
                                 ? typeof(Dictionary<,>).MakeGenericType(keyType, valueType)
                                 : type;

            var ret = (IDictionary)Activator.CreateInstance(createType);

            foreach (var kv in ((ObjectV)value).Value)
            {
                ret.Add(kv.Key, Decode(kv.Value, valueType));
            }

            return ret;
        }

        private static Array FromArray(Value value, Type type)
        {
            var elementType = type.GetElementType();

            if (!(value is ArrayV))
            {
                throw invalidCast(value, type);
            }

            var array = ((ArrayV)value).Value;

            var ret = Array.CreateInstance(elementType, array.Count);

            for (var i = 0; i < array.Count; i++)
            {
                ret.SetValue(Decode(array[i], elementType), i);
            }

            return ret;
        }

        private static object ToNumber<R>(Value value)
        {
            var ll = value as ScalarValue<long>;
            if (ll != null)
            {
                return ll.Value;
            }

            var dd = value as ScalarValue<double>;
            if (dd != null)
            {
                return dd.Value;
            }

            var ss = value as ScalarValue<string>;
            if (ss != null)
            {
                return ss.Value;
            }

            throw invalidCast(value, typeof(R));
        }

        private static string ToString(Value value) =>
            FromScalar<string>(value, typeof(string));

        private static bool ToBoolean(Value value) =>
            FromScalar<bool>(value, typeof(bool));

        private static DateTime ToDateTime(Value value) =>
            FromScalar<DateTime>(value, typeof(DateTime));

        private static byte[] ToByteArray(Value value) =>
            FromScalar<byte[]>(value, typeof(byte[]));

        private static T FromScalar<T>(Value value, Type type)
        {
            var scalar = value as ScalarValue<T>;

            if ((object)scalar == null)
            {
                throw invalidCast(value, type);
            }

            return scalar.Value;
        }

        private static object ToEnum(Value value, Type dstType)
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
                        converter = CreateEnumConverter(dstType);
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

        private static object FromObject(Value value, Type dstType)
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

        private static object ThrowError(Value value, Type dstType)
        {
            throw new InvalidOperationException($"Enumeration value '{((StringV)value).Value}' not found in {dstType}");
        }

        private static Converter CreateEnumConverter(Type dstType)
        {
            var objExpr = Expression.Parameter(typeof(Value), "obj");
            var varExpr = Expression.Variable(dstType, "output");
            var target = Expression.Label(dstType);

            var switchValue = Expression.MakeMemberAccess(
                Expression.Convert(objExpr, typeof(StringV)),
                typeof(StringV).GetMember("Value")[0]);

            var defaultValue = Expression.Call(
                typeof(DecoderImpl).GetMethod("ThrowError", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Value), typeof(Type) }, null),
                objExpr,
                Expression.Constant(dstType, typeof(Type)));

            var cases = dstType.GetTypeInfo()
                .GetEnumValues()
                .OfType<Enum>()
                .Select(CreateSwitchCase)
                .ToArray();

            return Expression.Lambda<Converter>(
                Expression.Switch(switchValue, defaultValue, cases),
                objExpr)
            .Compile();
        }

        private static SwitchCase CreateSwitchCase(Enum enumValue)
        {
            var dstType = enumValue.GetType();
            var enumName = Enum.GetName(dstType, enumValue);
            var member = dstType.GetMember(enumName)[0];

            var faunaEnum = member.GetCustomAttribute<FaunaEnum>();

            return Expression.SwitchCase(
                Expression.Constant(enumValue, typeof(object)),
                Expression.Constant(
                    faunaEnum != null ? faunaEnum.Alias : enumName,
                    typeof(string)));
        }

        private static Converter CreateConverter(Type dstType)
        {
            var creator = GetCreator(dstType);

            if (creator != null)
            {
                return FromMethodCreator(creator, dstType);
            }

            var constructor = GetConstructor(dstType);
            if (constructor == null)
            {
                if (dstType.GetTypeInfo().IsValueType)
                {
                    return FromValueType(dstType);
                }

                throw new InvalidOperationException($"No default constructor or constructor/static method annotated with attribute [FaunaConstructor] found on type `{dstType}`");
            }

            return FromConstructor(constructor, dstType);
        }

        private static MethodInfo GetCreator(Type dstType)
        {
            var methods = dstType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Has<FaunaConstructorAttribute>());

            if (methods.Count() > 1)
            {
                throw new InvalidOperationException($"More than one static method creator found on type `{dstType}`");
            }

            if (methods.Count() == 1)
            {
                return methods.First();
            }

            return null;
        }

        private static ConstructorInfo GetConstructor(Type dstType)
        {
            var constructors = dstType
                .GetConstructors()
                .Where(c => c.Has<FaunaConstructorAttribute>());

            if (constructors.Count() > 1)
            {
                throw new InvalidOperationException($"More than one constructor found on type `{dstType}`");
            }

            if (constructors.Count() == 1)
            {
                return constructors.First();
            }

            return dstType.GetConstructor(Type.EmptyTypes);
        }

        private static Expression AssignProperties(Expression objExpr, Expression varExpr, Type dstType, IEnumerable<string> ignoreProperties)
        {
            var fields = dstType
                .GetAllFields()
                .Where(f => !ignoreProperties.Contains(f.GetName()))
                .Where(f => !f.Has<CompilerGeneratedAttribute>() && !f.Has<FaunaIgnoreAttribute>())
                .Select(f => Expression.Assign(Expression.Field(varExpr, f), CallDecode(CallGetValue(objExpr, f, f.FieldType), f.FieldType, f.GetOverrideType())));

            var properties = dstType
                .GetProperties()
                .Where(p => !ignoreProperties.Contains(p.GetName()))
                .Where(p => p.CanWrite && !p.Has<FaunaIgnoreAttribute>())
                .Select(p => Expression.Assign(Expression.MakeMemberAccess(varExpr, p), CallDecode(CallGetValue(objExpr, p, p.PropertyType), p.PropertyType, p.GetOverrideType())));

            var concat = fields.Concat(properties);

            return concat.Any()
                ? Expression.Block(concat)
                : (Expression)Expression.Empty();
        }

        private static Converter Create(Func<IEnumerable<Expression>, Expression> creatorExpr, ParameterInfo[] parameters, Type dstType)
        {
            var objExpr = Expression.Parameter(typeof(Value), "obj");
            var varExpr = Expression.Variable(dstType, "output");
            var target = Expression.Label(dstType);

            var parametersExpr = parameters
                .Select(p => CallDecode(CallGetValue(objExpr, p), p.ParameterType, p.GetOverrideType()));

            var ignoreProperties = parameters.Select(p => p.GetName());

            var block = Expression.Block(
                new ParameterExpression[] { varExpr },
                Expression.Assign(varExpr, creatorExpr(parametersExpr)),
                AssignProperties(objExpr, varExpr, dstType, ignoreProperties),
                Expression.Label(target, varExpr)
            );

            return Expression.Lambda<Converter>(
                Expression.Convert(block, typeof(object)), objExpr
            ).Compile();
        }

        /*
         * Func<Value, object> = value =>
         * {
         *      DstType output = new DstType();
         *
         *      output.Field1 = (T1)Decode( GetValue(value, "attrib1", defaultValue1), typeof(T1) );
         *      output.Field2 = (T2)Decode( GetValue(value, "attrib2", defaultValue2), typeof(T2) );
         *
         *      return output;
         * }
         */
        private static Converter FromValueType(Type dstType)
        {
            return Create(
                _ => Expression.New(dstType),
                new ParameterInfo[0],
                dstType
            );
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
        private static Converter FromConstructor(ConstructorInfo constructor, Type dstType)
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
        private static Converter FromMethodCreator(MethodInfo creator, Type dstType)
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
        private static Expression CallDecode(Expression objExpr, Type type, Type overrideType = null)
        {
            var decodeMethod = typeof(DecoderImpl).GetMethod(
                "Decode", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Value), typeof(Type), typeof(Type) }, null
            );

            var decodeExpr = Expression.Call(
                decodeMethod,
                new Expression[] { objExpr, Expression.Constant(type), Expression.Constant(overrideType, typeof(Type)) }
            );

            return type.GetTypeInfo().IsValueType
                ? Expression.Unbox(decodeExpr, type)
                : Expression.Convert(decodeExpr, type);
        }

        private static Expression CallGetValue(Expression objExpr, MemberInfo member, Type memberType)
        {
            var field = member.GetCustomAttribute<FaunaFieldAttribute>();
            var defaultValue = field != null ? Encoder.Encode(field.DefaultValue) : null;

            var hasDefaultValue = field != null && field.DefaultValue != null;
            if (!hasDefaultValue && memberType.GetTypeInfo().IsValueType)
            {
                defaultValue = Encoder.Encode(Activator.CreateInstance(memberType));
            }

            if (defaultValue == null)
            {
                defaultValue = NullV.Instance;
            }

            if (member.GetCustomAttribute<FaunaIgnoreAttribute>() != null)
            {
                return Expression.Constant(defaultValue, typeof(Value));
            }

            return CallGetValue(objExpr, member.GetName(), defaultValue);
        }

        private static Expression CallGetValue(Expression objExpr, ParameterInfo parameter)
        {
            var field = parameter.GetCustomAttribute<FaunaFieldAttribute>();
            var hasDefaultValue = field != null && field.DefaultValue != null;

            var defaultValue = hasDefaultValue
                ? Encoder.Encode(field.DefaultValue)
                : Encoder.Encode(parameter.DefaultValue);

            if (!hasDefaultValue && parameter.ParameterType.GetTypeInfo().IsValueType)
            {
                defaultValue = Encoder.Encode(Activator.CreateInstance(parameter.ParameterType));
            }

            if (defaultValue == null)
            {
                defaultValue = NullV.Instance;
            }

            if (parameter.GetCustomAttribute<FaunaIgnoreAttribute>() != null)
            {
                return Expression.Constant(defaultValue, typeof(Value));
            }

            return CallGetValue(objExpr, parameter.GetName(), defaultValue);
        }

        /*
         * GetValue( objExpr, property, defaultValue )
         */
        private static Expression CallGetValue(Expression objExpr, string property, Value defaultValue)
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

        private static Value GetValue(ObjectV obj, string property, Value defaultValue)
        {
            var values = obj.Value;

            Value value;
            if (values.TryGetValue(property, out value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
