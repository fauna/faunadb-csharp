using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FaunaDB.Client;
using FaunaDB.Types;
using FaunaDB.Query;
using static FaunaDB.Query.Language;

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
            await TestClient.Query(Create(Ref("classes"), Obj("name", "my_class")));
        }

        async Task<ObjectV> CreateInstance() =>
            (ObjectV) await TestClient.Query(Create(Ref("classes/my_class"), Obj()));

        [Test] public async Task TestPing()
        {
            Assert.AreEqual("Scope all is OK", await TestClient.Ping("all"));
        }

        [Test] public async Task TestGet()
        {
            ObjectV obj = (ObjectV) await TestClient.Query(Get(Ref("classes")));
            Assert.AreEqual(obj["ref"], Ref("classes/my_class"));
            Assert.AreEqual(obj["name"], (Value) "my_class");
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
    }
}

