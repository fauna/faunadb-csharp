using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaunaDB.Client.Utils;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture]
    public class PageTest : TestCase
    {
        Expr classRef, indexRef;
        Dictionary<long, Value> instanceRefs = new Dictionary<long, Value>();
        Dictionary<Value, long> refsToIndex = new Dictionary<Value, long>();

        Expr tsClassRef, tsIndexRef, tsInstance1Ref, tsInstance1Ts;

        [OneTimeSetUp]
        new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            await SetupTimestampedThings();
            await SetupPagedThings();
        }

        async Task SetupTimestampedThings()
        {
            var task = await client.Query(CreateCollection(Obj("name", "timestamped_things")));
            tsClassRef = task.At("ref");

            var task1 = await client.Query(CreateIndex(Obj(
                    "name", "timestamped_things_by_class",
                    "active", true,
                    "source", tsClassRef
            )));
            tsIndexRef = task1.At("ref");

            var task2 = await client.Query(Create(tsClassRef, Obj()));
            tsInstance1Ref = task2.At("ref");
            tsInstance1Ts = task2.At("ts");

            await client.Query(Create(tsClassRef, Obj()));
        }

        async Task SetupPagedThings()
        {
            var task = await client.Query(CreateCollection(Obj("name", "paged_things")));
            classRef = task.At("ref");

            var task2 = await client.Query(CreateIndex(Obj(
                    "name", "things_by_class",
                    "active", true,
                    "source", classRef,
                    "values", Arr(Obj("field", Arr("data", "i")), Obj("field", "ref"))
            )));
            indexRef = task2.At("ref");

            for (int i = 0; i < 100; i++)
            {
                var result = await client.Query(Create(classRef, Obj("data", Obj("i", i))));
                var index = result.At("data", "i").To<long>().Value;

                instanceRefs.Add(index, result.At("ref"));
                refsToIndex.Add(result.At("ref"), index);
            }
        }

        [Test]
        public async Task TestPagination()
        {
            var page = new PageHelper(client, Match(indexRef));

            await page.Each(p =>
            {
                var array = p as ArrayV;

                foreach (var value in array)
                {
                    var i = value.At(0).To<long>().Value;
                    var r = value.At(1);

                    Assert.AreEqual(instanceRefs[i], r);
                }
            });
        }

        [Test]
        public async Task TestReversePagination()
        {
            var page = new PageHelper(client, Match(indexRef), before: NullV.Instance);

            var item = 100;
            await page.EachReverse(p =>
            {
                var array = (p as ArrayV).Reverse();

                foreach (var value in array)
                {
                    var @ref = value.At(1);

                    Assert.AreEqual(item - 1, refsToIndex[@ref]);

                    item--;
                }
            });

            Assert.AreEqual(0, item);
        }

        [Test]
        public async Task MapsPagination()
        {
            var page = new PageHelper(client, Match(indexRef));

            var pageMapped = page.Map(Lambda(obj => Select(Path(1), obj)));

            var item = 0;
            await pageMapped.Each(p => {
                var array = p as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreEqual(item, refsToIndex[value]);

                    item++;
                }
            });

            Assert.AreEqual(100, item);
        }

        [Test]
        public async Task FilterPagination()
        {
            var page = new PageHelper(client, Match(indexRef));

            var pageFiltered = page.Filter(Lambda(obj => {
                return EqualsFn(Modulo(Select(Path(0), obj), 2), 0);
            }));

            var item = 0;
            await pageFiltered.Each(p =>
            {
                var array = p as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreEqual(item, refsToIndex[value.At(1)]);

                    item += 2;
                }
            });

            Assert.AreEqual(100, item);
        }

        [Test]
        public async Task HonorsPassedInCursor()
        {
            var page = new PageHelper(client, Match(indexRef), after: 50);

            var item = 50;
            await page.Each(p => {
                var array = p as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreEqual(item, refsToIndex[value.At(1)]);

                    item++;
                }
            });

            Assert.AreEqual(item, 100);
        }

        [Test]
        public async Task HonorsPassedInCursorInTheReverseDirection()
        {
            var page = new PageHelper(client, Match(indexRef), before: 51);

            var item = 51;
            await page.EachReverse(p => {
                var array = (p as ArrayV).Reverse();

                foreach (var value in array)
                {
                    Assert.AreEqual(item - 1, refsToIndex[value.At(1)]);

                    item--;
                }
            });

            Assert.AreEqual(item, 0);
        }

        [Test]
        public async Task HonorsSize()
        {
            var numPages = 20;
            var pageSize = 100 / numPages;

            var page = new PageHelper(client, Match(indexRef), size: pageSize);

            var item = 0;
            await page.Each(p => {
                var array = p as ArrayV;

                Assert.AreEqual(array.Length, pageSize);

                item++;
            });

            Assert.AreEqual(item, numPages);
        }

        [Test]
        public async Task HonorsTs()
        {
            var page = new PageHelper(client, Match(tsIndexRef));

            await page.Each(item => {
                Assert.That(item, Has.Length.EqualTo(2));
            });

            var page2 = new PageHelper(client, Match(tsIndexRef), ts: tsInstance1Ts);
            await page2.Each(item => {
                var array = item as ArrayV;

                Assert.That(array, Has.Length.EqualTo(1));
                Assert.AreEqual(tsInstance1Ref, array[0]);
            });
        }

        [Test]
        public async Task HonorsEvents()
        {
            var page = new PageHelper(client, Match(indexRef), events: true);

            await page.Each(item => {
                var array = item as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreNotSame(value.At("ts"), NullV.Instance);
                    Assert.AreNotSame(value.At("action"), NullV.Instance);
                    Assert.AreNotSame(value.At("document"), NullV.Instance);
                    Assert.AreNotSame(value.At("data"), NullV.Instance);
                }
            });
        }

        [Test]
        public async Task HornosSource()
        {
            var page = new PageHelper(client, Match(indexRef), sources: true);

            await page.Each(item => {
                var array = item as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreNotSame(value.At("sources"), NullV.Instance);
                }
            });
        }

        [Test]
        public async Task HornosCombinationParameters()
        {
            var page = new PageHelper(client, Match(indexRef), before: NullV.Instance, events: true, sources: true);

            await page.Each(item => {
                var array = item as ArrayV;

                foreach (var value in array)
                {
                    Assert.AreNotSame(value.At("value"), NullV.Instance);
                    Assert.AreNotSame(value.At("sources"), NullV.Instance);

                    Assert.AreNotSame(value.At("value", "ts"), NullV.Instance);
                    Assert.AreNotSame(value.At("value", "action"), NullV.Instance);
                    Assert.AreNotSame(value.At("value", "document"), NullV.Instance);
                    Assert.AreNotSame(value.At("value", "data"), NullV.Instance);

                }
            });
        }

        [Test]
        public async Task IterativelyPaginatePages()
        {
            var page = new PageHelper(client, Match(indexRef), size: 2);

            var item1 = await page.NextPage();

            Assert.That(item1, Has.Length.EqualTo(2));
            Assert.AreEqual(0, refsToIndex[item1.At(0, 1)]);
            Assert.AreEqual(1, refsToIndex[item1.At(1, 1)]);

            var item2 = await page.NextPage();

            Assert.That(item2, Has.Length.EqualTo(2));
            Assert.AreEqual(2, refsToIndex[item2.At(0, 1)]);
            Assert.AreEqual(3, refsToIndex[item2.At(1, 1)]);
        }

        [Test]
        public async Task IterativelyPaginatePagesInReverseDirection()
        {
            var page = new PageHelper(client, Match(indexRef), before: NullV.Instance, size: 2);

            var item1 = await page.PreviousPage();

            Assert.That(item1, Has.Length.EqualTo(2));
            Assert.AreEqual(98, refsToIndex[item1.At(0, 1)]);
            Assert.AreEqual(99, refsToIndex[item1.At(1, 1)]);

            var item2 = await page.PreviousPage();

            Assert.That(item2, Has.Length.EqualTo(2));
            Assert.AreEqual(96, refsToIndex[item2.At(0, 1)]);
            Assert.AreEqual(97, refsToIndex[item2.At(1, 1)]);
        }
    }
}

