using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;
using Newtonsoft.Json;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    public class TestCase
    {
        FaunaClient rootClient;
        protected RefV DbRef;
        protected FaunaClient client;

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

            rootClient = new FaunaClient(domain: domain, scheme: scheme, port: int.Parse(port), secret: secret);

            const string dbName = "faunadb-csharp-test";
            DbRef = new RefV($"databases/{dbName}");

            try {
                await rootClient.Query(Delete(DbRef));
            } catch (BadRequest) {}

            await rootClient.Query(Create(Ref("databases"), Obj("name", dbName)));

            var key = await rootClient.Query(Create(Ref("keys"), Obj("database", DbRef, "role", "server")));
            client = rootClient.NewSessionClient(key.At("secret").To(Codec.STRING).Value);
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
            var mock = new MockClientIO(resp);
            return new FaunaClient(secret: "secret", clientIO: mock);
        }
    }

    class MockClientIO : IClientIO
    {
        RequestResult resp;

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
        public string Port { get; set; }
        public string Secret { get; set; }

        public override string ToString() =>
            $"Config(domain: {Domain}, scheme: {Scheme}, port: {Port}, secret: {Secret})";
    }
}
