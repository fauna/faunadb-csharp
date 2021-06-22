using System;
using System.Collections.Generic;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;
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

            Assert.AreEqual(new DateV("2001-01-01"), Encode(new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            Assert.AreEqual(new TimeV("2000-01-01T01:10:30.123Z"), Encode(new DateTimeOffset(2000, 1, 1, 1, 10, 30, 123, TimeSpan.Zero)));

            // float point
            Assert.AreEqual(DoubleV.Of(3.14F), Encode(3.14F));
            Assert.AreEqual(DoubleV.Of(3.14), Encode((double)3.14));
            Assert.AreEqual(DoubleV.Of(3.14), Encode(3.14M));

            // signed values
            Assert.AreEqual(LongV.Of(10), Encode((sbyte)10));
            Assert.AreEqual(LongV.Of(10), Encode((short)10));
            Assert.AreEqual(LongV.Of(10), Encode((int)10));
            Assert.AreEqual(LongV.Of(10), Encode(10L));

            // unsigned values
            Assert.AreEqual(LongV.Of(10), Encode((char)10));
            Assert.AreEqual(LongV.Of(10), Encode((byte)10));
            Assert.AreEqual(LongV.Of(10), Encode((ushort)10));
            Assert.AreEqual(LongV.Of(10), Encode(10U));
            Assert.AreEqual(LongV.Of(10), Encode(10UL));
        }

        [Test]
        public void TestIntegerLimits()
        {
            // signed values
            Assert.DoesNotThrow(() => Encode(sbyte.MinValue));
            Assert.DoesNotThrow(() => Encode(short.MinValue));
            Assert.DoesNotThrow(() => Encode(int.MinValue));
            Assert.DoesNotThrow(() => Encode(long.MinValue));

            Assert.DoesNotThrow(() => Encode(sbyte.MaxValue));
            Assert.DoesNotThrow(() => Encode(short.MaxValue));
            Assert.DoesNotThrow(() => Encode(int.MaxValue));
            Assert.DoesNotThrow(() => Encode(long.MaxValue));

            // unsigned values
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
                Encode(new Dictionary<string, object> { { "field1", "value1" }, { "field2", 10 } })
            );
        }

        private class Address
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

        private class Person
        {
            public static byte[] DefaultAvatar = { 1, 2, 3, 4 };

            [FaunaField("name")]
            public string Name { get; }

            [FaunaField("birth_date")]
            public DateTime BirthDate { get; }

            [FaunaField("birth_date2")]
            public DateTimeOffset BirthDate2 { get; }

            [FaunaField("address")]
            public Address Address { get; }

            [FaunaField("avatar_image")]
            public byte[] Avatar { get; }

            public Person(string name, DateTime birthDate, Address address, byte[] avatar)
            {
                Name = name;
                BirthDate = birthDate;
                BirthDate2 = new DateTimeOffset(birthDate, new TimeSpan(-1, 0, 0));
                Address = address;
                Avatar = avatar;
            }
        }

        [Test]
        public void TestUserClass()
        {
            Assert.AreEqual(
                ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "birth_date2", new TimeV("1980-01-01T01:00:00.0Z"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                Encode(new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar))
            );

            // list of person
            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "birth_date2", new TimeV("1980-01-01T01:00:00.0Z"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    ObjectV.With("name", "Mary", "birth_date", new DateV("1975-01-01"), "birth_date2", new TimeV("1975-01-01T01:00:00.0Z"), "address", ObjectV.With("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new List<Person>
                {
                    new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar),
                    new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar),
                })
            );

            // dictionary of string => person
            Assert.AreEqual(
                ObjectV.With(
                    "first", ObjectV.With("name", "John", "birth_date", new DateV("1980-01-01"), "birth_date2", new TimeV("1980-01-01T01:00:00.0Z"), "address", ObjectV.With("number", 123, "street", "Market St"), "avatar_image", new BytesV(1, 2, 3, 4)),
                    "second", ObjectV.With("name", "Mary", "birth_date", new DateV("1975-01-01"), "birth_date2", new TimeV("1975-01-01T01:00:00.0Z"), "address", ObjectV.With("number", 321, "street", "Mission St"), "avatar_image", new BytesV(1, 2, 3, 4))
                ),
                Encode(new Dictionary<string, Person>
                {
                    {"first", new Person("John", new DateTime(1980, 1, 1), new Address("Market St", 123), Person.DefaultAvatar) },
                    {"second", new Person("Mary", new DateTime(1975, 1, 1), new Address("Mission St", 321), Person.DefaultAvatar) },
                })
            );
        }

        private struct Rect
        {
            [FaunaField("x")]
            public int X1;

            [FaunaField("y")]
            public int Y1;

            [FaunaIgnore]
            public int X2;

            [FaunaIgnore]
            public int Y2;

            [FaunaField("width")]
            public int Width { get { return X2 - X1; } }

            [FaunaField("height")]
            public int Height { get { return Y2 - Y1; } }

            public Rect(int x1, int y1, int x2, int y2)
            {
                this.X1 = x1;
                this.Y1 = y1;
                this.X2 = x2;
                this.Y2 = y2;
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

        private class Node
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
        }

        [Test]
        public void TestFaunaTypes()
        {
            Assert.AreEqual(
                ObjectV.With(new Dictionary<string, Value>
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
                }),
                Encode(new FaunaTypes())
            );
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
            Assert.AreEqual(StringV.Of("x86_32"), Encode(CpuTypes.X86));
            Assert.AreEqual(StringV.Of("x86_64"), Encode(CpuTypes.X86_64));
            Assert.AreEqual(StringV.Of("ARM"), Encode(CpuTypes.ARM));
            Assert.AreEqual(StringV.Of("MIPS"), Encode(CpuTypes.MIPS));
        }

        private class DateTimeOverride
        {
            [FaunaField("timev")]
            [FaunaTime]
            public DateTime TimeV { get; set; }

            [FaunaField("datev")]
            [FaunaDate]
            public DateTime DateV { get; set; }

            public DateTimeOverride(DateTime timeV, DateTime dateV)
            {
                this.TimeV = timeV;
                this.DateV = dateV;
            }
        }

        [Test]
        public void TestDateTimeOverride()
        {
            DateTime testDateTime = new DateTime(2000, 1, 1, 1, 1, 1, 1);
            Assert.AreEqual(
                ObjectV.With("timev", new TimeV(testDateTime), "datev", new DateV(testDateTime.Date)),
                Encode(new DateTimeOverride(testDateTime, testDateTime))
            );
        }

        [Test]
        public void TestEncodeRawExpressions()
        {
            var indexCfg = new Dictionary<string, object>()
            {
                { "name", "index_name" },
                { "source",  Collection("class_name") },
            };

            Assert.AreEqual(
                ObjectV.With("name", "index_name", "source", ExprV.Of(Collection("class_name"))),
                Encode(indexCfg)
            );
        }

        private class StringOverride
        {
            [FaunaField("uri")]
            [FaunaString]
            public Uri Uri { get; set; }

            [FaunaField("guid")]
            [FaunaString]
            public Guid Guid { get; set; }

            public StringOverride(Uri uri, Guid guid)
            {
                this.Uri = uri;
                this.Guid = guid;
            }
        }

        [Test]
        public void TestStringOverride()
        {
            Uri testUri = new Uri("https://fauna.com");
            Guid testGuid = Guid.NewGuid();
            Assert.AreEqual(
                ObjectV.With("uri", new StringV(testUri.ToString()), "guid", new StringV(testGuid.ToString())),
                Encode(new StringOverride(testUri, testGuid))
            );
        }
    }
}
