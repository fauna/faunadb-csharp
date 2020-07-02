using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;
using FaunaDB.Query;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;

using static FaunaDB.Query.Language;
using static FaunaDB.Types.Option;
using static FaunaDB.Types.Encoder;

namespace Test
{
    [TestFixture]
    public class ClientTest : TestCase
    {
        private static Field<Value> DATA = Field.At("data");
        private static Field<RefV> REF_FIELD = Field.At("ref").To<RefV>();
        private static Field<long> TS_FIELD = Field.At("ts").To<long>();
        private static Field<RefV> DOCUMENT_FIELD = Field.At("document").To<RefV>();
        private static Field<IReadOnlyList<RefV>> REF_LIST = DATA.Collect(Field.To<RefV>());

        private static Field<string> NAME_FIELD = DATA.At(Field.At("name")).To<string>();
        private static Field<string> ELEMENT_FIELD = DATA.At(Field.At("element")).To<string>();
        private static Field<Value> ELEMENTS_LIST = DATA.At(Field.At("elements"));
        private static Field<long> COST_FIELD = DATA.At(Field.At("cost")).To<long>();

        private static RefV magicMissile;
        private static RefV fireball;
        private static RefV faerieFire;
        private static RefV summon;
        private static RefV thor;
        private static RefV thorSpell1;
        private static RefV thorSpell2;

        RefV GetRef(Value v) =>
            v.Get(REF_FIELD);

        [OneTimeSetUp]
        new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            await client.Query(CreateCollection(Obj("name", "spells")));
            await client.Query(CreateCollection(Obj("name", "characters")));
            await client.Query(CreateCollection(Obj("name", "spellbooks")));

