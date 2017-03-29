using System;
using System.Collections.Generic;
using FaunaDB.Attributes;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;
using static FaunaDB.Encoding.Encoder;

namespace Test
{
    [TestFixture]
    public class EncoderTest
    {
        [Test]
        public void TestPrimitive()
        {
            AreEqual(StringV.Of("a string"), Encode("a string"));

            AreEqual(BooleanV.True, Encode(true));
            AreEqual(BooleanV.False, Encode(false));

            AreEqual(NullV.Instance, Encode((object)null));

            AreEqual(new BytesV(1, 2, 3, 4), Encode(new byte[] { 1, 2, 3, 4 }));

            AreEqual(new DateV("2001-01-01"), Encode(new DateTime(2001, 1, 1)));
            AreEqual(new TimeV("2000-01-01T01:10:30.123Z"), Encode(new DateTime(2000, 1, 1, 1, 10, 30, 123)));

            //float point
            AreEqual(DoubleV.Of((float)3.14), Encode((float)3.14));
            AreEqual(DoubleV.Of(3.14), Encode((double)3.14));
            AreEqual(DoubleV.Of(3.14), Encode((decimal)3.14));

            //signed values
            AreEqual(LongV.Of(10), Encode((sbyte)10));
            AreEqual(LongV.Of(10), Encode((short)10));
            AreEqual(LongV.Of(10), Encode((int)10));
            AreEqual(LongV.Of(10), Encode((long)10));

            //unsigned values
            AreEqual(LongV.Of(10), Encode((char)10));
            AreEqual(LongV.Of(10), Encode((byte)10));
            AreEqual(LongV.Of(10), Encode((ushort)10));
            AreEqual(LongV.Of(10), Encode((uint)10));
            AreEqual(LongV.Of(10), Encode((ulong)10));
        }

        [Test]
        public void TestIntegerLimits()
        {
            //signed values
            Assert.DoesNotThrow(() => Encode(sbyte.MinValue));
            Assert.DoesNotThrow(() => Encode(short.MinValue));
            Assert.DoesNotThrow(() => Encode(int.MinValue));
            Assert.DoesNotThrow(() => Encode(long.MinValue));

            Assert.DoesNotThrow(() => Encode(sbyte.MaxValue));
            Assert.DoesNotThrow(() => Encode(short.MaxValue));
            Assert.DoesNotThrow(() => Encode(int.MaxValue));
            Assert.DoesNotThrow(() => Encode(long.MaxValue));

            //unsigned values
            Assert.DoesNotThrow(() => Encode(char.MinValue));
            Assert.DoesNotThrow(() => Encode(byte.MinValue));
            Assert.DoesNotThrow(() => Encode(ushort.MinValue));
            Assert.DoesNotThrow(() => Encode(uint.MinValue));
            Assert.DoesNotThrow(() => Encode(ulong.MinValue));

            Assert.DoesNotThrow(() => Encode(char.MaxValue));
            Assert.DoesNotThrow(() => Encode(byte.MaxValue));
            Assert.DoesNotThrow(() => Encode(ushort.MaxValue));
            Assert.DoesNotThrow(() => Encode(uint.MaxValue));
            Assert.Throws<OverflowException>(() => Encode(ulong.MaxValue));
        }

        [Test]
        public void TestFloatPointLimits()
        {
            Assert.DoesNotThrow(() => Encode(decimal.MinValue));
            Assert.DoesNotThrow(() => Encode(float.MinValue));
            Assert.DoesNotThrow(() => Encode(double.MinValue));

            Assert.DoesNotThrow(() => Encode(decimal.MaxValue));
            Assert.DoesNotThrow(() => Encode(float.MaxValue));
            Assert.DoesNotThrow(() => Encode(double.MaxValue));
        }

        [Test]
        public void TestCollection()
        {
            AreEqual(
                Arr(1, 2, 3),
                Encode(new int[] { 1, 2, 3 })
            );

            AreEqual(
                Arr(1, 2, 3),
                Encode(new List<int> { 1, 2, 3 })
            );

            AreEqual(
                Arr("a string", 3.14, NullV.Instance, 10, true),
                Encode(new object[] { "a string", 3.14, null, 10, true })
            );

            AreEqual(
                Obj("field1", "value1", "field2", 10),
                Encode(new Dictionary<string, object> { {"field1", "value1"}, {"field2", 10} })
            );
        }

        class Address
        {
            [Field("street")]
            public string Street { get; set; }

