using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FaunaDB;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Values;
using FaunaDB.Query;
using static FaunaDB.Query.Language;

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
            DbRef = new Ref($"databases/{dbName}");

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

        protected Client GetClient(string user = null, string password = null) =>
            new Client(domain: domain, scheme: scheme, port: port, user: user, password: password ?? serverKey);

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

        protected static Ref GetRef(Expr v) =>
            (Ref) ((ObjectV) v)["ref"];

        protected Task<Expr> Q(Language query) =>
            TestClient.Query(query);
    }

    class MockClientIO : IClientIO
    {
        HttpResponseMessage resp;

        public MockClientIO(HttpResponseMessage resp)
        {
            this.resp = resp;
        }

        public Task<HttpResponseMessage> DoRequest(HttpRequestMessage req) =>
            Task.FromResult(this.resp);

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
            string directory = System.AppDomain.CurrentDomain.BaseDirectory;

            while (!File.Exists(Path.Combine(directory, "testConfig.json")))
                directory = Path.GetDirectoryName(directory);

            string configPath = Path.Combine(directory, "testConfig.json");

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

        public override string ToString() =>
            $"Config(domain: {Domain}, scheme: {Scheme}, port: {Port}, user: {User}, password: {Password})";
    }
}
