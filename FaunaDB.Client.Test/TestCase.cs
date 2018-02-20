using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    public class TestCase
    {
        protected static Field<string> SECRET_FIELD = Field.At("secret").To<string>();

        protected FaunaClient rootClient;
        protected Expr DbRef;
        protected FaunaClient client;
        protected Value clientKey;

        [OneTimeSetUp]
        public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            Func<string, string, string> Env = (name, @default) =>
                Environment.GetEnvironmentVariable(name) ?? @default;

            var cfg = await Config.GetConfig();

            var domain = Env("FAUNA_DOMAIN", cfg.Domain);
            var scheme = Env("FAUNA_SCHEME", cfg.Scheme);
            var port = Env("FAUNA_PORT", cfg.Port);
            var secret = Env("FAUNA_ROOT_KEY", cfg.Secret);
            var endpoint = $"{scheme}://{domain}:{port}";

            rootClient = new FaunaClient(secret: secret, endpoint: endpoint);

            const string dbName = "faunadb-csharp-test";
            DbRef = Database(dbName);

            try {
                await rootClient.Query(Delete(DbRef));
            } catch (BadRequest) {}

            await rootClient.Query(CreateDatabase(Obj("name", dbName)));

            clientKey = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "server")));
            client = rootClient.NewSessionClient(clientKey.Get(SECRET_FIELD));
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

        protected FaunaClient GetClient(string secret) =>
            rootClient.NewSessionClient(secret);

        protected FaunaClient MockClient(string responseText, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var resp = new RequestResult(HttpMethodKind.Get, "", null, "", responseText, (int)statusCode, null, DateTime.UtcNow, DateTime.UtcNow);
            return new FaunaClient(clientIO: new MockClientIO(resp));
        }
    }

    class MockClientIO : IClientIO
    {
        readonly RequestResult resp;

        public MockClientIO(RequestResult resp)
        {
            this.resp = resp;
        }

        public IClientIO NewSessionClient(string secret) =>
            new MockClientIO(resp);

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null) =>
            Task.FromResult(resp);
    }

    // Use a class to make conversion from Json easier.
    class Config {
        public static async Task<Config> GetConfig()
        {
            var directory = Directory.GetCurrentDirectory();

            var config = Directory.GetFiles(directory, "testConfig.json", SearchOption.AllDirectories);

            if (config.Length > 0)
            {
                var text = await File.OpenText(config[0]).ReadToEndAsync();
                return JsonConvert.DeserializeObject<Config>(text);
            }

            throw new Exception("testConfig.json not found");
        }

        public string Domain { get; set; }
        public string Scheme { get; set; }
        public string Port { get; set; }
        public string Secret { get; set; }

        public override string ToString() =>
            $"Config(domain: {Domain}, scheme: {Scheme}, port: {Port}, secret: {Secret})";
    }
}
