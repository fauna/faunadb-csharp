using FaunaDB.Client;
using FaunaDB.Collections;
using FaunaDB.Errors;
using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

using static FaunaDB.Query.Language;
using static FaunaDB.Types.Option;

namespace Test
{
    [TestFixture] public class ClientTest : TestCase
    {
        private static Field<Value> DATA = Field.At("data");
        private static Field<Ref> REF_FIELD = Field.At("ref").To(Codec.REF);
        private static Field<ArrayList<Ref>> REF_LIST = DATA.Collect(Field.As(Codec.REF));

        private static Field<string> NAME_FIELD = DATA.At(Field.At("name")).To(Codec.STRING);
        private static Field<string> ELEMENT_FIELD = DATA.At(Field.At("element")).To(Codec.STRING);
        private static Field<Value> ELEMENTS_LIST = DATA.At(Field.At("elements"));
        private static Field<long> COST_FIELD = DATA.At(Field.At("cost")).To(Codec.LONG);

        private static Ref magicMissile;
        private static Ref fireball;
        private static Ref faerieFire;
        private static Ref summon;
        private static Ref thor;
        private static Ref thorSpell1;
        private static Ref thorSpell2;

        [OneTimeSetUp] new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            await client.Query(Create(Ref("classes"), Obj("name", "spells")));
            await client.Query(Create(Ref("classes"), Obj("name", "characters")));
            await client.Query(Create(Ref("classes"), Obj("name", "spellbooks")));

            await client.Query(Create(Ref("indexes"), Obj(
                "name", "all_spells",
                "source", Ref("classes/spells")
              )));

            await client.Query(Create(Ref("indexes"), Obj(
                "name", "spells_by_element",
                "source", Ref("classes/spells"),
                "terms", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(Create(Ref("indexes"), Obj(
                "name", "elements_of_spells",
                "source", Ref("classes/spells"),
                "values", Arr(Obj("field", Arr("data", "element")))
              )));

            await client.Query(Create(Ref("indexes"), Obj(
                "name", "spellbooks_by_owner",
                "source", Ref("classes/spellbooks"),
                "terms", Arr(Obj("field", Arr("data", "owner")))
              )));

            await client.Query(Create(Ref("indexes"), Obj(
                "name", "spells_by_spellbook",
                "source", Ref("classes/spells"),
                "terms", Arr(Obj("field", Arr("data", "spellbook")))
              )));

            magicMissile = GetRef(await client.Query(
              Create(Ref("classes/spells"),
                Obj("data",
                  Obj(
                    "name", "Magic Missile",
                    "element", "arcane",
                    "cost", 10)))
            ));

            fireball = GetRef(await client.Query(
              Create(Ref("classes/spells"),
                Obj("data",
                  Obj(
                    "name", "Fireball",
                    "element", "fire",
                    "cost", 10)))
            ));

            faerieFire = GetRef(await client.Query(
              Create(Ref("classes/spells"),
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
              Create(Ref("classes/spells"),
                Obj("data",
                  Obj(
                    "name", "Summon Animal Companion",
                    "element", "nature",
                    "cost", 10)))
            ));

            thor = GetRef(await client.Query(
              Create(Ref("classes/characters"),
                Obj("data", Obj("name", "Thor")))
            ));

            Ref thorsSpellbook = GetRef(await client.Query(
              Create(Ref("classes/spellbooks"),
                Obj("data",
                  Obj("owner", thor)))
            ));

            thorSpell1 = GetRef(await client.Query(
              Create(Ref("classes/spells"),
                Obj("data",
                  Obj("spellbook", thorsSpellbook)))
            ));

            thorSpell2 = GetRef(await client.Query(
              Create(Ref("classes/spells"),
                Obj("data",
                  Obj("spellbook", thorsSpellbook)))
            ));
        }

        [Test] public void TestUnauthorizedOnInvalidSecret()
        {
            Assert.ThrowsAsync<Unauthorized>(async() =>
                await GetClient(secret: "invalid secret").Query(Ref("classes/spells/1234"))
            );
        }

        [Test] public void TestNotFoundWhenInstanceDoesntExists()
        {
            Assert.ThrowsAsync<NotFound>(async() =>
                await client.Query(Get(Ref("classes/spells/1234")))
            );
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
            Assert.AreEqual(Some("sup"), testField.At("string").To(Codec.STRING).ValueOption);
            Assert.AreEqual(Some(1234L), testField.At("num").To(Codec.LONG).ValueOption);
            Assert.AreEqual(Some(true), testField.At("bool").To(Codec.BOOLEAN).ValueOption);
            Assert.AreEqual(None<string>(), testField.At("bool").To(Codec.STRING).ValueOption);
            Assert.AreEqual(None<Value>(), testField.At("credentials").To(Codec.VALUE).ValueOption);
            Assert.AreEqual(None<string>(), testField.At("credentials", "password").To(Codec.STRING).ValueOption);

            Value array = testField.At("array");
            Assert.AreEqual(4, array.To(Codec.ARRAY).ValueOption.Value.Count);
            Assert.AreEqual(Some(1L), array.At(0).To(Codec.LONG).ValueOption);
            Assert.AreEqual(Some("2"), array.At(1).To(Codec.STRING).ValueOption);
            Assert.AreEqual(Some(3.4), array.At(2).To(Codec.DOUBLE).ValueOption);
            Assert.AreEqual(Some("JR"), array.At(3).At("name").To(Codec.STRING).ValueOption);
            Assert.AreEqual(None<Value>(), array.At(4).To(Codec.VALUE).ValueOption);
        }

        [Test] public async Task TestGetAnInstance()
        {
            Value instance = await client.Query(Get(magicMissile));
            Assert.AreEqual("Magic Missile", instance.Get(NAME_FIELD));
        }

        //todo batch query

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
            Assert.AreEqual(None<long>(), updatedInstance.GetOption(COST_FIELD));
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
            Assert.AreEqual(ImmutableList.Of("fire", "earth"), replacedInstance.Get(ELEMENTS_LIST).Collect(Field.As(Codec.STRING)));
        }

        [Test] public async Task TestDeleteAnInstance()
        {
            Value createdInstance = await client.Query(
                Create(await RandomClass(),
                Obj("data", Obj("name", "Magic Missile"))));

            Value @ref = createdInstance.Get(REF_FIELD);
            await client.Query(Delete(@ref));

            Value exists = await client.Query(Exists(@ref));
            Assert.AreEqual(Some(false), exists.To(Codec.BOOLEAN).ValueOption);

            Assert.ThrowsAsync<NotFound>(async() => await client.Query(Get(@ref)));
        }

        [Test] public async Task TestInsertAndRemoveEvents()
        {
            Value createdInstance = await client.Query(
                Create(await RandomClass(),
                    Obj("data", Obj("name", "Magic Missile"))));

            Value insertedEvent = await client.Query(
                Insert(createdInstance.Get(REF_FIELD), 1L, ActionType.CREATE,
                    Obj("data", Obj("cooldown", 5L))));

            Assert.AreEqual(createdInstance.Get(REF_FIELD), insertedEvent.Get(REF_FIELD));
            Assert.AreEqual(1, insertedEvent.Get(DATA).To(Codec.OBJECT).ValueOption.Value.Count);
            Assert.AreEqual(Some(5L), insertedEvent.Get(DATA).At("cooldown").To(Codec.LONG).ValueOption);

            Value removedEvent = await client.Query(
                Remove(createdInstance.Get(REF_FIELD), 2L, ActionType.DELETE)
            );

            Assert.AreEqual(Null(), removedEvent);
        }

        [Test] public async Task TestHandleConstraintViolations()
        {
            Ref classRef = await RandomClass();

            await client.Query(
                Create(Ref("indexes"),
                    Obj(
                        "name", RandomStartingWith("class_index_"),
                        "source", classRef,
                        "terms", Arr(Obj("field", Arr("data", "uniqueField"))),
                        "unique", true)));

            await client.Query(
                Create(classRef,
                    Obj("data", Obj("uniqueField", "same value"))));

            Assert.ThrowsAsync<BadRequest>(async () =>
            {
                await client.Query(
                    Create(classRef,
                        Obj("data", Obj("uniqueField", "same value"))));
            });
        }

        [Test] public async Task TestFindASingleInstanceFromIndex()
        {
            Value singleMatch = await client.Query(
                Paginate(Match(Ref("indexes/spells_by_element"), "fire")));

            Assert.AreEqual(ImmutableList.Of(fireball), singleMatch.Get(REF_LIST));
        }

        [Test] public async Task TestCountElementsOnAIndex()
        {
            Value count = await client.Query(Count(Match(Ref("indexes/all_spells"))));
            Assert.AreEqual(Some(6L), count.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestListAllItensOnAClassIndex()
        {
            Value allInstances = await client.Query(
                Paginate(Match(Ref("indexes/all_spells"))));

            Assert.AreEqual(
                ImmutableList.Of(magicMissile, fireball, faerieFire, summon, thorSpell1, thorSpell2),
                allInstances.Get(REF_LIST));
        }

        [Test] public async Task TestPaginateOverAnIndex()
        {
            Value page1 = await client.Query(
                Paginate(Match(Ref("indexes/all_spells")), size: 3));

            Assert.AreEqual(3, page1.Get(DATA).To(Codec.ARRAY).ValueOption.Value.Count);
            Assert.NotNull(page1.At("after"));
            Assert.AreEqual(None<Value>(), page1.At("before").To(Codec.VALUE).ValueOption);

            Value page2 = await client.Query(
              Paginate(Match(Ref("indexes/all_spells")), after: page1.At("after"), size: 3));

            Assert.AreEqual(3, page2.Get(DATA).To(Codec.ARRAY).ValueOption.Value.Count);
            Assert.AreNotEqual(page1.At("data"), page2.Get(DATA));
            Assert.NotNull(page2.At("before"));
            Assert.AreEqual(None<Value>(), page2.At("after").To(Codec.VALUE).ValueOption);
        }

        [Test] public async Task TestDealWithSetRef()
        {
            Value res = await client.Query(
                Match(Ref("indexes/spells_by_element"), "arcane"));

            OrderedDictionary<string, Value> set = res.To(Codec.SETREF).ValueOption.Value.Value;
            Assert.AreEqual(Some("arcane"), set["terms"].To(Codec.STRING).ValueOption);
            Assert.AreEqual(Some(new Ref("indexes/spells_by_element")), set["match"].To(Codec.REF).ValueOption);
        }

        [Test] public async Task TestEvalLetExpression()
        {
            Value res = await client.Query(
                Let("x", 1, "y", 2).In(Arr(Var("y"), Var("x")))
            );

            Assert.AreEqual(ImmutableList.Of(2L, 1L),
                res.Collect(Field.As(Codec.LONG)));

            res = await client.Query(
                Let("x", 1, "y", 2).In((x, y) => Arr(y, x))
            );

            Assert.AreEqual(ImmutableList.Of(2L, 1L),
                res.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestEvalIfExpression()
        {
            Value res = await client.Query(
                If(true, "was true", "was false")
            );

            Assert.AreEqual(Some("was true"), res.To(Codec.STRING).ValueOption);
        }

        [Test] public async Task TestEvalDoExpression()
        {
            Ref @ref = new Ref(RandomStartingWith((await RandomClass()).Value, "/"));

            Value res = await client.Query(
                Do(Create(@ref, Obj("data", Obj("name", "Magic Missile"))),
                    Get(@ref))
            );

            Assert.AreEqual(@ref, res.Get(REF_FIELD));
        }

        [Test] public async Task TestEchoAnObjectBack()
        {
            Value res = await client.Query(Obj("name", "Hen Wen", "age", 123));
            Assert.AreEqual(Some("Hen Wen"), res.At("name").To(Codec.STRING).ValueOption);
            Assert.AreEqual(Some(123L), res.At("age").To(Codec.LONG).ValueOption);

            res = await client.Query(res);
            Assert.AreEqual(Some("Hen Wen"), res.At("name").To(Codec.STRING).ValueOption);
            Assert.AreEqual(Some(123L), res.At("age").To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestMapOverCollections()
        {
            Value res = await client.Query(
                Map(Arr(1, 2, 3),
                    Lambda("i", Add(Var("i"), 1))));

            Assert.AreEqual(ImmutableList.Of(2L, 3L, 4L),
                res.Collect(Field.As(Codec.LONG)));

            res = await client.Query(
                Map(Arr(1, 2, 3),
                    Lambda(i => Add(i, 1))));

            Assert.AreEqual(ImmutableList.Of(2L, 3L, 4L),
                res.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestExecuteForeachExpression()
        {
            Value res = await client.Query(
                Foreach(Arr("Fireball Level 1", "Fireball Level 2"),
                    Lambda("spell", Create(await RandomClass(), Obj("data", Obj("name", Var("spell"))))))
            );

            Assert.AreEqual(ImmutableList.Of("Fireball Level 1", "Fireball Level 2"),
                res.Collect(Field.As(Codec.STRING)));

            var clazz = await RandomClass();

            res = await client.Query(
                Foreach(Arr("Fireball Level 1", "Fireball Level 2"),
                    Lambda(spell => Create(clazz, Obj("data", Obj("name", spell)))))
            );

            Assert.AreEqual(ImmutableList.Of("Fireball Level 1", "Fireball Level 2"),
                res.Collect(Field.As(Codec.STRING)));
        }

        [Test] public async Task TestFilterACollection()
        {
            Value filtered = await client.Query(
                Filter(Arr(1, 2, 3),
                    Lambda("i", EqualsFn(0, Modulo(Var("i"), 2))))
            );

            Assert.AreEqual(ImmutableList.Of(2L),
                filtered.Collect(Field.As(Codec.LONG)));

            filtered = await client.Query(
                Filter(Arr(1, 2, 3),
                    Lambda(i => EqualsFn(0, Modulo(i, 2))))
            );

            Assert.AreEqual(ImmutableList.Of(2L),
                filtered.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestTakeElementsFromCollection()
        {
            Value taken = await client.Query(Take(2, Arr(1, 2, 3)));
            Assert.AreEqual(ImmutableList.Of(1L, 2L), taken.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestDropElementsFromCollection()
        {
            Value dropped = await client.Query(Drop(2, Arr(1, 2, 3)));
            Assert.AreEqual(ImmutableList.Of(3L), dropped.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestPrependElementsInACollection()
        {
            Value prepended = await client.Query(
                Prepend(Arr(1, 2), Arr(3, 4))
            );

            Assert.AreEqual(ImmutableList.Of(1L, 2L, 3L, 4L),
                prepended.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestAppendElementsInACollection()
        {
            Value appended = await client.Query(
                Append(Arr(3, 4), Arr(1, 2))
            );

            Assert.AreEqual(ImmutableList.Of(1L, 2L, 3L, 4L),
                appended.Collect(Field.As(Codec.LONG)));
        }

        [Test] public async Task TestReadEventsFromIndex()
        {
            Value events = await client.Query(
                Paginate(Match(Ref("indexes/spells_by_element"), "arcane"), events: true)
            );

            Assert.AreEqual(ImmutableList.Of(magicMissile, faerieFire),
                events.Get(DATA).Collect(Field.At("resource").To(Codec.REF)));
        }

        [Test] public async Task TestPaginateUnion()
        {
            Value union = await client.Query(
                Paginate(
                    Union(
                        Match(Ref("indexes/spells_by_element"), "arcane"),
                        Match(Ref("indexes/spells_by_element"), "fire"))
                )
            );

            Assert.AreEqual(ImmutableList.Of(magicMissile, fireball, faerieFire),
                union.Get(REF_LIST));
        }

        [Test] public async Task TestPaginateIntersection()
        {
            Value intersection = await client.Query(
                Paginate(
                    Intersection(
                        Match(Ref("indexes/spells_by_element"), "arcane"),
                        Match(Ref("indexes/spells_by_element"), "nature")
                    )
                )
            );

            Assert.AreEqual(ImmutableList.Of(faerieFire),
                intersection.Get(REF_LIST));
        }

        [Test] public async Task TestPaginateDifference()
        {
            Value difference = await client.Query(
                Paginate(
                    Difference(
                        Match(Ref("indexes/spells_by_element"), "nature"),
                        Match(Ref("indexes/spells_by_element"), "arcane")
                    )
                )
            );

            Assert.AreEqual(ImmutableList.Of(summon), difference.Get(REF_LIST));
        }

        [Test] public async Task TestPaginateDistinctSets()
        {
            Value distinct = await client.Query(
                Paginate(Distinct(Match(Ref("indexes/elements_of_spells"))))
            );

            Assert.AreEqual(ImmutableList.Of("arcane", "fire", "nature"),
                distinct.Get(DATA).Collect(Field.As(Codec.STRING)));
        }

        [Test] public async Task TestPaginateJoin()
        {
            Value join = await client.Query(
                Paginate(
                    Join(
                        Match(Ref("indexes/spellbooks_by_owner"), thor),
                        Lambda(spellbook => Match(Ref("indexes/spells_by_spellbook"), spellbook))
                    )
                )
            );

            Assert.AreEqual(ImmutableList.Of(thorSpell1, thorSpell2),
                join.Get(REF_LIST));
        }

        [Test] public async Task TestEvalEqualsExpression()
        {
            Value equals = await client.Query(EqualsFn("fire", "fire"));
            Assert.AreEqual(Some(true), equals.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalConcatExpression()
        {
            Value simpleConcat = await client.Query(Concat(Arr("Magic", "Missile")));
            Assert.AreEqual(Some("MagicMissile"), simpleConcat.To(Codec.STRING).ValueOption);

            Value concatWithSeparator = await client.Query(
                Concat(Arr("Magic", "Missile"), " ")
            );

            Assert.AreEqual(Some("Magic Missile"), concatWithSeparator.To(Codec.STRING).ValueOption);
        }

        [Test] public async Task TestEvalCasefoldExpression()
        {
            Value res = await client.Query(CaseFold("Hen Wen"));
            Assert.AreEqual(Some("hen wen"), res.To(Codec.STRING).ValueOption);
        }

        [Test] public async Task TestEvalContainsExpression()
        {
            Value contains = await client.Query(
                Contains(
                    Path("favorites", "foods"),
                    Obj("favorites",
                        Obj("foods", Arr("crunchings", "munchings"))))
            );

            Assert.AreEqual(Some(true), contains.To(Codec.BOOLEAN).ValueOption);
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

            Assert.AreEqual(Some("munchings"), selected.To(Codec.STRING).ValueOption);
        }

        [Test] public async Task TestEvalLTExpression()
        {
            Value res = await client.Query(LT(Arr(1, 2, 3)));
            Assert.AreEqual(Some(true), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalLTEExpression()
        {
            Value res = await client.Query(LTE(Arr(1, 2, 2)));
            Assert.AreEqual(Some(true), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalGTxpression()
        {
            Value res = await client.Query(GT(Arr(3, 2, 1)));
            Assert.AreEqual(Some(true), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalGTExpression()
        {
            Value res = await client.Query(GTE(Arr(3, 2, 2)));
            Assert.AreEqual(Some(true), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalAddExpression()
        {
            Value res = await client.Query(Add(100, 10));
            Assert.AreEqual(Some(110L), res.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestEvalMultiplyExpression()
        {
            Value res = await client.Query(Multiply(100, 10));
            Assert.AreEqual(Some(1000L), res.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestEvalSubtractExpression()
        {
            Value res = await client.Query(Subtract(100, 10));
            Assert.AreEqual(Some(90L), res.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestEvalDivideExpression()
        {
            Value res = await client.Query(Divide(100, 10));
            Assert.AreEqual(Some(10L), res.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestEvalModuloExpression()
        {
            Value res = await client.Query(Modulo(101, 10));
            Assert.AreEqual(Some(1L), res.To(Codec.LONG).ValueOption);
        }

        [Test] public async Task TestEvalAndExpression()
        {
            Value res = await client.Query(And(true, false));
            Assert.AreEqual(Some(false), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalOrExpression()
        {
            Value res = await client.Query(Or(true, false));
            Assert.AreEqual(Some(true), res.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalNotExpression()
        {
            Value notR = await client.Query(Not(false));
            Assert.AreEqual(Some(true), notR.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestEvalTimeExpression()
        {
            Value res = await client.Query(Time("1970-01-01T00:00:00-04:00"));
            Assert.AreEqual(Some(new DateTime(1970, 1, 1, 4, 0, 0)), res.To(Codec.TS).ValueOption);
        }

        [Test] public async Task TestEvalEpochExpression()
        {
            Value res = await client.Query(Epoch(30, "second"));
            Assert.AreEqual(Some(new DateTime(1970, 1, 1, 0, 0, 30)), res.To(Codec.TS).ValueOption);
        }

        [Test] public async Task TestEvalDateExpression()
        {
            Value res = await client.Query(Date("1970-01-02"));
            Assert.AreEqual(Some(new DateTime(1970, 1, 2)), res.To(Codec.DATE).ValueOption);
        }

        [Test] public async Task TestGetNextId()
        {
            Value res = await client.Query(NextId());
            Assert.IsNotNull(res.To(Codec.STRING).ValueOption);
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

            Client sessionClient = GetClient(secret: auth.At("secret").To(Codec.STRING).ValueOption.Value);

            Value loggedOut = await sessionClient.Query(Logout(true));
            Assert.AreEqual(Some(true), loggedOut.To(Codec.BOOLEAN).ValueOption);

            Value identified = await client.Query(
                Identify(createdInstance.Get(REF_FIELD), "wrong-password")
            );

            Assert.AreEqual(Some(false), identified.To(Codec.BOOLEAN).ValueOption);
        }

        [Test] public async Task TestPing()
        {
            Assert.AreEqual("Scope all is OK", await client.Ping("all"));
        }

        private async Task<Ref> RandomClass()
        {
            Value clazz = await client.Query(
              Create(Ref("classes"),
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
    }
}

