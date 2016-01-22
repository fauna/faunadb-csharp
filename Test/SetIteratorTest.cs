﻿using NUnit.Framework;
using System;
using System.Threading.Tasks;

using FaunaDB;
using FaunaDB.Values;
using static FaunaDB.Query;

namespace Test
{
    [TestFixture] public class SetIteratorTest : TestCase
    {
        [TestFixtureSetUp]
        new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        Ref indexRef;
        Value a, b;
        Query gadgetsSet;

        async Task SetUpAsync()
        {
            var classRef = GetRef(await TestClient.Post("classes", new ObjectV("name", "gadgets")));
            indexRef = GetRef(await TestClient.Post("indexes", new ObjectV(
                "name", "gadgets_by_n",
                "source", classRef,
                "path", "data.n",
                "active", true)));

            Func<Value, Task<Ref>> create = async n =>
                GetRef(await Q(Create(classRef, Quote(new ObjectV("data", new ObjectV("n", n))))));

            a = await create(0);
            await create(1);
            b = await create(0);

            gadgetsSet = Match(indexRef, 0);
        }

        [Test] public async void TestSetIterator()
        {
            Assert.AreEqual(new ArrayV(a, b), await new SetIterator(TestClient, gadgetsSet, pageSize: 1).ToArrayV());
        }

        [Test] public async void TestMapper()
        {
            var queryMapper = Lambda(@ref => Select(new ArrayV("data", "n"), Get(@ref)));
            var queryMappedIter = new SetIterator(TestClient, gadgetsSet, mapLambda: queryMapper);
            Assert.AreEqual(new ArrayV(0, 0), await queryMappedIter.ToArrayV());
        }
    }
}
