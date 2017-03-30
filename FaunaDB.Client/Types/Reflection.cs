using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FaunaDB.Types
{
    internal static class Reflection
    {
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        public static string GetName(this ParameterInfo parameter)
        {
            var field = parameter.GetCustomAttribute<FaunaFieldAttribute>();

            if (field != null)
                return field.Name;

            return parameter.Name;
        }

        public static string GetName(this MemberInfo member)
        {
            var field = member.GetCustomAttribute<FaunaFieldAttribute>();

            if (field != null)
                return field.Name;

            return member.Name;
        }

        public static bool Has<T>(this MemberInfo member) where T : Attribute =>
            member.GetCustomAttribute<T>() != null;

        public static bool Is(this Type type, Type other)
        {
            if (type.GetGenericTypeDefinition() == other)
                return true;

            return type.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == other);
        }

    }
}
