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

        [Test] public void TestAbort()
        {
            AssertJsonEqual(Abort("message"),
                "{\"abort\":\"message\"}");
        }

        [Test] public void TestAt()
        {
            AssertJsonEqual(At(1, Paginate(Classes())),
                "{\"at\":1,\"expr\":{\"paginate\":{\"classes\":null}}}");

            AssertJsonEqual(At(Time("1970-01-01T00:00:00Z"), Paginate(Classes())),
                "{\"at\":{\"time\":\"1970-01-01T00:00:00Z\"},\"expr\":{\"paginate\":{\"classes\":null}}}");
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
            AssertJsonEqual(Get(Ref(Class("thing"), "123456789")),
                "{\"get\":{\"ref\":{\"class\":\"thing\"},\"id\":\"123456789\"}}");
        }

        [Test] public void TestKeyFromSecret()
        {
            AssertJsonEqual(KeyFromSecret("s3cr3t"),
                "{\"key_from_secret\":\"s3cr3t\"}");
        }

        [Test] public void TestPaginate()
        {
            AssertJsonEqual(
                Paginate(Databases()),
                "{\"paginate\":{\"databases\":null}}");

            AssertJsonEqual(
                Paginate(Databases(), after: Ref(Class("thing"), "123456789")),
                "{\"paginate\":{\"databases\":null},\"after\":{\"ref\":{\"class\":\"thing\"},\"id\":\"123456789\"}}");

            AssertJsonEqual(
                Paginate(Databases(), before: Ref(Class("thing"), "123456789")),
                "{\"paginate\":{\"databases\":null},\"before\":{\"ref\":{\"class\":\"thing\"},\"id\":\"123456789\"}}");

            AssertJsonEqual(
                Paginate(Databases(), ts: Time("1970-01-01T00:00:00Z")),
                "{\"paginate\":{\"databases\":null},\"ts\":{\"time\":\"1970-01-01T00:00:00Z\"}}");

            AssertJsonEqual(
                Paginate(Databases(), size: 10),
                "{\"paginate\":{\"databases\":null},\"size\":10}");

            AssertJsonEqual(
                Paginate(Databases(), events: true),
                "{\"paginate\":{\"databases\":null},\"events\":true}");

            AssertJsonEqual(
                Paginate(Databases(), sources: true),
                "{\"paginate\":{\"databases\":null},\"sources\":true}");
        }

        [Test] public void TestExists()
        {
            AssertJsonEqual(Exists(Ref(Class("thing"), "123456789")),
                "{\"exists\":{\"ref\":{\"class\":\"thing\"},\"id\":\"123456789\"}}");

            AssertJsonEqual(Exists(Ref(Class("thing"), "123456789"), Time("1970-01-01T00:00:00.123Z")),
                "{\"exists\":{\"ref\":{\"class\":\"thing\"},\"id\":\"123456789\"},\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}}");
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
                Update(Ref(Class("widgets"), "123456789"), Obj("name", "things")),
                "{\"update\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"name\":\"things\"}}}");
        }

        [Test] public void TestReplace()
        {
            AssertJsonEqual(
                Replace(Ref(Class("widgets"), "123456789"), Obj("data", Obj("name", "Computer"))),
                "{\"replace\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestDelete()
        {
            AssertJsonEqual(
                Delete(Ref(Class("widgets"), "123456789")),
                "{\"delete\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"}}");
        }

        [Test] public void TestInsert()
        {
            AssertJsonEqual(
                Insert(
                    Ref(Class("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    "create",
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"}," +
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"," +
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");

            AssertJsonEqual(
                Insert(
                    Ref(Class("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    ActionType.Create,
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},"+
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"},"+
                "\"action\":\"create\","+
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestRemove()
        {
            AssertJsonEqual(
                Remove(
                    Ref(Class("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    "create"),
                "{\"remove\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},"+
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");

            AssertJsonEqual(
                Remove(
                    Ref(Class("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    ActionType.Create),
                "{\"remove\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"}," +
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
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
                Union(Native.DATABASES),
                "{\"union\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Union(Native.DATABASES, Class("widgets")),
                "{\"union\":[{\"@ref\":{\"id\":\"databases\"}},{\"class\":\"widgets\"}]}");
        }

        [Test] public void TestIntersection()
        {
            AssertJsonEqual(
                Intersection(Native.DATABASES),
                "{\"intersection\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Intersection(Native.DATABASES, Class("widgets")),
                "{\"intersection\":[{\"@ref\":{\"id\":\"databases\"}},{\"class\":\"widgets\"}]}");
        }

        [Test] public void TestDifference()
        {
            AssertJsonEqual(
                Difference(Native.DATABASES),
                "{\"difference\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Difference(Native.DATABASES, Class("widgets")),
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
            AssertJsonEqual(Login(Ref(Class("widgets"), "123456789"), Obj("password", "P455w0rd")),
                 "{\"login\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"password\":\"P455w0rd\"}}}");
        }

        [Test] public void TestLogout()
        {
            AssertJsonEqual(Logout(true), "{\"logout\":true}");
            AssertJsonEqual(Logout(false), "{\"logout\":false}");
        }

        [Test] public void TestIdentify()
        {
            AssertJsonEqual(Identify(Ref(Class("widgets"), "123456789"), "P455w0rd"),
                "{\"identify\":{\"ref\":{\"class\":\"widgets\"},\"id\":\"123456789\"},\"password\":\"P455w0rd\"}");
        }

        [Test] public void TestIdentity()
        {
            AssertJsonEqual(Identity(),
                "{\"identity\":null}");
        }

        [Test] public void TestHasIdentity()
        {
            AssertJsonEqual(HasIdentity(),
                "{\"has_identity\":null}");
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

            AssertJsonEqual(Casefold("a string", "NFD"),
                "{\"casefold\":\"a string\",\"normalizer\":\"NFD\"}");

            AssertJsonEqual(Casefold("a string", Normalizer.NFD),
                "{\"casefold\":\"a string\",\"normalizer\":\"NFD\"}");
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

        [Test] public void TestNewId()
        {
            AssertJsonEqual(NewId(),
                "{\"new_id\":null}");
        }

        [Test] public void TestDatabase()
        {
            AssertJsonEqual(Database("db_name"), "{\"database\":\"db_name\"}");
            AssertJsonEqual(Database("db_name", Database("scope")), "{\"database\":\"db_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestIndex()
        {
            AssertJsonEqual(Index("index_name"), "{\"index\":\"index_name\"}");
            AssertJsonEqual(Index("index_name", Database("scope")), "{\"index\":\"index_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestClass()
        {
            AssertJsonEqual(Class("class_name"), "{\"class\":\"class_name\"}");
            AssertJsonEqual(Class("class_name", Database("scope")), "{\"class\":\"class_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestFunction()
        {
            AssertJsonEqual(Function("function_name"), "{\"function\":\"function_name\"}");
            AssertJsonEqual(Function("function_name", Database("scope")), "{\"function\":\"function_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestNativeRefs()
        {
            AssertJsonEqual(Classes(), "{\"classes\":null}");
            AssertJsonEqual(Databases(), "{\"databases\":null}");
            AssertJsonEqual(Indexes(), "{\"indexes\":null}");
            AssertJsonEqual(Functions(), "{\"functions\":null}");
            AssertJsonEqual(Keys(), "{\"keys\":null}");
            AssertJsonEqual(Tokens(), "{\"tokens\":null}");
            AssertJsonEqual(Credentials(), "{\"credentials\":null}");

            AssertJsonEqual(Classes(Database("scope")), "{\"classes\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Databases(Database("scope")), "{\"databases\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Indexes(Database("scope")), "{\"indexes\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Functions(Database("scope")), "{\"functions\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Keys(Database("scope")), "{\"keys\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Tokens(Database("scope")), "{\"tokens\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Credentials(Database("scope")), "{\"credentials\":{\"database\":\"scope\"}}");
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
                    @class: new RefV(id: "child-class", @class: Native.CLASSES)),
                "{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"child-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}}}"
            );

            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    @class: new RefV(
                        id: "child-class",
                        @class: Native.CLASSES,
                        database: new RefV(id: "child-database", @class: Native.DATABASES))),
                "{\"@ref\":{\"id\":\"123456789\",\"class\":{\"@ref\":{\"id\":\"child-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}},\"database\":{\"@ref\":{\"id\":\"child-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}}}"
            );
        }

        [Test]
        public void TestClassRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-class", @class: Native.CLASSES),
                "{\"@ref\":{\"id\":\"a-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-class", @class: Native.CLASSES, database: new RefV(id: "a-database", @class: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-class\",\"class\":{\"@ref\":{\"id\":\"classes\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestDatabaseRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-database", @class: Native.DATABASES),
                "{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "child-database", @class: Native.DATABASES, database: new RefV(id: "parent-database", @class: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"child-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}},\"database\":{\"@ref\":{\"id\":\"parent-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestIndexRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-index", @class: Native.INDEXES),
                "{\"@ref\":{\"id\":\"a-index\",\"class\":{\"@ref\":{\"id\":\"indexes\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-index", @class: Native.INDEXES, database: new RefV(id: "a-database", @class: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-index\",\"class\":{\"@ref\":{\"id\":\"indexes\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestKeyRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-key", @class: Native.KEYS),
                "{\"@ref\":{\"id\":\"a-key\",\"class\":{\"@ref\":{\"id\":\"keys\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-key", @class: Native.KEYS, database: new RefV(id: "a-database", @class: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-key\",\"class\":{\"@ref\":{\"id\":\"keys\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestFunctionRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-function", @class: Native.FUNCTIONS),
                "{\"@ref\":{\"id\":\"a-function\",\"class\":{\"@ref\":{\"id\":\"functions\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-function", @class: Native.FUNCTIONS, database: new RefV(id: "a-database", @class: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-function\",\"class\":{\"@ref\":{\"id\":\"functions\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"class\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestQuery()
        {
            AssertJsonEqual(Query(Lambda("x", Add(Var("x"), 1))), "{\"query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");
            AssertJsonEqual(Query(Lambda(x => Add(x, 1))), "{\"query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}");

            AssertJsonEqual(
                new QueryV(new Dictionary<string, Expr> { { "lambda", "x" }, { "expr", Add(Var("x"), 1) } }),
                "{\"@query\":{\"lambda\":\"x\",\"expr\":{\"add\":[{\"var\":\"x\"},1]}}}"
            );
        }
    }
}
