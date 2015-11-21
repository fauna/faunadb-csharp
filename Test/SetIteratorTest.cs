using NUnit.Framework;
using System;
using System.Threading.Tasks;

using FaunaDB;
using FaunaDB.Values;
using static FaunaDB.Query;

namespace Test
{
    [TestFixture] public class SetIteratorTest : TestCase
    {
        [Test] public async void TestSetIterator()
        {
            var classRef = GetRef(await TestClient.Post("classes", new ObjectV("name", "gadgets")));
            var indexRef = GetRef(await TestClient.Post("indexes", new ObjectV(
                "name", "gadgets_by_n",
                "source", classRef,
                "path", "data.n",
                "active", true)));

            Func<Value, Task<Ref>> create = async n =>
                GetRef(await Q(Create(classRef, Quote(new ObjectV("data", new ObjectV("n", n))))));

            var a = await create(0);
            await create(1);
            var b = await create(0);

            var gadgetsSet = Match(0, indexRef);

            Assert.AreEqual(new ArrayV(a, b), await new SetIterator(TestClient, gadgetsSet, pageSize: 1).ToArrayV());

            var queryMapper = Lambda(@ref => Select(new ArrayV("data", "n"), Get(@ref)));
            var queryMappedIter = new SetIterator(TestClient, gadgetsSet, mapLambda: queryMapper);
            Assert.AreEqual(new ArrayV(0, 0), await queryMappedIter.ToArrayV());
        }
    }
}
