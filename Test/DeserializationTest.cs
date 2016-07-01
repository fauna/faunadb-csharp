using FaunaDB.Collections;
using FaunaDB.Types;
using NUnit.Framework;

namespace Test
{
    [TestFixture] public class DeserializationTest
    {
        [Test] public void TestString()
        {
            Assert.AreEqual(StringV.Of("a string"), Json.FromJson("\"a string\""));
        }

        [Test] public void TestBoolean()
        {
            Assert.AreEqual(BooleanV.True, Json.FromJson("true"));
            Assert.AreEqual(BooleanV.False, Json.FromJson("false"));
        }

        [Test] public void TestLong()
        {
            Assert.AreEqual(LongV.Of(long.MaxValue), Json.FromJson(long.MaxValue.ToString()));
            Assert.AreEqual(LongV.Of(long.MinValue), Json.FromJson(long.MinValue.ToString()));
            Assert.AreEqual(LongV.Of(10), Json.FromJson("10"));
        }

        [Test] public void TestDouble()
        {
            Assert.AreEqual(DoubleV.Of(3.14), Json.FromJson("3.14"));
        }

        [Test] public void TestRef()
        {
            Assert.AreEqual(new Ref("classes"), Json.FromJson("{\"@ref\":\"classes\"}"));
        }

        [Test] public void TestArray()
        {
            Assert.AreEqual(ArrayV.Of(LongV.Of(1), StringV.Of("a string"), DoubleV.Of(3.14), BooleanV.True, NullV.Instance),
                Json.FromJson("[1, \"a string\", 3.14, true, null]"));
        }

        [Test] public void TestDate()
        {
            Assert.AreEqual(new DateV("2000-01-01"), Json.FromJson("{\"@date\":\"2000-01-01\"}"));
        }

        [Test] public void TestTimestamp()
        {
            Assert.AreEqual(new TsV("2000-01-01T01:10:30.123Z"), Json.FromJson("{\"@ts\":\"2000-01-01T01:10:30.123Z\"}"));
        }

        [Test] public void TestObject()
        {
            Assert.AreEqual(ObjectV.With("foo", ObjectV.With("bar", ArrayV.Of(BooleanV.True, NullV.Instance))),
                Json.FromJson("{\"foo\":{\"bar\":[true,null]}}"));
        }

        [Test] public void TestNull()
        {
            Assert.AreEqual(NullV.Instance, Json.FromJson("null"));
        }

        [Test] public void TestObjectLiteral()
        {
            Assert.AreEqual(ObjectV.With("@name", StringV.Of("Test")),
                Json.FromJson("{\"@obj\":{\"@name\":\"Test\"}}"));
        }

        [Test] public void TestSetRef()
        {
            
            Assert.AreEqual(new SetRef(ImmutableDictionary.Of<string, Value>("match", new Ref("indexes/spells_by_element"), "terms", StringV.Of("fire"))),
                Json.FromJson("{" +
                "  \"@set\": {" +
                "    \"match\": { \"@ref\": \"indexes/spells_by_element\" }," +
                "    \"terms\": \"fire\"" +
                "  }" +
                "}"));
        }
    }
}
