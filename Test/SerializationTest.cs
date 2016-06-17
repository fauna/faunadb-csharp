using FaunaDB.Query;
using FaunaDB.Types;
using FaunaDB.Utils;
using NUnit.Framework;
using System;
using System.Collections.Specialized;
using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class SerializationTest
    {
        private static void AssertJsonEqual(Expr expr, string value)
        {
            Assert.AreEqual(expr.ToJson(), value);
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

        [Test]
        public void TestObjectAndArrays()
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

        [Test]
        public void TestAnonymousObj()
        {
            Expr obj = Obj(new
            {
                foo = 10,
                bar = "bar",
                array = new int[] { 1, 2, 3 },
                obj = new
                {
                    a = 3.14,
                    b = true
                }
            });

            Assert.AreEqual(obj, Obj(
                "foo", 10,
                "bar", "bar",
                "array", new ArrayV(1, 2, 3),
                "obj", Obj(
                    "a", 3.14,
                    "b", true)
                ));

            AssertJsonEqual(obj, "{\"object\":{\"foo\":10,\"bar\":\"bar\",\"array\":[1,2,3],\"obj\":{\"object\":{\"a\":3.14,\"b\":true}}}}");
        }

        [Test] public void TestRef()
        {
            AssertJsonEqual(Ref("classes"), "{\"@ref\":\"classes\"}");

            AssertJsonEqual(Ref(Ref("classes/people"), "id1"), "{\"ref\":{\"@ref\":\"classes/people\"},\"id\":\"id1\"}");
        }

        [Test] public void TestTimestamp()
        {
            AssertJsonEqual(Ts("1970-01-01T00:00:00Z"), "{\"@ts\":\"1970-01-01T00:00:00Z\"}");

            AssertJsonEqual(Ts(new DateTime(1970, 1, 1, 0, 0, 0, 0)), "{\"@ts\":\"1970-01-01T00:00:00Z\"}");
        }

        [Test] public void TetDate()
        {
            AssertJsonEqual(Dt("2000-01-01"), "{\"@date\":\"2000-01-01\"}");

            AssertJsonEqual(Dt(new DateTime(2000, 1, 1)), "{\"@date\":\"2000-01-01\"}");
        }

        [Test] public void TestLet()
        {
            AssertJsonEqual(Let("x", 10).In(Var("x")),
                "{\"let\":{\"x\":10},\"in\":{\"var\":\"x\"}}");

            AssertJsonEqual(Let("x", 10).In(x => x),
                "{\"let\":{\"x\":10},\"in\":{\"var\":\"x\"}}");

            AssertJsonEqual(Let(10, In: x => x),
                "{\"let\":{\"x\":10},\"in\":{\"var\":\"x\"}}");

            ////

            AssertJsonEqual(Let("x", 10, "y", 20).In(Add(Var("x"), Var("y"))),
                "{\"let\":{\"x\":10,\"y\":20},\"in\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");

            AssertJsonEqual(Let("x", 10, "y", 20).In((x, y) => Add(x, y)),
                "{\"let\":{\"x\":10,\"y\":20},\"in\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}}");

            AssertJsonEqual(Let(10, 20, In: (x, y) => Add(x, y)),
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

            AssertJsonEqual(Map(Arr(1, 2, 3), x => x),
                "{\"map\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Map(Arr(Arr(1, 2), Arr(3, 4)), (x, y) => Add(x, y)),
                "{\"map\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");
        }

        [Test] public void TestForeach()
        {
            AssertJsonEqual(Foreach(Arr(1, 2, 3), Lambda("x", Var("x"))),
                "{\"foreach\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Foreach(Arr(1, 2, 3), x => x),
                "{\"foreach\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Foreach(Arr(Arr(1, 2), Arr(3, 4)), (x, y) => Add(x, y)),
                "{\"foreach\":{\"lambda\":[\"x\",\"y\"],\"expr\":{\"add\":[{\"var\":\"x\"},{\"var\":\"y\"}]}},\"collection\":[[1,2],[3,4]]}");
        }

        [Test] public void TestFilter()
        {
            AssertJsonEqual(Filter(Arr(1, 2, 3), Lambda("x", Var("x"))),
                "{\"filter\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

            AssertJsonEqual(Filter(Arr(1, 2, 3), x => x),
                "{\"filter\":{\"lambda\":\"x\",\"expr\":{\"var\":\"x\"}},\"collection\":[1,2,3]}");

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
            AssertJsonEqual(Get(Ref("classes/thing/123456789")),
                "{\"get\":{\"@ref\":\"classes/thing/123456789\"}}");
        }

        [Test] public void TestPaginate()
        {
            AssertJsonEqual(Paginate(Ref("databases")),
                "{\"paginate\":{\"@ref\":\"databases\"}}");

            AssertJsonEqual(Paginate(Ref("databases"), after: Ref("databases/thing/123456789")),
                "{\"paginate\":{\"@ref\":\"databases\"},\"after\":{\"@ref\":\"databases/thing/123456789\"}}");

            AssertJsonEqual(Paginate(Ref("databases"), before: Ref("databases/thing/123456789")),
                "{\"paginate\":{\"@ref\":\"databases\"},\"before\":{\"@ref\":\"databases/thing/123456789\"}}");

            AssertJsonEqual(Paginate(Ref("databases"), ts: Ts("1970-01-01T00:00:00Z")),
                "{\"paginate\":{\"@ref\":\"databases\"},\"ts\":{\"@ts\":\"1970-01-01T00:00:00Z\"}}");

            AssertJsonEqual(Paginate(Ref("databases"), size: 10),
                "{\"paginate\":{\"@ref\":\"databases\"},\"size\":10}");

            AssertJsonEqual(Paginate(Ref("databases"), events: true),
                "{\"paginate\":{\"@ref\":\"databases\"},\"events\":true}");

            AssertJsonEqual(Paginate(Ref("databases"), sources: true),
                "{\"paginate\":{\"@ref\":\"databases\"},\"sources\":true}");
        }

        [Test] public void TestExists()
        {
            AssertJsonEqual(Exists(Ref("classes/thing/123456789")),
                "{\"exists\":{\"@ref\":\"classes/thing/123456789\"}}");

            AssertJsonEqual(Exists(Ref("classes/thing/123456789"), Ts("1970-01-01T00:00:00.123Z")),
                "{\"exists\":{\"@ref\":\"classes/thing/123456789\"},\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}}");
        }

        [Test] public void TestCount()
        {
            AssertJsonEqual(Count(Ref("databases")),
                "{\"count\":{\"@ref\":\"databases\"}}");

            AssertJsonEqual(Count(Ref("databases"), true),
                "{\"count\":{\"@ref\":\"databases\"},\"events\":true}");
        }

        [Test] public void TestCreate()
        {
            AssertJsonEqual(Create(Ref("database"), Obj("name", "widgets")),
                "{\"create\":{\"@ref\":\"database\"},\"params\":{\"object\":{\"name\":\"widgets\"}}}");
        }

        [Test] public void TestUpdate()
        {
            AssertJsonEqual(Update(Ref("database/widgets"), Obj("name", "things")),
                "{\"update\":{\"@ref\":\"database/widgets\"},\"params\":{\"object\":{\"name\":\"things\"}}}");
        }

        [Test] public void TestReplace()
        {
            AssertJsonEqual(Replace(Ref("classes/widgets/123456789"), Obj("data", Obj("name", "Computer"))),
                "{\"replace\":{\"@ref\":\"classes/widgets/123456789\"},\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestDelete()
        {
            AssertJsonEqual(Delete(Ref("classes/widgets/123456789")),
                "{\"delete\":{\"@ref\":\"classes/widgets/123456789\"}}");
        }

        [Test] public void TestInsert()
        {
            AssertJsonEqual(Insert(
                    Ref("classes/widgets/123456789"),
                    Ts("1970-01-01T00:00:00.123Z"),
                    "create",
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"@ref\":\"classes/widgets/123456789\"}," +
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"," +
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");

            AssertJsonEqual(Insert(
                    Ref("classes/widgets/123456789"),
                    Ts("1970-01-01T00:00:00.123Z"),
                    Language.Action.CREATE,
                    Obj("data", Obj("name", "Computer"))),
                "{\"insert\":{\"@ref\":\"classes/widgets/123456789\"},"+
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"},"+
                "\"action\":\"create\","+
                "\"params\":{\"object\":{\"data\":{\"object\":{\"name\":\"Computer\"}}}}}");
        }

        [Test] public void TestRemove()
        {
            AssertJsonEqual(Remove(
                    Ref("classes/widgets/123456789"),
                    Ts("1970-01-01T00:00:00.123Z"),
                    "create"),
                "{\"remove\":{\"@ref\":\"classes/widgets/123456789\"},"+
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");

            AssertJsonEqual(Remove(
                    Ref("classes/widgets/123456789"),
                    Ts("1970-01-01T00:00:00.123Z"),
                    Language.Action.CREATE),
                "{\"remove\":{\"@ref\":\"classes/widgets/123456789\"}," +
                "\"ts\":{\"@ts\":\"1970-01-01T00:00:00.123Z\"}," +
                "\"action\":\"create\"}");
        }

        [Test] public void TestMatch()
        {
            AssertJsonEqual(Match(Ref("indexes/all_the_things")),
                "{\"match\":{\"@ref\":\"indexes/all_the_things\"}}");

            AssertJsonEqual(Match(Ref("indexes/widgets_by_name"), "Computer"),
                "{\"match\":{\"@ref\":\"indexes/widgets_by_name\"},\"terms\":\"Computer\"}");

            AssertJsonEqual(Match(Ref("indexes/widgets_by_name"), "Computer", "Monitor"),
                "{\"match\":{\"@ref\":\"indexes/widgets_by_name\"},\"terms\":[\"Computer\",\"Monitor\"]}");
        }

        [Test] public void TestUnion()
        {
            AssertJsonEqual(Union(Ref("databases")),
                "{\"union\":{\"@ref\":\"databases\"}}");

            AssertJsonEqual(Union(Ref("databases"), Ref("classes/widgets")),
                "{\"union\":[{\"@ref\":\"databases\"},{\"@ref\":\"classes/widgets\"}]}");
        }

        [Test] public void TestIntersection()
        {
            AssertJsonEqual(Intersection(Ref("databases")),
                "{\"intersection\":{\"@ref\":\"databases\"}}");

            AssertJsonEqual(Intersection(Ref("databases"), Ref("classes/widgets")),
                "{\"intersection\":[{\"@ref\":\"databases\"},{\"@ref\":\"classes/widgets\"}]}");
        }

        [Test] public void TestDifference()
        {
            AssertJsonEqual(Difference(Ref("databases")),
                "{\"difference\":{\"@ref\":\"databases\"}}");

            AssertJsonEqual(Difference(Ref("databases"), Ref("classes/widgets")),
                "{\"difference\":[{\"@ref\":\"databases\"},{\"@ref\":\"classes/widgets\"}]}");
        }

        [Test] public void TestDistinct()
        {
            AssertJsonEqual(Distinct(Match(Ref("indexes/widgets"))),
                "{\"distinct\":{\"match\":{\"@ref\":\"indexes/widgets\"}}}");
        }

        [Test] public void TestJoin()
        {
            AssertJsonEqual(Join(Match(Ref("indexes/widgets")), Ref("indexes/other_widgets")),
                "{\"join\":{\"match\":{\"@ref\":\"indexes/widgets\"}}," +
                "\"with\":{\"@ref\":\"indexes/other_widgets\"}}");

            AssertJsonEqual(Join(Match(Ref("indexes/widgets")), widget => Match(Ref("indexes/widgets"), widget)),
                "{\"join\":{\"match\":{\"@ref\":\"indexes/widgets\"}}," +
                "\"with\":{\"lambda\":\"widget\",\"expr\":{\"match\":{\"@ref\":\"indexes/widgets\"},\"terms\":{\"var\":\"widget\"}}}}");
        }

        [Test] public void TestLogin() { }

        [Test] public void TestLogout() { }

        [Test] public void TestIdentity() { }

        [Test] public void TestConcat() { }

        [Test] public void TestCasefold() { }

        [Test] public void TestTime() { }

        [Test] public void TestEpoch() { }

        [Test] public void TestDate() { }

        [Test] public void TestNextId() { }

        [Test] public void TestEquals() { }

        [Test] public void TestContains() { }

        [Test] public void TestSelect() { }

        [Test] public void TestAdd() { }

        [Test] public void TestMultiply() { }

        [Test] public void TestSubtract() { }

        [Test] public void TestDivide() { }

        [Test] public void TestModulo() { }

        [Test] public void TestLT() { }

        [Test] public void TestLTE() { }

        [Test] public void TestGT() { }

        [Test] public void TestGTE() { }

        [Test] public void TestAnd() { }

        [Test] public void TestOr() { }

        [Test] public void TestNot() { }

    }
}
