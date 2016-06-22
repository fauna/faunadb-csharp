using FaunaDB.Client;
using FaunaDB.Collections;
using FaunaDB.Errors;
using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

        [Test] public async Task TestUnauthorizedOnInvalidSecret()
        {
            await AssertU.Throws<Unauthorized>(() =>
                GetClient(password: "invalid secret").Query(Ref("classes/spells/1234"))
            );
        }

        [Test] public async Task TestNotFoundWhenInstanceDoesntExists()
        {
            await AssertU.Throws<NotFound>(() =>
                client.Query(Get(Ref("classes/spells/1234")))
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
            Assert.AreEqual(Some("sup"), testField.At("string").To(Codec.STRING).Get());
            Assert.AreEqual(Some(1234L), testField.At("num").To(Codec.LONG).Get());
            Assert.AreEqual(Some(true), testField.At("bool").To(Codec.BOOLEAN).Get());
            Assert.AreEqual(None<string>(), testField.At("bool").To(Codec.STRING).Get());
            Assert.AreEqual(None<Value>(), testField.At("credentials").To(Codec.VALUE).Get());
            Assert.AreEqual(None<string>(), testField.At("credentials", "password").To(Codec.STRING).Get());

            Value array = testField.At("array");
            Assert.AreEqual(4, array.To(Codec.ARRAY).Get().Value.Count);
            Assert.AreEqual(Some(1L), array.At(0).To(Codec.LONG).Get());
            Assert.AreEqual(Some("2"), array.At(1).To(Codec.STRING).Get());
            Assert.AreEqual(Some(3.4), array.At(2).To(Codec.DOUBLE).Get());
            Assert.AreEqual(Some("JR"), array.At(3).At("name").To(Codec.STRING).Get());
            Assert.AreEqual(None<Value>(), array.At(4).To(Codec.VALUE).Get());
        }

        [Test] public async Task TestGetAnInstance()
        {
            ObjectV instance = GetData(await client.Query(Get(magicMissile)));
            Assert.AreEqual(StringV.Of("Magic Missile"), instance["name"]);
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

            ObjectV updatedInstance = GetData(await client.Query(
                Update(GetRef(createdInstance),
                    Obj("data",
                        Obj(
                        "name", "Faerie Fire",
                        "cost", Null())))));

            Assert.AreEqual(StringV.Of("Faerie Fire"), updatedInstance["name"]);
            Assert.AreEqual(StringV.Of("arcane"), updatedInstance["element"]);
            //Assert.AreEqual(NullV.Instance, updatedInstance["cost"]);
        }

        [Test] public async Task TestPing()
        {
            Assert.AreEqual("Scope all is OK", await client.Ping("all"));
        }

        [Test] public async Task TestLogging()
        {
            string logged = null;
            Action<string> log = str => {
                Assert.AreEqual(null, logged);
                logged = str;
            };
            var client = GetClient();
            client.OnResponse += ClientLogger.Logger(log);
            await client.Ping();

            Func<string> readLine = new StringReader(logged).ReadLine;
            Action<string> AssertRead = str => Assert.AreEqual(str, readLine());
            Action<string> AssertRgx = rgx => Assert.That(new Regex(rgx).IsMatch(readLine()));

            AssertRead("Fauna GET /ping");
            AssertRgx("^  Credentials: ");
            AssertRead("  Response headers:");
            // Skip through headers
            while (true)
            {
                var line = readLine();
                if (!line.StartsWith("    "))
                {
                    Assert.AreEqual(line, "  Response JSON:");
                    break;
                }
            }
            //todo: this should be flush with "response json: "
            AssertRead("    {");
            AssertRead("      \"object\": {");
            AssertRead("        \"resource\": \"Scope global is OK\"");
            AssertRead("      }");
            AssertRead("    }");
            AssertRgx("^  Response \\(OK\\): API processing (\\d+ms|N/A), network latency \\d+ms$");
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

