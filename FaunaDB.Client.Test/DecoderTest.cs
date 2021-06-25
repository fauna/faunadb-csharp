using System;
using System.Collections;
using System.Collections.Generic;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Types.Decoder;

namespace Test
{
    [TestFixture]
    public class DecoderTest
    {
        [Test]
        public void TestPrimitive()
        {
            Assert.AreEqual("a string", Decode<string>("a string"));
            Assert.AreEqual(true, Decode<bool>(true));
            Assert.AreEqual(null, Decode<object>(NullV.Instance));

            Assert.AreEqual(10, Decode<long>(10));
            Assert.AreEqual(10, Decode<int>(10));
            Assert.AreEqual(10, Decode<short>(10));
            Assert.AreEqual(10, Decode<sbyte>(10));

            Assert.AreEqual(10, Decode<ulong>(10));
            Assert.AreEqual(10, Decode<uint>(10));
            Assert.AreEqual(10, Decode<ushort>(10));
            Assert.AreEqual(10, Decode<byte>(10));
            Assert.AreEqual(10, Decode<char>(10));

            Assert.AreEqual(3.14F, Decode<float>(3.14));
            Assert.AreEqual(3.14M, Decode<double>(3.14));
            Assert.AreEqual(3.14M, Decode<decimal>(3.14));

            Assert.AreEqual(new DateTime(2001, 1, 1), Decode<DateTime>(new DateV("2001-01-01")));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 10, 30, 123), Decode<DateTime>(new TimeV("2000-01-01T01:10:30.123Z")));

