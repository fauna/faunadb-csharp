using System;
using System.Collections.Generic;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class SerializationTest
    {
        static void AssertJsonEqual(Expr expr, string value)
        {
            Assert.AreEqual(value, JsonConvert.SerializeObject(expr, Formatting.None));
        }

        [Test] public void TestLiteralValues()
        {
            AssertJsonEqual(10L, "10");
            AssertJsonEqual(10, "10");
            AssertJsonEqual("a string", "\"a string\"");
            AssertJsonEqual(3.14, "3.14");
            AssertJsonEqual(true, "true");
            AssertJsonEqual(false, "false");
            AssertJsonEqual(NullV.Instance, "null");
        }

        [Test] public void TestArrayValues()
        {
            AssertJsonEqual(Arr(10, 3.14, "a string", true, false, NullV.Instance), "[10,3.14,\"a string\",true,false,null]");
        }

        [Test] public void TestObjectValues()
        {
            AssertJsonEqual(Obj(), "{\"object\":{}}");

            AssertJsonEqual(Obj("k0", "v0", "k1", "v1"), "{\"object\":{\"k0\":\"v0\",\"k1\":\"v1\"}}");

            AssertJsonEqual(Obj("foo", "bar"), "{\"object\":{\"foo\":\"bar\"}}");

            AssertJsonEqual(Obj("long", 10, "double", 2.78), "{\"object\":{\"long\":10,\"double\":2.78}}");
        }

        [Test] public void TestObjectAndArrays()
        {
            AssertJsonEqual(Obj("foo", Arr("bar")), "{\"object\":{\"foo\":[\"bar\"]}}");

            AssertJsonEqual(
                Obj("foo", Arr("bar", Obj("foo", "bar"))),
                "{\"object\":{\"foo\":[\"bar\",{\"object\":{\"foo\":\"bar\"}}]}}");
        }

        [Test] public void TestComplexObjects()
        {
            AssertJsonEqual(Obj("a", Obj("b", Obj("c", "d"))), "{\"object\":{\"a\":{\"object\":{\"b\":{\"object\":{\"c\":\"d\"}}}}}}");
        }

        [Test] public void TestRef()
        {
            AssertJsonEqual(BuiltIn.CLASSES, "{\"@ref\":{\"id\":\"classes\"}}");

            AssertJsonEqual(Ref(Class("people"), "id1"), "{\"ref\":{\"class\":\"people\"},\"id\":\"id1\"}");
        }

        [Test] public void TestTimestamp()
        {
            AssertJsonEqual(new TimeV("1970-01-01T00:00:00Z"), "{\"@ts\":\"1970-01-01T00:00:00Z\"}");

            AssertJsonEqual(new TimeV(new DateTime(1970, 1, 1, 0, 0, 0, 0)), "{\"@ts\":\"1970-01-01T00:00:00Z\"}");
        }

        [Test] public void TestDate()
        {
            AssertJsonEqual(new DateV("2000-01-01"), "{\"@date\":\"2000-01-01\"}");

            AssertJsonEqual(new DateV(new DateTime(2000, 1, 1)), "{\"@date\":\"2000-01-01\"}");
        }

        [Test] public void TestBytes()
        {
            AssertJsonEqual(new BytesV(0x1, 0x2, 0x3, 0x4), "{\"@bytes\":\"AQIDBA==\"}");
        }

        [Test] public void TestBytesUrlSafe()
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

        [Test] public void TestAt()
        {
            AssertJsonEqual(At(1, Paginate(BuiltIn.CLASSES)),
                "{\"at\":1,\"expr\":{\"paginate\":{\"@ref\":{\"id\":\"classes\"}}}}");

            AssertJsonEqual(At(Time("1970-01-01T00:00:00Z"), Paginate(BuiltIn.CLASSES)),
                "{\"at\":{\"time\":\"1970-01-01T00:00:00Z\"},\"expr\":{\"paginate\":{\"@ref\":{\"id\":\"classes\"}}}}");
        }

        [Test] public void TestLet()
        {
            var variables = new Dictionary<string, Expr> {
                { "x", 10 }
            };

            AssertJsonEqual(Let(variables, Var("x")),
                "{\"let\":{\"x\":10},\"in\":{\"var\":\"x\"}}");

            ////

            AssertJsonEqual(Let("x", 10).In(Var("x")),
                "{\"let\":{\"x\":10},\"in\":{\"var\":\"x\"}}");

            ////

            AssertJsonEqual(Let("x", 10, "y", 20).In(Add(Var("x"), Var("y"))),
                "{\"let\":{\"x\":10,\"y\":20},\"in\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");
        }

        [Test] public void TestVar()
        {
            AssertJsonEqual(Var("x"), "{\"var\":\"x\"}");
        }

        [Test] public void TestIf()
        {
            AssertJsonEqual(If(true, 1, 0), "{\"if\":true,\"then\":1,\"else\":0}");
        }

        [Test] public void TestDo()
        {
            AssertJsonEqual(Do(If(true, 1, 0), "a string"), "{\"do\":[{\"if\":true,\"then\":1,\"else\":0},\"a string\"]}");
        }

        [Test] public void TestLamda()
        {
            AssertJsonEqual(Lambda("x", Var("x")),
                "{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}}");

            AssertJsonEqual(Lambda(x => x),
                "{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}}");

            ////

            AssertJsonEqual(Lambda(Arr("x", "y"), Add(Var("x"), Var("y"))),
                "{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");

            AssertJsonEqual(Lambda((x, y) => Add(x, y)),
                "{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");
        }

        [Test] public void TestMap()
        {
            AssertJsonEqual(Map(Arr(1, 2, 3), Lambda("x", Var("x"))),
                "{\"map\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Map(Arr(1, 2, 3), Lambda(x => x)),
                "{\"map\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Map(Arr(1, 2, 3), x => x),
                "{\"map\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Map(Arr(Arr(1, 2), Arr(3, 4)), Lambda(Arr("x", "y"), Add(Var("x"), Var("y")))),
                "{\"map\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Map(Arr(Arr(1, 2), Arr(3, 4)), Lambda((x, y) => Add(x, y))),
                "{\"map\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Map(Arr(Arr(1, 2), Arr(3, 4)), (x, y) => Add(x, y)),
                "{\"map\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");
        }

        [Test] public void TestForeach()
        {
            AssertJsonEqual(Foreach(Arr(1, 2, 3), Lambda("x", Var("x"))),
                "{\"foreach\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Foreach(Arr(1, 2, 3), Lambda(x => x)),
                "{\"foreach\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Foreach(Arr(1, 2, 3), x => x),
                "{\"foreach\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Foreach(Arr(Arr(1, 2), Arr(3, 4)), Lambda(Arr("x", "y"), Add(Var("x"), Var("y")))),
                "{\"foreach\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Foreach(Arr(Arr(1, 2), Arr(3, 4)), Lambda((x, y) => Add(x, y))),
                "{\"foreach\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Foreach(Arr(Arr(1, 2), Arr(3, 4)), (x, y) => Add(x, y)),
                "{\"foreach\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");
        }

        [Test] public void TestFilter()
        {
            AssertJsonEqual(Filter(Arr(1, 2, 3), Lambda("x", Var("x"))),
                "{\"filter\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Filter(Arr(1, 2, 3), Lambda(x => x)),
                "{\"filter\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Filter(Arr(1, 2, 3), x => x),
                "{\"filter\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Filter(Arr(Arr(1, 2), Arr(3, 4)), Lambda(Arr("x", "y"), Add(Var("x"), Var("y")))),
                "{\"filter\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Filter(Arr(Arr(1, 2), Arr(3, 4)), Lambda((x, y) => Add(x, y))),
                "{\"filter\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");

            AssertJsonEqual(Filter(Arr(Arr(1, 2), Arr(3, 4)), (x, y) => Add(x, y)),
                "{\"filter\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");
        }

        [Test] public void TestTake()
        {
            AssertJsonEqual(Take(2, Arr(1, 2, 3)),
                "{\"take\":2,\"collection\":[1,2,3]}");
        }

        [Test] public void TestDrop()
        {
            AssertJsonEqual(Drop(1, Arr(1, 2, 3)),
                "{\"drop\":1,\"collection\":[1,2,3]}");
        }

        [Test] public void TestPrepend()
        {
            AssertJsonEqual(Prepend(Arr(1, 2, 3), Arr(4, 5, 6)),
                "{\"prepend\":[1,2,3],\"collection\":[4,5,6]}");
        }

        [Test] public void TestAppend()
        {
            AssertJsonEqual(Append(Arr(1, 2, 3), Arr(4, 5, 6)),
                "{\"append\":[1,2,3],\"collection\":[4,5,6]}");
        }

        [Test] public void TestGet()
        {
            AssertJsonEqual(Get(new RefV(id: "123456789", @class: new ClassV("thing"))),
                "{\"get\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"thing\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}}");
        }

        [Test] public void TestKeyFromSecret()
        {
            AssertJsonEqual(KeyFromSecret("s3cr3t"),
                "{\"key_from_secret\":\"s3cr3t\"}");
        }

        [Test] public void TestPaginate()
        {
            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, after: new RefV(id: "123456789", @class: new ClassV("thing"))),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"after\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"thing\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, before: new RefV(id: "123456789", @class: new ClassV("thing"))),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"before\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"thing\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, ts: new TimeV("1970-01-01T00:00:00Z")),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"ts\":{\"@ts\":\"1970-01-01T00:00:00Z\"}}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, size: 10),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"size\":10}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, events: true),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"events\":true}");

            AssertJsonEqual(
                Paginate(BuiltIn.DATABASES, sources: true),
                "{\"paginate\":{\"@ref\":{\"id\":\"databases\"}},\"sources\":true}");
        }

        [Test] public void TestExists()
        {
            AssertJsonEqual(Exists(new RefV(id: "123456789", @class: new ClassV("thing"))),
                "{\"exists\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"thing\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}}");

            AssertJsonEqual(Exists(new RefV(id: "123456789", @class: new ClassV("thing")), new TimeV("1970-01-01T00:00:00.123Z")),
                "{\"exists\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"thing\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}}");
        }

        [Test] public void TestCreate()
        {
            AssertJsonEqual(
                Create(Class("widgets"), Obj("data", "some-data")),
                "{\"create\":{\"class\":\"widgets\"},\"params\":{\"object\":{\"data\":\"some-data\"}}}");
        }

        [Test] public void TestUpdate()
        {
            AssertJsonEqual(
                Update(new RefV(id: "123456789", @class: new ClassV("widgets")), Obj("name", "things")),
                "{\"update\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},\"params\":{\"object\":{\"name\":\"things\"}}}");
        }

        [Test] public void TestReplace()
        {
            AssertJsonEqual(
                Replace(new RefV(id: "123456789", @class: new ClassV("widgets")), Obj("data", Obj("name", "Computer"))),
                "{\"replace\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestDelete()
        {
            AssertJsonEqual(
                Delete(new RefV(id: "123456789", @class: new ClassV("widgets"))),
                "{\"delete\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}}");
        }

        [Test] public void TestInsert()
        {
            AssertJsonEqual(
                Insert(
                    new RefV(id: "123456789", @class: new ClassV("widgets")),
                    new TimeV("1970-01-01T00:00:00.123Z"),
                    "create",
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}," +
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"," +
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");

            AssertJsonEqual(
                Insert(
                    new RefV(id: "123456789", @class: new ClassV("widgets")),
                    new TimeV("1970-01-01T00:00:00.123Z"),
                    ActionType.Create,
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},"+
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"},"+
                "\"action\":\"create\","+
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestRemove()
        {
            AssertJsonEqual(
                Remove(
                    new RefV(id: "123456789", @class: new ClassV("widgets")),
                    new TimeV("1970-01-01T00:00:00.123Z"),
                    "create"),
                "{\"remove\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},"+
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");

            AssertJsonEqual(
                Remove(
                    new RefV(id: "123456789", @class: new ClassV("widgets")),
                    new TimeV("1970-01-01T00:00:00.123Z"),
                    ActionType.Create),
                "{\"remove\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}," +
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");
        }

        [Test] public void TestCreateClass()
        {
            AssertJsonEqual(CreateClass(Obj("name", "class_name")),
                "{\"create_class\":{\"object\":{\"name\":\"class_name\"}}}");
        }

        [Test] public void TestCreateDatabase()
        {
            AssertJsonEqual(CreateDatabase(Obj("name", "db_name")),
                "{\"create_database\":{\"object\":{\"name\":\"db_name\"}}}");
        }

        [Test] public void TestCreateIndex()
        {
            AssertJsonEqual(
                CreateIndex(Obj("name", "index_name", "source", Class("class_name"))),
                "{\"create_index\":{\"object\":{\"name\":\"index_name\",\"source\":{\"class\":\"class_name\"}}}}");
        }

        [Test] public void TestCreateKey()
        {
            AssertJsonEqual(
                CreateKey(Obj("database", Database("db_name"), "role", "client")),
                "{\"create_key\":{\"object\":{\"database\":{\"database\":\"db_name\"},\"role\":\"client\"}}}");
        }

        [Test] public void TestMatch()
        {
            AssertJsonEqual(
                Match(Index("all_the_things")),
                "{\"match\":{\"index\":\"all_the_things\"}}");

            AssertJsonEqual(
                Match(Index("widgets_by_name"), "Computer"),
                "{\"match\":{\"index\":\"widgets_by_name\"},\"terms\":\"Computer\"}");

            AssertJsonEqual(
                Match(Index("widgets_by_name"), "Computer", "Monitor"),
                "{\"match\":{\"index\":\"widgets_by_name\"},\"terms\":[\"Computer\",\"Monitor\"]}");
        }

        [Test] public void TestUnion()
        {
            AssertJsonEqual(
                Union(BuiltIn.DATABASES),
                "{\"union\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Union(BuiltIn.DATABASES, Class("widgets")),
                "{\"union\":[{\"@ref\":{\"id\":\"databases\"}},{\"class\":\"widgets\"}]}");
        }

        [Test] public void TestIntersection()
        {
            AssertJsonEqual(
                Intersection(BuiltIn.DATABASES),
                "{\"intersection\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Intersection(BuiltIn.DATABASES, Class("widgets")),
                "{\"intersection\":[{\"@ref\":{\"id\":\"databases\"}},{\"class\":\"widgets\"}]}");
        }

        [Test] public void TestDifference()
        {
            AssertJsonEqual(
                Difference(BuiltIn.DATABASES),
                "{\"difference\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Difference(BuiltIn.DATABASES, Class("widgets")),
                "{\"difference\":[{\"@ref\":{\"id\":\"databases\"}},{\"class\":\"widgets\"}]}");
        }

        [Test] public void TestDistinct()
        {
            AssertJsonEqual(Distinct(Match(Index("widgets"))),
                "{\"distinct\":{\"match\":{\"index\":\"widgets\"}}}");
        }

        [Test] public void TestJoin()
        {
            AssertJsonEqual(Join(Match(Index("widgets")), Index("other_widgets")),
                "{\"join\":{\"match\":{\"index\":\"widgets\"}}," +
                "\"with\":{\"index\":\"other_widgets\"}}");

            AssertJsonEqual(Join(Match(Index("widgets")), widget => Match(Index("widgets"), widget)),
                "{\"join\":{\"match\":{\"index\":\"widgets\"}}," +
                "\"with\":{\"lambda\":\"widget\",\"expr\":{\"match\":{\"index\":\"widgets\"},\"terms\":{\"var\":\"widget\"}}}}");
        }

        [Test] public void TestLogin()
        {
            AssertJsonEqual(Login(new RefV(id: "123456789", @class: new ClassV("widgets")), Obj("password", "P455w0rd")),
                "{\"login\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},\"params\":{\"object\":{\"password\":\"P455w0rd\"}}}");
        }

        [Test] public void TestLogout()
        {
            AssertJsonEqual(Logout(true), "{\"logout\":true}");
            AssertJsonEqual(Logout(false), "{\"logout\":false}");
        }

        [Test] public void TestIdentify()
        {
            AssertJsonEqual(Identify(new RefV(id: "123456789", @class: new ClassV("widgets")), "P455w0rd"),
                "{\"identify\":{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"widgets\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}},\"password\":\"P455w0rd\"}");
        }

        [Test] public void TestConcat()
        {
            AssertJsonEqual(Concat("str"),
                "{\"concat\":\"str\"}");

            AssertJsonEqual(Concat("str", "/"),
                "{\"concat\":\"str\",\"separator\":\"/\"}");

            AssertJsonEqual(Concat(Arr("str0", "str1")),
                "{\"concat\":[\"str0\",\"str1\"]}");

            AssertJsonEqual(Concat(Arr("str0", "str1"), "/"),
                "{\"concat\":[\"str0\",\"str1\"],\"separator\":\"/\"}");
        }

        [Test] public void TestCasefold()
        {
            AssertJsonEqual(Casefold("a string"),
                "{\"casefold\":\"a string\"}");
        }

        [Test] public void TestTime()
        {
            AssertJsonEqual(Time("1970-01-01T00:00:00+00:00"),
                "{\"time\":\"1970-01-01T00:00:00+00:00\"}");

            AssertJsonEqual(Time("now"),
                "{\"time\":\"now\"}");
        }

        [Test] public void TestEpoch()
        {
            AssertJsonEqual(Epoch(1, "second"),
                "{\"epoch\":1,\"unit\":\"second\"}");

            AssertJsonEqual(Epoch(1, "millisecond"),
                "{\"epoch\":1,\"unit\":\"millisecond\"}");

            AssertJsonEqual(Epoch(1, "microsecond"),
                "{\"epoch\":1,\"unit\":\"microsecond\"}");

            AssertJsonEqual(Epoch(1, "nanosecond"),
                "{\"epoch\":1,\"unit\":\"nanosecond\"}");

            AssertJsonEqual(Epoch(1, TimeUnit.Second),
                "{\"epoch\":1,\"unit\":\"second\"}");

            AssertJsonEqual(Epoch(1, TimeUnit.Millisecond),
                "{\"epoch\":1,\"unit\":\"millisecond\"}");

            AssertJsonEqual(Epoch(1, TimeUnit.Microsecond),
                "{\"epoch\":1,\"unit\":\"microsecond\"}");

            AssertJsonEqual(Epoch(1, TimeUnit.Nanosecond),
                "{\"epoch\":1,\"unit\":\"nanosecond\"}");
        }

        [Test] public void TestDateFn()
        {
            AssertJsonEqual(Date("1970-01-01"),
                "{\"date\":\"1970-01-01\"}");
        }

        [Test] public void TestNextId()
        {
            AssertJsonEqual(NextId(),
                "{\"next_id\":null}");
        }

        [Test] public void TestDatabase()
        {
            AssertJsonEqual(Database("db_name"),
                "{\"database\":\"db_name\"}");
        }

        [Test] public void TestIndex()
        {
            AssertJsonEqual(Index("index_name"),
                "{\"index\":\"index_name\"}");
        }

        [Test] public void TestClass()
        {
            AssertJsonEqual(Class("class_name"),
                "{\"class\":\"class_name\"}");
        }

        [Test] public void TestEquals()
        {
            AssertJsonEqual(EqualsFn("value"),
                "{\"equals\":\"value\"}");

            AssertJsonEqual(EqualsFn("value", 10),
                "{\"equals\":[\"value\",10]}");
        }

        [Test] public void TestContains()
        {
            AssertJsonEqual(Contains(Arr("favorites", "foods"), Obj("favorites", Obj("foods", Arr("crunchings", "munchings", "lunchings")))),
                "{\"contains\":[\"favorites\",\"foods\"],\"in\":{\"object\":{\"favorites\":{\"object\":{\"foods\":[\"crunchings\",\"munchings\",\"lunchings\"]}}}}}");
        }

        [Test] public void TestSelect()
        {
            AssertJsonEqual(Select(Arr("favorites", "foods", 1), Obj("favorites", Obj("foods", Arr("crunchings", "munchings", "lunchings")))),
                "{\"select\":[\"favorites\",\"foods\",1]," +
                "\"from\":{\"object\":{\"favorites\":{\"object\":{\"foods\":[\"crunchings\",\"munchings\",\"lunchings\"]}}}}}");

            AssertJsonEqual(Select(Arr("favorites", "foods", 1), Obj("favorites", Obj("foods", Arr("crunchings", "munchings", "lunchings"))), "defaultValue"),
                "{\"select\":[\"favorites\",\"foods\",1]," +
                "\"from\":{\"object\":{\"favorites\":{\"object\":{\"foods\":[\"crunchings\",\"munchings\",\"lunchings\"]}}}}," +
                "\"default\":\"defaultValue\"}");
        }

        [Test] public void TestAdd()
        {
            AssertJsonEqual(Add(1), "{\"add\":1}");
            AssertJsonEqual(Add(1, 2), "{\"add\":[1,2]}");
        }

        [Test] public void TestMultiply()
        {
            AssertJsonEqual(Multiply(1), "{\"multiply\":1}");
            AssertJsonEqual(Multiply(1, 2), "{\"multiply\":[1,2]}");
        }

        [Test] public void TestSubtract()
        {
            AssertJsonEqual(Subtract(1), "{\"subtract\":1}");
            AssertJsonEqual(Subtract(1, 2), "{\"subtract\":[1,2]}");
        }

        [Test] public void TestDivide()
        {
            AssertJsonEqual(Divide(1), "{\"divide\":1}");
            AssertJsonEqual(Divide(1, 2), "{\"divide\":[1,2]}");
        }

        [Test] public void TestModulo()
        {
            AssertJsonEqual(Modulo(1), "{\"modulo\":1}");
            AssertJsonEqual(Modulo(1, 2), "{\"modulo\":[1,2]}");
        }

        [Test] public void TestLT()
        {
            AssertJsonEqual(LT(1), "{\"lt\":1}");
            AssertJsonEqual(LT(1, 2), "{\"lt\":[1,2]}");
        }

        [Test] public void TestLTE()
        {
            AssertJsonEqual(LTE(1), "{\"lte\":1}");
            AssertJsonEqual(LTE(1, 2), "{\"lte\":[1,2]}");
        }

        [Test] public void TestGT()
        {
            AssertJsonEqual(GT(1), "{\"gt\":1}");
            AssertJsonEqual(GT(1, 2), "{\"gt\":[1,2]}");
        }

        [Test] public void TestGTE()
        {
            AssertJsonEqual(GTE(1), "{\"gte\":1}");
            AssertJsonEqual(GTE(1, 2), "{\"gte\":[1,2]}");
        }

        [Test] public void TestAnd()
        {
            AssertJsonEqual(And(false), "{\"and\":false}");
            AssertJsonEqual(And(true, false), "{\"and\":[true,false]}");
        }

        [Test] public void TestOr()
        {
            AssertJsonEqual(Or(true), "{\"or\":true}");
            AssertJsonEqual(Or(true, false), "{\"or\":[true,false]}");
        }

        [Test] public void TestNot()
        {
            AssertJsonEqual(Not(true), "{\"not\":true}");
            AssertJsonEqual(Not(false), "{\"not\":false}");
        }

        [Test]
        public void TestInstanceRef()
        {
            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    @class: new ClassV(id: "child-class")),
                "{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"child-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}"
            );

            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    @class: new ClassV(
                        id: "child-class",
                        database: new DatabaseV("child-database"))),
                "{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"child-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}},\"database\":{\"@ref\":{\"id\":\"child-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}}}"
            );
        }

        [Test]
        public void TestClassRef()
        {
            AssertJsonEqual(
                new ClassV(id: "a-class", database: new DatabaseV("a-database")),
                "{\"@ref\":{\"id\":\"a-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestDatabaseRef()
        {
            AssertJsonEqual(
                new DatabaseV(id: "a-database"),
                "{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}"
            );

            AssertJsonEqual(
                new DatabaseV(id: "child-database", database: new DatabaseV("parent-database")),
                "{\"@ref\":{\"id\":\"child-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}},\"database\":{\"@ref\":{\"id\":\"parent-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestIndexRef()
        {
            AssertJsonEqual(
                new IndexV(id: "a-index"),
                "{\"@ref\":{\"id\":\"a-index\",\"class\":{\"@ref\":{\"id\":\"indexes\"}}}}"
            );

            AssertJsonEqual(
                new IndexV(id: "a-index", database: new DatabaseV("a-database")),
                "{\"@ref\":{\"id\":\"a-index\",\"class\":{\"@ref\":{\"id\":\"indexes\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestKeyRef()
        {
            AssertJsonEqual(
                new KeyV(id: "a-key"),
                "{\"@ref\":{\"id\":\"a-key\",\"class\":{\"@ref\":{\"id\":\"keys\"}}}}"
            );

            AssertJsonEqual(
                new KeyV(id: "a-key", database: new DatabaseV("a-database")),
                "{\"@ref\":{\"id\":\"a-key\",\"class\":{\"@ref\":{\"id\":\"keys\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestFunctionRef()
        {
            AssertJsonEqual(
                new FunctionV(id: "a-function"),
                "{\"@ref\":{\"id\":\"a-function\",\"class\":{\"@ref\":{\"id\":\"functions\"}}}}"
            );

            AssertJsonEqual(
                new FunctionV(id: "a-function", database: new DatabaseV("a-database")),
                "{\"@ref\":{\"id\":\"a-function\",\"class\":{\"@ref\":{\"id\":\"functions\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestQuery()
        {
            AssertJsonEqual(Query(Lambda("x", Add(Var("x"), 1))), "{\"query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
            AssertJsonEqual(Query(Lambda(x => Add(x, 1))), "{\"query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");

            AssertJsonEqual(QueryV.Of("x", Add(Var("x"), 1)), "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
            AssertJsonEqual(QueryV.Of(x => Add(x, 1)), "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
        }
    }
}
