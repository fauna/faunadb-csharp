using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;
using FaunaDB.Query;
using System;
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
        private static Field<RefV> INSTANCE_FIELD = Field.At("instance").To<RefV>();
        private static Field<IReadOnlyList<RefV>> REF_LIST = DATA.Collect(Field.To<RefV>());

        private static Field<string> NAME_FIELD = DATA.At(Field.At("name")).To<string>();
        private static Field<string> ELEMENT_FIELD = DATA.At(Field.At("element")).To<string>();
        private static Field<Value> ELEMENTS_LIST = DATA.At(Field.At("elements"));
        private static Field<long> COST_FIELD = DATA.At(Field.At("cost")).To<long>();

        private static Value adminKey;
        private static FaunaClient adminClient;

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
            adminKey = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "admin")));
            adminClient = rootClient.NewSessionClient(adminKey.Get(SECRET_FIELD));

            await client.Query(CreateClass(Obj("name", "spells")));
            await client.Query(CreateClass(Obj("name", "characters")));
            await client.Query(CreateClass(Obj("name", "spellbooks")));

            await client.Query(CreateIndex(Obj(
                "name", "all_spells",
                "source", Class("spells")
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spells_by_element",
                "source", Class("spells"),
                "terms", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "elements_of_spells",
                "source", Class("spells"),
                "values", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spellbooks_by_owner",
                "source", Class("spellbooks"),
                "terms", Arr(Obj("field", Arr("data", "owner")))
              )));

            await client.Query(CreateIndex(Obj(
                "name", "spells_by_spellbook",
                "source", Class("spells"),
                "terms", Arr(Obj("field", Arr("data", "spellbook")))
              )));

            magicMissile = GetRef(await client.Query(
              Create(Class("spells"),
                Obj("data",
                  Obj(
                    "name", "Magic Missile",
                    "element", "arcane",
                    "cost", 10)))
            ));

            fireball = GetRef(await client.Query(
              Create(Class("spells"),
                Obj("data",
                  Obj(
                    "name", "Fireball",
                    "element", "fire",
                    "cost", 10)))
            ));

            faerieFire = GetRef(await client.Query(
              Create(Class("spells"),
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
              Create(Class("spells"),
                Obj("data",
                  Obj(
                    "name", "Summon Animal Companion",
                    "element", "nature",
                    "cost", 10)))
            ));

            thor = GetRef(await client.Query(
              Create(Class("characters"),
                Obj("data", Obj("name", "Thor")))
            ));

            var thorsSpellbook = GetRef(await client.Query(
              Create(Class("spellbooks"),
                Obj("data",
                  Obj("owner", thor)))
            ));

            thorSpell1 = GetRef(await client.Query(
              Create(Class("spells"),
                Obj("data",
                  Obj("spellbook", thorsSpellbook)))
            ));

            thorSpell2 = GetRef(await client.Query(
              Create(Class("spells"),
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
                async () => await GetClient(secret: "invalid secret").Query(Ref(Class("spelss"), "1234"))
            );

            AssertErrors(ex, code: "unauthorized", description: "Unauthorized");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public void TestNotFoundWhenInstanceDoesntExists()
        {
            var ex = Assert.ThrowsAsync<NotFound>(
                async () => await client.Query(Get(Ref(Class("spells"), "1234")))
            );

            AssertErrors(ex, code: "instance not found", description: "Instance not found.");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test]
        public async Task TestCreateAComplexInstance()
        {
            Value instance = await client.Query(
                Create(await RandomClass(),
                    Obj("data",
                        Obj("testField",
                            Obj(
                                "array", Arr(1, "2", 3.4, Obj("name", "JR")),
                                "bool", true,
                                "num", 1234,
                                "string", "sup",
                                "float", 1.234)
                            ))));

            Value testField = instance.Get(DATA).At("testField");
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
            Value instance = await client.Query(Get(magicMissile));
            Assert.AreEqual("Magic Missile", instance.Get(NAME_FIELD));
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
                Create(await RandomClass(),
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
                Create(await RandomClass(),
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
                Create(await RandomClass(),
                Obj("data", Obj("name", "Magic Missile"))));

            Value @ref = createdInstance.Get(REF_FIELD);
            await client.Query(Delete(@ref));

            Value exists = await client.Query(Exists(@ref));
            Assert.AreEqual(false, exists.To<bool>().Value);

            var ex = Assert.ThrowsAsync<NotFound>(async() => await client.Query(Get(@ref)));

            AssertErrors(ex, code: "instance not found", description: "Instance not found.");

            AssertEmptyFailures(ex);

            AssertPosition(ex, positions: Is.EquivalentTo(new List<string> { }));
        }

        [Test] public async Task TestInsertAndRemoveEvents()
        {
            Value createdInstance = await client.Query(
                Create(await RandomClass(),
                    Obj("data", Obj("name", "Magic Missile"))));

            Value insertedEvent = await client.Query(
                Insert(createdInstance.Get(REF_FIELD), 1L, ActionType.Create,
                    Obj("data", Obj("cooldown", 5L))));

            Assert.AreEqual(insertedEvent.Get(INSTANCE_FIELD), createdInstance.Get(REF_FIELD));

            Value removedEvent = await client.Query(
                Remove(createdInstance.Get(REF_FIELD), 2L, ActionType.Delete)
            );

            Assert.AreEqual(Null(), removedEvent);
        }

        class Event
        {
            string action;
            RefV instance;

            [FaunaConstructor]
            public Event(string action, RefV instance)
            {
                this.action = action;
                this.instance = instance;
            }

            public override string ToString() => $"Event({action}, {instance})";
            public override int GetHashCode() => 0;

            public override bool Equals(object obj)
            {
                var other = obj as Event;
                return other != null && action == other.action && instance == other.instance;
            }
        }

        [Test] public async Task TestEvents()
        {
            var createdInstance = (await client.Query(
                Create(await RandomClass(), Obj("data", Obj("x", 1)))
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
                Create(await RandomClass(), Obj("data", Obj("x", 1)))
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
            RefV classRef = await RandomClass();

            await client.Query(
                CreateIndex(Obj(
                    "name", RandomStartingWith("class_index_"),
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

            Assert.AreEqual("instance not unique: Instance is not unique.", ex.Message);

            AssertErrors(ex, code: "instance not unique", description: "Instance is not unique.");

            AssertPosition(ex, positions: Is.Empty);
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

        [Test] public async Task TestDealWithSetRef()
        {
            Value res = await client.Query(
                Match(Index("spells_by_element"), "arcane"));

            IReadOnlyDictionary<string, Value> set = res.To<SetRefV>().Value.Value;
            Assert.AreEqual("arcane", set["terms"].To<string>().Value);
            Assert.AreEqual(new RefV(id: "spells_by_element", @class: Native.INDEXES), set["match"].To<RefV>().Value);
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
            RefV @ref = await RandomClass();

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
            var clazz = await RandomClass();

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

        [Test] public async Task TestReadEventsFromIndex()
        {
            Value events = await client.Query(
                Paginate(Match(Index("spells_by_element"), "arcane"), events: true)
            );

            Assert.That(events.Get(DATA).Collect(Field.At("instance").To<RefV>()),
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

        [Test] public async Task TestEvalContainsExpression()
        {
            Value contains = await client.Query(
                Contains(
                    Path("favorites", "foods"),
                    Obj("favorites",
                        Obj("foods", Arr("crunchings", "munchings"))))
            );

            Assert.AreEqual(BooleanV.True, contains);
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
            await client.Query(CreateClass(Obj("name", "class_for_test")));

            Assert.AreEqual(BooleanV.True, await client.Query(Exists(Class("class_for_test"))));
        }

        [Test] public async Task TestCreateDatabase()
        {
            await adminClient.Query(CreateDatabase(Obj("name", "database_for_test")));

            Assert.AreEqual(BooleanV.True, await adminClient.Query(Exists(Database("database_for_test"))));
        }

        [Test] public async Task TestCreateIndex()
        {
            await client.Query(CreateIndex(Obj("name", "index_for_test", "source", Class("characters"))));

            Assert.AreEqual(BooleanV.True, await client.Query(Exists(Index("index_for_test"))));
        }

        [Test] public async Task TestCreateKey()
        {
            await adminClient.Query(CreateDatabase(Obj("name", "database_for_key_test")));

            var key = await adminClient.Query(CreateKey(Obj("database", Database("database_for_key_test"), "role", "server")));

            var newClient = adminClient.NewSessionClient(secret: key.Get(SECRET_FIELD));

            await newClient.Query(CreateClass(Obj("name", "class_for_key_test")));

            Assert.AreEqual(BooleanV.True, await newClient.Query(Exists(Class("class_for_key_test"))));
        }

        [Test] public async Task TestAuthenticateSession()
        {
            Value createdInstance = await client.Query(
                Create(await RandomClass(),
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
                Create(await RandomClass(),
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
                Create(await RandomClass(),
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
                    Class("spells"),
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
                new RefV(id: "idx", @class: Native.INDEXES),
                await client.Query(Index("idx"))
            );

            Assert.AreEqual(
                new RefV(id: "cls", @class: Native.CLASSES),
                await client.Query(Class("cls"))
            );

            Assert.AreEqual(
                new RefV(id: "db", @class: Native.DATABASES),
                await client.Query(Database("db"))
            );

            Assert.AreEqual(
                new RefV(id: "fn", @class: Native.FUNCTIONS),
                await client.Query(Function("fn"))
            );

            Assert.AreEqual(
                new RefV(id: "1", @class: new RefV(id: "spells", @class: Native.CLASSES)),
                await client.Query(Ref(Class("spells"), "1"))
            );

            Assert.AreEqual(
                new RefV(id: "1", @class: new RefV(id: "spells", @class: Native.CLASSES)),
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

            await client2.Query(CreateClass(Obj("name", aClass)));

            var client3 = client2.NewSessionClient(secret: clientKey.Get(SECRET_FIELD));

            Assert.AreEqual(BooleanV.True, await client3.Query(Exists(Class(aClass, Database(childDb, Database(parentDb))))));

            var ret = await client3.Query(Paginate(Classes(Database(childDb, Database(parentDb)))));

            var nestedClassRef = new RefV(
                id: aClass,
                @class: Native.CLASSES,
                database: new RefV(
                    id: childDb,
                    @class: Native.DATABASES,
                    database: new RefV(
                        id: parentDb,
                        @class: Native.DATABASES)));

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

            Assert.That((await client.Query(Paginate(Keys()))).Get(DATA),
                        Is.EquivalentTo(new List<RefV> { serverKey.Get(REF_FIELD), adminKey.Get(REF_FIELD) }));

            Assert.That((await adminClient.Query(Paginate(Keys(Database(parentDb))))).Get(DATA),
                        Is.EquivalentTo(new List<RefV> { serverKey.Get(REF_FIELD), adminKey.Get(REF_FIELD) }));
        }

        static async Task<FaunaClient> CreateNewDatabase(FaunaClient client, string name)
        {
            await client.Query(CreateDatabase(Obj("name", name)));
            var key = await client.Query(CreateKey(Obj("database", Database(name), "role", "admin")));
            return client.NewSessionClient(secret: key.Get(SECRET_FIELD));
        }

        [Test]
        public async Task TestEchoQuery()
        {
            var query = Query(Lambda((x, y) => Concat(Arr(x, "/", y))));

            var result = await client.Query(query);

            Assert.IsInstanceOf<QueryV>(result);

            Assert.That(((QueryV)result).Value, Is.EquivalentTo(new Dictionary<string, Expr>
            {
                {"lambda", Arr("x", "y")},
                {"expr", Concat(Arr(Var("x"), "/", Var("y")))}
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

        private async Task<RefV> RandomClass()
        {
            Value clazz = await client.Query(
              CreateClass(
                Obj("name", RandomStartingWith("some_class_")))
            );

            return GetRef(clazz);
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