            [Field("number")]
            private int number;

            [FaunaDB.Attributes.Ignore]
            public int Number
            {
                get
                {
                    return number;
                }
            }

            public Address(string street, int number)
            {
                this.Street = street;
                this.number = number;
            }
        }

        class Person
        {
            public static byte[] DefaultAvatar = { 1, 2, 3, 4 };

            [Field("name")]
            public string Name { get; }

            [Field("birth_date")]
            public DateTime BirthDate { get; }

            [Field("address")]
            public Address Address { get; }

            [Field("avatar_image")]
            public byte[] Avatar { get; }

            public Person(string name, DateTime birthDate, Address address, byte[] avatar)
            {
                Name = name;
                BirthDate = birthDate;
                Address = address;
                Avatar = avatar;
            }
        }

        [Test]
        public void TestUserClass()
        {
            AreEqual(
                Obj("name", "John", "birth_date", new DateV("1980-01-01"), "address", Obj("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                Encode(new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar))
            );

            // list of person

            AreEqual(
                Arr(
                    Obj("name", "John", "birth_date", new DateV("1980-01-01"), "address", Obj("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    Obj("name", "Mary", "birth_date", new DateV("1975-01-01"), "address", Obj("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new List<Person> {
                    new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar),
                    new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar)
                })
            );

            // dictionary of string => person

            AreEqual(
                Obj(
                    "first", Obj("name", "John", "birth_date", new DateV("1980-01-01"), "address", Obj("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    "second", Obj("name", "Mary", "birth_date", new DateV("1975-01-01"), "address", Obj("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new Dictionary<string, Person> {
                    {"first", new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar)},
                    {"second", new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar)}
                })
            );
        }

        struct Rect
        {
            [Field("x")]
            public int x1;

            [Field("y")]
            public int y1;

            [FaunaDB.Attributes.Ignore]
            public int x2;

            [FaunaDB.Attributes.Ignore]
            public int y2;

            [Field("width")]
            public int Width { get { return x2 - x1; } }

            [Field("height")]
            public int Height { get { return y2 - y1; } }

            public Rect(int x1, int y1, int x2, int y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
            }
        }

        [Test]
        public void TestUserStruct()
        {
            AreEqual(
                Obj("x", 10, "y", 20, "width", 10, "height", 10),
                Encode(new Rect(10, 20, 20, 30))
            );
        }

        class Node
        {
            public Node Left;
            public Node Right;

            public override string ToString() => "Node";
        }

        [Test]
        public void TestCycles()
        {
            var parent = new Node();
            parent.Left = new Node();
            parent.Right = new Node();

            parent.Right.Left = parent;

            var ex = Assert.Throws<InvalidOperationException>(() => Encode(parent));
            AreEqual("Self referencing loop detected for object `Node`", ex.Message);
        }

        class FaunaTypes
        {
            public StringV stringV = StringV.Of("a string");
            public LongV longV = LongV.Of(123);
            public BooleanV booleanV = BooleanV.True;
            public DoubleV doubleV = DoubleV.Of(3.14);
            public Value nullV = NullV.Instance;
            public DateV dateV = new DateV("2001-01-01");
            public TimeV timeV = new TimeV("2000-01-01T01:10:30.123Z");
            public RefV refV = new RefV("classes");
            public SetRefV setRefV = new SetRefV(new Dictionary<string, Value>());
            public ArrayV arrayV = ArrayV.Of(1, 2, 3);
            public ObjectV objectV = ObjectV.With("a", "b");
            public BytesV bytesV = new BytesV(1, 2, 3, 4);
        }

        [Test]
        public void TestFaunaTypes()
        {
            AreEqual(
                Obj(new Dictionary<string, Expr> {
                    {"stringV", "a string"},
                    {"longV", 123},
                    {"booleanV", true},
                    {"doubleV", 3.14},
                    {"nullV", Null()},
                    {"dateV", new DateV("2001-01-01")},
                    {"timeV", new TimeV("2000-01-01T01:10:30.123Z")},
                    {"refV", Ref("classes")},
                    {"setRefV", new SetRefV(new Dictionary<string, Value>())},
                    {"arrayV", Arr(1, 2, 3)},
                    {"objectV", Obj("a", "b")},
                    {"bytesV", new BytesV(1, 2, 3, 4)}
                }),
                Encode(new FaunaTypes())
            );
        }

        static void AreEqual(Expr expected, Expr actual) =>
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }
}
