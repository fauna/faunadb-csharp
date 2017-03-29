using System;

namespace FaunaDB.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; }

        public FieldAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    { }
}
