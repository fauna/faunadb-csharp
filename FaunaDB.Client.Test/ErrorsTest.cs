using System.Collections.Generic;
using System.Linq;
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
    [TestFixture]
    public class ErrorsTest : TestCase
    {
        [Test]
        public void TestInvalidResponse()
        {
            Assert.ThrowsAsync<UnknowException>(async () => await MockClient("I like fine wine").Query(Get(Collection("any-ref"))));
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await MockClient("{\"resoars\": 1}").Query(Get(Collection("any-ref"))));
        }

        #region HTTP errors
        [Test]
        public void TestHttpBadRequest()
        {
            Assert.ThrowsAsync<BadRequest>(async () => await client.Query(new InvalidExpression()));
        }

        [Test]
        public void TestHttpUnauthorized()
        {
            var client = GetClient(secret: "bad_key");
            Assert.ThrowsAsync<Unauthorized>(async () => await client.Query(Get(DbRef)), "unauthorized");
        }

        [Test]
        public void TestUnavailableError()
        {
            var client = MockClient("{\"errors\": [{\"code\": \"unavailable\", \"description\": \"on vacation\"}]}", HttpStatusCode.ServiceUnavailable);
            Assert.ThrowsAsync<UnavailableError>(async () => await client.Query(Get(Collection("any-ref"))), "unavailable");
        }

        [Test]
        public async Task TestPermissionDenied()
        {
            var key = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "client")));

            var client = GetClient(secret: key.Get(Field.At("secret").To<string>()));

            AssertQueryException<PermissionDenied>(client, Paginate(Native.DATABASES), "permission denied", "Insufficient privileges to perform the action.");
        }
        #endregion

        #region ErrorData
        [Test]
        public void TestInvalidExpression()
        {
            AssertQueryException<BadRequest>(new InvalidExpression(), "invalid expression", "No form/function found, or invalid argument keys: { foo }.");
        }

        [Test]
        public void TestUnboundVariable()
        {
            AssertQueryException<BadRequest>(Var("x"), "invalid expression", "Variable 'x' is not defined.");
        }

        [Test]
        public void TestInvalidArgument()
        {
            AssertQueryException<BadRequest>(Add(Arr(1, "two")), "invalid argument", "Number expected, String provided.", new List<string> { "add", "1" });
        }

        [Test]
        public void TestNonEmptyArray()
        {
            AssertQueryException<BadRequest>(EqualsFn(), "invalid argument", "Non-empty array expected.", new List<string> { "equals" });
            AssertQueryException<BadRequest>(Add(), "invalid argument", "Non-empty array expected.", new List<string> { "add" });
            AssertQueryException<BadRequest>(Multiply(), "invalid argument", "Non-empty array expected.", new List<string> { "multiply" });
            AssertQueryException<BadRequest>(Subtract(), "invalid argument", "Non-empty array expected.", new List<string> { "subtract" });
            AssertQueryException<BadRequest>(Divide(), "invalid argument", "Non-empty array expected.", new List<string> { "divide" });
            AssertQueryException<BadRequest>(Modulo(), "invalid argument", "Non-empty array expected.", new List<string> { "modulo" });
            AssertQueryException<BadRequest>(LT(), "invalid argument", "Non-empty array expected.", new List<string> { "lt" });
            AssertQueryException<BadRequest>(LTE(), "invalid argument", "Non-empty array expected.", new List<string> { "lte" });
            AssertQueryException<BadRequest>(GT(), "invalid argument", "Non-empty array expected.", new List<string> { "gt" });
            AssertQueryException<BadRequest>(GTE(), "invalid argument", "Non-empty array expected.", new List<string> { "gte" });
            AssertQueryException<BadRequest>(And(), "invalid expression", "Expression list cannot be empty.", new List<string> { "and" });
            AssertQueryException<BadRequest>(Or(), "invalid expression", "Expression list cannot be empty.", new List<string> { "or" });
            AssertQueryException<BadRequest>(Union(), "invalid argument", "Non-empty array expected.", new List<string> { "union" });
            AssertQueryException<BadRequest>(Intersection(), "invalid argument", "Non-empty array expected.", new List<string> { "intersection" });
            AssertQueryException<BadRequest>(Difference(), "invalid argument", "Non-empty array expected.", new List<string> { "difference" });
        }

        [Test]
        public async Task TestInstanceNotFound()
        {
            // Must be a reference to a real class or else we get InvalidExpression
            await client.Query(CreateCollection(Obj("name", "foofaws")));
            AssertQueryException<NotFound>(Get(Ref(Collection("foofaws"), "123")), "instance not found", "Document not found.");
        }

        [Test]
        public void TestValueNotFound()
        {
            AssertQueryException<NotFound>(Select("a", Obj()), "value not found", "Value not found at path [a].");
        }

        [Test]
        public async Task TestInstanceAlreadyExists()
        {
            await client.Query(CreateCollection(Obj("name", "duplicates")));
            var @ref = (await client.Query(Create(Collection("duplicates"), Obj()))).At("ref");
            AssertQueryException<BadRequest>(Create(@ref, Obj()), "instance already exists", "Document already exists.", new List<string> { "create" });
        }
        #endregion

        [Test]
        public async Task TestDuplicateValue()
        {
            await client.Query(CreateCollection(Obj("name", "gerbils")));
            await client.Query(CreateIndex(Obj(
                "name", "gerbils_by_x",
                "active", true,
                "source", Collection("gerbils"),
                "terms", Arr(Obj("field", Arr("data", "x"))),
                "unique", true
            )));
            await client.Query(Create(Collection("gerbils"), Obj("data", Obj("x", 1))));
        }

        private void AssertException(FaunaException exception, string code, string description, IReadOnlyList<string> position = null)
        {
            Assert.AreEqual(1, exception.Errors.Count());
            var error = exception.Errors.First();
            Assert.AreEqual(code, error.Code);
            Assert.AreEqual(description, error.Description);
            if (position != null)
            {
                Assert.True(position.SequenceEqual(error.Position));
            }
        }

        private void AssertQueryException<TException>(Expr query, string code, string description, IReadOnlyList<string> position = null)
            where TException : FaunaException
        {
            AssertQueryException<TException>(client, query, code, description, position);
        }

        private void AssertQueryException<TException>(FaunaClient client, Expr query, string code, string description, IReadOnlyList<string> position = null)
            where TException : FaunaException
        {
            var exception = Assert.ThrowsAsync<TException>(async () => await client.Query(query));
            AssertException(exception, code, description, position);
        }
    }

    internal class InvalidExpression : Expr
    {
        public override bool Equals(Expr v) => false;

        protected override int HashCode() => 0;

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("foo");
            writer.WriteValue("bar");
            writer.WriteEndObject();
        }
    }
}
