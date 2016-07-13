using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

using static FaunaDB.Types.Result;

namespace Test
{
    [TestFixture] public class CodecTest
    {
        [Test] public void TestRef()
        {
            Assert.AreEqual(Success(new RefV("databases")), new RefV("databases").To(Codec.REF));
            Assert.AreEqual(Fail<RefV>("Cannot convert StringV to RefV"), StringV.Of("a string").To(Codec.REF));
        }

        [Test] public void TestSetRef()
        {
            var dic = new Dictionary<string, Value> {
                {"@ref", "databases"}
            };
            Assert.AreEqual(Success(new SetRefV(dic)), new SetRefV(dic).To(Codec.SETREF));
            Assert.AreEqual(Fail<SetRefV>("Cannot convert StringV to SetRefV"), StringV.Of("a string").To(Codec.SETREF));
        }

        [Test] public void TestLong()
        {
            Assert.AreEqual(Success(1L), LongV.Of(1).To(Codec.LONG));
            Assert.AreEqual(Fail<long>("Cannot convert StringV to LongV"), StringV.Of("a string").To(Codec.LONG));
        }

        [Test] public void TestString()
        {
            Assert.AreEqual(Success("a string"), StringV.Of("a string").To(Codec.STRING));
            Assert.AreEqual(Fail<string>("Cannot convert ObjectV to StringV"), ObjectV.Empty.To(Codec.STRING));
        }

        [Test] public void TestBoolean()
        {
            Assert.AreEqual(Success(true), BooleanV.True.To(Codec.BOOLEAN));
            Assert.AreEqual(Success(false), BooleanV.False.To(Codec.BOOLEAN));
            Assert.AreEqual(Fail<bool>("Cannot convert ObjectV to BooleanV"), ObjectV.Empty.To(Codec.BOOLEAN));
        }

        [Test] public void TestDouble()
        {
            Assert.AreEqual(Success(3.14), DoubleV.Of(3.14).To(Codec.DOUBLE));
            Assert.AreEqual(Fail<double>("Cannot convert ObjectV to DoubleV"), ObjectV.Empty.To(Codec.DOUBLE));
        }

        [Test] public void TestTimestamp()
        {
            Assert.AreEqual(Success(new DateTime(2000, 1, 1, 0, 0, 0, 123)), new TimeV("2000-01-01T00:00:00.123Z").To(Codec.TIME));
            Assert.AreEqual(Fail<DateTime>("Cannot convert ObjectV to TimeV"), ObjectV.Empty.To(Codec.TIME));
        }

        [Test] public void TestDate()
        {
            Assert.AreEqual(Success(new DateTime(2000, 1, 1)), new DateV("2000-01-01").To(Codec.DATE));
            Assert.AreEqual(Fail<DateTime>("Cannot convert ObjectV to DateV"), ObjectV.Empty.To(Codec.DATE));
        }

        [Test] public void TestArray()
        {
            IReadOnlyList<Value> array = new List<Value> { "a string", true, 10 };

            Assert.AreEqual(Success(array), ArrayV.Of("a string", true, 10).To(Codec.ARRAY));
            Assert.AreEqual(Fail<IReadOnlyList<Value>>("Cannot convert ObjectV to ArrayV"), ObjectV.Empty.To(Codec.ARRAY));
        }

        [Test] public void TestObject()
        {
            IReadOnlyDictionary<string, Value> expected = new Dictionary<string, Value> {
                { "foo", StringV.Of("bar") }
            };

            Assert.AreEqual(Success(expected), ObjectV.With("foo", "bar").To(Codec.OBJECT));
            Assert.AreEqual(Fail<IReadOnlyDictionary<string, Value>>("Cannot convert StringV to ObjectV"), StringV.Of("a string").To(Codec.OBJECT));
        }
    }
}
