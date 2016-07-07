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
using FaunaDB.Query;
using System.Collections.Generic;
using System;

namespace Test
{
    public class TestCase
    {
        string domain;
        string scheme;
        int? port;
        Client rootClient;
        protected RefV DbRef;
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
            DbRef = new RefV($"databases/{dbName}");

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
            var resp = new RequestResult(HttpMethodKind.Get, "", null, "", responseText, (int)statusCode, null, DateTime.UtcNow, DateTime.UtcNow);
            var mock = new MockClientIO(resp);
            return new Client(domain: domain, scheme: scheme, port: port, clientIO: mock);
        }
    }

    class MockClientIO : IClientIO
    {
        RequestResult resp;

        public MockClientIO(RequestResult resp)
        {
            this.resp = resp;
        }

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
        public int? Port { get; set; }
        public string Secret { get; set; }

        public override string ToString() =>
            $"Config(domain: {Domain}, scheme: {Scheme}, port: {Port}, secret: {Secret})";
    }
}
