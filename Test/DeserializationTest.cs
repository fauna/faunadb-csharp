using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;

namespace Test
{
    [TestFixture] public class DeserializationTest
    {
        [Test] public void TestString()
        {
            Expr str = Value.FromJson("\"a string\"");
            Assert.AreEqual(StringV.Of("a string"), str);
        }

        [Test] public void TestBoolean()
        {
            Assert.AreEqual(BooleanV.True, Value.FromJson("true"));
            Assert.AreEqual(BooleanV.False, Value.FromJson("false"));
        }

        [Test] public void TestLong()
        {
            Assert.AreEqual(LongV.Of(long.MaxValue), Value.FromJson(long.MaxValue.ToString()));
            Assert.AreEqual(LongV.Of(long.MinValue), Value.FromJson(long.MinValue.ToString()));
            Assert.AreEqual(LongV.Of(10), Value.FromJson("10"));
        }

        [Test] public void TestDouble()
        {
            Assert.AreEqual(DoubleV.Of(3.14), Value.FromJson("3.14"));
        }

        [Test] public void TestRef()
        {
            Assert.AreEqual(new Ref("classes"), Value.FromJson("{\"@ref\":\"classes\"}"));
        }

        [Test] public void TestArray()
        {
            Assert.AreEqual(new ArrayV(LongV.Of(1), StringV.Of("a string"), DoubleV.Of(3.14), BooleanV.True, NullV.Instance),
                Value.FromJson("[1, \"a string\", 3.14, true, null]"));
        }

        [Test] public void TestDate()
        {
            Assert.AreEqual(new DateV("2000-01-01"), Value.FromJson("{\"@date\":\"2000-01-01\"}"));
        }

        [Test] public void TestTimestamp()
        {
            Assert.AreEqual(new TsV("2000-01-01T01:10:30.123Z"), Value.FromJson("{\"@ts\":\"2000-01-01T01:10:30.123Z\"}"));
        }

        [Test] public void TestObject()
        {
            Assert.AreEqual(ObjectV.With("foo", ObjectV.With("bar", new ArrayV(BooleanV.True, NullV.Instance))),
                Value.FromJson("{\"foo\":{\"bar\":[true,null]}}"));
        }

        [Test] public void TestNull()
        {
            Assert.AreEqual(NullV.Instance, Value.FromJson("null"));
        }

        [Test] public void TestObjectLiteral()
        {
            Assert.AreEqual(ObjectV.With("@name", StringV.Of("Test")),
                Value.FromJson("{\"@obj\":{\"@name\":\"Test\"}}"));
        }

        [Test] public void TestSetRef()
        {
            
            Assert.AreEqual(new SetRef(ObjectV.With("match", new Ref("indexes/spells_by_element"), "terms", StringV.Of("fire"))),
                Value.FromJson("{" +
                "  \"@set\": {" +
                "    \"match\": { \"@ref\": \"indexes/spells_by_element\" }," +
                "    \"terms\": \"fire\"" +
                "  }" +
                "}"));
        }
    }
}
