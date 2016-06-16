using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FaunaDB.Client;
using FaunaDB.Values;

namespace Test
{
    [TestFixture] public class ClientTest : TestCase
    {
        [OneTimeSetUp] new public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            await TestClient.Post("classes", new ObjectV("name", "my_class"));
        }

        async Task<ObjectV> CreateInstance() =>
            (ObjectV) await TestClient.Post("classes/my_class", ObjectV.Empty);

        [Test] public async Task TestPing()
        {
            Assert.AreEqual("Scope all is OK", await TestClient.Ping("all"));
        }

        [Test] public async Task TestGet()
        {
            Assert.That(((ObjectV) await TestClient.Get("classes"))["data"] is ArrayV);
        }

        [Test] public async Task TestPost()
        {
            var x = await TestClient.Post("classes/my_class", new ObjectV("data", new ObjectV("foo", 1)));
            Assert.AreEqual(((ObjectV) ((ObjectV) x)["data"])["foo"], (Value) 1);
        }

        [Test] public async Task TestPut()
        {
            var instance = await CreateInstance();
            instance = (ObjectV) await TestClient.Put((Ref) instance["ref"], new ObjectV("data", new ObjectV("a", 2)));

            Assert.AreEqual((Value) 2, ((ObjectV) instance["data"])["a"]);

            instance = (ObjectV) await TestClient.Put((Ref) instance["ref"], new ObjectV("data", new ObjectV("b", 3)));
            Assert.AreEqual(new ObjectV("b", 3), instance["data"]);
        }

        [Test] public async Task TestPatch()
        {
            var instance = await CreateInstance();
            instance = (ObjectV) await TestClient.Patch((Ref) instance["ref"], new ObjectV("data", new ObjectV("a", 1)));
            instance = (ObjectV) await TestClient.Patch((Ref) instance["ref"], new ObjectV("data", new ObjectV("b", 2)));
            Assert.AreEqual(new ObjectV("a", 1, "b", 2), instance["data"]);
        }

        [Test] public async Task TestDelete()
        {
            var instance = await CreateInstance();
            var rf = (Ref) instance["ref"];
            await TestClient.Delete(rf);
            await AssertU.Throws<Exception>(() => TestClient.Get(rf));
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
            AssertRead("      \"resource\": \"Scope global is OK\"");
            AssertRead("    }");
            AssertRgx("^  Response \\(OK\\): API processing (\\d+ms|N/A), network latency \\d+ms$");
        }
    }
}

