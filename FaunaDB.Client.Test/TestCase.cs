using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;

namespace Test
{
    public class TestCase
    {
        protected static Field<string> SECRET_FIELD = Field.At("secret").To<string>();
        protected static Field<RefV> REF_FIELD = Field.At("ref").To<RefV>();
        protected const string testDbName = "faunadb-csharp-test";

        protected FaunaClient rootClient;
        protected Expr DbRef;
        protected FaunaClient client;
        protected FaunaClient adminClient;
        protected Value clientKey;
        protected Value adminKey;
        protected String faunaEndpoint;
        protected String faunaSecret;
        
        protected RefV GetRef(Value v) =>
            v.Get(REF_FIELD);

        [OneTimeSetUp]
        public void SetUp()
        {
            SetUpAsync().Wait();
        }

        async Task SetUpAsync()
        {
            Func<string, string, string> Env = (name, @default) =>
                Environment.GetEnvironmentVariable(name) ?? @default;

            var domain = Env("FAUNA_DOMAIN", "localhost");
            var scheme = Env("FAUNA_SCHEME", "http");
            var port = Env("FAUNA_PORT", "8443");

            faunaSecret = Env("FAUNA_ROOT_KEY", "secret");
            faunaEndpoint = $"{scheme}://{domain}:{port}/";
            rootClient = new FaunaClient(secret: faunaSecret, endpoint: faunaEndpoint);

            DbRef = Database(testDbName);

            try {
                await rootClient.Query(Delete(DbRef));
            } catch (BadRequest) {}

            await rootClient.Query(CreateDatabase(Obj("name", testDbName)));

            clientKey = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "server")));
            adminKey = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "admin")));
            client = rootClient.NewSessionClient(clientKey.Get(SECRET_FIELD));
            adminClient = rootClient.NewSessionClient(adminKey.Get(SECRET_FIELD));
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
        
        protected async Task<RefV> RandomCollection()
        {
            Value coll = await client.Query(
                CreateCollection(
                    Obj("name", RandomStartingWith("some_coll_")))
            );

            return GetRef(coll);
        }
        
        protected string RandomStartingWith(params string[] strs)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var str in strs)
                builder.Append(str);

            builder.Append(new Random().Next(0, int.MaxValue));

            return builder.ToString();
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

        public Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null, TimeSpan? queryTimeout = null) =>
            Task.FromResult(resp);

        public Task<StreamingRequestResult> DoStreamingRequest(string data, IReadOnlyDictionary<string, string> query = null)
        {
            throw new NotImplementedException();
        }
    }

    class HttpClientWrapper : HttpClient
    {

        public HttpRequestMessage LastMessage { get; private set; }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastMessage = request;
            return base.SendAsync(request, cancellationToken);
        }

    }

}
