using FaunaDB.Types;
using NUnit.Framework;

namespace Test
{
    [TestFixture] public class FieldTest
    {
        [Test] public void TestObjectKey()
        {
            var obj = ObjectV.With("foo", "bar");

            Assert.AreEqual(Result.Success<Value>("bar"),
                obj.Get(Field.At("foo")));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"nonexistent\". Object key \"nonexistent\" not found"),
                obj.Get(Field.At("nonexistent")));
        }

        [Test] public void TestArrayIndex()
        {
            var array = ArrayV.Of("a string", 10);

            Assert.AreEqual(Result.Success<Value>(StringV.Of("a string")),
                array.Get(Field.At(0)));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(10)),
                array.Get(Field.At(1)));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"1234\". Array index \"1234\" not found"),
                array.Get(Field.At(1234)));
        }

        [Test] public void TestNestedObject()
        {
            var nested = ObjectV.With("foo", ObjectV.With("bar", "a string"));

            Assert.AreEqual(Result.Success<Value>(StringV.Of("a string")),
                nested.Get(Field.At("foo", "bar")));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"foo/nonexistent\". Object key \"nonexistent\" not found"),
                nested.Get(Field.At("foo", "nonexistent")));
        }

        [Test] public void TestNestedArray()
        {
            var nested = ArrayV.Of(10, ArrayV.Of(1234, ArrayV.Of(4321)));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(10)),
                nested.Get(Field.At(0)));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(1234)),
                nested.Get(Field.At(1, 0)));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(4321)),
                nested.Get(Field.At(1, 1, 0)));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"1/1/1\". Array index \"1\" not found"),
                nested.Get(Field.At(1, 1, 1)));
        }

        [Test] public void TestCodecConvertion()
        {
            var obj = ObjectV.With(
                "string", "a string",
                "bool", true,
                "double", 3.14,
                "long", 1234,
                "ref", new Ref("databases"),
                "setref", new SetRef("databases"));

            Assert.AreEqual(Result.Success("a string"),
                obj.Get(Field.At("string").To(Codec.STRING)));

            Assert.AreEqual(Result.Success(true),
                obj.Get(Field.At("bool").To(Codec.BOOLEAN)));

            Assert.AreEqual(Result.Success(3.14),
                obj.Get(Field.At("double").To(Codec.DOUBLE)));

            Assert.AreEqual(Result.Success(1234L),
                obj.Get(Field.At("long").To(Codec.LONG)));

            Assert.AreEqual(Result.Success(new Ref("databases")),
                obj.Get(Field.At("ref").To(Codec.REF)));

            Assert.AreEqual(Result.Success(new SetRef("databases")),
                obj.Get(Field.At("setref").To(Codec.SETREF)));
        }

        [Test] public void TestComplex()
        {
            var obj = ObjectV.With("foo", ArrayV.Of(1, 2, ObjectV.With("bar", "a string")));

            Assert.AreEqual(Result.Success("a string"),
                obj.Get(Field.At("foo").At(Field.At(2)).At(Field.At("bar").To(Codec.STRING))));
        }
    }
}
