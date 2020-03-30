using System;
using System.Collections.Generic;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;
using static FaunaDB.Types.Encoder;

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
            AssertJsonEqual(-1.0, "-1.0");
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
            AssertJsonEqual(Ref(Collection("people"), "id1"), "{\"ref\":{\"collection\":\"people\"},\"id\":\"id1\"}");
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
            AssertJsonEqual(At(1, Paginate(Collections())),
                "{\"at\":1,\"expr\":{\"paginate\":{\"collections\":null}}}");

            AssertJsonEqual(At(Time("1970-01-01T00:00:00Z"), Paginate(Collections())),
                "{\"at\":{\"time\":\"1970-01-01T00:00:00Z\"},\"expr\":{\"paginate\":{\"collections\":null}}}");
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
                "{\"let\":[{\"x\":10}],\"in\":{\"var\":\"x\"}}");

            ////

            AssertJsonEqual(Let("x", 10, "y", 20).In(Add(Var("x"), Var("y"))),
                "{\"let\":[{\"x\":10},{\"y\":20}],\"in\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");
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

        [Test] public void TestIsEmpty()
        {
            AssertJsonEqual(IsEmpty(Arr(1, 2, 3)),
                "{\"is_empty\":[1,2,3]}");
        }

        [Test] public void TestIsNonEmpty()
        {
            AssertJsonEqual(IsNonEmpty(Arr(1, 2, 3)),
                "{\"is_nonempty\":[1,2,3]}");
        }

        [Test] public void TestGet()
        {
            AssertJsonEqual(Get(Ref(Collection("thing"), "123456789")),
                "{\"get\":{\"ref\":{\"collection\":\"thing\"},\"id\":\"123456789\"}}");
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
                Paginate(Databases(), after: Ref(Collection("thing"), "123456789")),
                "{\"paginate\":{\"databases\":null},\"after\":{\"ref\":{\"collection\":\"thing\"},\"id\":\"123456789\"}}");

            AssertJsonEqual(
                Paginate(Databases(), before: Ref(Collection("thing"), "123456789")),
                "{\"paginate\":{\"databases\":null},\"before\":{\"ref\":{\"collection\":\"thing\"},\"id\":\"123456789\"}}");

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
            AssertJsonEqual(Exists(Ref(Collection("thing"), "123456789")),
                "{\"exists\":{\"ref\":{\"collection\":\"thing\"},\"id\":\"123456789\"}}");

            AssertJsonEqual(Exists(Ref(Collection("thing"), "123456789"), Time("1970-01-01T00:00:00.123Z")),
                "{\"exists\":{\"ref\":{\"collection\":\"thing\"},\"id\":\"123456789\"},\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}}");
        }

        [Test] public void TestCreate()
        {
            AssertJsonEqual(
                Create(Collection("widgets"), Obj("data", "some-data")),
                "{\"create\":{\"collection\":\"widgets\"},\"params\":{\"object\":{\"data\":\"some-data\"}}}");
        }

        [Test] public void TestUpdate()
        {
            AssertJsonEqual(
                Update(Ref(Collection("widgets"), "123456789"), Obj("name", "things")),
                "{\"update\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"name\":\"things\"}}}");
        }

        [Test] public void TestReplace()
        {
            AssertJsonEqual(
                Replace(Ref(Collection("widgets"), "123456789"), Obj("data", Obj("name", "Computer"))),
                "{\"replace\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestDelete()
        {
            AssertJsonEqual(
                Delete(Ref(Collection("widgets"), "123456789")),
                "{\"delete\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"}}");
        }

        [Test] public void TestInsert()
        {
            AssertJsonEqual(
                Insert(
                    Ref(Collection("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    "create",
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"}," +
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"," +
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");

            AssertJsonEqual(
                Insert(
                    Ref(Collection("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    ActionType.Create,
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},"+
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"},"+
                "\"action\":\"create\","+
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestRemove()
        {
            AssertJsonEqual(
                Remove(
                    Ref(Collection("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    "create"),
                "{\"remove\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},"+
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");

            AssertJsonEqual(
                Remove(
                    Ref(Collection("widgets"), "123456789"),
                    Time("1970-01-01T00:00:00.123Z"),
                    ActionType.Create),
                "{\"remove\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"}," +
                "\"ts\":{\"time\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");
        }

        [Test] public void TestCreateClass()
        {
            AssertJsonEqual(CreateCollection(Obj("name", "class_name")),
                "{\"create_collection\":{\"object\":{\"name\":\"class_name\"}}}");
        }

        [Test] public void TestCreateDatabase()
        {
            AssertJsonEqual(CreateDatabase(Obj("name", "db_name")),
                "{\"create_database\":{\"object\":{\"name\":\"db_name\"}}}");
        }

        [Test] public void TestCreateIndex()
        {
            AssertJsonEqual(
                CreateIndex(Obj("name", "index_name", "source", Collection("class_name"))),
                "{\"create_index\":{\"object\":{\"name\":\"index_name\",\"source\":{\"collection\":\"class_name\"}}}}");
        }

        [Test] public void TestCreateKey()
        {
            AssertJsonEqual(
                CreateKey(Obj("database", Database("db_name"), "role", "client")),
                "{\"create_key\":{\"object\":{\"database\":{\"database\":\"db_name\"},\"role\":\"client\"}}}");
        }

        [Test] public void TestCreateRole()
        {
            AssertJsonEqual(
                CreateRole(Obj(
                    "name", "role_name",
                    "privileges", Arr(Obj(
                        "resource", Databases(),
                        "actions", Obj("read", true)
                    ))
                )),
                "{\"create_role\":{\"object\":{\"name\":\"role_name\",\"privileges\":[{\"object\":{" +
                "\"resource\":{\"databases\":null},\"actions\":{\"object\":{\"read\":true}}}}]}}}");

            AssertJsonEqual(
                CreateRole(Obj(
                    "name", "role_name",
                    "privileges", Obj(
                        "resource", Databases(),
                        "actions", Obj("read", true)
                    )
                )),
                "{\"create_role\":{\"object\":{\"name\":\"role_name\",\"privileges\":{\"object\":{" +
                "\"resource\":{\"databases\":null},\"actions\":{\"object\":{\"read\":true}}}}}}}");
        }

        [Test] public void TestSingleton()
        {
            AssertJsonEqual(
                Singleton(Ref(Collection("widget"), "123")),
                "{\"singleton\":{\"ref\":{\"collection\":\"widget\"},\"id\":\"123\"}}");
        }

        [Test] public void TestEvents()
        {
            AssertJsonEqual(
                Events(Ref(Collection("widget"), "123")),
                "{\"events\":{\"ref\":{\"collection\":\"widget\"},\"id\":\"123\"}}");
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
                Union(Native.DATABASES, Collection("widgets")),
                "{\"union\":[{\"@ref\":{\"id\":\"databases\"}},{\"collection\":\"widgets\"}]}");
        }

        [Test] public void TestIntersection()
        {
            AssertJsonEqual(
                Intersection(Native.DATABASES),
                "{\"intersection\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Intersection(Native.DATABASES, Collection("widgets")),
                "{\"intersection\":[{\"@ref\":{\"id\":\"databases\"}},{\"collection\":\"widgets\"}]}");
        }

        [Test] public void TestDifference()
        {
            AssertJsonEqual(
                Difference(Native.DATABASES),
                "{\"difference\":{\"@ref\":{\"id\":\"databases\"}}}");

            AssertJsonEqual(
                Difference(Native.DATABASES, Collection("widgets")),
                "{\"difference\":[{\"@ref\":{\"id\":\"databases\"}},{\"collection\":\"widgets\"}]}");
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
            AssertJsonEqual(Login(Ref(Collection("widgets"), "123456789"), Obj("password", "P455w0rd")),
                 "{\"login\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},\"params\":{\"object\":{\"password\":\"P455w0rd\"}}}");
        }

        [Test] public void TestLogout()
        {
            AssertJsonEqual(Logout(true), "{\"logout\":true}");
            AssertJsonEqual(Logout(false), "{\"logout\":false}");
        }

        [Test] public void TestIdentify()
        {
            AssertJsonEqual(Identify(Ref(Collection("widgets"), "123456789"), "P455w0rd"),
                "{\"identify\":{\"ref\":{\"collection\":\"widgets\"},\"id\":\"123456789\"},\"password\":\"P455w0rd\"}");
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

        [Test] public void TestNGram()
        {
            AssertJsonEqual(NGram("str"),
                "{\"ngram\":\"str\"}");
            AssertJsonEqual(NGram("str", min: 1),
                "{\"ngram\":\"str\",\"min\":1}");
            AssertJsonEqual(NGram("str", max: 2),
                "{\"ngram\":\"str\",\"max\":2}");
            AssertJsonEqual(NGram("str", min: 1, max: 2),
                "{\"ngram\":\"str\",\"min\":1,\"max\":2}");
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
            AssertJsonEqual(Collection("class_name"), "{\"collection\":\"class_name\"}");
            AssertJsonEqual(Collection("class_name", Database("scope")), "{\"collection\":\"class_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestFunction()
        {
            AssertJsonEqual(Function("function_name"), "{\"function\":\"function_name\"}");
            AssertJsonEqual(Function("function_name", Database("scope")), "{\"function\":\"function_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestRole()
        {
            AssertJsonEqual(Role("role_name"), "{\"role\":\"role_name\"}");
            AssertJsonEqual(Role("role_name", Database("scope")), "{\"role\":\"role_name\",\"scope\":{\"database\":\"scope\"}}");
        }

        [Test] public void TestNativeRefs()
        {
            AssertJsonEqual(Collections(), "{\"collections\":null}");
            AssertJsonEqual(Databases(), "{\"databases\":null}");
            AssertJsonEqual(Indexes(), "{\"indexes\":null}");
            AssertJsonEqual(Functions(), "{\"functions\":null}");
            AssertJsonEqual(Keys(), "{\"keys\":null}");
            AssertJsonEqual(Tokens(), "{\"tokens\":null}");
            AssertJsonEqual(Credentials(), "{\"credentials\":null}");
            AssertJsonEqual(Roles(), "{\"roles\":null}");

            AssertJsonEqual(Collections(Database("scope")), "{\"collections\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Databases(Database("scope")), "{\"databases\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Indexes(Database("scope")), "{\"indexes\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Functions(Database("scope")), "{\"functions\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Keys(Database("scope")), "{\"keys\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Tokens(Database("scope")), "{\"tokens\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Credentials(Database("scope")), "{\"credentials\":{\"database\":\"scope\"}}");
            AssertJsonEqual(Roles(Database("scope")), "{\"roles\":{\"database\":\"scope\"}}");
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

        [Test] public void TestSelectAll()
        {
            AssertJsonEqual(SelectAll("foo", Obj("foo", "bar")),
                "{\"select_all\":\"foo\",\"from\":{\"object\":{\"foo\":\"bar\"}}}");
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

        [Test] public void TestToStringExpr()
        {
            AssertJsonEqual(ToStringExpr(42), "{\"to_string\":42}");
        }

        [Test] public void TestToNumber()
        {
            AssertJsonEqual(ToNumber("42"), "{\"to_number\":\"42\"}");
        }

        [Test] public void TestToTime()
        {
            AssertJsonEqual(ToTime("1970-01-01T00:00:00Z"),
                            "{\"to_time\":\"1970-01-01T00:00:00Z\"}");
        }

        [Test] public void TestToDate()
        {
            AssertJsonEqual(ToDate("1970-01-01"), "{\"to_date\":\"1970-01-01\"}");
        }

        [Test]
        public void TestInstanceRef()
        {
            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    collection: new RefV(id: "child-class", collection: Native.COLLECTIONS)),
                "{\"@ref\":{\"id\":\"123456789\",\"collection\":{\"@ref\":{\"id\":\"child-class\",\"collection\":{\"@ref\":{\"id\":\"collections\"}}}}}}"
            );

            AssertJsonEqual(
                new RefV(
                    id: "123456789",
                    collection: new RefV(
                        id: "child-class",
                        collection: Native.COLLECTIONS,
                        database: new RefV(id: "child-database", collection: Native.DATABASES))),
                "{\"@ref\":{\"id\":\"123456789\",\"collection\":{\"@ref\":{\"id\":\"child-class\",\"collection\":{\"@ref\":{\"id\":\"collections\"}},\"database\":{\"@ref\":{\"id\":\"child-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}}}"
            );
        }

        [Test]
        public void TestClassRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-class", collection: Native.COLLECTIONS),
                "{\"@ref\":{\"id\":\"a-class\",\"collection\":{\"@ref\":{\"id\":\"collections\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-class", collection: Native.COLLECTIONS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-class\",\"collection\":{\"@ref\":{\"id\":\"collections\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestDatabaseRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-database", collection: Native.DATABASES),
                "{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "child-database", collection: Native.DATABASES, database: new RefV(id: "parent-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"child-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}},\"database\":{\"@ref\":{\"id\":\"parent-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestIndexRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-index", collection: Native.INDEXES),
                "{\"@ref\":{\"id\":\"a-index\",\"collection\":{\"@ref\":{\"id\":\"indexes\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-index", collection: Native.INDEXES, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-index\",\"collection\":{\"@ref\":{\"id\":\"indexes\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestKeyRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-key", collection: Native.KEYS),
                "{\"@ref\":{\"id\":\"a-key\",\"collection\":{\"@ref\":{\"id\":\"keys\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-key", collection: Native.KEYS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-key\",\"collection\":{\"@ref\":{\"id\":\"keys\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestFunctionRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-function", collection: Native.FUNCTIONS),
                "{\"@ref\":{\"id\":\"a-function\",\"collection\":{\"@ref\":{\"id\":\"functions\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-function", collection: Native.FUNCTIONS, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-function\",\"collection\":{\"@ref\":{\"id\":\"functions\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
            );
        }

        [Test]
        public void TestRoleRef()
        {
            AssertJsonEqual(
                new RefV(id: "a-role", collection: Native.ROLES),
                "{\"@ref\":{\"id\":\"a-role\",\"collection\":{\"@ref\":{\"id\":\"roles\"}}}}"
            );

            AssertJsonEqual(
                new RefV(id: "a-role", collection: Native.ROLES, database: new RefV(id: "a-database", collection: Native.DATABASES)),
                "{\"@ref\":{\"id\":\"a-role\",\"collection\":{\"@ref\":{\"id\":\"roles\"}},\"database\":{\"@ref\":{\"id\":\"a-database\",\"collection\":{\"@ref\":{\"id\":\"databases\"}}}}}}"
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

        [Test]
        public void TestHandleNulls()
        {
            AssertJsonEqual(Year(null), "{\"year\":null}");
            AssertJsonEqual(Obj("key", null), "{\"object\":{\"key\":null}}");
            AssertJsonEqual(Arr("str", null, 10), "[\"str\",null,10]");
            AssertJsonEqual(Let("key", null).In(Var("key")), "{\"let\":[{\"key\":null}],\"in\":{\"var\":\"key\"}}");
            AssertJsonEqual(Add(new Expr[] { null }), "{\"add\":null}");
            AssertJsonEqual(Add(new Expr[] { null, null }), "{\"add\":[null,null]}");
            AssertJsonEqual(Add(null), "{\"add\":null}");
            AssertJsonEqual(Add(null, null), "{\"add\":[null,null]}");
        }

        [Test]
        public void TestMergeFunction()
        {
            AssertJsonEqual(
                Merge(Obj("x",10), Obj("y", 20)),
                "{\"merge\":{\"object\":{\"x\":10}},\"with\":{\"object\":{\"y\":20}}}"
            );

            AssertJsonEqual(
                Merge(Obj("x", 10), Obj("y", 20), Lambda(x => x)),
                "{\"merge\":{\"object\":{\"x\":10}},\"with\":{\"object\":{\"y\":20}},\"lambda\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}}}"
            );
        }

        [Test]
        public void TestFormatFunction()
        {
            AssertJsonEqual(
                Format("%f %d", 3.14, 10),
                "{\"format\":\"%f %d\",\"values\":[3.14,10]}"
            );
        }
        
        [Test]
        public void TestRangeFunction()
        {
            AssertJsonEqual(
                Range(Match(Index("some_index")), 1, 10),
                "{\"range\":{\"match\":{\"index\":\"some_index\"}},\"from\":1,\"to\":10}");
        }

        [Test]
        public void TestMathFunctions()
        {
            AssertJsonEqual(Abs(-100), "{\"abs\":-100}");
            AssertJsonEqual(Abs(-100L), "{\"abs\":-100}");
            AssertJsonEqual(Abs(-100.0), "{\"abs\":-100.0}");

            AssertJsonEqual(Acos(0), "{\"acos\":0}");
            AssertJsonEqual(Acos(0.0), "{\"acos\":0.0}");

            AssertJsonEqual(Asin(0), "{\"asin\":0}");
            AssertJsonEqual(Asin(0.0), "{\"asin\":0.0}");

            AssertJsonEqual(Atan(0), "{\"atan\":0}");

            AssertJsonEqual(BitAnd(7, 3), "{\"bitand\":[7,3]}");

            AssertJsonEqual(BitNot(-1), "{\"bitnot\":-1}");

            AssertJsonEqual(BitOr(7, 3), "{\"bitor\":[7,3]}");

            AssertJsonEqual(BitXor(7, 3), "{\"bitxor\":[7,3]}");

            AssertJsonEqual(Ceil(123.456), "{\"ceil\":123.456}");

            AssertJsonEqual(Cos(1), "{\"cos\":1}");

            AssertJsonEqual(Cosh(1), "{\"cosh\":1}");

            AssertJsonEqual(Degrees(1), "{\"degrees\":1}");

            AssertJsonEqual(Exp(1), "{\"exp\":1}");

            AssertJsonEqual(Floor(1), "{\"floor\":1}");

            AssertJsonEqual(Hypot(3, 4), "{\"hypot\":3,\"b\":4}");

            AssertJsonEqual(Ln(1), "{\"ln\":1}");

            AssertJsonEqual(Log(1), "{\"log\":1}");

            AssertJsonEqual(Max(100, 10), "{\"max\":[100,10]}");
            AssertJsonEqual(Max(Arr(100, 10)), "{\"max\":[100,10]}");

            AssertJsonEqual(Min(100, 10), "{\"min\":[100,10]}");
            AssertJsonEqual(Min(Arr(100, 10)), "{\"min\":[100,10]}");

            AssertJsonEqual(Multiply(100, 10), "{\"multiply\":[100,10]}");
            AssertJsonEqual(Multiply(Arr(100, 10)), "{\"multiply\":[100,10]}");

            AssertJsonEqual(Pow(4), "{\"pow\":4}");
            AssertJsonEqual(Pow(8, 3), "{\"pow\":8,\"exp\":3}");

            AssertJsonEqual(Radians(1), "{\"radians\":1}");

            AssertJsonEqual(Round(123.456), "{\"round\":123.456}");
            AssertJsonEqual(Round(555.666, 2), "{\"round\":555.666,\"precision\":2}");

            AssertJsonEqual(Sign(1), "{\"sign\":1}");

            AssertJsonEqual(Sin(1), "{\"sin\":1}");

            AssertJsonEqual(Sinh(1), "{\"sinh\":1}");

            AssertJsonEqual(Sqrt(1), "{\"sqrt\":1}");

            AssertJsonEqual(Subtract(100, 10), "{\"subtract\":[100,10]}");
            AssertJsonEqual(Subtract(Arr(100, 10)), "{\"subtract\":[100,10]}");

            AssertJsonEqual(Tan(1), "{\"tan\":1}");

            AssertJsonEqual(Tanh(1), "{\"tanh\":1}");

            AssertJsonEqual(Trunc(1), "{\"trunc\":1}");
            AssertJsonEqual(Trunc(123.456, 2), "{\"trunc\":123.456,\"precision\":2}");
        }

        [Test]
        public void TestStringFunctions()
        {
            AssertJsonEqual(FindStr("ABCDEF", "ABC"), "{\"findstr\":\"ABCDEF\",\"find\":\"ABC\"}");
            AssertJsonEqual(FindStr("ABCDEF", "ABC", 1), "{\"findstr\":\"ABCDEF\",\"find\":\"ABC\",\"start\":1}");

            AssertJsonEqual(FindStrRegex("ABCDEF", "BCD"), "{\"findstrregex\":\"ABCDEF\",\"pattern\":\"BCD\"}");
            AssertJsonEqual(FindStrRegex("ABCDEF", "BCD", 1), "{\"findstrregex\":\"ABCDEF\",\"pattern\":\"BCD\",\"start\":1}");
            AssertJsonEqual(FindStrRegex("ABCDEF", "BCD", 1, 3), "{\"findstrregex\":\"ABCDEF\",\"pattern\":\"BCD\",\"start\":1,\"num_results\":3}");

            AssertJsonEqual(Length("ABC"), "{\"length\":\"ABC\"}");

            AssertJsonEqual(LowerCase("ABC"), "{\"lowercase\":\"ABC\"}");

            AssertJsonEqual(LTrim("ABC"), "{\"ltrim\":\"ABC\"}");

            AssertJsonEqual(Repeat("ABC"), "{\"repeat\":\"ABC\"}");
            AssertJsonEqual(Repeat("ABC", 2), "{\"repeat\":\"ABC\",\"number\":2}");

            AssertJsonEqual(ReplaceStr("ABCDEF", "BCD", "CAR"), "{\"replacestr\":\"ABCDEF\",\"find\":\"BCD\",\"replace\":\"CAR\"}");

            AssertJsonEqual(ReplaceStrRegex("ABCDEF", "BCD", "CAR"), "{\"replacestrregex\":\"ABCDEF\",\"pattern\":\"BCD\",\"replace\":\"CAR\"}");
            AssertJsonEqual(ReplaceStrRegex("abcdef", "bcd", "car", true), "{\"replacestrregex\":\"abcdef\",\"pattern\":\"bcd\",\"replace\":\"car\",\"first\":true}");

            AssertJsonEqual(RTrim("ABC"), "{\"rtrim\":\"ABC\"}");

            AssertJsonEqual(Space(2), "{\"space\":2}");

            AssertJsonEqual(SubString("ABC"), "{\"substring\":\"ABC\"}");
            AssertJsonEqual(SubString("ABC", 2), "{\"substring\":\"ABC\",\"start\":2}");
            AssertJsonEqual(SubString("ABC", 2, 3), "{\"substring\":\"ABC\",\"start\":2,\"length\":3}");

            AssertJsonEqual(Trim("ABC"), "{\"trim\":\"ABC\"}");
            AssertJsonEqual(UpperCase("ABC"), "{\"uppercase\":\"ABC\"}");
            AssertJsonEqual(TitleCase("ABC"), "{\"titlecase\":\"ABC\"}");
        }

        [Test]
        public void TestDocuments()
        {
            AssertJsonEqual(Documents(Collection("foo")), "{\"documents\":{\"collection\":\"foo\"}}");
        }

        [Test]
        public void TestEncodeRawExpressions()
        {
            var indexCfg = new Dictionary<string, object>()
            {
                { "name", "index_name" },
                { "source", Collection("class_name") }
            };

            AssertJsonEqual(
                Encode(indexCfg),
                "{\"object\":{\"name\":\"index_name\",\"source\":{\"collection\":\"class_name\"}}}"
           );

            AssertJsonEqual(
                CreateIndex(Encode(indexCfg)),
                 "{\"create_index\":{\"object\":{\"name\":\"index_name\",\"source\":{\"collection\":\"class_name\"}}}}"
            );
        }

    }
}