            Assert.AreEqual(new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero), Decode<DateTimeOffset>(new DateV("2001-01-01")));
            Assert.AreEqual(new DateTimeOffset(2000, 1, 1, 1, 10, 30, 123, TimeSpan.Zero), Decode<DateTimeOffset>(new TimeV("2000-01-01T01:10:30.123Z")));

            Assert.AreEqual(new byte[] { 1, 2, 3 }, Decode<byte[]>(new BytesV(1, 2, 3)));
        }

        private struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private struct Rect
        {
            public Point UpperLeft;
            public Point BottomRight;

            public Rect(Point upperLeft, Point bottomRight)
            {
                UpperLeft = upperLeft;
                BottomRight = bottomRight;
            }
        }

        [Test]
        public void TestNullToValueTypes()
        {
            Assert.AreEqual(0, Decode<long>(NullV.Instance));
            Assert.AreEqual(0, Decode<int>(NullV.Instance));
            Assert.AreEqual(0, Decode<short>(NullV.Instance));
            Assert.AreEqual(0, Decode<sbyte>(NullV.Instance));

            Assert.AreEqual(0, Decode<ulong>(NullV.Instance));
            Assert.AreEqual(0, Decode<uint>(NullV.Instance));
            Assert.AreEqual(0, Decode<ushort>(NullV.Instance));
            Assert.AreEqual(0, Decode<byte>(NullV.Instance));
            Assert.AreEqual(0, Decode<char>(NullV.Instance));

            Assert.AreEqual(0F, Decode<float>(NullV.Instance));
            Assert.AreEqual(0F, Decode<double>(NullV.Instance));
            Assert.AreEqual(0M, Decode<decimal>(NullV.Instance));

            Assert.AreEqual(default(Point), Decode<Point>(NullV.Instance));
            Assert.AreEqual(default(Point), Decode<Point>(ObjectV.Empty));
            Assert.AreEqual(default(Point), Decode<Point>(ObjectV.With("x", NullV.Instance, "y", NullV.Instance)));

            Assert.AreEqual(default(Rect), Decode<Rect>(NullV.Instance));
            Assert.AreEqual(default(Rect), Decode<Rect>(ObjectV.Empty));
            Assert.AreEqual(default(Rect), Decode<Rect>(ObjectV.With("UpperLeft", NullV.Instance, "BottomRight", NullV.Instance)));

            Assert.AreEqual(0, Decode<long>(null));
            Assert.AreEqual(0, Decode<int>(null));
            Assert.AreEqual(0, Decode<short>(null));
            Assert.AreEqual(0, Decode<sbyte>(null));

            Assert.AreEqual(0, Decode<ulong>(null));
            Assert.AreEqual(0, Decode<uint>(null));
            Assert.AreEqual(0, Decode<ushort>(null));
            Assert.AreEqual(0, Decode<byte>(null));
            Assert.AreEqual(0, Decode<char>(null));

            Assert.AreEqual(0F, Decode<float>(null));
            Assert.AreEqual(0D, Decode<double>(null));
            Assert.AreEqual(0M, Decode<decimal>(null));

            Assert.AreEqual(default(Point), Decode<Point>(null));
            Assert.AreEqual(default(Rect), Decode<Rect>(null));
        }

        [Test]
        public void TestNullToClassTypes()
        {
            Assert.AreEqual(default(string), Decode<string>(NullV.Instance));
            Assert.AreEqual(default(string), Decode<string>(null));

            Assert.AreEqual(default(Product), Decode<Product>(NullV.Instance));
            Assert.AreEqual(default(Product), Decode<Product>(null));
        }

        [Test]
        public void TestIntegerLimits()
        {
            // signed values
            Assert.Throws<OverflowException>(() => Decode<sbyte>((long)sbyte.MinValue - 1));
            Assert.Throws<OverflowException>(() => Decode<short>((long)short.MinValue - 1));
            Assert.Throws<OverflowException>(() => Decode<int>((long)int.MinValue - 1));

            Assert.Throws<OverflowException>(() => Decode<sbyte>((long)sbyte.MaxValue + 1));
            Assert.Throws<OverflowException>(() => Decode<short>((long)short.MaxValue + 1));
            Assert.Throws<OverflowException>(() => Decode<int>((long)int.MaxValue + 1));

            // unsigned vlaues
            Assert.Throws<OverflowException>(() => Decode<char>((long)char.MaxValue + 1));
            Assert.Throws<OverflowException>(() => Decode<byte>((long)byte.MaxValue + 1));
            Assert.Throws<OverflowException>(() => Decode<ushort>((long)ushort.MaxValue + 1));
            Assert.Throws<OverflowException>(() => Decode<uint>((long)uint.MaxValue + 1));
        }

        [Test]
        public void TestCollection()
        {
            Assert.AreEqual(new byte[] { 1, 2, 3 }, Decode<byte[]>(new BytesV(1, 2, 3)));

            Assert.AreEqual(new int[] { 1, 2, 3 }, Decode<int[]>(ArrayV.Of(1, 2, 3)));
            Assert.AreEqual(new long[] { 1, 2, 3 }, Decode<long[]>(ArrayV.Of(1, 2, 3)));

            // interfaces
            Assert.That(
                Decode<IDictionary<string, Value>>(ObjectV.With("a", "b")),
                Is.EquivalentTo(new Dictionary<string, Value> { { "a", "b" } })
            );

            Assert.That(
                Decode<IList<string>>(ArrayV.Of("a", "b")),
                Is.EquivalentTo(new List<string> { "a", "b" })
            );

            Assert.That(
                Decode<IEnumerable<string>>(ArrayV.Of("a", "b")),
                Is.EquivalentTo(new List<string> { "a", "b" })
            );

            Assert.That(
                Decode<ISet<string>>(ArrayV.Of("a", "b")),
                Is.EquivalentTo(new HashSet<string> { "a", "b" })
            );

            // concrete types
            Assert.That(
                Decode<Dictionary<string, Value>>(ObjectV.With("a", "b")),
                Is.EquivalentTo(new Dictionary<string, Value> { { "a", "b" } })
            );

            Assert.That(
                Decode<List<string>>(ArrayV.Of("a", "b")),
                Is.EquivalentTo(new List<string> { "a", "b" })
            );

            Assert.That(
                Decode<HashSet<string>>(ArrayV.Of("a", "b")),
                Is.EquivalentTo(new HashSet<string> { "a", "b" })
            );

            Assert.That(
                Decode<SortedSet<int>>(ArrayV.Of(1, 2, 3)),
                Is.EquivalentTo(new SortedSet<int> { 1, 2, 3 })
            );
        }

        private class Product
        {
            public string Description { get; set; }

            public double Price { get; set; }

            public DateTime Created { get; set; }

            public DateTimeOffset LastUpdated { get; set; }

            [FaunaConstructor]
            public Product([FaunaField("Description")] string description,
                           [FaunaField("Price")] double price,
                           [FaunaField("Created")] DateTime created,
                           [FaunaField("LastUpdated")] DateTimeOffset lastUpdated)
            {
                Description = description;
                Price = price;
                Created = created;
                LastUpdated = lastUpdated;
            }

            public override bool Equals(object obj)
            {
                var product = obj as Product;

                if (product == null)
                {
                    return false;
                }

                return Description == product.Description &&
                    Price == product.Price &&
                    Created == product.Created &&
                    LastUpdated == product.LastUpdated;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [Test]
        public void TestUserClassConstructor()
        {
            var person = Decode<Product>(ObjectV.With("Description", "Laptop", "Price", 999.9, "Created", DateV.Of("2000-01-01"), "LastUpdated", TimeV.Of("2001-01-02T11:00:00.123Z")));
            Assert.AreEqual("Laptop", person.Description);
            Assert.AreEqual(999.9, person.Price);
            Assert.AreEqual(new DateTime(2000, 1, 1), person.Created);
            Assert.AreEqual(new DateTimeOffset(2001, 1, 2, 11, 0, 0, 123, TimeSpan.Zero), person.LastUpdated);
        }

        private class Order
        {
            [FaunaField("number")]
            public string Number { get; set; }

            [FaunaField("products")]
            public List<Product> Products { get; set; }

            [FaunaField("vouchers")]
            public ISet<int> Vouchers { get; set; }

            [FaunaField("addresses")]
            public SortedSet<string> Addresses { get; set; }
        }

        [Test]
        public void TestUserClassDefaultConstructor()
        {
            var product1 = ObjectV.With("Description", "Laptop", "Price", 999.9);
            var product2 = ObjectV.With("Description", "Mouse", "Price", 9.9);

            var order = Decode<Order>(ObjectV.With(
                "number", "XXXYYY999",
                "products", ArrayV.Of(product1, product2),
                "vouchers", ArrayV.Of(111111, 222222),
                "addresses", ArrayV.Of("744 Montgomery Street Suite 200")
            ));

            Assert.AreEqual("XXXYYY999", order.Number);

            Assert.AreEqual(2, order.Products.Count);

            Assert.AreEqual("Laptop", order.Products[0].Description);
            Assert.AreEqual(999.9, order.Products[0].Price);

            Assert.AreEqual("Mouse", order.Products[1].Description);
            Assert.AreEqual(9.9, order.Products[1].Price);

            Assert.IsTrue(order.Vouchers.Contains(111111));
            Assert.IsTrue(order.Vouchers.Contains(222222));

            Assert.IsTrue(order.Addresses.Contains("744 Montgomery Street Suite 200"));
        }

        private class OrderWithCustomer : Order
        {
            [FaunaField("customer")]
            public string Customer { get; set; }
        }

        [Test]
        public void TestUserClassInheritance()
        {
            var product = ObjectV.With("Description", "Laptop", "Price", 999.9);

            var order = Decode<OrderWithCustomer>(
                ObjectV.With("customer", "John", "number", "XXXYYY999", "products", ArrayV.Of(product))
            );
            Assert.AreEqual("John", order.Customer);
            Assert.AreEqual("XXXYYY999", order.Number);

            Assert.AreEqual(1, order.Products.Count);

            Assert.AreEqual("Laptop", order.Products[0].Description);
            Assert.AreEqual(999.9, order.Products[0].Price);
        }

        private class MethodCreator
        {
            public string Field { get; }

            public MethodCreator(string field)
            {
                Field = field;
            }

            [FaunaConstructor]
            public static MethodCreator Create(string field) =>
                new MethodCreator(field);
        }

        [Test]
        public void TestMethodCreator()
        {
            var obj = Decode<MethodCreator>(ObjectV.With("field", "a string"));
            Assert.AreEqual("a string", obj.Field);
        }

        private class DefaultValueOnProperty
        {
            [FaunaField("Field", DefaultValue = "a default value on property")]
            public string Field { get; set; }
        }

        private class DefaultValueOnConstructor
        {
            public string Field { get; }

            [FaunaConstructor]
            public DefaultValueOnConstructor([FaunaField("Field", DefaultValue = "a default value on constructor")] string field)
            {
                Field = field;
            }
        }

        private class DefaultValueOnMethodCreator
        {
            public string Field { get; private set; }

            [FaunaConstructor]
            public static DefaultValueOnMethodCreator Create([FaunaField("Field", DefaultValue = "a default value on method creator")] string field)
            {
                return new DefaultValueOnMethodCreator
                {
                    Field = field,
                };
            }
        }

        [Test]
        public void TestDefaultValues()
        {
            Assert.AreEqual(
                "a default value on property",
                Decode<DefaultValueOnProperty>(ObjectV.Empty).Field
            );

            Assert.AreEqual(
                "a default value on constructor",
                Decode<DefaultValueOnConstructor>(ObjectV.Empty).Field
            );

            Assert.AreEqual(
                "a default value on method creator",
                Decode<DefaultValueOnMethodCreator>(ObjectV.Empty).Field
            );
        }

        private class MissingPropertiesOnConstructor
        {
            public string Field1 { get; set; }

            [FaunaField("a_missing_field")]
            private string field2 = default(string);

            public string Field2 { get { return field2; } }

            [FaunaField("a_missing_property")]
            public string Field3 { get; set; }

            [FaunaConstructor]
            public MissingPropertiesOnConstructor([FaunaField("Field1")] string field1)
            {
                Field1 = field1;
            }
        }

        [Test]
        public void TestFillMissingPropertiesOnConstructor()
        {
            var obj = Decode<MissingPropertiesOnConstructor>(
                ObjectV.With("Field1", "field1", "a_missing_field", "field2", "a_missing_property", "field3")
            );
            Assert.AreEqual("field1", obj.Field1);
            Assert.AreEqual("field2", obj.Field2);
            Assert.AreEqual("field3", obj.Field3);
        }

        private class MissingPropertiesOnMethodCreator
        {
            public string Field1 { get; set; }

            [FaunaField("a_missing_field")]
            private string Field2Missing = default(string);

            public string Field2 { get { return Field2Missing; } }

            [FaunaField("a_missing_property")]
            public string Field3 { get; set; }

            [FaunaConstructor]
            public static MissingPropertiesOnMethodCreator Create([FaunaField("Field1")] string field1)
            {
                return new MissingPropertiesOnMethodCreator
                {
                    Field1 = field1,
                };
            }
        }

        [Test]
        public void TestFillMissingPropertiesOnMethodCreator()
        {
            var obj = Decode<MissingPropertiesOnMethodCreator>(
                ObjectV.With("Field1", "field1", "a_missing_field", "field2", "a_missing_property", "field3")
            );
            Assert.AreEqual("field1", obj.Field1);
            Assert.AreEqual("field2", obj.Field2);
            Assert.AreEqual("field3", obj.Field3);
        }

        private class FaunaTypes
        {
            public StringV StringV_Value = StringV.Of("a string");
            public LongV LongV_Value = LongV.Of(123);
            public BooleanV BooleanV_Value = BooleanV.True;
            public DoubleV DoubleV_Value = DoubleV.Of(3.14);
            public Value NullV_Value = NullV.Instance;
            public DateV DateV_Value = new DateV("2001-01-01");
            public TimeV TimeV_Value = new TimeV("2000-01-01T01:10:30.123Z");
            public RefV RefV_Value = new RefV("collections");
            public SetRefV SetRefV_Value = new SetRefV(new Dictionary<string, Value>());
            public ArrayV ArrayV_Value = ArrayV.Of(1, 2, 3);
            public ObjectV ObjectV_Value = ObjectV.With("a", "b");
            public BytesV BytesV_Value = new BytesV(1, 2, 3, 4);

            public override int GetHashCode() => 0;

            public override bool Equals(object obj)
            {
                FaunaTypes other = obj as FaunaTypes;
                return other != null &&
                    StringV_Value == other.StringV_Value &&
                    LongV_Value == other.LongV_Value &&
                    BooleanV_Value == other.BooleanV_Value &&
                    DoubleV_Value == other.DoubleV_Value &&
                    NullV_Value == other.NullV_Value &&
                    DateV_Value == other.DateV_Value &&
                    TimeV_Value == other.TimeV_Value &&
                    RefV_Value == other.RefV_Value &&
                    RefV_Value == other.RefV_Value &&
                    ArrayV_Value == other.ArrayV_Value &&
                    ObjectV_Value == other.ObjectV_Value &&
                    BytesV_Value == other.BytesV_Value;
            }
        }

        [Test]
        public void TestFaunaTypes()
        {
            Assert.AreEqual(
                new FaunaTypes(),
                Decode<FaunaTypes>(ObjectV.With(new Dictionary<string, Value>
                {
                    {"StringV_Value", "a string" },
                    {"LongV_Value", 123 },
                    {"BooleanV_Value", true },
                    {"DoubleV_Value", 3.14 },
                    {"NullV_Value", NullV.Instance },
                    {"DateV_Value", new DateV("2001-01-01") },
                    {"TimeV_Value", new TimeV("2000-01-01T01:10:30.123Z") },
                    {"RefV_Value", new RefV("collections") },
                    {"SetRefV_Value", new SetRefV(new Dictionary<string, Value>()) },
                    {"ArrayV_Value", ArrayV.Of(1, 2, 3) },
                    {"ObjectV_Value", ObjectV.With("a", "b") },
                    {"BytesV_Value", new BytesV(1, 2, 3, 4) },
                }))
            );
        }

        [Test]
        public void TestCastNumbers()
        {
            // to long
            Assert.AreEqual(10L, Decode<long>(10L));
            Assert.AreEqual(10L, Decode<long>(10d));
            Assert.AreEqual(10L, Decode<long>(10));
            Assert.AreEqual(10L, Decode<long>("10"));

            // to double
            Assert.AreEqual(10d, Decode<double>(10L));
            Assert.AreEqual(10d, Decode<double>(10d));
            Assert.AreEqual(10d, Decode<double>(10));
            Assert.AreEqual(10d, Decode<double>("10"));

            // to int
            Assert.AreEqual(10, Decode<int>(10L));
            Assert.AreEqual(10, Decode<int>(10d));
            Assert.AreEqual(10, Decode<int>(10));
            Assert.AreEqual(10, Decode<int>("10"));

            // to short
            Assert.AreEqual((short)10, Decode<short>(10L));
            Assert.AreEqual((short)10, Decode<short>(10d));
            Assert.AreEqual((short)10, Decode<short>(10));
            Assert.AreEqual((short)10, Decode<short>("10"));

            var time = DateTime.Now;

            Assert.AreEqual(
                new Product("MacBook", 10, time, time),
                Decode<Product>(ObjectV.With("Description", "MacBook", "Price", 10, "Created", time, "LastUpdated", time))
            );
        }

        [Test]
        public void TestCastErrors()
        {
            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.StringV' to 'FaunaDB.Types.ObjectV'.",
                Assert.Throws<InvalidCastException>(() => Decode<ObjectV>("a string")).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.StringV' to 'FaunaDB.Types.LongV'.",
                Assert.Throws<InvalidCastException>(() => Decode<LongV>("a string")).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.ObjectV' to 'FaunaDB.Types.RefV'.",
                Assert.Throws<InvalidCastException>(() => Decode<RefV>(ObjectV.Empty)).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'Double' to 'Char'.",
                Assert.Throws<InvalidCastException>(() => Decode<char>(3.14)).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.LongV' to 'System.Collections.Generic.List`1[System.Int32]'.",
                Assert.Throws<InvalidCastException>(() => Decode<List<int>>(10)).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.LongV' to 'System.Collections.Generic.Dictionary`2[System.String,System.Int32]'.",
                Assert.Throws<InvalidCastException>(() => Decode<Dictionary<string, int>>(10)).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.StringV' to 'System.Collections.Generic.IList`1[System.Int32]'.",
                Assert.Throws<InvalidCastException>(() => Decode<IList<int>>("a string")).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.StringV' to 'System.Collections.Generic.IDictionary`2[System.String,System.Int32]'.",
                Assert.Throws<InvalidCastException>(() => Decode<IDictionary<string, int>>("a string")).Message
            );

            Assert.AreEqual(
                "Invalid cast from 'FaunaDB.Types.StringV' to 'System.Int32[]'.",
                Assert.Throws<InvalidCastException>(() => Decode<int[]>("a string")).Message
            );
        }

        [Test]
        public void TestErrors()
        {
            Assert.AreEqual(
                "The type System.Collections.IList is not generic",
                Assert.Throws<InvalidOperationException>(() => Decode<IList>(ArrayV.Of("a string"))).Message
            );

            Assert.AreEqual(
                "The type System.Collections.IDictionary is not generic",
                Assert.Throws<InvalidOperationException>(() => Decode<IDictionary>(ObjectV.With("key", "vlaue"))).Message
            );

            Assert.AreEqual(
                "No default constructor or constructor/static method annotated with attribute [FaunaConstructor] found on type `Test.DecoderTest+NoDefaultConstructor`",
                Assert.Throws<InvalidOperationException>(() => Decode<NoDefaultConstructor>("a string")).Message
            );

            Assert.AreEqual(
                "More than one static method creator found on type `Test.DecoderTest+MoreThanOneStaticCreator`",
                Assert.Throws<InvalidOperationException>(() => Decode<MoreThanOneStaticCreator>("a string")).Message
            );

            Assert.AreEqual(
                "More than one constructor found on type `Test.DecoderTest+MoreThanOneConstructor`",
                Assert.Throws<InvalidOperationException>(() => Decode<MoreThanOneConstructor>("a string")).Message
            );
        }

        private class NoDefaultConstructor
        {
            public NoDefaultConstructor(string ign) { }
        }

        private class MoreThanOneStaticCreator
        {
            [FaunaConstructor]
            public static MoreThanOneStaticCreator Creator1() { return null; }

            [FaunaConstructor]
            public static MoreThanOneStaticCreator Creator2() { return null; }
        }

        private class MoreThanOneConstructor
        {
            [FaunaConstructor]
            public MoreThanOneConstructor(string ign) { }

            [FaunaConstructor]
            public MoreThanOneConstructor(int ign) { }
        }

        private class IgnoreConstructorParameter
        {
            public string Property1 { get; }

            public string Property2 { get; }

            [FaunaConstructor]
            public IgnoreConstructorParameter(string property1, [FaunaIgnore] string property2 = "property2")
            {
                Property1 = property1;
                Property2 = property2;
            }
        }

        [Test]
        public void TestIgnoreConstructorParameter()
        {
            var obj = Decode<IgnoreConstructorParameter>(ObjectV.With(
                "property1", "property1",
                "property2", "should not be assigned to property2"
            ));

            Assert.AreEqual("property1", obj.Property1);
            Assert.AreEqual("property2", obj.Property2);
        }

        private class MissingFields
        {
            public string NullableField { get; set; }

            public int NonNullableField { get; set; }
        }

        private class MissingFieldsOnConstructor
        {
            public string NullableField { get; }

            public int NonNullableField { get; }

            [FaunaConstructor]
            public MissingFieldsOnConstructor(string NullableField, int NonNullableField)
            {
                this.NullableField = NullableField;
                this.NonNullableField = NonNullableField;
            }
        }

        [Test]
        public void TestMissingFieldsToDefaltValues()
        {
            var missingFields1 = Decode<MissingFields>(ObjectV.Empty);
            Assert.AreEqual(default(string), missingFields1.NullableField);
            Assert.AreEqual(default(int), missingFields1.NonNullableField);

            var missingFields2 = Decode<MissingFieldsOnConstructor>(ObjectV.Empty);
            Assert.AreEqual(default(string), missingFields2.NullableField);
            Assert.AreEqual(default(int), missingFields2.NonNullableField);
        }

        private enum CpuTypes
        {
            [FaunaEnum("x86_32")]
            X86,
            [FaunaEnum("x86_64")]
            X86_64,
            ARM,
            MIPS,
        }

        [Test]
        public void TestEnumTypes()
        {
            Assert.AreEqual(CpuTypes.X86, Decode<CpuTypes>(StringV.Of("x86_32")));
            Assert.AreEqual(CpuTypes.X86_64, Decode<CpuTypes>(StringV.Of("x86_64")));
            Assert.AreEqual(CpuTypes.ARM, Decode<CpuTypes>(StringV.Of("ARM")));
            Assert.AreEqual(CpuTypes.MIPS, Decode<CpuTypes>(StringV.Of("MIPS")));

            var ex = Assert.Throws<InvalidOperationException>(() => Decode<CpuTypes>(StringV.Of("AVR")));
            Assert.AreEqual("Enumeration value 'AVR' not found in Test.DecoderTest+CpuTypes", ex.Message);
        }

        private struct NestedStruct
        {
            public byte? AByte;
            public short? AShort;

            public NestedStruct(byte? abyte, short? ashort)
            {
                AByte = abyte;
                AShort = ashort;
            }
        }

        private struct StructWithNullableFields
        {
            public int? AnInteger;
            public double? ADouble;
            public NestedStruct? AStruct;

            public StructWithNullableFields(int? intVal, double? doubleVal, NestedStruct? structVal)
            {
                AnInteger = intVal;
                ADouble = doubleVal;
                AStruct = structVal;
            }
        }

        [Test]
        public void TestNullableFields()
        {
            var structWithNullableFields = Decode<StructWithNullableFields>(ObjectV.With(
                "AnInteger", LongV.Of(10),
                "ADouble", DoubleV.Of(3.14),
                "AStruct", ObjectV.With("AByte", LongV.Of(10))
            ));

            Assert.AreEqual(10, structWithNullableFields.AnInteger);
            Assert.AreEqual(3.14, structWithNullableFields.ADouble);
            Assert.IsNotNull(structWithNullableFields.AStruct);
            Assert.AreEqual(10, structWithNullableFields.AStruct?.AByte);
            Assert.IsNull(structWithNullableFields.AStruct?.AShort);
        }

        private class StringOverride
        {
            [FaunaField("uri")]
            [FaunaString]
            public Uri Uri { get; set; }

            [FaunaField("guid")]
            [FaunaString]
            public Guid Guid { get; set; }

            [FaunaField("int")]
            [FaunaString]
            public int Int { get; set; }

            [FaunaField("double")]
            [FaunaString]
            public double Double { get; set; }

            [FaunaConstructor]
            public StringOverride()
            { }

            public StringOverride(Uri uri, Guid guid)
            {
                this.Uri = uri;
                this.Guid = guid;
            }
        }

        [Test]
        public void TestStringOverride()
        {
            string uriString = "https://fauna.com/";
            Uri testUri = new Uri(uriString);
            Guid testGuid = Guid.NewGuid();
            string guidString = testGuid.ToString();

            int @int = 1;
            double @double = 4.5;

            StringOverride obj = Decode<StringOverride>(ObjectV.With(
                "uri", new StringV(uriString),
                "guid", new StringV(guidString),
                "double", new StringV(@double.ToString()),
                "int", new StringV(@int.ToString())
                ));
            Assert.AreEqual(obj.Uri.ToString(), uriString);
            Assert.AreEqual(obj.Guid.ToString(), guidString);
            Assert.AreEqual(obj.Int, @int);
            Assert.AreEqual(obj.Double, @double);
        }

        private class StringOverrideOnConstuctorCreator
        {
            [FaunaField("uri")]
            [FaunaString]
            public Uri Uri { get; set; }

            [FaunaField("guid")]
            [FaunaString]
            public Guid Guid { get; set; }

            [FaunaField("int")]
            [FaunaString]
            public int Int { get; set; }

            [FaunaField("double")]
            [FaunaString]
            public double Double { get; set; }

            [FaunaConstructor]
            public StringOverrideOnConstuctorCreator(
                [FaunaString][FaunaField("uri")] Uri uri,
                [FaunaString][FaunaField("guid")]Guid guid,
                [FaunaString][FaunaField("int")]int @int,
                [FaunaString][FaunaField("double")]double @double)
            {
                this.Uri = uri;
                this.Guid = guid;
                this.Int = @int;
                this.Double = @double;
            }
        }

        [Test]
        public void TestStringOverrideOnConstuctorCreator()
        {
            string uriString = "https://fauna.com/";
            Uri testUri = new Uri(uriString);
            Guid testGuid = Guid.NewGuid();
            string guidString = testGuid.ToString();

            int @int = 1;
            double @double = 4.5;

            StringOverrideOnConstuctorCreator obj = Decode<StringOverrideOnConstuctorCreator>(ObjectV.With(
                "uri", new StringV(uriString),
                "guid", new StringV(guidString),
                "double", new StringV(@double.ToString()),
                "int", new StringV(@int.ToString())
                ));

            Assert.AreEqual(obj.Uri.ToString(), uriString);
            Assert.AreEqual(obj.Guid.ToString(), guidString);
            Assert.AreEqual(obj.Int, @int);
            Assert.AreEqual(obj.Double, @double);
        }
    }
}
