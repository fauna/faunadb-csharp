using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class OperatorTest
    {
        [Test] public void TestImplicit()
        {
            Assert.AreEqual(LongV.Of(10), (Expr)10);
            Assert.AreEqual(LongV.Of(10), (Expr)10L);
            Assert.AreEqual(BooleanV.True, (Expr)true);
            Assert.AreEqual(BooleanV.False, (Expr)false);
            Assert.AreEqual(DoubleV.Of(3.14), (Expr)3.14);
            Assert.AreEqual(StringV.Of("a string"), (Expr)"a string");
            Assert.AreEqual(StringV.Of("create"), (Expr)ActionType.Create);
            Assert.AreEqual(StringV.Of("delete"), (Expr)ActionType.Delete);
            Assert.AreEqual(StringV.Of("second"), (Expr)TimeUnit.Second);
            Assert.AreEqual(StringV.Of("millisecond"), (Expr)TimeUnit.Millisecond);
            Assert.AreEqual(StringV.Of("microsecond"), (Expr)TimeUnit.Microsecond);
            Assert.AreEqual(StringV.Of("nanosecond"), (Expr)TimeUnit.Nanosecond);
            Assert.AreEqual(NullV.Instance, (Expr)(string)null);
        }

        [Test] public void TestExplicit()
        {
            Assert.AreEqual(10, (int)LongV.Of(10));
            Assert.AreEqual(10L, (long)LongV.Of(10));
            Assert.AreEqual(true, (bool)BooleanV.True);
            Assert.AreEqual(false, (bool)BooleanV.False);
            Assert.AreEqual(3.14, (double)DoubleV.Of(3.14));
            Assert.AreEqual("a string", (string)StringV.Of("a string"));
            Assert.AreEqual(ActionType.Create, (ActionType)StringV.Of("create"));
            Assert.AreEqual(ActionType.Delete, (ActionType)StringV.Of("delete"));
            Assert.AreEqual(TimeUnit.Second, (TimeUnit)StringV.Of("second"));
            Assert.AreEqual(TimeUnit.Millisecond, (TimeUnit)StringV.Of("millisecond"));
            Assert.AreEqual(TimeUnit.Microsecond, (TimeUnit)StringV.Of("microsecond"));
            Assert.AreEqual(TimeUnit.Nanosecond, (TimeUnit)StringV.Of("nanosecond"));
            Assert.AreEqual(null, (string)(Expr)NullV.Instance);
        }
    }
}
