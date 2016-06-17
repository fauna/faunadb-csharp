using NUnit.Framework;

using FaunaDB;
using FaunaDB.Types;

namespace Test
{
    [TestFixture] public class PageTest : TestCase
    {
        [Test] public void TestPage()
        {
            var data = new ArrayV(1);
            var before = new Cursor(new ArrayV(new Ref("before")));
            var after = new Cursor(new ArrayV(new Ref("after")));
            var page = new Pagination(data, before, after);
            var value = ObjectV.With("data", data, "before", before, "after", after);
            Assert.AreEqual(page, (Pagination) value);
            Assert.AreEqual(value, (Value) page);

            // works with just "data" too
            var pageData = new Pagination(data);
            var valueData = ObjectV.With("data", data);
            Assert.AreEqual(pageData, (Pagination) valueData);
            Assert.AreEqual(valueData, (Value) pageData);
        }
    }
}
