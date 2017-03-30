using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FaunaDB.Types;
using FaunaDB.Attributes;
using System.Linq.Expressions;

namespace FaunaDB.Encoding
{
    using Converter = Func<EncoderImpl, object, Value>;

    public static class Encoder
    {
        public static Value Encode(object obj) =>
            new EncoderImpl().Encode(obj);
    }

    class EncoderImpl
    {
        static readonly Dictionary<Type, Converter> converters = new Dictionary<Type, Converter>();

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

        Value EncodeIntern(object obj)
        {
            var type = obj.GetType();

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

        Value WrapObj(object obj)
        {
            var type = obj.GetType();

            Converter converter;

            lock (converters)
            {
                if (!converters.TryGetValue(type, out converter))
                {
                    converter = CreateConverter(type);
                    converters.Add(type, converter);
                }
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

            var fields = GetAllFields(srcType)
                .Where(field => !Has<CompilerGeneratedAttribute>(field) && !Has<IgnoreAttribute>(field))
                .Select(field => AddElementToDic(encoderExpr, srcType, field, addMethod, objExpr));

            var properties = srcType
                .GetProperties()
                .Where(parameter => parameter.CanRead && !Has<IgnoreAttribute>(parameter))
                .Select(parameter => AddElementToDic(encoderExpr, srcType, parameter, addMethod, objExpr));

            var newDictExpr = Expression.ListInit(Expression.New(dictType), fields.Concat(properties));
            var lambda = Expression.Lambda<Converter>(
                Expression.New(objectVConstructor, newDictExpr),
                encoderExpr,
                objExpr
            );

            return lambda.Compile();
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
            var keyExpr = Expression.Constant(GetMemberName(member));
            var valueExpr = CallEncode(encoderExpr, AccessMember(srcType, member, objExpr));

            return Expression.ElementInit(
                addMethod, keyExpr, valueExpr
            );
        }

        static bool Has<T>(MemberInfo member) where T : Attribute =>
            member.GetCustomAttribute<T>() != null;

        static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        static string GetMemberName(MemberInfo member)
        {
            var field = member.GetCustomAttribute<FieldAttribute>();

            if (field != null)
                return field.Name;

            return member.Name;
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
