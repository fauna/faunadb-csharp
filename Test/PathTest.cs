using FaunaDB.Types;
using NUnit.Framework;

namespace Test
{
    [TestFixture] public class PathTest
    {
        [Test] public void TestObjectKey()
        {
            var obj = ObjectV.With("foo", "bar");

            Assert.AreEqual(Result.Success<Value>(StringV.Of("bar")),
                Path.From("foo").Get(obj));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"nonexistent\". Object key \"nonexistent\" not found"),
                Path.From("nonexistent").Get(obj));
        }

        [Test] public void TestArrayIndex()
        {
            var array = ArrayV.Of("a string", 10);

            Assert.AreEqual(Result.Success<Value>(StringV.Of("a string")),
                Path.From(0).Get(array));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(10)),
                Path.From(1).Get(array));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"1234\". Array index \"1234\" not found"),
                Path.From(1234).Get(array));
        }

        [Test] public void TestNestedObject()
        {
            var nested = ObjectV.With("foo", ObjectV.With("bar", "a string"));

            Assert.AreEqual(Result.Success<Value>(StringV.Of("a string")),
                Path.From("foo", "bar").Get(nested));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"foo/nonexistent\". Object key \"nonexistent\" not found"),
                Path.From("foo", "nonexistent").Get(nested));
        }

        [Test] public void TestNestedArray()
        {
            var nested = ArrayV.Of(10, ArrayV.Of(1234, ArrayV.Of(4321)));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(10)),
                Path.From(0).Get(nested));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(1234)),
                Path.From(1, 0).Get(nested));

            Assert.AreEqual(Result.Success<Value>(LongV.Of(4321)),
                Path.From(1, 1, 0).Get(nested));

            Assert.AreEqual(Result.Fail<Value>("Cannot find path \"1/1/1\". Array index \"1\" not found"),
                Path.From(1, 1, 1).Get(nested));
        }
    }
}
