using FaunaDB.Collections;
using FaunaDB.Types;
using NUnit.Framework;
using System;

namespace Test
{
    [TestFixture] public class CodecTest
    {
        [Test] public void TestRef()
        {
            Assert.AreEqual(Result.success(new Ref("databases")), Codec.REF(new Ref("databases")));
            Assert.AreEqual(Result.fail<Ref>("Cannot convert StringV to Ref"), Codec.REF(StringV.Of("a string")));
        }

        [Test] public void TestSetRef()
        {
            Assert.AreEqual(Result.success(new SetRef("databases")), Codec.SETREF(new SetRef("databases")));
            Assert.AreEqual(Result.fail<SetRef>("Cannot convert StringV to SetRef"), Codec.SETREF(StringV.Of("a string")));
        }

        [Test] public void TestLong()
        {
            Assert.AreEqual(Result.success(1L), Codec.LONG(LongV.Of(1)));
            Assert.AreEqual(Result.fail<long>("Cannot convert StringV to LongV"), Codec.LONG(StringV.Of("a string")));
        }

        [Test] public void TestString()
        {
            Assert.AreEqual(Result.success("a string"), Codec.STRING(StringV.Of("a string")));
            Assert.AreEqual(Result.fail<string>("Cannot convert ObjectV to StringV"), Codec.STRING(ObjectV.Empty));
        }

        [Test] public void TestBoolean()
        {
            Assert.AreEqual(Result.success(true), Codec.BOOLEAN(BooleanV.True));
            Assert.AreEqual(Result.success(false), Codec.BOOLEAN(BooleanV.False));
            Assert.AreEqual(Result.fail<bool>("Cannot convert ObjectV to BooleanV"), Codec.BOOLEAN(ObjectV.Empty));
        }

        [Test] public void TestDouble()
        {
            Assert.AreEqual(Result.success(3.14), Codec.DOUBLE(DoubleV.Of(3.14)));
            Assert.AreEqual(Result.fail<double>("Cannot convert ObjectV to DoubleV"), Codec.DOUBLE(ObjectV.Empty));
        }

        [Test] public void TestTimestamp()
        {
            Assert.AreEqual(Result.success(new DateTime(2000, 1, 1, 0, 0, 0, 123)), Codec.TS(new TsV("2000-01-01T00:00:00.123Z")));
            Assert.AreEqual(Result.fail<DateTime>("Cannot convert ObjectV to TsV"), Codec.TS(ObjectV.Empty));
        }

        [Test] public void TestDate()
        {
            Assert.AreEqual(Result.success(new DateTime(2000, 1, 1)), Codec.DATE(new DateV("2000-01-01")));
            Assert.AreEqual(Result.fail<DateTime>("Cannot convert ObjectV to DateV"), Codec.DATE(ObjectV.Empty));
        }

        [Test] public void TestArray()
        {
            var array = ImmutableArray.Of<Value>("a string", true, 10);

            Assert.AreEqual(Result.success(array), Codec.ARRAY(ArrayV.Of("a string", true, 10)));
            Assert.AreEqual(Result.fail<ArrayList<Value>>("Cannot convert ObjectV to ArrayV"), Codec.ARRAY(ObjectV.Empty));
        }

        [Test] public void TestObject()
        {
            var obj = ImmutableDictionary.Of<string, Value>("foo", StringV.Of("bar"));

            Assert.AreEqual(Result.success(obj), Codec.OBJECT(ObjectV.With("foo", "bar")));
            Assert.AreEqual(Result.fail<OrderedDictionary<string, Value>>("Cannot convert StringV to ObjectV"), Codec.OBJECT(StringV.Of("a string")));
        }
    }
}
