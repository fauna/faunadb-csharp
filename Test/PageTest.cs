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
            var page = new Page(data, before, after);
            var value = new ObjectV("data", data, "before", before, "after", after);
            Assert.AreEqual(page, (Page) value);
            Assert.AreEqual(value, (Value) page);

            // works with just "data" too
            var pageData = new Page(data);
            var valueData = new ObjectV("data", data);
            Assert.AreEqual(pageData, (Page) valueData);
            Assert.AreEqual(valueData, (Value) pageData);
        }
    }
}
