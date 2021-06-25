using System.Collections.Generic;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture]
    public class DeserializationTest
    {
        private static void AssertJsonEqual(Value value, string json)
        {
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            Assert.AreEqual(value, JsonConvert.DeserializeObject<Value>(json, settings));
        }

        [Test]
        public void TestString()
        {
            AssertJsonEqual(StringV.Of("a string"), "\"a string\"");
        }

        [Test]
        public void TestBoolean()
        {
            AssertJsonEqual(BooleanV.True, "true");
            AssertJsonEqual(BooleanV.False, "false");
        }

        [Test]
        public void TestLong()
        {
            AssertJsonEqual(LongV.Of(long.MaxValue), long.MaxValue.ToString());
            AssertJsonEqual(LongV.Of(long.MinValue), long.MinValue.ToString());
            AssertJsonEqual(LongV.Of(10), "10");
        }

        [Test]
        public void TestDouble()
        {
            AssertJsonEqual(DoubleV.Of(3.14), "3.14");
        }

        [Test]
        public void TestArray()
        {
            AssertJsonEqual(ArrayV.Of(LongV.Of(1), StringV.Of("a string"), DoubleV.Of(3.14), BooleanV.True, NullV.Instance),
                "[1, \"a string\", 3.14, true, null]");

            AssertJsonEqual(ArrayV.Empty, "[]");
        }

        [Test]
        public void TestDate()
        {
            AssertJsonEqual(new DateV("2000-01-01"), "{\"@date\":\"2000-01-01\"}");
        }

        [Test]
        public void TestTimestamp()
        {
            AssertJsonEqual(new TimeV("2000-01-01T01:10:30.123Z"), "{\"@ts\":\"2000-01-01T01:10:30.123Z\"}");
        }

        [Test]
        public void TestObject()
        {
            AssertJsonEqual(ObjectV.With("foo", ObjectV.With("bar", ArrayV.Of(BooleanV.True, NullV.Instance))),
                "{\"foo\":{\"bar\":[true,null]}}");

            AssertJsonEqual(ObjectV.Empty, "{}");
        }

        [Test]
        public void TestNull()
        {
            AssertJsonEqual(NullV.Instance, "null");
        }

        [Test]
        public void TestObjectLiteral()
        {
            AssertJsonEqual(ObjectV.With("@name", StringV.Of("Test")),
                "{\"@obj\":{\"@name\":\"Test\"}}");

            AssertJsonEqual(ObjectV.Empty, "{\"@obj\":{}}");
        }

        [Test]
        public void TestSetRef()
        {
            var expected = new SetRefV(new Dictionary<string, Value>
            {
                { "match", new RefV(id: "spells_by_element", collection: Native.INDEXES) },
                { "terms", StringV.Of("fire") },
            });

            AssertJsonEqual(expected,
                "{" +
                "  \"@set\": {" +
                "    \"match\": { \"@ref\": {\"id\": \"spells_by_element\", \"collection\": {\"@ref\": {\"id\": \"indexes\"} } } }," +
                "    \"terms\": \"fire\"" +
                "  }" +
                "}");

            AssertJsonEqual(new SetRefV(new Dictionary<string, Value>()),
                "{\"@set\":{}}");
        }

        [Test]
        public void TestBytes()
        {
            AssertJsonEqual(new BytesV(0x1, 0x2, 0x3, 0x4),
                "{\"@bytes\": \"AQIDBA==\"}");
        }

        [Test]
        public void TestBytesUrlSafe()
        {
            AssertJsonEqual(new BytesV(0xf8), "{\"@bytes\":\"-A==\"}");
            AssertJsonEqual(new BytesV(0xf9), "{\"@bytes\":\"-Q==\"}");
            AssertJsonEqual(new BytesV(0xfa), "{\"@bytes\":\"-g==\"}");
            AssertJsonEqual(new BytesV(0xfb), "{\"@bytes\":\"-w==\"}");
            AssertJsonEqual(new BytesV(0xfc), "{\"@bytes\":\"_A==\"}");
            AssertJsonEqual(new BytesV(0xfd), "{\"@bytes\":\"_Q==\"}");
            AssertJsonEqual(new BytesV(0xfe), "{\"@bytes\":\"_g==\"}");
            AssertJsonEqual(new BytesV(0xff), "{\"@bytes\":\"_w==\"}");
        }

        [Test]
        public void TestBuiltInCollections()
        {
            AssertJsonEqual(Native.COLLECTIONS, "{\"@ref\":{\"id\":\"collections\"}}");
            AssertJsonEqual(Native.INDEXES, "{\"@ref\":{\"id\":\"indexes\"}}");
            AssertJsonEqual(Native.DATABASES, "{\"@ref\":{\"id\":\"databases\"}}");
            AssertJsonEqual(Native.FUNCTIONS, "{\"@ref\":{\"id\":\"functions\"}}");
            AssertJsonEqual(Native.KEYS, "{\"@ref\":{\"id\":\"keys\"}}");
        }

        [Test]
        public void TestInstanceRef()
        {
            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    collection: new RefV(id: "child-class", collection: Native.COLLECTIONS)),
                "{\"@ref\": {\"id\": \"123456789\", \"collection\": {\"@ref\": {\"id\": \"child-class\", \"collection\": {\"@ref\": {\"id\": \"collections\"}}}}}}"
            );

            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    collection: new RefV(
                        id: "child-class",
                        collection: Native.COLLECTIONS,
                        database: new RefV(id: "child-database", collection: Native.DATABASES))),
                "{\"@ref\": {\"id\": \"123456789\", \"collection\": {\"@ref\": {\"id\": \"child-class\", \"collection\": {\"@ref\": {\"id\": \"collections\"}}, \"database\": {\"@ref\": {\"id\": \"child-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}}}"
            );
        }

        [Test]
        public void TestClassRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-class", collection: Native.COLLECTIONS),
                "{\"@ref\": {\"id\": \"a-class\", \"collection\": {\"@ref\": {\"id\": \"collections\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-class", collection: Native.COLLECTIONS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\": {\"id\": \"a-class\", \"collection\": {\"@ref\": {\"id\": \"collections\"}}, \"database\": {\"@ref\": {\"id\": \"a-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestDatabaseRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-database", collection: Native.DATABASES),
                "{\"@ref\": {\"id\": \"a-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "child-database", collection: Native.DATABASES, database: new RefV(id: "parent-database", collection: Native.DATABASES)),
                "{\"@ref\": {\"id\": \"child-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}, \"database\": {\"@ref\": {\"id\": \"parent-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestIndexRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-index", collection: Native.INDEXES),
                "{\"@ref\": {\"id\": \"a-index\", \"collection\": {\"@ref\": {\"id\": \"indexes\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-index", collection: Native.INDEXES, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\": {\"id\": \"a-index\", \"collection\": {\"@ref\": {\"id\": \"indexes\"}}, \"database\": {\"@ref\": {\"id\": \"a-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestKeyRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-key", collection: Native.KEYS),
                "{\"@ref\": {\"id\": \"a-key\", \"collection\": {\"@ref\": {\"id\": \"keys\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-key", collection: Native.KEYS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\": {\"id\": \"a-key\", \"collection\": {\"@ref\": {\"id\": \"keys\"}}, \"database\": {\"@ref\": {\"id\": \"a-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestFunctionRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-function", collection: Native.FUNCTIONS),
                "{\"@ref\": {\"id\": \"a-function\", \"collection\": {\"@ref\": {\"id\": \"functions\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-function", collection: Native.FUNCTIONS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\": {\"id\": \"a-function\", \"collection\": {\"@ref\": {\"id\": \"functions\"}}, \"database\": {\"@ref\": {\"id\": \"a-database\", \"collection\": {\"@ref\": {\"id\": \"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestQuery()
        {
            AssertJsonEqual(
                new QueryV(new Dictionary<string, Expr> { { "lambda", "x" }, { "expr", Add(Var("x"), 1) } }),
                "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}"
            );

            AssertJsonEqual(
              new QueryV(new Dictionary<string, Expr>
              {
                    { "lambda", "x" },
                    { "expr", Add(Var("x"), 1) },
                    { "api_version", "2.12" },
              }),
              "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]},\"api_version\":\"2.12\"}}"
          );
        }

        [Test]
        public void TestDocuments()
        {
            AssertJsonEqual(
                ObjectV.With("documents", ObjectV.With("collection", "foo")),
                "{\"documents\":{\"collection\":\"foo\"}}"
            );
        }
    }
}
