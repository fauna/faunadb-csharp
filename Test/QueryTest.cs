using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FaunaDB;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Values;
using static FaunaDB.Query;

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
        [TestFixtureSetUp]
        new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            classRef = GetRef(await TestClient.Post("classes", new ObjectV("name", "widgets")));
            nIndexRef = GetRef(await TestClient.Post("indexes", new ObjectV(
                "name", "widgets_by_n",
                "source", classRef,
                "path", "data.n",
                "active", true)));
            mIndexRef = GetRef(await TestClient.Post("indexes", new ObjectV(
                "name", "widgets_by_m",
                "source", classRef,
                "path", "data.m",
                "active", true)));

            refN1 = await CreateRef(n: 1);
            refM1 = await CreateRef(m: 1);
            refN1M1 = await CreateRef(n: 1, m: 1);

            thimbleClassRef = GetRef(await TestClient.Post("classes", new ObjectV("name", "thimbles")));
        }
        #endregion

        #region Helpers

        ObjectV NSet(int n)
        {
            return Match(n, nIndexRef);
        }

        ObjectV MSet(int m)
        {
            return Match(m, mIndexRef);
        }

        async Task<ObjectV> CreateInstance(int n = 0, Value m = null)
        {
            var data = m == null ? new ObjectV("n", n) : new ObjectV("n", n, "m", m);
            return (ObjectV) await Q(Create(classRef, Quote(new ObjectV("data", data))));
        }

        async Task<Ref> CreateRef(int n = 0, Value m = null)
        {
            return GetRef(await CreateInstance(n, m));
        }

        async Task<ObjectV> CreateThimble(ObjectV data)
        {
            return (ObjectV) await Q(Create(thimbleClassRef, Quote(new ObjectV("data", data))));
        }

        async Task<ArrayV> SetToArray(Value set)
        {
            var res = await Q(Paginate(set, size: 1000));
            return (ArrayV) ((ObjectV) res)["data"];
        }

        async Task AssertSet(Value set, params Value[] expectedSetValues)
        {
            Assert.AreEqual(new ArrayV(expectedSetValues), await SetToArray(set));
        }

        async Task AssertBadQuery(Value expression)
        {
            await AssertU.Throws<BadRequest>(() => Q(expression));
        }

        async Task AssertQuery(Value expected, Value expression)
        {
            Assert.AreEqual(expected, await Q(expression));
        }

        #endregion

        #region Basic forms

        [Test] public async void TestLetVar()
        {
            var let = Let(new ObjectV("auto0", 1), Var("auto0"));
            await AssertQuery(1, let);
            Assert.AreEqual(let, Let(1, a => a));
            Assert.AreEqual(
                Let(new ObjectV("auto0", 1, "auto1", 2), new ArrayV(Var("auto0"), Var("auto1"))),
                Let(1, 2, (a, b) => new ArrayV(a, b)));
        }

        [Test] public async void TestIf()
        {
            await AssertQuery("t", If(true, "t", "f"));
            await AssertQuery("f", If(false, "t", "f"));
        }

        [Test] public async void TestDo()
        {
            var rf = await CreateRef();
            await AssertQuery(1, Do(Delete(rf), 1));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async void TestObject()
        {
            // Unlike Quote, contents are evaluated.
            await AssertQuery(new ObjectV("x", 1), QueryObject(new ObjectV("x", Let(new ObjectV("x", 1), Var("x")))));
        }

        [Test] public async void TestQuote()
        {
            var quoted = Let(new ObjectV("x", 1), Var("x"));
            await AssertQuery(quoted, Quote(quoted));
        }

        [Test] public void TestLambda()
        {
            var expected = Lambda("auto0", Add(Var("auto0"), Var("auto0")));
            Assert.AreEqual(expected, Lambda(a => Add(a, a)));

            expected = Lambda("auto0", Lambda("auto1", Lambda("auto2", new ArrayV(Var("auto0"), Var("auto1"), Var("auto2")))));
            Assert.AreEqual(expected, Lambda(a => Lambda(b => Lambda(c => new ArrayV(a, b, c)))));

            // Error in lambda should not affect future queries.
            AssertU.Throws<Exception>(() => {
                Lambda(a => {
                    throw new Exception();
                });
            });
            Assert.AreEqual(LambdaId, Lambda(a => a));
        }

        static ObjectV LambdaId = new ObjectV("lambda", "auto0", "expr", new ObjectV("var", "auto0"));

        [Test] public void TestLambdaMultithreaded()
        {
            var events = new List<int>();

            var t1 = Task.Run(() =>
            {
                Assert.AreEqual(LambdaId, Lambda(a =>
                {
                    events.Add(0);
                    Thread.Sleep(1000);
                    events.Add(2);
                    return a;
                }));
            });

            var t2 = Task.Run(() =>
            {
                Thread.Sleep(500);
                // This happens while t1 has incremented its auto name to auto1,
                // but that doesn't affect this thread.
                Assert.AreEqual(LambdaId, Lambda(a => a));
                events.Add(1);
            });

            t1.Wait();
            t2.Wait();

            Assert.AreEqual(new List<int> { 0, 1, 2 }, events);
        }

        [Test] public void TestLambdaMultiVar()
        {
            var expected = Lambda(new ArrayV("auto0", "auto1"), new ArrayV(Var("auto0"), Var("auto1")));
            Assert.AreEqual(expected, Lambda((a, b) => new ArrayV(a, b)));
        }

        #endregion

        #region Collection functions

        [Test] public async void TestMap()
        {
            await AssertQuery(new ArrayV(2, 4, 6), Map(new ArrayV(1, 2, 3), a => Multiply(2, a)));

            var page = Paginate(NSet(1));
            var ns = Map(page, a => Select(new ArrayV("data", "n"), Get(a)));
            await AssertQuery(new ObjectV("data", new ArrayV(1, 1)), ns);
        }

        [Test] public async void TestForeach()
        {
            var refs = new ArrayV(await CreateRef(), await CreateRef());
            await Q(Foreach(refs, Delete));
            foreach (var @ref in refs)
                await AssertQuery(false, Exists(@ref));
        }

        [Test] public async void TestFilter()
        {
            var evens = Filter(new ArrayV(1, 2, 3, 4), a => EqualsExpr(Modulo(a, 2), 0));
            await AssertQuery(new ArrayV(2, 4), evens);

            // Works on page too
            var page = Paginate(NSet(1));
            var refsWithM = Filter(page, a => Contains(new ArrayV("data", "m"), Get(a)));
            await AssertQuery(new ObjectV("data", new ArrayV(refN1M1)), refsWithM);
        }

        [Test] public async void TestTake()
        {
            await AssertQuery(new ArrayV(1), Take(1, new ArrayV(1, 2)));
            await AssertQuery(new ArrayV(1, 2), Take(3, new ArrayV(1, 2)));
            await AssertQuery(ArrayV.Empty, Take(-1, new ArrayV(1, 2)));
        }

        [Test] public async void TestDrop()
        {
            await AssertQuery(new ArrayV(2), Drop(1, new ArrayV(1, 2)));
            await AssertQuery(ArrayV.Empty, Drop(3, new ArrayV(1, 2)));
            await AssertQuery(new ArrayV(1, 2), Drop(-1, new ArrayV(1, 2)));
        }

        [Test] public async void TestPrepend()
        {
            await AssertQuery(new ArrayV(1, 2, 3, 4, 5, 6), Prepend(new ArrayV(1, 2, 3), new ArrayV(4, 5, 6)));
        }

        [Test] public async void TestAppend() {
            await AssertQuery(new ArrayV(1, 2, 3, 4, 5, 6), Append(new ArrayV(4, 5, 6), new ArrayV(1, 2, 3)));
        }

        #endregion

        #region Read functions

        [Test] public async void TestGet()
        {
            var instance = await CreateInstance();
            await AssertQuery(instance, Get(GetRef(instance)));
        }

        [Test] public async void TestPaginate()
        {
            var testSet = NSet(1);
            await AssertQuery(new ObjectV("data", new ArrayV(refN1, refN1M1)), Paginate(testSet));
            await AssertQuery(new ObjectV("data", new ArrayV(refN1), "after", new ArrayV(refN1M1)), Paginate(testSet, size: 1));
            var sources = new ArrayV(new Set(testSet));
            var page = new ObjectV("data",
                new ArrayV(
                    new ObjectV("sources", sources, "value", refN1),
                    new ObjectV("sources", sources, "value", refN1M1)));
            await AssertQuery(page, Paginate(testSet, sources: true));
        }

        [Test] public async void TestExists()
        {
            var rf = await CreateRef();
            await AssertQuery(true, Exists(rf));
            await Q(Delete(rf));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async void TestCount()
        {
            await CreateInstance(123);
            await CreateInstance(123);
            var instances = NSet(123);
            // `Count` is currently only approximate. Should be 2.
            var n = await Q(Count(instances));
            Assert.IsInstanceOfType(typeof(LongV), n);
        }

        #endregion

        #region Write functions

        [Test] public async void TestCreate()
        {
            var instance = await CreateInstance();
            Assert.That(instance.Val.ContainsKey("ref"));
            Assert.That(instance.Val.ContainsKey("ts")); //ts
            Assert.AreEqual(classRef, instance["class"]);
        }

        [Test] public async void TestUpdate()
        {
            var rf = await CreateRef();
            var got = (ObjectV) await Q(Replace(rf, Quote(new ObjectV("data", new ObjectV("m", 123)))));
            Assert.AreEqual(new ObjectV("m", 123), got["data"]);
        }

        [Test] public async void TestReplace()
        {
            var rf = await CreateRef();
            var got = (ObjectV) await Q(Replace(rf, Quote(new ObjectV("data", new ObjectV("m", 123)))));
            Assert.AreEqual(new ObjectV("m", 123), got["data"]);
        }

        [Test] public async void TestDelete()
        {
            var rf = await CreateRef();
            await Q(Delete(rf));
            await AssertQuery(false, Exists(rf));
        }

        [Test] public async void TestInsert()
        {
            var instance = await CreateThimble(new ObjectV("weight", 1));
            var @ref = GetRef(instance);
            var ts = (long) instance["ts"];
            var prevTs = ts - 1;
            // Add previous event
            await Q(Insert(@ref, prevTs, EventType.Create, Quote(new ObjectV("data", new ObjectV("weight", 0)))));
            // Get version from previous event
            var old = (ObjectV) await Q(Get(@ref, prevTs));
            Assert.AreEqual(new ObjectV("weight", 0), old["data"]);
        }

        [Test] public async void TestRemove()
        {
            var instance = await CreateThimble(new ObjectV("weight", 0));
            var @ref = GetRef(instance);

            // Change it
            var newInstance = (ObjectV) await Q(Replace(@ref, Quote(new ObjectV("data", new ObjectV("weight", 1)))));
            Assert.AreEqual(newInstance, await Q(Get(@ref)));

            // Delete that event
            await Q(Remove(@ref, newInstance["ts"], "create"));
            // Assert that it was undone
            Assert.AreEqual(instance, await Q(Get(@ref)));
        }

        #endregion

        #region Sets

        [Test] public async void TestMatch()
        {
            await AssertSet(NSet(1), refN1, refN1M1);
        }

        [Test] public async void TestUnion()
        {
            await AssertSet(Union(NSet(1), MSet(1)), refN1, refM1, refN1M1);
        }

        [Test] public async void TestIntersection()
        {
            await AssertSet(Intersection(NSet(1), MSet(1)), refN1M1);
        }

        [Test] public async void TestDifference()
        {
            await AssertSet(Difference(NSet(1), MSet(1)), refN1); // but not refN1M1
        }

        [Test] public async void TestJoin()
        {
            var referenced = new ArrayV(await CreateRef(n: 12), await CreateRef(n: 12));
            var referencers = new ArrayV(await CreateRef(m: referenced[0]), await CreateRef(m: referenced[1]));

            var source = NSet(12);
            Assert.AreEqual(referenced, await SetToArray(source));

            // For each obj with n=12, get the set of elements whose data.m refers to it.
            var joined = Join(source, a => Match(a, mIndexRef));
            await AssertSet(joined, referencers.ToArray());
        }

        #endregion

        #region Authentication

        [Test] public async void TestAuthentication()
        {
            // Setup
            var userClassRef = GetRef(await TestClient.Post("classes", new ObjectV("name", "people")));
            const string password = "swordfish";
            var userRef = GetRef(await TestClient.Post(userClassRef,
                new ObjectV("credentials", new ObjectV("password", password))));

            // Identify
            await AssertQuery(true, Identify(userRef, password));
            await AssertQuery(false, Identify(userRef, "pass123"));

            // Login
            var loginResponse = (ObjectV) await Q(LoginWithPassword(userRef, password));
            Assert.AreEqual(loginResponse["instance"], userRef);
            var userClient = GetClient(password: (string) loginResponse["secret"]);

            // Logout
            await userClient.Get("/tokens/self");
            await userClient.Query(Logout(true));
            await AssertU.Throws<PermissionDenied>(() => userClient.Get("/tokens/self"));
        }

        #endregion

        #region String functions

        [Test] public async void TestConcat()
        {
            await AssertQuery("abc", Concat(new ArrayV("a", "b", "c")));
            await AssertQuery("", Concat(ArrayV.Empty));
            await AssertQuery("a.b.c", Concat(new ArrayV("a", "b", "c"), "."));
        }

        [Test] public async void TestCaseFold()
        {
            await AssertQuery("hen wen", CaseFold("Hen Wen"));
        }

        #endregion

        #region Time and date functions

        [Test] public async void TestTime()
        {
            var time = "1970-01-01T00:00:00.123456789Z";
            await AssertQuery(new FaunaTime(time), Time(time));

            // "now" refers to the current time.
            Assert.IsInstanceOfType(typeof(FaunaTime), await Q(Time("now")));
        }

        [Test] public async void TestEpoch()
        {
            await AssertQuery(new FaunaTime("1970-01-01T00:00:12Z"), Epoch(12, "second"));
            var nanoTime = new FaunaTime("1970-01-01T00:00:00.123456789Z");
            await AssertQuery(nanoTime, Epoch(123456789, "nanosecond"));
        }

        [Test] public async void TestDate()
        {
            await AssertQuery(new FaunaDate("1970-01-01"), Date("1970-01-01"));
        }

        #endregion

        #region Miscellaneous functions

        [Test] public async void TestEquals()
        {
            await AssertQuery(true, EqualsExpr(1, 1, 1));
            await AssertQuery(false, EqualsExpr(1, 1, 2));
            await AssertQuery(true, EqualsExpr(1));
            await AssertBadQuery(EqualsExpr());
        }

        [Test] public async void TestContains()
        {
            var obj = Quote(new ObjectV("a", new ObjectV("b", 1)));
            await AssertQuery(true, Contains(new ArrayV("a", "b"), obj));
            await AssertQuery(true, Contains("a", obj));
            await AssertQuery(false, Contains(new ArrayV("a", "c"), obj));
        }

        [Test] public async void TestSelect()
        {
            var obj = Quote(new ObjectV("a", new ObjectV("b", 1)));
            await AssertQuery(new ObjectV("b", 1), Select("a", obj));
            await AssertQuery(1, Select(new ArrayV("a", "b"), obj));
            await AssertQuery(Value.Null, Select("c", obj, Value.Null));
            await AssertU.Throws<NotFound>(() => Q(Select("c", obj)));
        }

        [Test] public async void TestSelectArray()
        {
            var arr = new ArrayV(1, 2, 3);
            await AssertQuery(3, Select(2, arr));
            await AssertU.Throws<NotFound>(() => Q(Select(3, arr)));
        }

        [Test] public async void TestAdd()
        {
            await AssertQuery(10, Add(2, 3, 5));
            await AssertBadQuery(Add());
        }

        [Test] public async void TestMultiply()
        {
            await AssertQuery(30, Multiply(2, 3, 5));
            await AssertBadQuery(Multiply());
        }

        [Test] public async void TestSubtract()
        {
            await AssertQuery(-6, Subtract(2, 3, 5));
            await AssertQuery(2, Subtract(2));
            await AssertBadQuery(Subtract());
        }

        [Test] public async void TestDivide()
        {
            await AssertQuery(2.0 / 15, Divide(2.0, 3, 5));
            await AssertQuery(2, Divide(2));
            await AssertBadQuery(Divide(1, 0));
            await AssertBadQuery(Divide());
        }

        [Test] public async void TestModulo()
        {
            await AssertQuery(1, Modulo(5, 2));
            // This is (15 % 10) % 2
            await AssertQuery(1, Modulo(15, 10, 2));
            await AssertQuery(2, Modulo(2));
            await AssertBadQuery(Modulo(1, 0));
            await AssertBadQuery(Modulo());
        }

        [Test] public async void TestAnd()
        {
            await AssertQuery(false, And(true, true, false));
            await AssertQuery(true, And(true, true, true));
            await AssertQuery(true, And(true));
            await AssertQuery(false, And(false));
            await AssertBadQuery(And());
        }

        [Test] public async void TestOr()
        {
            await AssertQuery(true, Or(false, false, true));
            await AssertQuery(false, Or(false, false, false));
            await AssertQuery(true, Or(true));
            await AssertQuery(false, Or(false));
            await AssertBadQuery(Or());
        }

        [Test] public async void TestNot()
        {
            await AssertQuery(false, Not(true));
            await AssertQuery(true, Not(false));
        }

        #endregion

        [Test] public async void TestVarargs()
        {
            // Works for arrays too
            await AssertQuery(10, Add(new ArrayV(2, 3, 5)));
            // Works for a variable equal to an array
            await AssertQuery(10, Let(new ArrayV(2, 3, 5), a => Add(a)));
        }
    }
}
