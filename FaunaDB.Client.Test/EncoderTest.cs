using System;
using System.Collections.Generic;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Types.Encoder;

namespace Test
{
    [TestFixture]
    public class EncoderTest
    {
        [Test]
        public void TestPrimitive()
        {
            Assert.AreEqual(StringV.Of("a string"), Encode("a string"));

            Assert.AreEqual(BooleanV.True, Encode(true));
            Assert.AreEqual(BooleanV.False, Encode(false));

            Assert.AreEqual(NullV.Instance, Encode((object)null));

            Assert.AreEqual(new BytesV(1, 2, 3, 4), Encode(new byte[] { 1, 2, 3, 4 }));

            Assert.AreEqual(new DateV("2001-01-01"), Encode(new DateTime(2001, 1, 1)));
            Assert.AreEqual(new TimeV("2000-01-01T01:10:30.123Z"), Encode(new DateTime(2000, 1, 1, 1, 10, 30, 123)));

            //float point
            Assert.AreEqual(DoubleV.Of((float)3.14), Encode((float)3.14));
            Assert.AreEqual(DoubleV.Of(3.14), Encode((double)3.14));
            Assert.AreEqual(DoubleV.Of(3.14), Encode((decimal)3.14));

            //signed values
            Assert.AreEqual(LongV.Of(10), Encode((sbyte)10));
            Assert.AreEqual(LongV.Of(10), Encode((short)10));
            Assert.AreEqual(LongV.Of(10), Encode((int)10));
            Assert.AreEqual(LongV.Of(10), Encode((long)10));

            //unsigned values
            Assert.AreEqual(LongV.Of(10), Encode((char)10));
            Assert.AreEqual(LongV.Of(10), Encode((byte)10));
            Assert.AreEqual(LongV.Of(10), Encode((ushort)10));
            Assert.AreEqual(LongV.Of(10), Encode((uint)10));
            Assert.AreEqual(LongV.Of(10), Encode((ulong)10));
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
            Assert.AreEqual(
                ArrayV.Of(1, 2, 3),
                Encode(new int[] { 1, 2, 3 })
            );

            Assert.AreEqual(
                ArrayV.Of(1, 2, 3),
                Encode(new List<int> { 1, 2, 3 })
            );

            Assert.AreEqual(
                ArrayV.Of("a string", 3.14, NullV.Instance, 10, true),
                Encode(new object[] { "a string", 3.14, null, 10, true })
            );

            Assert.AreEqual(
                ObjectV.With("field1", "value1", "field2", 10),
                Encode(new Dictionary<string, object> { {"field1", "value1"}, {"field2", 10} })
            );
        }

        class Address
        {
            [FaunaField("street")]
            public string Street { get; set; }

            [FaunaField("number")]
            private int number;

            [FaunaIgnore]
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

            [FaunaField("name")]
            public string Name { get; }

            [FaunaField("birth_date")]
            public DateTime BirthDate { get; }

            [FaunaField("address")]
            public Address Address { get; }

            [FaunaField("avatar_image")]
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
            Assert.AreEqual(
                ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                Encode(new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar))
            );

            // list of person

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    ObjectV.With("name", "Mary", "birth_date", new DateV("1975-01-01"), "address", ObjectV.With("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new List<Person> {
                    new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar),
                    new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar)
                })
            );

            // dictionary of string => person

            Assert.AreEqual(
                ObjectV.With(
                    "first", ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    "second", ObjectV.With("name", "Mary", "birth_date", new DateV("1975-01-01"), "address", ObjectV.With("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new Dictionary<string, Person> {
                    {"first", new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar)},
                    {"second", new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar)}
                })
            );
        }

        struct Rect
        {
            [FaunaField("x")]
            public int x1;

            [FaunaField("y")]
            public int y1;

            [FaunaIgnore]
            public int x2;

            [FaunaIgnore]
            public int y2;

            [FaunaField("width")]
            public int Width { get { return x2 - x1; } }

            [FaunaField("height")]
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
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20, "width", 10, "height", 10),
                Encode(new Rect(10, 20, 20, 30))
            );
        }

        class Node
        {
            public string Id { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public override string ToString() => $"Node({Id})";
            public override bool Equals(object obj) => obj is Node && Id == ((Node)obj).Id;
            public override int GetHashCode() => Id.GetHashCode();
        }

        [Test]
        public void TestCycles()
        {
            var parent = new Node { Id = "parent" };
            parent.Left = new Node { Id = "left" };
            parent.Right = new Node { Id = "right" };

            parent.Right.Left = parent;

            var ex = Assert.Throws<InvalidOperationException>(() => Encode(parent));
            Assert.AreEqual("Self referencing loop detected for object `Node(parent)`", ex.Message);
        }

        [Test]
        public void TestCyclesForReferenceNotEquality()
        {
            var parent = new Node { Id = "id" };
            parent.Left = new Node { Id = "id" };
            parent.Right = new Node { Id = "id" };

            Assert.DoesNotThrow(() => Encode(parent));
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
            Assert.AreEqual(
                ObjectV.With(new Dictionary<string, Value> {
                    {"stringV", "a string"},
                    {"longV", 123},
                    {"booleanV", true},
                    {"doubleV", 3.14},
                    {"nullV", NullV.Instance},
                    {"dateV", new DateV("2001-01-01")},
                    {"timeV", new TimeV("2000-01-01T01:10:30.123Z")},
                    {"refV", new RefV("classes")},
                    {"setRefV", new SetRefV(new Dictionary<string, Value>())},
                    {"arrayV", ArrayV.Of(1, 2, 3)},
                    {"objectV", ObjectV.With("a", "b")},
                    {"bytesV", new BytesV(1, 2, 3, 4)}
                }),
                Encode(new FaunaTypes())
            );
        }
    }
}
