using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;

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
        protected Client client;

        string serverKey;

        [OneTimeSetUp]
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

            rootClient = GetClient(secret: cfg.Secret);

            const string dbName = "faunadb-csharp-test";
            DbRef = new Ref($"databases/{dbName}");

            try {
                await rootClient.Query(Delete(DbRef));
            } catch (BadRequest) {}

            await rootClient.Query(Create(Ref("databases"), Obj("name", dbName)));

            var key = (ObjectV) await rootClient.Query(Create(Ref("keys"), Obj("database", DbRef, "role", "server")));
            serverKey = (string) key["secret"];
            client = GetClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownAsync().Wait();
        }

        async Task TearDownAsync()
        {
            await rootClient.Query(Delete(DbRef));
        }

        protected Client GetClient(string secret = null) =>
            new Client(domain: domain, scheme: scheme, port: port, secret: secret ?? serverKey);

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

        protected static Ref GetRef(Value v) =>
            (Ref) ((ObjectV) v)["ref"];
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

            while (!File.Exists(System.IO.Path.Combine(directory, "testConfig.json")))
                directory = System.IO.Path.GetDirectoryName(directory);

            string configPath = System.IO.Path.Combine(directory, "testConfig.json");

            if (File.Exists(configPath))
            {
                string text = await File.OpenText(configPath).ReadToEndAsync();
                return JsonConvert.DeserializeObject<Config>(text);
            }
            return new Config();
        }

        public string Domain { get; set; }
        public string Scheme { get; set; }
        public int? Port { get; set; }
        public string Secret { get; set; }

        public override string ToString() =>
            $"Config(domain: {Domain}, scheme: {Scheme}, port: {Port}, secret: {Secret})";
    }
}
