using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Values;

namespace Test
{
    public class TestCase
    {
        string domain;
        string scheme;
        int? port;
        Client rootClient;
        protected Ref DbRef;
        protected Client TestClient;

        string serverKey;

        [TestFixtureSetUp]
        public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            var cfg = await Config.GetConfig();
            domain = cfg.Domain;
            scheme = cfg.Scheme;
            if (domain == null)
                domain = "rest.faunadb.com";
            if (scheme == null)
                scheme = "https";
            port = cfg.Port;

            rootClient = GetClient(user: cfg.User, password: cfg.Password);

            const string dbName = "faunadb-csharp-test";
            DbRef = new Ref("databases", dbName);

            try {
                await rootClient.Delete(DbRef);
            } catch (NotFound) {}

            await rootClient.Post("databases", new ObjectV("name", dbName));

            var key = (ObjectV) await rootClient.Post("keys", new ObjectV("database", DbRef, "role", "server"));
            serverKey = (string) key["secret"];
            TestClient = GetClient();
        }

        public async Task TearDown()
        {
            await rootClient.Delete(DbRef);
        }

        protected Client GetClient(string user = null, string password = null)
        {
            return new Client(domain: domain, scheme: scheme, port: port, user: user, password: password ?? serverKey);
        }

        protected Client MockClient(string responseText, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            //var req = new HttpRequestMessage(new HttpMethod(action.Name()), path) { Content = dataString == null ? null : new StringContent(dataString) };
            var resp = new HttpResponseMessage(statusCode) { Content = new StringContent(responseText) };
            //IClientIO mock = Mock.Of<IClientIO>(_ =>
            //    _.DoRequest(rq) == res);
            // Values other than clientIO don't really matter.
            var mock = new MockClientIO(resp);
            return new Client(domain: domain, scheme: scheme, port: port, clientIO: mock);
        }

        protected static Ref GetRef(Value v)
        {
            return (Ref) ((ObjectV) v)["ref"];
        }

        protected async Task<Value> Q(Value query)
        {
            return await TestClient.Query(query);
        }
    }

    class MockClientIO : IClientIO
    {
        //HttpRequestMessage req;
        HttpResponseMessage resp;

        public MockClientIO(HttpResponseMessage resp)
        {
            //this.req = req;
            this.resp = resp;
        }

        public Task<HttpResponseMessage> DoRequest(HttpRequestMessage req)
        {
            //Assert.AreEqual(req, this.req);
            return Task.FromResult(this.resp);
        }

    }

    // Use a class to make conversion from Json easier.
    class Config {
        public static async Task<Config> GetConfig()
        {
            /*
            TODO: Would be nice to read from environment variables, but they don't work on Mono.
            Func<string, string> env = Environment.GetEnvironmentVariable;
            domain = env("FAUNA_DOMAIN");
            ...
            */
            var configPath = "../../../testConfig.json";
            if (File.Exists(configPath)) {
                string text = await File.OpenText(configPath).ReadToEndAsync();
                return JsonConvert.DeserializeObject<Config>(text);
            } else
                // use Client defaults.
                return new Config();
        }

        public string Domain { get; set; }
        public string Scheme { get; set; }
        public int? Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return string.Format("Config(domain: {0}, scheme: {1}, port: {2}, user: {3}, password: {4})", Domain, Scheme, Port, User, Password);
        }
    }
}
