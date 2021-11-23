using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Maps a property/field member constructor parameter to FaunaDB object property while encoding/deconding an object.
    /// If this attribute is not specified the property/field name constructor parameter name will be used instead.
    /// </summary>
    /// <example>
    /// class Car
    /// {
    ///     [FaunaField("model")]
    ///     public string Model { get; set; }
    ///
    ///     [FaunaField("manufacturer")]
    ///     public string Manufacturer { get; set; }
    /// }
    ///
    /// var car = new Car { Model = "DeLorean DMC-12", Manufacturer = "DeLorean Motor Company" };
    ///
    /// var encoded = Encoder.Encode(car);
    ///
    /// //encoded will be equivalent to:
    ///
    /// ObjectV.With("model", "DeLorean DMC-12", "manufacturer", "DeLorean Motor Company")
    ///
    /// class Product
    /// {
    ///     private string description;
    ///     private double price;
    ///
    ///     [FaunaConstructor]
    ///     public Product([FaunaField("Description")] string description, [FaunaField("Price")] double price)
    ///     {
    ///         this.description = description;
    ///         this.price = price;
    ///     }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FaunaFieldAttribute : Attribute
    {
        /// <summary>
        /// The name of the property when encoding/decoding objects.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The default value used for a missing property when decoding an object.
        /// </summary>
        public object DefaultValue { get; set; }

        public FaunaFieldAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Instruct the encoder that this <see cref="DateTime"/> property should always be
    /// converted to <see cref="Types.DateV"/>
    /// </summary>
    /// <example>
    /// class User
    /// {
    ///     [FaunaDate]
    ///     public DateTime Birthday { get; set; }
    /// }
    ///
    /// var user = new User { Birthday = DateTime.Now };
    ///
    /// var encoded = Encoder.Encode(user);
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FaunaDate : Attribute { }

    /// <summary>
    /// Instruct the encoder that this <see cref="DateTime"/> property should always be
    /// converted to <see cref="Types.TimeV"/>
    /// </summary>
    /// <example>
    /// class User
    /// {
    ///     [FaunaTime]
    ///     public DateTime TimeSignedUp { get; set; }
    /// }
    ///
    /// var user = new User { TimeSignedUp = DateTime.Now };
    ///
    /// var encoded = Encoder.Encode(user);
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FaunaTime : Attribute
    { }

    /// <summary>
    /// Instruct the encoder that this object should be treated as a string when
    /// stored in Fauna. The Encoder will call the object or primative type's
    /// .ToString() method. The Decoder will attempt to create an object with the
    /// constructor with a single string parameter. If it does not have that
    /// constructor the Decoder will fail. For primatives the Decoder will attempt
    /// to use standard system conversions to convert the string back to the
    /// primative type.
    /// </summary>
    /// <example>
    /// class Website
    /// {
    ///     [FaunaString]
    ///     public Uri MyLink { get; set; }
    /// }
    ///
    /// var web = new Website { MyLink = new Uri("http://fauanadb.com") };
    ///
    /// var encodedWebsite = Encoder.Encode(web);
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FaunaString : Attribute
    { }

    /// <summary>
    /// Instruct the encoder to not encode the specified member.
    /// </summary>
    /// <example>
    /// class User
    /// {
    ///     [FaunaField("user_name")]
    ///     public string UserName { get; set; }
    ///
    ///     [FaunaIgnore]
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
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FaunaIgnoreAttribute : Attribute
    { }

    /// <summary>
    /// Instruct the decoder which constructor to use when decoding an object.
    /// It can also be used in a public static method instead of a constructor.
    /// This attribute can only be used once per class.
    /// </summary>
    /// <example>
    /// class Product
    /// {
    ///     public string Description { get; set; }
    ///     public double Price { get; set; }
    ///
    ///     [FaunaConstructor]
    ///     public Product(string description, double price)
    ///     {
    ///         Description = description;
    ///         Price = price;
    ///     }
    /// }
    ///
    /// class Order
    /// {
    ///     public string Number { get; set; }
    ///     public List&lt;Product&gt; Products { get; set; }
    ///
    ///     [FaunaConstructor]
    ///     public static Order CreateOrder(string number, List&lt;Product&gt; products)
    ///     {
    ///         return new Order
    ///         {
    ///             Number = number,
    ///             Products = products
    ///         }
    ///     }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class FaunaConstructorAttribute : Attribute
    { }

    /// <summary>
    /// Instruct the encoder/decoder to rename the annotated enum field.
    /// If not used the method <see cref="Enum.GetName(Type, object)"/> will be used instead.
    /// </summary>
    /// <example>
    /// enum CpuTypes
    /// {
    ///     [FaunaEnum("x86_32")] X86,
    ///     [FaunaEnum("x86_64")] X86_64,
    ///     ARM,
    ///     MIPS
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Field)]
    public class FaunaEnum : Attribute
    {
        /// <summary>
        /// Alias used to encode/decode the enum value
        /// </summary>
        public string Alias { get; }

        public FaunaEnum(string alias)
        {
            Alias = alias;
        }
    }
}
