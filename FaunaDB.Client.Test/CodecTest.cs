using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Test
{
    [TestFixture] public class CodecTest
    {
        [Test] public void TestRef()
        {
            AssertSuccess(BuiltIn.DATABASES, BuiltIn.DATABASES.To(Codec.REF));
            AssertFailure("Cannot convert StringV to RefV", StringV.Of("a string").To(Codec.REF));
        }

        [Test] public void TestSetRef()
        {
            var dic = new Dictionary<string, Value> {
                {"@ref", "databases"}
            };
            AssertSuccess(new SetRefV(dic), new SetRefV(dic).To(Codec.SETREF));
            AssertFailure("Cannot convert StringV to SetRefV", StringV.Of("a string").To(Codec.SETREF));
        }

        [Test] public void TestLong()
        {
            AssertSuccess(1L, LongV.Of(1).To(Codec.LONG));
            AssertFailure("Cannot convert StringV to LongV", StringV.Of("a string").To(Codec.LONG));
        }

        [Test] public void TestString()
        {
            AssertSuccess("a string", StringV.Of("a string").To(Codec.STRING));
            AssertFailure("Cannot convert ObjectV to StringV", ObjectV.Empty.To(Codec.STRING));
        }

        [Test] public void TestBoolean()
        {
            AssertSuccess(true, BooleanV.True.To(Codec.BOOLEAN));
            AssertSuccess(false, BooleanV.False.To(Codec.BOOLEAN));
            AssertFailure("Cannot convert ObjectV to BooleanV", ObjectV.Empty.To(Codec.BOOLEAN));
        }

        [Test] public void TestDouble()
        {
            AssertSuccess(3.14, DoubleV.Of(3.14).To(Codec.DOUBLE));
            AssertFailure("Cannot convert ObjectV to DoubleV", ObjectV.Empty.To(Codec.DOUBLE));
        }

        [Test] public void TestTimestamp()
        {
            AssertSuccess(new DateTime(2000, 1, 1, 0, 0, 0, 123), new TimeV("2000-01-01T00:00:00.123Z").To(Codec.TIME));
            AssertFailure("Cannot convert ObjectV to TimeV", ObjectV.Empty.To(Codec.TIME));
        }

        [Test] public void TestDate()
        {
            AssertSuccess(new DateTime(2000, 1, 1), new DateV("2000-01-01").To(Codec.DATE));
            AssertFailure("Cannot convert ObjectV to DateV", ObjectV.Empty.To(Codec.DATE));
        }

        [Test] public void TestBytes()
        {
            AssertSuccess(new byte[] { 0x1, 0x2, 0x3 }, new BytesV(0x1, 0x2, 0x3).To(Codec.BYTES));
            AssertFailure("Cannot convert BytesV to BooleanV", new BytesV(0x1).To(Codec.BOOLEAN));
        }

        [Test] public void TestArray()
        {
            var array = new List<Value> { "a string", true, 10 };

            AssertSuccess(array, ArrayV.Of("a string", true, 10).To(Codec.ARRAY));
            AssertFailure("Cannot convert ObjectV to ArrayV", ObjectV.Empty.To(Codec.ARRAY));
        }

        [Test] public void TestObject()
        {
            var expected = new Dictionary<string, Value> {
                { "foo", "bar" }
            };

            AssertSuccess(expected, ObjectV.With("foo", "bar").To(Codec.OBJECT));
            AssertFailure("Cannot convert StringV to ObjectV", StringV.Of("a string").To(Codec.OBJECT));
        }

        class Product
        {
            [FaunaField("description")]
            public string Description { get; }

            [FaunaField("price")]
            public int Price { get; }

            [FaunaConstructor]
            public Product(string description, int price)
            {
                Description = description;
                Price = price;
            }

            public override int GetHashCode() => 0;

            public override bool Equals(object obj)
            {
                var other = obj as Product;
                return other != null && Description == other.Description && Price == other.Price;
            }
        }

        [Test]
        public void TestUserObject()
        {
            var product1 = ObjectV.With("description", "laptop", "price", 100);
            var product2 = ObjectV.With("description", "mouse", "price", 10);

            AssertSuccess(new Product("laptop", 100), product1.To(Codec.DECODE<Product>));
            AssertSuccess(new Product("mouse", 10), product2.To(Codec.DECODE<Product>));

            var products = ArrayV
                .Of(product1, product2)
                .Collect(Field.To(Codec.DECODE<Product>));

            Assert.That(
                products,
                Is.EquivalentTo(new List<Product> {
                    new Product("laptop", 100), new Product("mouse", 10),
                })
            );
        }

        [Test]
        public void TestValueToType()
        {
            AssertSuccess("a string", StringV.Of("a string").To<string>());
            AssertSuccess(true, BooleanV.True.To<bool>());
            AssertSuccess(null, NullV.Instance.To<object>());

            AssertSuccess((long)10, LongV.Of(10).To<long>());
            AssertSuccess((int)10, LongV.Of(10).To<int>());
            AssertSuccess((short)10, LongV.Of(10).To<short>());
            AssertSuccess((sbyte)10, LongV.Of(10).To<sbyte>());

            AssertSuccess((ulong)10, LongV.Of(10).To<ulong>());
            AssertSuccess((uint)10, LongV.Of(10).To<uint>());
            AssertSuccess((ushort)10, LongV.Of(10).To<ushort>());
            AssertSuccess((byte)10, LongV.Of(10).To<byte>());
            AssertSuccess((char)10, LongV.Of(10).To<char>());

            AssertSuccess(3.14f, DoubleV.Of(3.14).To<float>());
            AssertSuccess(3.14, DoubleV.Of(3.14).To<double>());
            AssertSuccess((decimal)3.14, DoubleV.Of(3.14).To<decimal>());

            AssertSuccess(new DateTime(2001, 1, 1), new DateV("2001-01-01").To<DateTime>());
            AssertSuccess(new DateTime(2000, 1, 1, 1, 10, 30, 123), new TimeV("2000-01-01T01:10:30.123Z").To<DateTime>());

            AssertSuccess(new byte[] { 1, 2, 3 }, new BytesV(1, 2, 3).To<byte[]>());

            AssertSuccess(
                new Product("Laptop", 999),
                ObjectV.With("description", "Laptop", "price", 999).To<Product>()
            );
        }

        [Test]
        public void TestReturnFailureOnOverflow()
        {
            AssertFailure("Value was either too large or too small for a signed byte.", LongV.Of(sbyte.MinValue - 1).To(Codec.DECODE<sbyte>));
            AssertFailure("Value was either too large or too small for a UInt16.", LongV.Of(ushort.MaxValue + 1).To(Codec.DECODE<ushort>));
        }

        static void AssertSuccess<T>(T expected, IResult<T> actual)
        {
            actual.Match(
                Success: value => Assert.AreEqual(expected, value),
                Failure: reason => Assert.Fail(reason, "AssertSuccess")
            );
        }

        static void AssertFailure<T>(string expected, IResult<T> actual)
        {
            actual.Match(
                Success: value => Assert.Fail("Expected a failure result"),
                Failure: reason => Assert.AreEqual(expected, reason, "AssertFail")
            );
        }
    }
}
