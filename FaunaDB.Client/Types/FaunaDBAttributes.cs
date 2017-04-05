using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Maps a property/field member to FaunaDB object property while encoding an object.
    /// If this attribute if not specified the property/field name will be used instead.
    /// </summary>
    /// <example>
    /// class User
    /// {
    ///     [Field("user_name")]
    ///     public string UserName { get; set; }
    ///
    ///     [Field("passwd")]
    ///     public string Password { get; set; }
    /// }
    ///
    /// var user = new User { UserName = "john", Password = "s3cr3t" };
    ///
    /// var encoded = Encoder.Encode(user);
    ///
    /// //encoded will be equivalent to:
    ///
    /// ObjectV.With("user_name", "john", "passwd", "s3cr3t")
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; }

        public FieldAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Instruct the encoder to not encode the specified member.
    /// </summary>
    /// <example>
    /// class User
    /// {
    ///     [Field("user_name")]
    ///     public string UserName { get; set; }
    ///
    ///     [Ignore]
    ///     public string Password { get; set; }
    /// }
    ///
    /// var user = new User { UserName = "john", Password = "s3cr3t" };
    ///
    /// var encoded = Encoder.Encode(user);
    ///
    /// //encoded will be equivalent to:
    ///
    /// ObjectV.With("user_name", "john")
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    { }
}
