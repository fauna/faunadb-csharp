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

            return type.GetFields(flags).Concat(GetAllFields(type.GetTypeInfo().BaseType));
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

        public static Type GetOverrideType(this MemberInfo member)
        {
            var date = member.GetCustomAttribute<FaunaDate>();
            var time = member.GetCustomAttribute<FaunaTime>();

            if (date != null && time != null)
                throw (new InvalidOperationException("Can't use both FaunaDate and FaunaTime on the same property."));
            
            if (time != null)
                return typeof(TimeV);
            else if (date != null)
                return typeof(DateV);

            return null;
        }

        public static bool Has<T>(this MemberInfo member) where T : Attribute =>
            member.GetCustomAttribute<T>() != null;

        public static bool Is(this Type type, Type other)
        {
            if (type.GetGenericTypeDefinition() == other)
                return true;

            return type.GetInterfaces()
                       .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == other);
        }

        public static MethodInfo GetMethod(this Type typeInfo, string name, BindingFlags bindingAttr, object binder, Type[] types, object modifiers)
        {
            return typeInfo.GetMethods(bindingAttr)
                           .Where(method => method.Name == name)
                           .Where(method => method.GetParameters().Select(p => p.ParameterType).SequenceEqual(types))
                           .Single();
        }

        public static ConstructorInfo GetConstructor(this Type typeInfo, BindingFlags bindingAttr, object binder, Type[] types, object modifiers)
        {
            return typeInfo.GetConstructors(bindingAttr)
                           .Where(method => method.GetParameters().Select(p => p.ParameterType).SequenceEqual(types))
                           .Single();
        }
    }
}
