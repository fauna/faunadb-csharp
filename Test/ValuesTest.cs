using NUnit.Framework;
using System;

using FaunaDB;
using FaunaDB.Values;

namespace Test {
    [TestFixture] public class ValuesTest : TestCase
    {
        Ref @ref = new Ref("classes", "frogs", "123");
        const string jsonRef = "{\"@ref\":\"classes/frogs/123\"}";

        [Test] public void TestRef()
        {
            Assert.AreEqual(@ref, Value.FromJson(jsonRef));
            Assert.AreEqual(jsonRef, @ref.ToJson());

            var keys = new Ref("keys");
            Assert.AreEqual(keys, keys.Class);

            /*
            TODO: enabling this line causes every test to stop running.
            Let's hope this is just a Mono error.
            AssertU.Throws<InvalidValue>(() => keys.Id++);
            */

            var key = new Ref(keys, "123");
            Assert.AreEqual(keys, key.Class);
            Assert.AreEqual("123", key.Id);
        }

        [Test] public void TestObj()
        {
            Assert.AreEqual(new ObjectV("@foo", 1), Value.FromJson("{\"@obj\": {\"@foo\": 1}}"));
        }

        [Test] public void TestSet()
        {
            var index = new Ref("indexes", "frogs_by_size");
            var match = new SetRef(Query.Match(index, @ref));
            var jsonMatch = $"{{\"@set\":{{\"terms\":{jsonRef},\"match\":{index.ToJson()}}}}}";
            Assert.AreEqual(match, Value.FromJson(jsonMatch));
            Assert.AreEqual(jsonMatch, match.ToJson());
        }

        [Test] public void TestTimeConversion()
        {
            var dt = DateTime.UtcNow;
            var ft = (FaunaTime) dt;
            Assert.AreEqual(dt, (DateTime) ft);

            dt = UnixTimestamp(0);
            ft = (FaunaTime) dt;
            Assert.AreEqual(ft, new FaunaTime("1970-01-01T00:00:00.0000000Z"));
            Assert.AreEqual(dt, (DateTime) ft);
        }

        [Test] public void TestDateConversion()
        {
            var dt = DateTime.UtcNow.Date;
            var fd = (FaunaDate) dt;
            Assert.AreEqual(dt, (DateTime) fd);

            dt = UnixTimestamp(0).Date;
            fd = (FaunaDate) dt;
            Assert.AreEqual(new FaunaDate("1970-01-01"), fd);
            Assert.AreEqual(dt, (DateTime) fd);
        }

        [Test] public void TestTime()
        {
            var testTs = new FaunaTime("1970-01-01T00:00:00.123456789Z");
            const string testTsJson = "{\"@ts\":\"1970-01-01T00:00:00.123456789Z\"}";
            Assert.AreEqual(testTsJson, testTs.ToJson());
            Assert.AreEqual(testTs, Value.FromJson(testTsJson));
        }

        [Test] public void TestDate()
        {
            var testDate = new FaunaDate("1970-01-01");
            var testDateJson = "{\"@date\":\"1970-01-01\"}";
            Assert.AreEqual(testDateJson, testDate.ToJson());
            Assert.AreEqual(testDate, Value.FromJson(testDateJson));
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
