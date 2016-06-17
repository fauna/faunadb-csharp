using NUnit.Framework;
using System;

using FaunaDB;
using FaunaDB.Types;
using FaunaDB.Query;
using static FaunaDB.Query.Language;

namespace Test {
    [TestFixture] public class ValuesTest : TestCase
    {
        Ref @ref = new Ref("classes/frogs/123");
        const string jsonRef = "{\"@ref\":\"classes/frogs/123\"}";

        [Test] public void TestRef()
        {
            Assert.AreEqual(@ref, Expr.FromJson(jsonRef));
            Assert.AreEqual(jsonRef, @ref.ToJson());
        }

        [Test] public void TestObj()
        {
            Assert.AreEqual(new ObjectV("@foo", 1), Expr.FromJson("{\"@obj\": {\"@foo\": 1}}"));
        }

        [Test] public void TestAnonymousObj()
        {
            Expr obj = Obj(new {
                foo = 10,
                bar = "bar",
                array = new int[] { 1, 2, 3 },
                obj = new {
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
        }

        [Test] public void TestSet()
        {
            var index = new Ref("indexes/frogs_by_size");
            var match = new SetRef(Language.Match(index, @ref));
            var jsonMatch = $"{{\"@set\":{{\"terms\":{jsonRef},\"match\":{index.ToJson()}}}}}";
            Assert.AreEqual(match, Expr.FromJson(jsonMatch));
            Assert.AreEqual(jsonMatch, match.ToJson());
        }

        [Test] public void TestTimeConversion()
        {
            var dt = DateTime.UtcNow;
            var ft = (TsV) dt;
            Assert.AreEqual(dt, (DateTime) ft);

            dt = UnixTimestamp(0);
            ft = (TsV) dt;
            Assert.AreEqual(ft, new TsV("1970-01-01T00:00:00.0000000Z"));
            Assert.AreEqual(dt, (DateTime) ft);
        }

        [Test] public void TestDateConversion()
        {
            var dt = DateTime.UtcNow.Date;
            var fd = (DateV) dt;
            Assert.AreEqual(dt, (DateTime) fd);

            dt = UnixTimestamp(0).Date;
            fd = (DateV) dt;
            Assert.AreEqual(new DateV("1970-01-01"), fd);
            Assert.AreEqual(dt, (DateTime) fd);
        }

        [Test] public void TestTime()
        {
            var testTs = new TsV("1970-01-01T00:00:00.1234567Z");
            const string testTsJson = "{\"@ts\":\"1970-01-01T00:00:00.1234567Z\"}";
            Assert.AreEqual(testTsJson, testTs.ToJson());
            Assert.AreEqual(testTs, Expr.FromJson(testTsJson));
        }

        [Test] public void TestDate()
        {
            var testDate = new DateV("1970-01-01");
            var testDateJson = "{\"@date\":\"1970-01-01\"}";
            Assert.AreEqual(testDateJson, testDate.ToJson());
            Assert.AreEqual(testDate, Expr.FromJson(testDateJson));
        }

        DateTime UnixTimestamp(double seconds)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(seconds);
        }

        [Test] public void TestObjectEquality()
        {
            var o1 = new ObjectV("a", 1, "b", 2);
            var o2 = new ObjectV("b", 2, "a", 1);
            var o3 = new ObjectV("x", 0, "y", 0);
            Assert.AreEqual(o1, o2);
            Assert.AreNotEqual(o1, o3);
            Assert.AreEqual(o1.GetHashCode(), o2.GetHashCode());
            Assert.AreNotEqual(o1.GetHashCode(), o3.GetHashCode());
        }
    }
}