            await client.Query(CreateIndex(Obj(
                "name", "all_spells",
                "active", true,
                "source", Collection("spells")
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spells_by_element",
                "active", true,
                "source", Collection("spells"),
                "terms", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "elements_of_spells",
                "active", true,
                "source", Collection("spells"),
                "values", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spellbooks_by_owner",
                "active", true,
                "source", Collection("spellbooks"),
                "terms", Arr(Obj("field", Arr("data", "owner")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spells_by_spellbook",
                "active", true,
                "source", Collection("spells"),
                "terms", Arr(Obj("field", Arr("data", "spellbook")))
              )));

            magicMissile = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj(
                    "name", "Magic Missile",
                    "element", "arcane",
                    "cost", 10)))
            ));

            fireball = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj(
                    "name", "Fireball",
                    "element", "fire",
                    "cost", 10)))
            ));

            faerieFire = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj(
                    "name", "Faerie Fire",
                    "cost", 10,
                    "element", Arr(
                      "arcane",
                      "nature"
                    ))))
            ));

            summon = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj(
                    "name", "Summon Animal Companion",
                    "element", "nature",
                    "cost", 10)))
            ));

            thor = GetRef(await client.Query(
              Create(Collection("characters"),
                Obj("data", Obj("name", "Thor")))
            ));

            var thorsSpellbook = GetRef(await client.Query(
              Create(Collection("spellbooks"),
                Obj("data",
                  Obj("owner", thor)))
            ));

            thorSpell1 = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj("spellbook", thorsSpellbook)))
            ));

            thorSpell2 = GetRef(await client.Query(
              Create(Collection("spells"),
                Obj("data",
                  Obj("spellbook", thorsSpellbook)))
            ));
        }

        [Test]
        public void TestAbort()
        {
            var ex = Assert.ThrowsAsync<BadRequest>(
                async () => await client.Query(Abort("error message"))
            );

            AssertErrors(ex, code: "transaction aborted", description: "error message");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public void TestUnauthorizedOnInvalidSecret()
        {
            var ex = Assert.ThrowsAsync<Unauthorized>(
                async () => await GetClient(secret: "invalid secret").Query(Ref(Collection("spelss"), "1234"))
            );

            AssertErrors(ex, code: "unauthorized", description: "Unauthorized");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public void TestNotFoundWhenInstanceDoesntExists()
        {
            var ex = Assert.ThrowsAsync<NotFound>(
                async () => await client.Query(Get(Ref(Collection("spells"), "1234")))
            );

            AssertErrors(ex, code: "instance not found", description: "Document not found.");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public async Task TestCreateAComplexInstance()
        {
            Value document = await client.Query(
                Create(await RandomCollection(),
                    Obj("data",
                        Obj("testField",
                            Obj(
                                "array", Arr(1, "2", 3.4, Obj("name", "JR")),
                                "bool", true,
                                "num", 1234,
                                "string", "sup",
                                "float", 1.234)
                            ))));

            Value testField = document.Get(DATA).At("testField");
            Assert.AreEqual("sup", testField.At("string").To<string>().Value);
            Assert.AreEqual(1234L, testField.At("num").To<long>().Value);
            Assert.AreEqual(true, testField.At("bool").To<bool>().Value);
            Assert.AreEqual(None(), testField.At("bool").To<string>().ToOption);
            Assert.AreEqual(None(), testField.At("credentials").To<Value>().ToOption);
            Assert.AreEqual(None(), testField.At("credentials", "password").To<string>().ToOption);

            Value array = testField.At("array");
            Assert.AreEqual(4, array.To<Value[]>().Value.Length);
            Assert.AreEqual(1L, array.At(0).To<long>().Value);
            Assert.AreEqual("2", array.At(1).To<string>().Value);
            Assert.AreEqual(3.4, array.At(2).To<double>().Value);
            Assert.AreEqual("JR", array.At(3).At("name").To<string>().Value);
            Assert.AreEqual(None(), array.At(4).To<Value>().ToOption);
        }

        [Test] public async Task TestGetAnInstance()
        {
            Value document = await client.Query(Get(magicMissile));
            Assert.AreEqual("Magic Missile", document.Get(NAME_FIELD));
        }

        [Test] public async Task TestIssueABatchedQueryWithVarargs()
        {
            var result0 = await client.Query(
                Get(thorSpell1),
                Get(thorSpell2)
            );

            Assert.That(ArrayV.Of(result0).Collect(REF_FIELD),
                        Is.EquivalentTo(new List<RefV> { thorSpell1, thorSpell2 }));

            var result1 = await client.Query(Add(1, 2), Subtract(1, 2));

            Assert.That(result1, Is.EquivalentTo(new List<Value> { 3, -1 }));
        }

        [Test] public async Task TestIssueABatchedQueryWithEnumerable()
        {
            var result0 = await client.Query(new List<Expr> {
                Get(thorSpell1),
                Get(thorSpell2)
            });

            Assert.That(ArrayV.Of(result0).Collect(REF_FIELD),
                        Is.EquivalentTo(new List<RefV> { thorSpell1, thorSpell2 }));

            var result1 = await client.Query(new List<Expr> {
                Add(1, 2),
                Subtract(1, 2)
            });

            Assert.That(result1, Is.EquivalentTo(new List<Value> { 3, -1 }));
        }

        [Test] public async Task TestUpdateInstanceData()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("data",
                        Obj(
                            "name", "Magic Missile",
                            "element", "arcane",
                            "cost", 10))));

            Value updatedInstance = await client.Query(
                Update(GetRef(createdInstance),
                    Obj("data",
                        Obj(
                        "name", "Faerie Fire",
                        "cost", Null()))));

            Assert.AreEqual(createdInstance.Get(REF_FIELD), updatedInstance.Get(REF_FIELD));
            Assert.AreEqual("Faerie Fire", updatedInstance.Get(NAME_FIELD));
            Assert.AreEqual("arcane", updatedInstance.Get(ELEMENT_FIELD));
            Assert.AreEqual(None(), updatedInstance.GetOption(COST_FIELD));
        }

        [Test] public async Task TestReplaceAnInstancesData()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("data",
                        Obj(
                            "name", "Magic Missile",
                            "element", "arcane",
                            "cost", 10))));

            Value replacedInstance = await client.Query(
                Replace(createdInstance.Get(REF_FIELD),
                    Obj("data",
                        Obj(
                            "name", "Volcano",
                            "elements", Arr("fire", "earth"),
                            "cost", 10)))
            );

            Assert.AreEqual(createdInstance.Get(REF_FIELD), replacedInstance.Get(REF_FIELD));
            Assert.AreEqual("Volcano", replacedInstance.Get(NAME_FIELD));
            Assert.AreEqual(10L, replacedInstance.Get(COST_FIELD));
            Assert.That(replacedInstance.Get(ELEMENTS_LIST).Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "fire", "earth" }));
        }

        [Test] public async Task TestDeleteAnInstance()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                Obj("data", Obj("name", "Magic Missile"))));

            Value @ref = createdInstance.Get(REF_FIELD);
            await client.Query(Delete(@ref));

            Value exists = await client.Query(Exists(@ref));
            Assert.AreEqual(false, exists.To<bool>().Value);

            var ex = Assert.ThrowsAsync<NotFound>(async() => await client.Query(Get(@ref)));

            AssertErrors(ex, code: "instance not found", description: "Document not found.");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test] public async Task TestInsertAndRemoveEvents()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("data", Obj("name", "Magic Missile"))));

            Value insertedEvent = await client.Query(
                Insert(createdInstance.Get(REF_FIELD), 1L, ActionType.Create,
                    Obj("data", Obj("cooldown", 5L))));

            Assert.AreEqual(insertedEvent.Get(DOCUMENT_FIELD), createdInstance.Get(REF_FIELD));

            Value removedEvent = await client.Query(
                Remove(createdInstance.Get(REF_FIELD), 2L, ActionType.Delete)
            );

            Assert.AreEqual(Null(), removedEvent);
        }

        class Event
        {
            string action;
            RefV document;

            [FaunaConstructor]
            public Event(string action, RefV document)
            {
                this.action = action;
                this.document = document;
            }

            public override string ToString() => $"Event({action}, {document})";
            public override int GetHashCode() => 0;

            public override bool Equals(object obj)
            {
                var other = obj as Event;
                return other != null && action == other.action && document == other.document;
            }
        }

        [Test] public async Task TestEvents()
        {
            var createdInstance = (await client.Query(
                Create(await RandomCollection(), Obj("data", Obj("x", 1)))
            )).Get(REF_FIELD);

            await client.Query(Update(createdInstance, Obj("data", Obj("x", 2))));
            await client.Query(Delete(createdInstance));

            var events = (
                await client.Query(Paginate(Events(createdInstance)))
            ).Get(DATA.To<List<Event>>());

            Assert.AreEqual(3, events.Count);

            Assert.That(events, Is.EquivalentTo(new List<Event> {
                new Event("create", createdInstance),
                new Event("update", createdInstance),
                new Event("delete", createdInstance)
            }));
        }

        [Test] public async Task TestSingleton()
        {
            var createdInstance = (await client.Query(
                Create(await RandomCollection(), Obj("data", Obj("x", 1)))
            )).Get(REF_FIELD);

            await client.Query(Update(createdInstance, Obj("data", Obj("x", 2))));
            await client.Query(Delete(createdInstance));

            var events = (
                await client.Query(Paginate(Events(Singleton(createdInstance))))
            ).Get(DATA.To<List<Event>>());

            Assert.AreEqual(2, events.Count);

            Assert.That(events, Is.EquivalentTo(new List<Event> {
                new Event("add", createdInstance),
                new Event("remove", createdInstance),
            }));
        }

        [Test] public async Task TestHandleConstraintViolations()
        {
            RefV classRef = await RandomCollection();

            await client.Query(
                CreateIndex(Obj(
                    "name", RandomStartingWith("class_index_"),
                    "active", true,
                    "source", classRef,
                    "terms", Arr(Obj("field", Arr("data", "uniqueField"))),
                    "unique", true)));

            AsyncTestDelegate create = async () =>
            {
                await client.Query(
                    Create(classRef,
                        Obj("data", Obj("uniqueField", "same value"))));
            };

            Assert.DoesNotThrowAsync(create);

            var ex = Assert.ThrowsAsync<BadRequest>(create);

            Assert.AreEqual("instance not unique: document is not unique.", ex.Message);

            AssertErrors(ex, code: "instance not unique", description: "document is not unique.");

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { "create" }));
        }

        [Test] public async Task TestFindASingleInstanceFromIndex()
        {
            Value singleMatch = await client.Query(
                Paginate(Match(Index("spells_by_element"), "fire")));

            Assert.That(singleMatch.Get(REF_LIST), Is.EquivalentTo(new List<RefV> { fireball }));
        }

        [Test] public async Task TestListAllItensOnAClassIndex()
        {
            Value allInstances = await client.Query(
                Paginate(Match(Index("all_spells"))));

            Assert.That(allInstances.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { magicMissile, fireball, faerieFire, summon, thorSpell1, thorSpell2 }));
        }

        [Test] public async Task TestPaginateOverAnIndex()
        {
            Value page1 = await client.Query(
                Paginate(Match(Index("all_spells")), size: 3));

            Assert.AreEqual(3, page1.Get(DATA).To<Value[]>().Value.Length);
            Assert.NotNull(page1.At("after"));
            Assert.AreEqual(None(), page1.At("before").To<Value>().ToOption);

            Value page2 = await client.Query(
              Paginate(Match(Index("all_spells")), after: page1.At("after"), size: 3));

            Assert.AreEqual(3, page2.Get(DATA).To<Value[]>().Value.Length);
            Assert.AreNotEqual(page1.At("data"), page2.Get(DATA));
            Assert.NotNull(page2.At("before"));
            Assert.AreEqual(None(), page2.At("after").To<Value>().ToOption);           
        }

        [Test] public async Task TestPaginateWithCursor()
        {
            string idxName = RandomStartingWith("foo_idx_");
            string collName = RandomStartingWith("foo_coll_");

            await NewCollectionWithValues(collName, idxName, indexWithAllValues: true);
            
            Expr matcher = Match(Index(idxName));

            Func<Value, Value[]> getData = value => value.Get(DATA).To<Value[]>().Value;

            Func<Cursor, Task<Value[]>> paginateCursor = async cursor =>
                getData(await client.Query(Paginate(matcher, cursor: cursor)));

            Value firstPage = await client.Query(Paginate(matcher, size: 7));

            Assert.AreEqual(
                ArrayV.Of(1, 2, 3, 4, 5, 6, 7),
                getData(firstPage));

            Assert.AreEqual(
                ArrayV.Of(8, 9, 10),
                await paginateCursor(After(firstPage.At("after"))));
            
            Assert.AreEqual(
                ArrayV.Of(1, 2),
                await paginateCursor(Before(3)));

            Assert.AreEqual(
                ArrayV.Of(1, 2),
                await paginateCursor(RawCursor(Obj("before", Arr(3)))));

            Assert.AreEqual(
                ArrayV.Of(8, 9, 10),
                await paginateCursor(RawCursor(Obj("after", 8))));

            // complex RawCursor

            Expr matcherValues = Match(Index($"{idxName}_values"));
            Value afterValue = (await client.Query(Paginate(matcherValues, size: 7))).At("after");
            Value[] afterValueArr = afterValue.To<Value[]>().Value;

            Cursor rawCursor1 = RawCursor(Obj("after", Arr(afterValueArr[0], afterValueArr[1], afterValueArr[2], afterValueArr[3])));
            Cursor rawCursor2 = RawCursor(Obj("after", afterValue));

            Value[] lastPageValues1 = getData(await client.Query(Paginate(matcherValues, cursor: rawCursor1)));
            Value[] lastPageValues2 = getData(await client.Query(Paginate(matcherValues, cursor: rawCursor2)));

            Assert.AreEqual(3, lastPageValues1.Length);
            Assert.AreEqual(3, lastPageValues2.Length);
            Assert.AreEqual(lastPageValues1, lastPageValues2);
        }

        [Test] public async Task TestDealWithSetRef()
        {
            Value res = await client.Query(
                Match(Index("spells_by_element"), "arcane"));

            IReadOnlyDictionary<string, Value> set = res.To<SetRefV>().Value.Value;
            Assert.AreEqual("arcane", set["terms"].To<string>().Value);
            Assert.AreEqual(new RefV(id: "spells_by_element", collection: Native.INDEXES), set["match"].To<RefV>().Value);
        }

        [Test] public async Task TestEvalAtExpression()
        {
            var summonData = await client.Query(Get(summon));

            var beforeSummon = summonData.Get(TS_FIELD) - 1;

            var spells = await client.Query(
                At(beforeSummon, Paginate(Match(Index("all_spells")))));

            Assert.That(spells.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { magicMissile, fireball, faerieFire }));
        }

        [Test] public async Task TestEvalLetExpression()
        {
            Value res = await client.Query(
                Let("x", 1, "y", 2).In(Arr(Var("y"), Var("x")))
            );

            Assert.That(res.Collect(Field.To<long>()), Is.EquivalentTo(new List<long> { 2L, 1L }));
        }

        [Test] public async Task TestEvalIfExpression()
        {
            Value res = await client.Query(
                If(true, "was true", "was false")
            );

            Assert.AreEqual("was true", res.To<string>().Value);
        }

        [Test] public async Task TestEvalDoExpression()
        {
            RefV @ref = await RandomCollection();

            Value res = await client.Query(
                Do(Create(@ref, Obj("data", Obj("name", "Magic Missile"))),
                    Get(@ref))
            );

            Assert.AreEqual(@ref, res.Get(REF_FIELD));
        }

        [Test] public async Task TestEchoAnObjectBack()
        {
            Value res = await client.Query(Obj("name", "Hen Wen", "age", 123));
            Assert.AreEqual("Hen Wen", res.At("name").To<string>().Value);
            Assert.AreEqual(123L, res.At("age").To<long>().Value);

            res = await client.Query(res);
            Assert.AreEqual("Hen Wen", res.At("name").To<string>().Value);
            Assert.AreEqual(123L, res.At("age").To<long>().Value);
        }

        [Test] public async Task TestMapOverCollections()
        {
            Value res = await client.Query(
                Map(Arr(1, 2, 3),
                    Lambda("i", Add(Var("i"), 1)))
            );

            Assert.That(res.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L, 3L, 4L }));

            //////////////////

            res = await client.Query(
                Map(Arr(1, 2, 3),
                    Lambda(i => Add(i, 1)))
            );

            Assert.That(res.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L, 3L, 4L }));

            //////////////////

            res = await client.Query(
                Map(Arr(1, 2, 3),
                    i => Add(i, 1))
            );

            Assert.That(res.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L, 3L, 4L }));
        }

        [Test] public async Task TestExecuteForeachExpression()
        {
            var clazz = await RandomCollection();

            Value res = await client.Query(
                Foreach(Arr("Fireball Level 1", "Fireball Level 2"),
                    Lambda("spell", Create(clazz, Obj("data", Obj("name", Var("spell"))))))
            );

            Assert.That(res.Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "Fireball Level 1", "Fireball Level 2" }));

            //////////////////

            res = await client.Query(
                Foreach(Arr("Fireball Level 1", "Fireball Level 2"),
                        Lambda(spell => Create(clazz, Obj("data", Obj("name", spell)))))
            );

            Assert.That(res.Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "Fireball Level 1", "Fireball Level 2" }));

            //////////////////

            res = await client.Query(
                Foreach(Arr("Fireball Level 1", "Fireball Level 2"),
                        spell => Create(clazz, Obj("data", Obj("name", spell))))
            );

            Assert.That(res.Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "Fireball Level 1", "Fireball Level 2" }));
        }

        [Test] public async Task TestFilterACollection()
        {
            Value filtered = await client.Query(
                Filter(Arr(1, 2, 3),
                    Lambda("i", EqualsFn(0, Modulo(Var("i"), 2))))
            );

            Assert.That(filtered.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L }));

            //////////////////

            filtered = await client.Query(
                Filter(Arr(1, 2, 3),
                    Lambda(i => EqualsFn(0, Modulo(i, 2))))
            );

            Assert.That(filtered.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L }));

            //////////////////

            filtered = await client.Query(
                Filter(Arr(1, 2, 3),
                    i => EqualsFn(0, Modulo(i, 2)))
            );

            Assert.That(filtered.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 2L }));

        }

        [Test] public async Task TestTakeElementsFromCollection()
        {
            Value taken = await client.Query(Take(2, Arr(1, 2, 3)));
            Assert.That(taken.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 1L, 2L }));
        }

        [Test] public async Task TestDropElementsFromCollection()
        {
            Value dropped = await client.Query(Drop(2, Arr(1, 2, 3)));
            Assert.That(dropped.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 3L }));
        }

        [Test] public async Task TestPrependElementsInACollection()
        {
            Value prepended = await client.Query(
                Prepend(Arr(1, 2), Arr(3, 4))
            );

            Assert.That(prepended.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 1L, 2L, 3L, 4L }));
        }

        [Test] public async Task TestAppendElementsInACollection()
        {
            Value appended = await client.Query(
                Append(Arr(3, 4), Arr(1, 2))
            );

            Assert.That(appended.Collect(Field.To<long>()),
                        Is.EquivalentTo(new List<long> { 1L, 2L, 3L, 4L }));
        }

        [Test] public async Task TestIsEmpty()
        {
            Assert.True((await client.Query(IsEmpty(Arr()))).To<bool>().Value);
            Assert.False((await client.Query(IsEmpty(Arr(1, 2, 3)))).To<bool>().Value);

            Assert.True((await client.Query(IsEmpty(Paginate(Match(Index("spells_by_element"), "iron"))))).To<bool>().Value);
            Assert.False((await client.Query(IsEmpty(Paginate(Match(Index("spells_by_element"), "fire"))))).To<bool>().Value);
        }

        [Test] public async Task TestIsNonEmpty()
        {
            Assert.False((await client.Query(IsNonEmpty(Arr()))).To<bool>().Value);
            Assert.True((await client.Query(IsNonEmpty(Arr(1, 2, 3)))).To<bool>().Value);

            Assert.False((await client.Query(IsNonEmpty(Paginate(Match(Index("spells_by_element"), "iron"))))).To<bool>().Value);
            Assert.True((await client.Query(IsNonEmpty(Paginate(Match(Index("spells_by_element"), "fire"))))).To<bool>().Value);
        }

        [Test] public async Task TestReadEventsFromIndex()
        {
            Value events = await client.Query(
                Paginate(Match(Index("spells_by_element"), "arcane"), events: true)
            );

            Assert.That(events.Get(DATA).Collect(DOCUMENT_FIELD),
                        Is.EquivalentTo(new List<RefV> { magicMissile, faerieFire }));
        }

        [Test] public async Task TestPaginateUnion()
        {
            Value union = await client.Query(
                Paginate(
                    Union(
                        Match(Index("spells_by_element"), "arcane"),
                        Match(Index("spells_by_element"), "fire"))
                )
            );

            Assert.That(union.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { magicMissile, fireball, faerieFire }));
        }

        [Test] public async Task TestPaginateIntersection()
        {
            Value intersection = await client.Query(
                Paginate(
                    Intersection(
                        Match(Index("spells_by_element"), "arcane"),
                        Match(Index("spells_by_element"), "nature")
                    )
                )
            );

            Assert.That(intersection.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { faerieFire }));
        }

        [Test] public async Task TestPaginateDifference()
        {
            Value difference = await client.Query(
                Paginate(
                    Difference(
                        Match(Index("spells_by_element"), "nature"),
                        Match(Index("spells_by_element"), "arcane")
                    )
                )
            );

            Assert.That(difference.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { summon }));
        }

        [Test] public async Task TestPaginateDistinctSets()
        {
            Value distinct = await client.Query(
                Paginate(Distinct(Match(Index("elements_of_spells"))))
            );

            Assert.That(distinct.Get(DATA).Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "arcane", "fire", "nature" }));
        }

        [Test] public async Task TestPaginateJoin()
        {
            Value join = await client.Query(
                Paginate(
                    Join(
                        Match(Index("spellbooks_by_owner"), thor),
                        Lambda(spellbook => Match(Index("spells_by_spellbook"), spellbook))
                    )
                )
            );

            Assert.That(join.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { thorSpell1, thorSpell2 }));
        }

        [Test] public async Task TestEvalEqualsExpression()
        {
            Value equals = await client.Query(EqualsFn("fire", "fire"));
            Assert.AreEqual(true, equals.To<bool>().Value);
        }

        [Test] public async Task TestEvalConcatExpression()
        {
            Value simpleConcat = await client.Query(Concat(Arr("Magic", "Missile")));
            Assert.AreEqual("MagicMissile", simpleConcat.To<string>().Value);

            Value concatWithSeparator = await client.Query(
                Concat(Arr("Magic", "Missile"), " ")
            );

            Assert.AreEqual("Magic Missile", concatWithSeparator.To<string>().Value);
        }

        [Test] public async Task TestEvalCasefoldExpression()
        {
            Assert.AreEqual("hen wen", (await client.Query(Casefold("Hen Wen"))).To<string>().Value);

            // https://unicode.org/reports/tr15/
            Assert.AreEqual("A\u030A", (await client.Query(Casefold("\u212B", Normalizer.NFD))).To<string>().Value);
            Assert.AreEqual("\u00C5", (await client.Query(Casefold("\u212B", Normalizer.NFC))).To<string>().Value);
            Assert.AreEqual("\u0073\u0323\u0307", (await client.Query(Casefold("\u1E9B\u0323", Normalizer.NFKD))).To<string>().Value);
            Assert.AreEqual("\u1E69", (await client.Query(Casefold("\u1E9B\u0323", Normalizer.NFKC))).To<string>().Value);

            Assert.AreEqual("\u00E5", (await client.Query(Casefold("\u212B", Normalizer.NFKCCaseFold))).To<string>().Value);
        }

        [Test] public async Task TestEvalNGramExpression()
        {
            Assert.AreEqual(
                new string[] { "w", "wh", "h", "ha", "a", "at", "t" },
                (await client.Query(NGram("what"))).To<string[]>().Value
            );

            Assert.AreEqual(
                new string[] { "wh", "wha", "ha", "hat", "at" },
                (await client.Query(NGram("what", min: 2, max: 3))).To<string[]>().Value
            );

            Assert.AreEqual(
                new string[] { "j", "jo", "o", "oh", "h", "hn", "n", "d", "do", "o", "oe", "e" },
                (await client.Query(NGram(Arr("john", "doe")))).To<string[]>().Value
            );

            Assert.AreEqual(
                new string[] { "joh", "john", "ohn", "doe" },
                (await client.Query(NGram(Arr("john", "doe"), min: 3, max: 4))).To<string[]>().Value
            );
        }

        [Test] public async Task TestEvalContainsExpressions()
        {
            var foodsObj = Obj("foods",
                Arr(
                    Obj("crunchings", "apple"),
                    Obj("munchings", "peanuts")
                ));

            var favoritesObj = Obj("favorites", foodsObj);

            // Deprecated

            Assert.AreEqual(BooleanV.True,
                await client.Query(Contains(Path("favorites", "foods"), favoritesObj)));

            // Field

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsField(StringV.Of("favorites"), favoritesObj)));
                        
            Assert.AreEqual(
                BooleanV.False,
                await client.Query(ContainsField(StringV.Of("foods"), favoritesObj)));

            // Path

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsPath(Path("favorites"), favoritesObj)));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsPath(Path("favorites", "foods"), favoritesObj)));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsPath(Path(1), Arr("favorites", "foods"))));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(
                    ContainsPath(Path("wrapped", "favorites", "foods"),
                    Obj("wrapped", favoritesObj))));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(
                    ContainsPath(Path("wrapped", "favorites", "foods", 1, "munchings"),
                    Obj("wrapped", favoritesObj))));

            Assert.AreEqual(
                BooleanV.False,
                await client.Query(
                    ContainsPath(Path("wrapped", "favorites", "foods", 2),
                    Obj("wrapped", favoritesObj))));

            // Values

            Assert.AreEqual(
                BooleanV.False,
                await client.Query(ContainsValue("foo", Obj("foo", "bar"))));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsValue("bar", Obj("foo", "bar"))));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsValue("bar", Arr("foo", "bar"))));

            // refs and sets

            RefV aCollection = await RandomCollection();

            var indexName = RandomStartingWith("foo_index_");

            await client.Query(CreateIndex(Obj(
                "name", indexName,
                "source", aCollection,
                "active", true,
                "terms", Arr(Obj("field", Arr("data", "name"))),
                "values", Arr(Obj("field", Arr("data", "color")))
            )));

            RefV newRef = GetRef(await client.Query(Create(
                Ref(aCollection, "122333"),
                Obj("data", Obj("name", "apple", "color", "green"))
             )));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsValue("122333", newRef)));

            Assert.AreEqual(
                BooleanV.True,
                await client.Query(ContainsValue(
                    "green",
                    Match(Index(indexName), "apple"))));
        }

        [Test] public async Task TestEvalSelectExpression()
        {
            Value selected = await client.Query(
                Select(
                    Path("favorites", "foods").At(1),
                    Obj("favorites",
                        Obj("foods", Arr("crunchings", "munchings", "lunchings")))
                )
            );

            Assert.AreEqual("munchings", selected.To<string>().Value);
        }

        [Test] public async Task TestEvalSelectAllExpression()
        {
            var bar = await client.Query(
                SelectAll("foo", Arr(Obj("foo", "bar"), Obj("foo", "baz")))
            );

            Assert.AreEqual(new string[] { "bar", "baz" }, bar.To<string[]>().Value);

            var numbers = await client.Query(
                SelectAll(Arr("foo", 0), Arr(Obj("foo", Arr(0, 1)), Obj("foo", Arr(2, 3))))
            );

            Assert.AreEqual(new int[] { 0, 2 }, numbers.To<int[]>().Value);
        }

        [Test] public async Task TestEvalLTExpression()
        {
            Value res = await client.Query(LT(Arr(1, 2, 3)));
            Assert.AreEqual(true, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalLTEExpression()
        {
            Value res = await client.Query(LTE(Arr(1, 2, 2)));
            Assert.AreEqual(true, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalGTxpression()
        {
            Value res = await client.Query(GT(Arr(3, 2, 1)));
            Assert.AreEqual(true, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalGTExpression()
        {
            Value res = await client.Query(GTE(Arr(3, 2, 2)));
            Assert.AreEqual(true, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalAddExpression()
        {
            Value res = await client.Query(Add(100, 10));
            Assert.AreEqual(110L, res.To<long>().Value);
        }

        [Test] public async Task TestEvalMultiplyExpression()
        {
            Value res = await client.Query(Multiply(100, 10));
            Assert.AreEqual(1000L, res.To<long>().Value);
        }

        [Test] public async Task TestEvalSubtractExpression()
        {
            Value res = await client.Query(Subtract(100, 10));
            Assert.AreEqual(90L, res.To<long>().Value);
        }

        [Test] public async Task TestEvalDivideExpression()
        {
            Value res = await client.Query(Divide(100, 10));
            Assert.AreEqual(10L, res.To<long>().Value);
        }

        [Test] public async Task TestEvalModuloExpression()
        {
            Value res = await client.Query(Modulo(101, 10));
            Assert.AreEqual(1L, res.To<long>().Value);
        }

        [Test] public async Task TestEvalAndExpression()
        {
            Value res = await client.Query(And(true, false));
            Assert.AreEqual(false, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalOrExpression()
        {
            Value res = await client.Query(Or(true, false));
            Assert.AreEqual(true, res.To<bool>().Value);
        }

        [Test] public async Task TestEvalNotExpression()
        {
            Value notR = await client.Query(Not(false));
            Assert.AreEqual(true, notR.To<bool>().Value);
        }

        [Test] public async Task TestEvalToStringExpression()
        {
            Value str = await client.Query(ToStringExpr(42));
            Assert.AreEqual("42", str.To<string>().Value);
        }

        [Test] public async Task TestEvalToNumberExpression()
        {
            Value num = await client.Query(ToNumber("42"));
            Assert.AreEqual(42, num.To<long>().Value);
        }

        [Test] public async Task TestEvalToTimeExpression()
        {
            Value time = await client.Query(ToTime("1970-01-01T00:00:00Z"));
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0), time.To<DateTime>().Value);
        }

        [Test] public async Task TestEvalToDateExpression()
        {
            Value date = await client.Query(ToDate("1970-01-01"));
            Assert.AreEqual(new DateTime(1970, 1, 1), date.To<DateTime>().Value);
        }

        [Test] public async Task TestEvalTimeExpression()
        {
            Value res = await client.Query(Time("1970-01-01T00:00:00-04:00"));
            Assert.AreEqual(new DateTime(1970, 1, 1, 4, 0, 0), res.To<DateTime>().Value);
            Assert.AreEqual(new DateTimeOffset(1970, 1, 1, 4, 0, 0, 0, TimeSpan.Zero), res.To<DateTimeOffset>().Value);
        }

        [Test] public async Task TestEvalEpochExpression()
        {
            Func<long, long> TicksToMicro = ticks => ticks / 10;
            Func<long, long> TicksToNano = ticks => ticks * 100;

            IReadOnlyList<Value> res = await client.Query(
                Epoch(30, "second"),
                Epoch(500, TimeUnit.Millisecond),
                Epoch(TicksToMicro(1000), TimeUnit.Microsecond),
                Epoch(TicksToNano(2), TimeUnit.Nanosecond)
            );

            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 30), res[0].To<DateTime>().Value);
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, 500), res[1].To<DateTime>().Value);
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddTicks(1000), res[2].To<DateTime>().Value);
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddTicks(2), res[3].To<DateTime>().Value);

            Assert.AreEqual(new DateTimeOffset(1970, 1, 1, 0, 0, 30, TimeSpan.Zero), res[0].To<DateTimeOffset>().Value);
            Assert.AreEqual(new DateTimeOffset(1970, 1, 1, 0, 0, 0, 500, TimeSpan.Zero), res[1].To<DateTimeOffset>().Value);
            Assert.AreEqual(new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddTicks(1000), res[2].To<DateTimeOffset>().Value);
            Assert.AreEqual(new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).AddTicks(2), res[3].To<DateTimeOffset>().Value);
        }

        [Test] public async Task TestEvalDateExpression()
        {
            Value res = await client.Query(Date("1970-01-02"));
            Assert.AreEqual(new DateTime(1970, 1, 2), res.To<DateTime>().Value);
            Assert.AreEqual(new DateTimeOffset(1970, 1, 2, 0, 0, 0, TimeSpan.Zero), res.To<DateTimeOffset>().Value);
        }

        [Test] public async Task TestGetNewId()
        {
            Value res = await client.Query(NewId());
            Assert.IsNotNull(res.To<string>().Value);
        }

        [Test] public async Task TestCreateClass()
        {
            await client.Query(CreateCollection(Obj("name", "class_for_test")));

            Assert.AreEqual(BooleanV.True, await client.Query(Exists(Collection("class_for_test"))));
        }

        [Test] public async Task TestCreateDatabase()
        {
            await adminClient.Query(CreateDatabase(Obj("name", "database_for_test")));

            Assert.AreEqual(BooleanV.True, await adminClient.Query(Exists(Database("database_for_test"))));
        }

        [Test] public async Task TestCreateIndex()
        {
            await client.Query(CreateIndex(Obj("name", "index_for_test", "active", true, "source", Collection("characters"))));

            Assert.AreEqual(BooleanV.True, await client.Query(Exists(Index("index_for_test"))));
        }

        [Test] public async Task TestCreateKey()
        {
            await adminClient.Query(CreateDatabase(Obj("name", "database_for_key_test")));

            var key = await adminClient.Query(CreateKey(Obj("database", Database("database_for_key_test"), "role", "server")));

            var newClient = adminClient.NewSessionClient(secret: key.Get(SECRET_FIELD));

            await newClient.Query(CreateCollection(Obj("name", "class_for_key_test")));

            Assert.AreEqual(BooleanV.True, await newClient.Query(Exists(Collection("class_for_key_test"))));
        }

        [Test] public async Task TestAuthenticateSession()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("credentials",
                        Obj("password", "abcdefg")))
            );

            Value auth = await client.Query(
                Login(
                    createdInstance.Get(REF_FIELD),
                    Obj("password", "abcdefg"))
            );

            FaunaClient sessionClient = GetClient(secret: auth.Get(SECRET_FIELD));

            Value loggedOut = await sessionClient.Query(Logout(true));
            Assert.AreEqual(true, loggedOut.To<bool>().Value);

            Value identified = await client.Query(
                Identify(createdInstance.Get(REF_FIELD), "wrong-password")
            );

            Assert.AreEqual(false, identified.To<bool>().Value);
        }

        [Test] public async Task TestHasIdentity()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("credentials",
                        Obj("password", "abcdefg")))
            );

            Value auth = await client.Query(
                Login(
                    createdInstance.Get(REF_FIELD),
                    Obj("password", "abcdefg"))
            );

            FaunaClient sessionClient = GetClient(secret: auth.Get(SECRET_FIELD));

            Assert.AreEqual(
                BooleanV.True,
                await sessionClient.Query(HasIdentity())
            );
        }

        [Test] public async Task TestIdentity()
        {
            Value createdInstance = await client.Query(
                Create(await RandomCollection(),
                    Obj("credentials",
                        Obj("password", "abcdefg")))
            );

            Value auth = await client.Query(
                Login(
                    createdInstance.Get(REF_FIELD),
                    Obj("password", "abcdefg"))
            );

            FaunaClient sessionClient = GetClient(secret: auth.Get(SECRET_FIELD));

            Assert.AreEqual(
                createdInstance.Get(REF_FIELD),
                await sessionClient.Query(Identity())
            );
        }

        [Test] public async Task TestKeyFromSecret()
        {
            var key = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "server")));

            var secret = key.Get(SECRET_FIELD);

            Assert.AreEqual(await rootClient.Query(Get(key.Get(REF_FIELD))),
                            await rootClient.Query(KeyFromSecret(secret)));
        }

        [Test] public async Task TestBytes()
        {
            Value bytes = await client.Query(new BytesV(0x1, 0x2, 0x3));

            Assert.AreEqual(new BytesV(0x1, 0x2, 0x3), bytes);
        }

        class Spell
        {
            [FaunaField("name")]
            public string Name { get; }

            [FaunaField("element")]
            public string Element { get; }

            [FaunaField("cost")]
            public int Cost { get; }

            [FaunaConstructor]
            public Spell(string name, string element, int cost)
            {
                Name = name;
                Element = element;
                Cost = cost;
            }

            public override int GetHashCode() => 0;

            public override bool Equals(object obj)
            {
                var other = obj as Spell;
                return other != null && Name == other.Name && Element == other.Element && Cost == other.Cost;
            }
        }

        [Test]
        public async Task TestUserClass()
        {
            var spellCreated = await client.Query(
                Create(
                    Collection("spells"),
                    Obj("data", Encode(new Spell("Magic Missile", "arcane", 10)))
                )
            );

            var spellField = DATA.To<Spell>();

            Assert.AreEqual(
                new Spell("Magic Missile", "arcane", 10),
                spellCreated.Get(spellField)
            );
        }

        [Test] public async Task TestPing()
        {
            Assert.AreEqual("Scope node is OK", await client.Ping("node"));
        }

        [Test]
        public async Task TestRef()
        {
            Assert.AreEqual(
                new RefV(id: "idx", collection: Native.INDEXES),
                await client.Query(Index("idx"))
            );

            Assert.AreEqual(
                new RefV(id: "cls", collection: Native.COLLECTIONS),
                await client.Query(Collection("cls"))
            );

            Assert.AreEqual(
                new RefV(id: "db", collection: Native.DATABASES),
                await client.Query(Database("db"))
            );

            Assert.AreEqual(
                new RefV(id: "fn", collection: Native.FUNCTIONS),
                await client.Query(Function("fn"))
            );

            Assert.AreEqual(
                new RefV(id: "rl", collection: Native.ROLES),
                await client.Query(Role("rl"))
            );

            Assert.AreEqual(
                new RefV(id: "1", collection: new RefV(id: "spells", collection: Native.COLLECTIONS)),
                await client.Query(Ref(Collection("spells"), "1"))
            );

            Assert.AreEqual(
                new RefV(id: "1", collection: new RefV(id: "spells", collection: Native.COLLECTIONS)),
                await client.Query(Ref("classes/spells/1"))
            );
        }

        [Test] public async Task TestNestedClassRef()
        {
            var parentDb = RandomStartingWith("parent-database-");
            var childDb = RandomStartingWith("child-database-");
            var aClass = RandomStartingWith("a-class-");

            var client1 = await CreateNewDatabase(adminClient, parentDb);
            await CreateNewDatabase(client1, childDb);

            var key = await client1.Query(CreateKey(Obj("database", Database(childDb), "role", "server")));

            var client2 = client1.NewSessionClient(secret: key.Get(SECRET_FIELD));

            await client2.Query(CreateCollection(Obj("name", aClass)));

            var client3 = client2.NewSessionClient(secret: clientKey.Get(SECRET_FIELD));

            Assert.AreEqual(BooleanV.True, await client3.Query(Exists(Collection(aClass, Database(childDb, Database(parentDb))))));

            var ret = await client3.Query(Paginate(Collections(Database(childDb, Database(parentDb)))));

            var nestedClassRef = new RefV(
                id: aClass,
                collection: Native.COLLECTIONS,
                database: new RefV(
                    id: childDb,
                    collection: Native.DATABASES,
                    database: new RefV(
                        id: parentDb,
                        collection: Native.DATABASES)));

            Assert.That(ret.Get(REF_LIST),
                        Is.EquivalentTo(new List<RefV> { nestedClassRef }));
        }

        [Test] public async Task TestNestedKeyRef()
        {
            var parentDb = RandomStartingWith("db-for-keys");
            var childDb = RandomStartingWith("db-test");

            var client = await CreateNewDatabase(adminClient, parentDb);
            await client.Query(CreateDatabase(Obj("name", childDb)));

            var serverKey = await client.Query(CreateKey(Obj(
                "database", Database(childDb),
                "role", "server"
            )));

            var adminKey = await client.Query(CreateKey(Obj(
                "database", Database(childDb),
                "role", "admin"
            )));

            RefV serverKeyRef = serverKey.Get(REF_FIELD);
            RefV adminKeyRef = adminKey.Get(REF_FIELD);

            Assert.That((await client.Query(Paginate(Keys()))).Get(DATA),
                        Is.EquivalentTo(new List<RefV> { serverKeyRef, adminKeyRef }));

            RefV parentDbRef = new RefV(id: parentDb, collection: new RefV("databases"));
            RefV nestedKeyRef = new RefV(id: "keys", database: parentDbRef);

            Assert.That((await adminClient.Query(Paginate(Keys(Database(parentDb))))).Get(DATA),
                        Is.EquivalentTo(new List<RefV> {
                            new RefV(id: serverKeyRef.Id, collection: nestedKeyRef),
                            new RefV(id: adminKeyRef.Id, collection: nestedKeyRef)
                        }));
        }

        [Test]
        public async Task TestMathFunctions()
        {
            Assert.AreEqual(LongV.Of(42), await client.Query(Abs(-42)));
            Assert.AreEqual(DoubleV.Of(12.34), await client.Query(Abs(-12.34)));

            Assert.AreEqual(DoubleV.Of(1.04), await client.Query(Trunc(Acos(0.5), 2)));
            Assert.AreEqual(DoubleV.Of(0.52), await client.Query(Trunc(Asin(0.5), 2)));
            Assert.AreEqual(DoubleV.Of(0.46), await client.Query(Trunc(Atan(0.5), 2)));

            Assert.AreEqual(LongV.Of(3), await client.Query(BitAnd(15, 7, 3)));
            Assert.AreEqual(LongV.Of(-4), await client.Query(BitNot(3)));
            Assert.AreEqual(LongV.Of(15), await client.Query(BitOr(15, 7, 3)));
            Assert.AreEqual(LongV.Of(3), await client.Query(BitXor(2, 1)));

            Assert.AreEqual(DoubleV.Of(2), await client.Query(Ceil(1.01)));

            Assert.AreEqual(DoubleV.Of(0.87), await client.Query(Trunc(Cos(0.5), 2)));
            Assert.AreEqual(DoubleV.Of(3.76), await client.Query(Trunc(Cosh(2), 2)));
            Assert.AreEqual(DoubleV.Of(114.59), await client.Query(Trunc(Degrees(2.0), 2)));

            Assert.AreEqual(DoubleV.Of(7.38), await client.Query(Trunc(Exp(2.0), 2)));

            Assert.AreEqual(DoubleV.Of(1), await client.Query(Floor(1.99)));

            Assert.AreEqual(DoubleV.Of(5), await client.Query(Hypot(3, 4)));

            Assert.AreEqual(DoubleV.Of(0.69), await client.Query(Trunc(Ln(2), 2)));
            Assert.AreEqual(DoubleV.Of(0.3), await client.Query(Trunc(Log(2), 2)));

            Assert.AreEqual(LongV.Of(101), await client.Query(Max(1, 101, 10)));
            Assert.AreEqual(DoubleV.Of(101.1), await client.Query(Max(1.1, 101.1, 10.9)));
            Assert.AreEqual(LongV.Of(1), await client.Query(Min(1, 101, 10)));
            Assert.AreEqual(DoubleV.Of(1.1), await client.Query(Min(1.1, 101.1, 10.9)));

            Assert.AreEqual(DoubleV.Of(8.72), await client.Query(Trunc(Radians(500), 2)));

            Assert.AreEqual(DoubleV.Of(12345.68), await client.Query(Round(12345.6789)));

            Assert.AreEqual(LongV.Of(1), await client.Query(Sign(3)));

            Assert.AreEqual(DoubleV.Of(0.47), await client.Query(Trunc(Sin(0.5), 2)));
            Assert.AreEqual(DoubleV.Of(0.52), await client.Query(Trunc(Sinh(0.5), 2)));

            Assert.AreEqual(DoubleV.Of(3), await client.Query(Sqrt(9)));

            Assert.AreEqual(DoubleV.Of(0.54), await client.Query(Trunc(Tan(0.5), 2)));
            Assert.AreEqual(DoubleV.Of(0.46), await client.Query(Trunc(Tanh(0.5), 2)));

            Assert.AreEqual(DoubleV.Of(3.1415), await client.Query(Trunc(3.14159265359, 4)));
            Assert.AreEqual(DoubleV.Of(3.14), await client.Query(Trunc(3.14159265359)));

        }

        static async Task<FaunaClient> CreateNewDatabase(FaunaClient client, string name)
        {
            await client.Query(CreateDatabase(Obj("name", name)));
            var key = await client.Query(CreateKey(Obj("database", Database(name), "role", "admin")));
            return client.NewSessionClient(secret: key.Get(SECRET_FIELD));
        }

        [Test]
        public async Task TestVersionedQuery()
        {
            var query = Query(Lambda((x, y) => Concat(Arr(x, "/", y))));

            var result = await client.Query(query);

            Assert.IsInstanceOf<QueryV>(result);

            Assert.That(((QueryV)result).Value, Is.EquivalentTo(new Dictionary<string, Expr>
            {
                {"lambda", Arr("x", "y")},
                {"expr", Concat(Arr(Var("x"), "/", Var("y")))},
                { "api_version", "3" }
            }));
        }

        [Test]
        public async Task TestQueryLegacy()
        {
            var query = new QueryV(new Dictionary<string, Expr>
            {
                {"lambda", Arr("x")},
                {"expr", Add(Var("x"), 1)}
            });

            var result = await client.Query(query);

            Assert.IsInstanceOf<QueryV>(result);

            Assert.That(((QueryV)result).Value, Is.EquivalentTo(new Dictionary<string, Expr>
            {
                {"lambda", Arr("x")},
                {"expr", Add(Var("x"), 1)},
                { "api_version", "2.12" }
            }));
        }

        [Test]
        public async Task TestCreateFunction()
        {
            var query = Query(Lambda((x, y) => Concat(Arr(x, "/", y))));

            await client.Query(CreateFunction(Obj("name", "concat_with_slash", "body", query)));

            Assert.AreEqual(BooleanV.True, await client.Query(Exists(Function("concat_with_slash"))));
        }

        [Test]
        public async Task TestCallFunction()
        {
            var query = Query(Lambda((x, y) => Concat(Arr(x, "/", y))));

            await client.Query(CreateFunction(Obj("name", "my_concat", "body", query)));

            var result = await client.Query(Call(Function("my_concat"), "a", "b"));

            Assert.AreEqual(StringV.Of("a/b"), result);
        }

        [Test]
        public async Task TestCreateRole()
        {
            var name = RandomStartingWith("role");

            await adminClient.Query(CreateRole(Obj(
                "name", name,
                "privileges", Arr(Obj(
                    "resource", Databases(),
                    "actions", Obj("read", true)
                ))
            )));

            Assert.AreEqual(BooleanV.True, await adminClient.Query(Exists(Role(name))));
        }

        [Test]
        public async Task TestMergeFunction()
        {
            // simple merge
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20, "z", 30),
                await client.Query(
                    Merge(Obj("x", 10, "y", 20), Obj("z", 30))
                )
            );
    
            // replace field
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20, "z", 30),
                await client.Query(
                    Merge(Obj("x", 10, "y", 20, "z", -1), Obj("z", 30))
                )
            );
    
            // empty obj
            Assert.AreEqual(
                ObjectV.With("foo", 4.2),
                await client.Query(Merge(Obj(), Obj("foo", 4.2)))
            );
    
            // remove field
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20),
                await client.Query(
                    Merge(Obj("x", 10, "y", 20, "z", 1), Obj("z", Null()))
                )
            );
    
            // with expr lambda, replace with the 'left' value
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20, "z", -1),
                await client.Query(
                    Merge(
                        Obj("x", 10, "y", 20, "z", -1),
                        Obj("z", 30),
                        Lambda(Arr("key", "left", "right"), Var("left"))
                    )
                )
            );
    
            // with native lambda, replace with the 'right' value
            Assert.AreEqual(
                ObjectV.With("x", 10, "y", 20, "z", 30),
                await client.Query(
                    Merge(
                        Obj("x", 10, "y", 20, "z", 1),
                        Obj("z", 30),
                        ((k, l, r) => r)
                    )
                )
            );
    
        }

        [Test]
        public async Task TestFormatFunction()
        {
            Assert.AreEqual(
                StringV.Of("FaunaDB rocks"),
                await client.Query(Format("%3$s%1$s %2$s", "DB", "rocks", "Fauna")));

            Assert.AreEqual(
                StringV.Of("Thrilled to see our community grow to 10000 strong"),
                await client.Query(Format("Thrilled to see our community grow to %d strong", 10_000)));
        }

        [Test]
        public async Task TestRangeFunction()
        {
            RefV aCollection = await RandomCollection();

            var indexName = RandomStartingWith("foo_index_");

            Value index = await client.Query(CreateIndex(Obj(
                "name", indexName,
                "source", aCollection,
                "active", true,
                "values", Arr(Obj("field", Arr("data", "value"))))));

            await client.Query(
                Foreach(
                    Arr(1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
                    Lambda(i => Create(aCollection, Obj("data", Obj("value", i))))
                )
            );

            var m = Match(Index(indexName));

            Assert.AreEqual(
                ArrayV.Of(3, 4, 5, 6, 7),
                await SelectData(Range(m, 3, 7)));

            Assert.AreEqual(
                ArrayV.Of(4, 5, 6, 7, 8, 9, 10),
                await SelectData(Union(Range(m, 4, 7), Range(m, 8, 10))));

            Assert.AreEqual(
                ArrayV.Of(1, 2, 3, 10),
                await SelectData(Difference(Range(m, 1, 10), Range(m, 4, 9))));

            Assert.AreEqual(
                ArrayV.Of(4, 5, 6, 7, 8, 9),
                await SelectData(Intersection(Range(m, 1, 10), Range(m, 4, 9))));

            Assert.AreEqual(
                ArrayV.Of(),
                await SelectData(Range(m, 10, 0)));

            Assert.AreEqual(
                ArrayV.Of(),
                await SelectData(Range(m, 11, 10)));
        }

        private async Task<Value> SelectData(Expr set)
        {
            return await client.Query(Select("data", Paginate(set)));
        }

        [Test]
        public async Task TestMoveDatabaseFunction()
        {
            var db1Name = RandomStartingWith("db_source_");
            var db2Name = RandomStartingWith("db_target_");

            var db1Client = await CreateNewDatabase(adminClient, db1Name);
            var db2Client = await CreateNewDatabase(adminClient, db2Name);

            var childDb = await CreateNewDatabase(db1Client, "child");
            await childDb.Query(CreateCollection(Obj("name", "any_collection")));

            Func<Expr, Expr> selectNames = set => Select("data", Map(Paginate(set), x => Select("name", Get(x))));

            Assert.AreEqual(
                ArrayV.Of("child"),
                await db1Client.Query(selectNames(Databases())));

            Assert.AreEqual(
                ArrayV.Empty,
                await db2Client.Query(selectNames(Databases())));

            Assert.AreEqual(
                ArrayV.Of("any_collection"),
                await db1Client.Query(selectNames(Collections(Database("child")))));
            
            await adminClient.Query(MoveDatabase(Database("child", Database(db1Name)), Database(db2Name)));

            Assert.AreEqual(
                ArrayV.Of("child"),
                await db2Client.Query(selectNames(Databases())));

            Assert.AreEqual(
                ArrayV.Empty,
                await db1Client.Query(selectNames(Databases())));

            var key = await db2Client.Query(CreateKey(Obj("database", Database("child"), "role", "server")));
            var childClient = db2Client.NewSessionClient(secret: key.Get(SECRET_FIELD));

            Assert.AreEqual(
                ArrayV.Of("any_collection"),
                await childClient.Query(selectNames(Collections())));
        }

        [Test]
        public async Task TestReduceFunction()
        {
            RefV aCollection = await RandomCollection();
            var indexName = RandomStartingWith("foo_index_");

            Value index = await client.Query(CreateIndex(Obj(
                "name", indexName,
                "source", aCollection,
                "active", true,
                "values", Arr(
                    Obj("field", Arr("data", "value")),
                    Obj("field", Arr("data", "foo"))
            ))));

            Expr[] values = Enumerable.Range(1, 100).Select(i => LongV.Of(i)).ToArray();

            await adminClient.Query(
                Foreach(
                    Arr(values),
                    i => Create(aCollection, Obj("data", Obj("value", i, "foo", "bar")))
                )
            );

            // arrays
            Assert.AreEqual(
                LongV.Of(5060),
                await adminClient.Query(
                    Reduce(
                        Lambda((acc, i) => Add(acc, i)),
                        10,
                        Arr(values)
                    )
                )
            );

            // set
            Assert.AreEqual(
                LongV.Of(5060),
                 await adminClient.Query(
                    Reduce(
                        (acc, i) => Add(acc, Select(0, i)),
                        10,
                        Match(Index(indexName))
                    )
                )
            );

            // page
            Assert.AreEqual(
                ArrayV.Of(5060),
                (await adminClient.Query(
                    Reduce(
                        Lambda((acc, i) => Add(acc, Select(0, i))),
                        10,
                        Paginate(Match(Index(indexName)), size: 100)
                    )
               )).Get(DATA));
        }

        [Test]
        public async Task TestRegexStringFunctions()
        {
            Assert.AreEqual(
                StringV.Of("bye world"),
                await client.Query(ReplaceStrRegex("hello world", "hello", "bye")));

            Assert.AreEqual(
                StringV.Of("One Dog Two Dog"),
                await client.Query(ReplaceStrRegex("One FIsh Two fish", "[Ff][Ii]sh", "Dog")));

            Assert.AreEqual(
                StringV.Of("One Dog Two fish"),
                await client.Query(ReplaceStrRegex("One FIsh Two fish", "[Ff][Ii]sh", "Dog", true)));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 6, "end", 10, "data", "Fauna"),
                    ObjectV.With("start", 19, "end", 23, "data", "fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna")));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 6, "end", 10, "data", "Fauna"),
                    ObjectV.With("start", 19, "end", 23, "data", "fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna", 6)));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 19, "end", 23, "data", "fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna", 7)));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 6, "end", 10, "data", "Fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna", 0, 1)));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 6, "end", 10, "data", "Fauna"),
                    ObjectV.With("start", 19, "end", 23, "data", "fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna", 0, 2)));

            Assert.AreEqual(
                ArrayV.Of(
                    ObjectV.With("start", 19, "end", 23, "data", "fauna")
                ),
                await client.Query(FindStrRegex("heLLo FaunaDB from fauna", "[fF]auna", 7, 2)));
        }

        [Test]
        public async Task TestStringFunctions()
        {
            Assert.AreEqual(LongV.Of(6), await client.Query(FindStr("heLLo world", "world")));
            Assert.AreEqual(LongV.Of(11), await client.Query(Length("heLLo world")));
            Assert.AreEqual(StringV.Of("hello world"), await client.Query(LowerCase("hEllO wORLd")));
            Assert.AreEqual(StringV.Of("hello world"), await client.Query(LTrim("   hello world")));
            Assert.AreEqual(StringV.Of("bye bye "), await client.Query(Repeat("bye ")));
            Assert.AreEqual(StringV.Of("bye bye bye "), await client.Query(Repeat("bye ", 3)));
            Assert.AreEqual(StringV.Of("bye world"), await client.Query(ReplaceStr("hello world", "hello", "bye")));
            Assert.AreEqual(StringV.Of("hello world"), await client.Query(RTrim("hello world    ")));
            Assert.AreEqual(StringV.Of("    "), await client.Query(Space(4)));
            Assert.AreEqual(StringV.Of("world"), await client.Query(SubString("heLLo world", 6)));
            Assert.AreEqual(StringV.Of("hello world"), await client.Query(Trim("    hello world    ")));
            Assert.AreEqual(StringV.Of("Hello World"), await client.Query(TitleCase("heLLo worlD")));
            Assert.AreEqual(StringV.Of("HELLO WORLD"), await client.Query(UpperCase("hello world")));

            Assert.AreEqual(BooleanV.Of(true), await client.Query(StartsWith("ABCDEF", "ABC")));
            Assert.AreEqual(BooleanV.Of(false), await client.Query(StartsWith("ABCDEF", "DEF")));

            Assert.AreEqual(BooleanV.Of(false), await client.Query(EndsWith("ABCDEF", "ABC")));
            Assert.AreEqual(BooleanV.Of(true), await client.Query(EndsWith("ABCDEF", "DEF")));

            Assert.AreEqual(BooleanV.Of(true), await client.Query(ContainsStr("ABCDEF", "ABC")));
            Assert.AreEqual(BooleanV.Of(false), await client.Query(ContainsStr("ABCDEF", "GHI")));

            Assert.AreEqual(BooleanV.Of(true), await client.Query(ContainsStrRegex("ABCDEF", "[A-Z]")));
            Assert.AreEqual(BooleanV.Of(false), await client.Query(ContainsStrRegex("ABCDEF", "[a-z]")));
            Assert.AreEqual(BooleanV.Of(false), await client.Query(ContainsStrRegex("ABCDEF", "[0-9]")));

            Assert.AreEqual(StringV.Of("\\QABCDEF\\E"), await client.Query(RegexEscape("ABCDEF")));
        }

        [Test]
        public async Task TestNowFunction()
        {
            Assert.IsTrue((await client.Query(EqualsFn(Now(), Time("now")))).To<Boolean>().Value);

            Value t1 = await client.Query(Now());
            Value t2 = await client.Query(Now());

            Assert.IsTrue((await client.Query(LTE(t1, t2, Now()))).To<Boolean>().Value);
        }

        [Test]
        public async Task TestCountFunction()
        {
            var colName = RandomStartingWith("foo_coll");
            var idxName = RandomStartingWith("foo_idx");
            await NewCollectionWithValues(colName, idxName);

            // array
            Assert.AreEqual(LongV.Of(4), await client.Query(Count(Arr(1, "2", 3.4, Obj("name", "JR")))));

            Expr[] values = Enumerable.Range(1, 100).Select(i => LongV.Of(i)).ToArray();
            Assert.AreEqual(LongV.Of(100), await client.Query(Count(Arr(values))));

            // sets
            var match = Match(Index(idxName));

            Assert.AreEqual(LongV.Of(10), await client.Query(Count(match)));
            Assert.AreEqual(LongV.Of(5), await client.Query(Count(Range(match, 2, 6))));

            // page
            Assert.AreEqual(
                ArrayV.Of(10),
                (await client.Query(Count(Paginate(match)))).Get(DATA));
        }

        [Test]
        public async Task TestSumFunction()
        {
            var colName = RandomStartingWith("foo_coll");
            var idxName = RandomStartingWith("foo_idx");
            await NewCollectionWithValues(colName, idxName);

            // array
            Assert.AreEqual(DoubleV.Of(6.75), await client.Query(Sum(Arr(1, 2, 3.5, 0.25))));

            // sets
            var match = Match(Index(idxName));

            Assert.AreEqual(LongV.Of(55), await client.Query(Sum(match)));
            Assert.AreEqual(LongV.Of(20), await client.Query(Sum(Range(match, 2, 6))));

            // page
            Assert.AreEqual(
                ArrayV.Of(55),
                (await client.Query(Sum(Paginate(match)))).Get(DATA));
        }

        [Test]
        public async Task TestMeanFunction()
        {
            var colName = RandomStartingWith("foo_coll");
            var idxName = RandomStartingWith("foo_idx");
            await NewCollectionWithValues(colName, idxName);

            // array
            Assert.AreEqual(DoubleV.Of(1.6875), await client.Query(Mean(Arr(1, 2, 3.5, 0.25))));
            
            Expr[] values = Enumerable.Range(1, 10).Select(i => LongV.Of(i)).ToArray();
            Assert.AreEqual(DoubleV.Of(5.5), await client.Query(Mean(Arr(values))));

            // sets
            var match = Match(Index(idxName));

            Assert.AreEqual(DoubleV.Of(5.5), await client.Query(Mean(match)));
            Assert.AreEqual(DoubleV.Of(4), await client.Query(Mean(Range(match, 2, 6))));

            // page
            Assert.AreEqual(
                ArrayV.Of(5.5),
                (await client.Query(Mean(Paginate(match)))).Get(DATA));
        }

        [Test]
        public async Task TestDocuments()
        {
            var coll = await RandomCollection();

            await client.Query(
                Foreach(
                    Arr(1, 2, 3, 4, 5, 76, 7, 8, 9, 10),
                    i => Create(coll, Obj("data", Obj("foo", i)))
                ));

            var page = await client.Query(Paginate(Documents(coll)));

            Assert.AreEqual(10, page.Get(DATA).To<Value[]>().Value.Length);
        }

        [Test]
        public async Task TestCreateObjectsWithRawExpressions()
        {
            var collectionConfig = new Dictionary<string, Expr>()
            {
                { "name",  RandomStartingWith("coll_") },
                { "history_days",  2 }
            };

            Value collValue = await client.Query(CreateCollection(collectionConfig));
            
            Assert.AreEqual(2, collValue.Get(Field.At("history_days")).To<long>().Value);

            RefV collRef = GetRef(collValue);

            var indexCfg = new Dictionary<string, Expr>()
            {
                { "name", RandomStartingWith("idx_") },
                { "source", collRef },
                { "active", true },
                { "values", Arr(Obj("field", Arr("data", "foo"))) }
            };

            Value idxValue = await client.Query(CreateIndex((Expr)indexCfg));

            Assert.AreEqual(1, idxValue.Get(Field.At("values")).To<ArrayV>().Value.Length);

            await client.Query(
                Foreach(
                    Arr(1, 2, 3, 4, 5, 76, 7, 8, 9, 10),
                    i => Create(collRef, Obj("data", Obj("foo", i)))
                ));

            var page = await client.Query(Paginate(Match(GetRef(idxValue))));

            Assert.AreEqual(10, page.Get(DATA).To<Value[]>().Value.Length);
        }

        [Test]
        public async Task TestRequestMessageHeaders()
        {
            var customHttp = new HttpClientWrapper();

            var faunaClient0 = new FaunaClient(
                secret: faunaSecret,
                endpoint: faunaEndpoint,
                httpClient: customHttp);

            await faunaClient0.Query(Count(Databases()));

            var headers = customHttp.LastMessage.Headers;
            Assert.AreEqual("csharp", headers.GetValues("X-Fauna-Driver").First());
            Assert.AreEqual("3", headers.GetValues("X-FaunaDB-API-Version").First());
            Assert.IsFalse(headers.Contains("X-Last-Seen-Txn"));

            // the default HttpClient.Timeout is 100 seconds
            Assert.IsTrue(headers.Contains("X-Query-Timeout"));
            Assert.AreEqual("100000", headers.GetValues("X-Query-Timeout").First());

            var faunaClient1 = new FaunaClient(
                secret: faunaSecret,
                endpoint: faunaEndpoint,
                httpClient: customHttp,
                timeout: TimeSpan.FromSeconds(42));

            await faunaClient1.Query(Count(Databases()));
            await faunaClient1.Query(Count(Collections()));

            headers = customHttp.LastMessage.Headers;

            Assert.AreEqual("3", headers.GetValues("X-FaunaDB-API-Version").First());
            Assert.AreEqual("42000", headers.GetValues("X-Query-Timeout").First());
            Assert.IsTrue(long.Parse(headers.GetValues("X-Last-Seen-Txn").First()) > 0);

            // specific query timeout
            await faunaClient1.Query(Count(Collections()), TimeSpan.FromSeconds(10));
            Assert.AreEqual("10000", customHttp.LastMessage.Headers.GetValues("X-Query-Timeout").First());

            await faunaClient1.Query(TimeSpan.FromSeconds(11), Count(Collections()), Count(Databases()));
            Assert.AreEqual("11000", customHttp.LastMessage.Headers.GetValues("X-Query-Timeout").First());

            // unmodified client timeout
            await faunaClient1.Query(Count(Collections()), Count(Databases()));
            Assert.AreEqual("42000", customHttp.LastMessage.Headers.GetValues("X-Query-Timeout").First());

            await faunaClient1.Query(Count(Collections()));
            Assert.AreEqual("42000", customHttp.LastMessage.Headers.GetValues("X-Query-Timeout").First());

            // set timeout on HttpClient

            var customHttp2 = new HttpClientWrapper
            {
                Timeout = TimeSpan.FromSeconds(33)
            };

            var faunaClient2 = new FaunaClient(
                secret: faunaSecret,
                endpoint: faunaEndpoint,
                httpClient: customHttp2);

            await faunaClient2.Query(Now());

            headers = customHttp2.LastMessage.Headers;
            Assert.IsTrue(headers.Contains("X-Query-Timeout"));
            Assert.AreEqual("33000", headers.GetValues("X-Query-Timeout").First());
        }

        [Test]
        public async Task TestCustomHttp()
        {
            var myHttpClient = new HttpClientWrapper();
            myHttpClient.BaseAddress = new Uri("https://www.wrongbaseaddress.com");
            var client = new FaunaClient(secret: faunaSecret, endpoint: faunaEndpoint, httpClient: myHttpClient);

            Assert.IsTrue(
                (await client.Query(Count(Collections(Database(testDbName))))).To<long>().Value >= 1
            );

            Assert.AreEqual("POST", myHttpClient.LastMessage.Method.ToString());
            Assert.AreEqual(faunaEndpoint, myHttpClient.LastMessage.RequestUri.ToString());
        }

        [Test]
        public async Task TestAuthProviders()
        {
            string roleName = RandomStartingWith("role_");
            string accessProviderName = RandomStartingWith("access_prov_");
            string issuerName = RandomStartingWith("issuer_");
            string jwksUri = "https://xxxx.auth0.com/";

            RefV collection = await RandomCollection();

            RefV role = GetRef(await adminClient.Query(CreateRole(Obj(
                    "name", roleName,
                    "privileges", Arr(Obj(
                        "resource", collection,
                        "actions", Obj("read", true)
                    ))
            ))));

            Value accessProvider = await adminClient.Query(CreateAccessProvider(
                Obj(
                    "name", accessProviderName,
                    "issuer", issuerName,
                    "jwks_uri", jwksUri,
                    "allowed_collections", Arr(collection),
                    "allowed_roles", Arr(role)
                )));

            Assert.AreEqual(
                accessProviderName,
                accessProvider.Get(Field.At("name")).To<string>().Value);

            Assert.AreEqual(
                issuerName,
                accessProvider.Get(Field.At("issuer")).To<string>().Value);

            Assert.AreEqual(
                "https://xxxx.auth0.com/",
                accessProvider.Get(Field.At("jwks_uri")).To<string>().Value);

            Assert.AreEqual(
                collection,
                accessProvider.Get(Field.At("allowed_collections")).To<ArrayV>().Value.First());

            Assert.AreEqual(
                role,
                accessProvider.Get(Field.At("allowed_roles")).To<ArrayV>().Value.First());

            // Retrieving

            Value accessProviderFromDB =
                await adminClient.Query(Get(AccessProvider(accessProviderName)));

            Assert.AreEqual(accessProvider, accessProviderFromDB);
            Assert.AreEqual(
                jwksUri,
                accessProviderFromDB.Get(Field.At("jwks_uri")).To<string>().Value);

            // Retrieving: Denied

            var ex = Assert.ThrowsAsync<PermissionDenied>(
              async () => await client.Query(Get(AccessProvider(accessProviderName)))
            );

            AssertErrors(ex, code: "permission denied", description: "Insufficient privileges to perform the action.");
            AssertEmptyFailures(ex);
            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));

            // Paginating

            var otherName = RandomStartingWith("ap_");

            await adminClient.Query(CreateAccessProvider(
                Obj(
                    "name", otherName,
                    "issuer", RandomStartingWith("ap_"),
                    "jwks_uri", jwksUri
                )));

            Value page = await adminClient.Query(Paginate(AccessProviders()));
            RefV[] pageData = page.Get(DATA).To<RefV[]>().Value;

            Assert.AreEqual(2, pageData.Length);
            Assert.AreEqual(accessProviderName, pageData.First().Id);
            Assert.AreEqual(otherName, pageData.Last().Id);
        }

        private async Task<Value> NewCollectionWithValues(string colName, string indexName, int size = 10, bool indexWithAllValues = false)
        {
            RefV aCollection = (await client.Query(CreateCollection(Obj("name", colName))))
                                .Get(REF_FIELD);

            Expr[] values = Enumerable.Range(1, size).Select(i => LongV.Of(i)).ToArray();

            Value index = await client.Query(CreateIndex(Obj(
                "name", indexName,
                "source", aCollection,
                "active", true,
                "values", Arr(Obj("field", Arr("data", "value"))))));

            if (indexWithAllValues)
            {
                await client.Query(CreateIndex(Obj(
                    "name", $"{indexName}_values",
                    "source", aCollection,
                    "active", true,
                    "values", Arr(
                        Obj("field", Arr("data", "value")),
                        Obj("field", "ref"),
                        Obj("field", "ts")
                ))));
            }
                
            return await client.Query(
                Foreach(
                    Arr(values),
                    Lambda(i => Create(aCollection, Obj("data", Obj("value", i))))
                )
            );
        }

        private async Task<RefV> RandomCollection()
        {
            Value coll = await client.Query(
              CreateCollection(
                Obj("name", RandomStartingWith("some_coll_")))
            );

            return GetRef(coll);
        }

        private string RandomStartingWith(params string[] strs)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var str in strs)
                builder.Append(str);

            builder.Append(new Random().Next(0, int.MaxValue));

            return builder.ToString();
        }

        static void AssertErrors(FaunaException ex, string code, string description)
        {
            Assert.That(ex.Errors, Has.Count.EqualTo(1));
            Assert.AreEqual(code, ex.Errors[0].Code);
            Assert.AreEqual(description, ex.Errors[0].Description);
        }

        static void AssertFailures(FaunaException ex, string code, string description, IResolveConstraint fields)
        {
            Assert.That(ex.Errors[0].Failures, Has.Count.EqualTo(1));
            Assert.AreEqual(code, ex.Errors[0].Failures[0].Code);
            Assert.AreEqual(description, ex.Errors[0].Failures[0].Description);

            Assert.That(ex.Errors[0].Failures[0].Field, fields);
        }

        static void AssertEmptyFailures(FaunaException ex)
        {
            Assert.That(ex.Errors[0].Failures, Is.Empty);
        }

        static void AssertPosition(FaunaException ex, IResolveConstraint positions)
        {
            Assert.That(ex.Errors[0].Position, positions);
        }
    }
}

