using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Naming strategy for <see cref="Value"/> encoder/decoder.
    /// </summary>
    public abstract class NamingStrategy
    {
        public static NamingStrategy Default { get; set; } = new DefaultNamingStrategy();

        public abstract string ResolvePropertyName(string name);
    }

    public sealed class DefaultNamingStrategy : NamingStrategy
    {
        public override string ResolvePropertyName(string name)
        {
            return name;
        }
    }

    public sealed class CamelCaseNamingStrategy : NamingStrategy
    {
        public override string ResolvePropertyName(string name) =>
            System.Text.RegularExpressions.Regex.Replace(name, @"([A-Z])([A-Z]+|[a-z0-9_]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                } );
    }
}
