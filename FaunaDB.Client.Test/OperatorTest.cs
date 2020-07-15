using System;
using System.Collections.Generic;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class OperatorTest
    {
        [Test] public void TestImplicitExpr()
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
            Assert.AreEqual(DateV.Of("2000-01-01"), (Expr)new DateTime(2000,1, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.AreEqual(DateV.Of("2000-01-01"), (Expr)new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));
            Assert.AreEqual(TimeV.Of("2000-01-01T01:01:01.123Z"), (Expr)new DateTime(2000, 1, 1, 1, 1, 1, 123, DateTimeKind.Utc));
            Assert.AreEqual(TimeV.Of("2000-01-01T01:01:01.123Z"), (Expr)new DateTimeOffset(2000, 1, 1, 1, 1, 1, 123, TimeSpan.Zero));
            Assert.AreEqual(NullV.Instance, (Expr)(string)null);
            Assert.AreEqual(
                ObjectV.With("name", "foo", "count", 42),
                (Expr)new Dictionary<string, Expr>() {{ "name", "foo" }, { "count", 42 }});
            Assert.AreEqual(BytesV.Of(1, 2, 3), (Expr)new byte[] { 1, 2, 3 });
        }

        [Test]
        public void TestImplicitValue()
        {
            Assert.AreEqual(LongV.Of(10), (Value)10);
            Assert.AreEqual(LongV.Of(10), (Value)10L);
            Assert.AreEqual(BooleanV.True, (Value)true);
            Assert.AreEqual(BooleanV.False, (Value)false);
            Assert.AreEqual(DoubleV.Of(3.14), (Value)3.14);
            Assert.AreEqual(StringV.Of("a string"), (Value)"a string");
            Assert.AreEqual(StringV.Of("create"), (Value)ActionType.Create);
            Assert.AreEqual(StringV.Of("delete"), (Value)ActionType.Delete);
            Assert.AreEqual(StringV.Of("second"), (Value)TimeUnit.Second);
            Assert.AreEqual(StringV.Of("millisecond"), (Value)TimeUnit.Millisecond);
            Assert.AreEqual(StringV.Of("microsecond"), (Value)TimeUnit.Microsecond);
            Assert.AreEqual(StringV.Of("nanosecond"), (Value)TimeUnit.Nanosecond);
            Assert.AreEqual(DateV.Of("2000-01-01"), (Value)new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.AreEqual(DateV.Of("2000-01-01"), (Value)new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));
            Assert.AreEqual(TimeV.Of("2000-01-01T01:01:01.123Z"), (Value)new DateTime(2000, 1, 1, 1, 1, 1, 123, DateTimeKind.Utc));
            Assert.AreEqual(TimeV.Of("2000-01-01T01:01:01.123Z"), (Value)new DateTimeOffset(2000, 1, 1, 1, 1, 1, 123, TimeSpan.Zero));
            Assert.AreEqual(NullV.Instance, (Value)(string)null);
            Assert.AreEqual(BytesV.Of(1, 2, 3), (Value)new byte[] { 1, 2, 3 });
        }


        [Test]
        public void TestExplicitExpr()
        {
            Assert.AreEqual(10, (int)(Expr)LongV.Of(10));
            Assert.AreEqual(10L, (long)(Expr)LongV.Of(10));
            Assert.AreEqual(true, (bool)(Expr)BooleanV.True);
            Assert.AreEqual(false, (bool)(Expr)BooleanV.False);
            Assert.AreEqual(3.14, (double)(Expr)DoubleV.Of(3.14));
            Assert.AreEqual("a string", (string)(Expr)StringV.Of("a string"));
            Assert.AreEqual(ActionType.Create, (ActionType)(Expr)StringV.Of("create"));
            Assert.AreEqual(ActionType.Delete, (ActionType)(Expr)StringV.Of("delete"));
            Assert.AreEqual(TimeUnit.Second, (TimeUnit)(Expr)StringV.Of("second"));
            Assert.AreEqual(TimeUnit.Millisecond, (TimeUnit)(Expr)StringV.Of("millisecond"));
            Assert.AreEqual(TimeUnit.Microsecond, (TimeUnit)(Expr)StringV.Of("microsecond"));
            Assert.AreEqual(TimeUnit.Nanosecond, (TimeUnit)(Expr)StringV.Of("nanosecond"));
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), (DateTime)(Expr)DateV.Of("2000-01-01"));
            Assert.AreEqual(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), (DateTimeOffset)(Expr)DateV.Of("2000-01-01"));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 1, 1, 123, DateTimeKind.Utc), (DateTime)(Expr)TimeV.Of("2000-01-01T01:01:01.123Z"));
            Assert.AreEqual(new DateTimeOffset(2000, 1, 1, 1, 1, 1, 123, TimeSpan.Zero), (DateTimeOffset)(Expr)TimeV.Of("2000-01-01T01:01:01.123Z"));
            Assert.AreEqual(null, (string)(Expr)NullV.Instance);
        }

        [Test] public void TestExplicitValue()
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
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), (DateTime)DateV.Of("2000-01-01"));
            Assert.AreEqual(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), (DateTimeOffset)DateV.Of("2000-01-01"));
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 1, 1, 123, DateTimeKind.Utc), (DateTime)TimeV.Of("2000-01-01T01:01:01.123Z"));
            Assert.AreEqual(new DateTimeOffset(2000, 1, 1, 1, 1, 1, 123, TimeSpan.Zero), (DateTimeOffset)TimeV.Of("2000-01-01T01:01:01.123Z"));
        }
    }
}
