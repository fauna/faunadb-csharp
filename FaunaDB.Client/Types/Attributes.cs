using System;

namespace FaunaDB.Types
{
    /// <summary>
    /// Maps a property/field member constructor parameter to FaunaDB object property while encoding/deconding an object.
    /// If this attribute if not specified the property/field name constructor parameter name will be used instead.
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
    /// Instruct the decoder wich constructor to use when decoding a object.
    /// It can also be used in a public static method instead in a constructor.
    /// That attribute can only be used once per class.
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
}
