using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class QueryTest : TestCase
    {
        Ref classRef;
        Ref nIndexRef;
        Ref mIndexRef;
        Ref refN1;
        Ref refM1;
        Ref refN1M1;
        Ref thimbleClassRef;

        #region Setup
        [OneTimeSetUp]
        new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            classRef = GetRef(await client.Query(Create(Ref("classes"), Obj("name", "widgets"))));
            nIndexRef = GetRef(await client.Query(Create(Ref("indexes"), Obj(
                "name", "widgets_by_n",
                "source", classRef,
                "path", "data.n",
                "active", true))));
            mIndexRef = GetRef(await client.Query(Create(Ref("indexes"), Obj(
                "name", "widgets_by_m",
                "source", classRef,
                "path", "data.m",
                "active", true))));

            refN1 = await CreateRef(n: 1);
            refM1 = await CreateRef(m: 1);
            refN1M1 = await CreateRef(n: 1, m: 1);

            thimbleClassRef = GetRef(await client.Query(Create(Ref("classes"), Obj("name", "thimbles"))));
        }
        #endregion

        #region Helpers
        Expr NSet(int n) =>
            Match(nIndexRef, n);

        Expr MSet(int m) =>
            Match(mIndexRef, m);

        async Task<ObjectV> CreateInstance(int n = 0, Expr m = null)
        {
            var data = m == null ? Obj("n", n) : Obj("n", n, "m", m);
            return (ObjectV) await Q(Create(classRef, Obj("data", data)));
        }

        async Task<Ref> CreateRef(int n = 0, Expr m = null) =>
            GetRef(await CreateInstance(n, m));

        async Task<ObjectV> CreateThimble(Expr data) =>
            (ObjectV) await Q(Create(thimbleClassRef, Obj("data", data)));

        async Task<ArrayV> SetToArray(Expr set)
        {
            var res = await Q(Paginate(set, size: 1000));
            return (ArrayV) ((ObjectV) res)["data"];
        }

        async Task AssertSet(Expr set, params Value[] expectedSetValues)
        {
            Assert.AreEqual(ArrayV.FromEnumerable(expectedSetValues), await SetToArray(set));
        }

        async Task AssertBadQuery(Expr expression)
        {
            await AssertU.Throws<BadRequest>(() => Q(expression));
        }

        async Task AssertQuery(Expr expected, Expr expression)
        {
            Assert.AreEqual(expected, await Q(expression));
        }
        #endregion

        #region Basic forms

        [Test] public async Task TestLetVar()
        {
            await AssertQuery(2, Let(UnescapedObject.With("a", 1), Add(Var("a"), Var("a"))));

            await AssertQuery(2, Let("a", 1).In(Add(Var("a"), Var("a"))));
            await AssertQuery(2, Let("a", 1).In(a => Add(a, a)));

            await AssertQuery(2, Let("a", 1, "b", 1).In(Add(Var("a"), Var("b"))));
            await AssertQuery(2, Let("a", 1, "b", 1).In((a, b) => Add(a, b)));

            await AssertQuery(2, Let(1, In: a => Add(a, a)));
        }

        [Test] public async Task TestIf()
        {
            await AssertQuery("t", If(true, "t", "f"));
            await AssertQuery("f", If(false, "t", "f"));
        }

        [Test] public async Task TestDo()
        {
            var rf = await CreateRef();
            await AssertQuery(1, Do(Delete(rf), 1));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async Task TestObject()
        {
            // Unlike Quote, contents are evaluated.
            await AssertQuery(ObjectV.With("x", 1), Obj("x", Let("x", 1).In(x => x)));
        }

        [Test] [Ignore("improve assertion")] public void TestLambda()
        {
            Expr lambdaId = ObjectV.With("lambda", "a", "expr", ObjectV.With("var", "a"));
            Assert.AreEqual(lambdaId, Lambda(a => a));

            Assert.AreEqual(Lambda("a", Add(Var("a"), Var("a"))), Lambda(a => Add(a, a)));

            var expected = Lambda("a", Lambda("b", Lambda("c", Arr(Var("a"), Var("b"), Var("c")))));
            Assert.AreEqual(expected, Lambda(a => Lambda(b => Lambda(c => Arr(a, b, c)))));

            // Error in lambda should not affect future queries.
            AssertU.Throws<Exception>(() => {
                Lambda(a => {
                    throw new Exception();
                });
            });
        }

        [Test] public void TestLambdaMultiVar()
        {
            var expected = Lambda(Arr("a", "b"), Arr(Var("a"), Var("b")));
            Assert.AreEqual(expected, Lambda((a, b) => Arr(a, b)));
        }
        #endregion

        #region Collection functions

        [Test] public async Task TestMap()
        {
            await AssertQuery(new ArrayV(2, 4, 6), Map(new ArrayV(1, 2, 3), a => Multiply(2, a)));

            var page = Paginate(NSet(1));
            var ns = Map(page, a => Select(new ArrayV("data", "n"), Get(a)));
            await AssertQuery(ObjectV.With("data", new ArrayV(1, 1)), ns);
        }

        [Test] public async Task TestForeach()
        {
            var refs = new ArrayV(await CreateRef(), await CreateRef());
            await Q(Foreach(refs, Delete));
            foreach (var @ref in refs)
                await AssertQuery(false, Exists(@ref));
        }

        [Test] public async Task TestFilter()
        {
            var evens = Filter(new ArrayV(1, 2, 3, 4), a => Language.EqualsFn(Modulo(a, 2), 0));
            await AssertQuery(new ArrayV(2, 4), evens);

            // Works on page too
            var page = Paginate(NSet(1));
            var refsWithM = Filter(page, a => Contains(new ArrayV("data", "m"), Get(a)));
            await AssertQuery(ObjectV.With("data", new ArrayV(refN1M1)), refsWithM);
        }

        [Test] public async Task TestTake()
        {
            await AssertQuery(new ArrayV(1), Take(1, new ArrayV(1, 2)));
            await AssertQuery(new ArrayV(1, 2), Take(3, new ArrayV(1, 2)));
            await AssertQuery(ArrayV.Empty, Take(-1, new ArrayV(1, 2)));
        }

        [Test] public async Task TestDrop()
        {
            await AssertQuery(new ArrayV(2), Drop(1, new ArrayV(1, 2)));
            await AssertQuery(ArrayV.Empty, Drop(3, new ArrayV(1, 2)));
            await AssertQuery(new ArrayV(1, 2), Drop(-1, new ArrayV(1, 2)));
        }

        [Test] public async Task TestPrepend()
        {
            await AssertQuery(new ArrayV(1, 2, 3, 4, 5, 6), Prepend(new ArrayV(1, 2, 3), new ArrayV(4, 5, 6)));
        }

        [Test] public async Task TestAppend() {
            await AssertQuery(new ArrayV(1, 2, 3, 4, 5, 6), Append(new ArrayV(4, 5, 6), new ArrayV(1, 2, 3)));
        }
        #endregion

        #region Read functions

        [Test] public async Task TestGet()
        {
            var instance = await CreateInstance();
            await AssertQuery(instance, Get(GetRef(instance)));
        }

        [Test] [Ignore("improve assertion")] public async Task TestPaginate()
        {
            var testSet = NSet(1);
            await AssertQuery(UnescapedObject.With("data", Arr(refN1, refN1M1)), Paginate(testSet));
            await AssertQuery(UnescapedObject.With("data", Arr(refN1), "after", Arr(refN1M1)), Paginate(testSet, size: 1));

            var sources = Arr(SetRef(testSet));
            var page = UnescapedObject.With("data", Arr(
                    UnescapedObject.With("sources", sources, "value", refN1),
                    UnescapedObject.With("sources", sources, "value", refN1M1)));
            await AssertQuery(page, Paginate(testSet, sources: true));
        }

        [Test] public async Task TestExists()
        {
            var rf = await CreateRef();
            await AssertQuery(true, Exists(rf));
            await Q(Delete(rf));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async Task TestCount()
        {
            await CreateInstance(123);
            await CreateInstance(123);
            var instances = NSet(123);
            // `Count` is currently only approximate. Should be 2.
            var n = await Q(Count(instances));
            Assert.IsInstanceOf<LongV>(n);
        }
        #endregion

        #region Write functions

        [Test] public async Task TestCreate()
        {
            var instance = await CreateInstance();
            Assert.That(instance.Value.ContainsKey("ref"));
            Assert.That(instance.Value.ContainsKey("ts")); //ts
            Assert.AreEqual(classRef, instance["class"]);
        }

        [Test] public async Task TestUpdate()
        {
            var rf = await CreateRef();
            var got = (ObjectV) await Q(Replace(rf, Obj("data", Obj("m", 123))));
            Assert.AreEqual(ObjectV.With("m", 123), got["data"]);
        }

        [Test] public async Task TestReplace()
        {
            var rf = await CreateRef();
            var got = (ObjectV) await Q(Replace(rf, Obj("data", Obj("m", 123))));
            Assert.AreEqual(ObjectV.With("m", 123), got["data"]);
        }

        [Test] public async Task TestDelete()
        {
            var rf = await CreateRef();
            await Q(Delete(rf));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async Task TestRemove()
        {
            var instance = await CreateThimble(Obj("weight", 0));
            var @ref = GetRef(instance);

            // Change it
            var newInstance = (ObjectV) await Q(Replace(@ref, Obj("data", Obj("weight", 1))));
            Assert.AreEqual(newInstance, await Q(Get(@ref)));

            // Delete that event
            await Q(Remove(@ref, newInstance["ts"], "create"));
            // Assert that it was undone
            Assert.AreEqual(instance, await Q(Get(@ref)));
        }
        #endregion

        #region Sets
        [Test] public async Task TestMatch()
        {
            await AssertSet(NSet(1), refN1, refN1M1);
        }

        [Test] public async Task TestUnion()
        {
            await AssertSet(Union(NSet(1), MSet(1)), refN1, refM1, refN1M1);
        }

        [Test] public async Task TestIntersection()
        {
            await AssertSet(Intersection(NSet(1), MSet(1)), refN1M1);
        }

        [Test] public async Task TestDifference()
        {
            await AssertSet(Difference(NSet(1), MSet(1)), refN1); // but not refN1M1
        }

        [Test] public async Task TestJoin()
        {
            var referenced = new ArrayV(await CreateRef(n: 12), await CreateRef(n: 12));
            var referencers = new ArrayV(await CreateRef(m: referenced[0]), await CreateRef(m: referenced[1]));

            var source = NSet(12);
            Assert.AreEqual(referenced, await SetToArray(source));

            // For each obj with n=12, get the set of elements whose data.m refers to it.
            var joined = Join(source, a => Match(mIndexRef, a));
            await AssertSet(joined, referencers.ToArray());
        }
        #endregion

        #region Authentication

        [Test] public async Task TestLoginLogout()
        {
            var instanceRef = GetRef(await client.Query(
                Create(classRef, Obj("credentials", Obj("password", "sekrit")))));
            var secret = (string) ((ObjectV) await client.Query(
                Login(instanceRef, Obj("password", "sekrit"))))["secret"];
            var instanceClient = GetClient(password: secret);
            Assert.AreEqual(instanceRef, await instanceClient.Query(Select("ref", Get(new Ref("classes/widgets/self")))));
            Assert.AreEqual(BooleanV.True, await instanceClient.Query(Language.Logout(true)));
        }

        [Test] public async Task TestIdentify()
        {
            var instanceRef = GetRef(await client.Query(
                Create(classRef, Obj("credentials", Obj("password", "sekrit")))));
            await AssertQuery(true, Identify(instanceRef, "sekrit"));
        }

        #endregion

        #region String functions

        [Test] public async Task TestConcat()
        {
            await AssertQuery("abc", Concat(new ArrayV("a", "b", "c")));
            await AssertQuery("", Concat(ArrayV.Empty));
            await AssertQuery("a.b.c", Concat(new ArrayV("a", "b", "c"), "."));
        }

        [Test] public async Task TestCaseFold()
        {
            await AssertQuery("hen wen", CaseFold("Hen Wen"));
        }
        #endregion

        #region Time and date functions

        [Test] public async Task TestTime()
        {
            var time = "1970-01-01T00:00:00.123456789Z";
            await AssertQuery(new TsV(time), Time(time));

            // "now" refers to the current time.
            Assert.IsInstanceOf<TsV>(await Q(Time("now")));
        }

        [Test] public async Task TestEpoch()
        {
            await AssertQuery(new TsV("1970-01-01T00:00:12.000Z"), Epoch(12, "second"));
            //var nanoTime = new FaunaTime("1970-01-01T00:00:00.0012345Z");
            //await AssertQuery(nanoTime, Epoch(1234567, "nanosecond"));
        }

        [Test] public async Task TestDate()
        {
            await AssertQuery(new DateV("1970-01-01"), Date("1970-01-01"));
        }
        #endregion

        #region Miscellaneous functions

        [Test] public async Task TestEquals()
        {
            await AssertQuery(true, EqualsFn(1, 1, 1));
            await AssertQuery(false, EqualsFn(1, 1, 2));
            await AssertQuery(true, EqualsFn(1));
        }

        [Test] public async Task TestContains()
        {
            var obj = Obj("a", Obj("b", 1));
            await AssertQuery(true, Contains(new ArrayV("a", "b"), obj));
            await AssertQuery(true, Contains("a", obj));
            await AssertQuery(false, Contains(new ArrayV("a", "c"), obj));
        }

        [Test] public async Task TestSelect()
        {
            var obj = Obj("a", Obj("b", 1));
            await AssertQuery(ObjectV.With("b", 1), Select("a", obj));
            await AssertQuery(1, Select(new ArrayV("a", "b"), obj));
            await AssertQuery(NullV.Instance, Select("c", obj, NullV.Instance));
            await AssertU.Throws<NotFound>(() => Q(Select("c", obj)));
        }

        [Test] public async Task TestSelectArray()
        {
            var arr = new ArrayV(1, 2, 3);
            await AssertQuery(3, Select(2, arr));
            await AssertU.Throws<NotFound>(() => Q(Select(3, arr)));
        }

        [Test] public async Task TestAdd()
        {
            await AssertQuery(10, Add(2, 3, 5));
        }

        [Test] public async Task TestMultiply()
        {
            await AssertQuery(30, Multiply(2, 3, 5));
        }

        [Test] public async Task TestSubtract()
        {
            await AssertQuery(-6, Subtract(2, 3, 5));
            await AssertQuery(2, Subtract(2));
        }

        [Test] public async Task TestDivide()
        {
            await AssertQuery(2.0 / 15, Divide(2.0, 3, 5));
            await AssertQuery(2, Divide(2));
            await AssertBadQuery(Divide(1, 0));
        }

        [Test] public async Task TestModulo()
        {
            await AssertQuery(1, Modulo(5, 2));
            // This is (15 % 10) % 2
            await AssertQuery(1, Modulo(15, 10, 2));
            await AssertQuery(2, Modulo(2));
            await AssertBadQuery(Modulo(1, 0));
        }

        [Test] public async Task TestLess()
        {
            await AssertQuery(true, LT(1, 2));
        }

        [Test] public async Task TestLessOrEqual()
        {
            await AssertQuery(true, LTE(1, 1));
        }

        [Test] public async Task TestGreater()
        {
            await AssertQuery(true, GT(2, 1));
        }

        [Test] public async Task TestGreaterOrEqual()
        {
            await AssertQuery(true, GTE(1, 1));
        }

        [Test] public async Task TestAnd()
        {
            await AssertQuery(false, And(true, true, false));
            await AssertQuery(true, And(true, true, true));
            await AssertQuery(true, And(true));
            await AssertQuery(false, And(false));
        }

        [Test] public async Task TestOr()
        {
            await AssertQuery(true, Or(false, false, true));
            await AssertQuery(false, Or(false, false, false));
            await AssertQuery(true, Or(true));
            await AssertQuery(false, Or(false));
        }

        [Test] public async Task TestNot()
        {
            await AssertQuery(false, Not(true));
            await AssertQuery(true, Not(false));
        }
        #endregion

        [Test] public async Task TestVarargs()
        {
            // Works for arrays too
            await AssertQuery(10, Add(new ArrayV(2, 3, 5)));
            // Works for a variable equal to an array
            await AssertQuery(10, Let("a", new ArrayV(2, 3, 5)).In(a => Add(a)));
        }
    }
}
