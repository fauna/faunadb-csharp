using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FaunaDB.Query;
using FaunaDB.Types;
using FaunaDB.Attributes;

namespace FaunaDB.Encoding
{
    public static class Encoder
    {
        public static Expr Encode(object obj) =>
            new EncoderImpl().Encode(obj);
    }

    class EncoderImpl
    {
        Stack<object> stack = new Stack<object>();

        public Expr Encode(object obj)
        {
            if (obj == null)
                return NullV.Instance;

            if (typeof(Expr).IsInstanceOfType(obj))
                return (Expr)obj;

            if (stack.Contains(obj))
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

        Expr EncodeIntern(object obj)
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

                    if (type.IsArray)
                        return WrapEnumerable((IEnumerable)obj);

                    if (typeof(IDictionary).IsInstanceOfType(obj))
                        return WrapDictionary((IDictionary)obj);

                    if (typeof(IEnumerable).IsInstanceOfType(obj))
                        return WrapEnumerable((IEnumerable)obj);

                    return WrapObj(obj);
            }

            throw new InvalidOperationException($"Could not encode object `{obj}`");
        }

        Expr WrapDateTime(DateTime date)
        {
            if (date.Ticks % (24 * 60 * 60 * 10000) > 0)
                return new TimeV(date);

            return new DateV(date);
        }

        Expr WrapEnumerable(IEnumerable array)
        {
            var ret = new List<Expr>();

            foreach (var value in array)
                ret.Add(Encode(value));

            return new UnescapedArray(ret);
        }

        Expr WrapDictionary(IDictionary dic)
        {
            var ret = new Dictionary<string, Expr>();

            foreach (DictionaryEntry entry in dic)
                ret.Add(entry.Key.ToString(), Encode(entry.Value));

            return UnescapedObject.With("object", new UnescapedObject(ret));
        }

        Expr WrapObj(object obj)
        {
            var type = obj.GetType();

            var fields = GetAllFields(type)
                .Where(f => !Has<CompilerGeneratedAttribute>(f) && !Has<IgnoreAttribute>(f))
                .ToDictionary(f => GetMemberName(f), f => f.GetValue(obj));

            var properties = type
                .GetProperties()
                .Where(p => p.CanRead && !Has<IgnoreAttribute>(p))
                .ToDictionary(p => GetMemberName(p), p => p.GetValue(obj));

            var dictionary = fields
                .Concat(properties)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            return WrapDictionary(dictionary);
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
    }
}
