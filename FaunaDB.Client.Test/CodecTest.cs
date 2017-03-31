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

        static void AssertSuccess<T>(T expected, IResult<T> actual)
        {
            actual.Match(
                Success: value => Assert.AreEqual(expected, value),
                Failure: reason => Assert.Fail("Expected a success result", "AssertSuccess")
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
