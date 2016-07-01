using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class ErrorsTest : TestCase
    {
        [Test] public void TestRequestResult()
        {
            var err = Assert.ThrowsAsync<BadRequest>(async () => await client.Query(UnescapedObject.With("foo", "bar")));
            Assert.AreEqual(err.RequestResult.RequestContent, UnescapedObject.With("foo", "bar"));
        }

        [Test] public void TestInvalidResponse()
        {
            Assert.ThrowsAsync<InvalidResponseException>(async() => await MockClient("I like fine wine").Query(Get(Ref(""))));
            Assert.ThrowsAsync<KeyNotFoundException>(async() => await MockClient("{\"resoars\": 1}").Query(Get(Ref(""))));
        }

        #region HTTP errors
        [Test] public void TestHttpBadRequest()
        {
            Assert.ThrowsAsync<BadRequest>(async() => await client.Query(UnescapedObject.With("foo", "bar")));
        }

        [Test] public void TestHttpUnauthorized()
        {
            var client = GetClient(secret: "bad_key");
            Assert.ThrowsAsync<Unauthorized>(async() => await client.Query(Get(DbRef)), "unauthorized");
        }

        [Test] public void TestUnavailableError()
        {
            var client = MockClient("{\"errors\": [{\"code\": \"unavailable\", \"description\": \"on vacation\"}]}", HttpStatusCode.ServiceUnavailable);
            Assert.ThrowsAsync<UnavailableError>(async() => await client.Query(Get(Ref(""))), "unavailable");
        }
        #endregion

        #region ErrorData
        [Test] public void TestInvalidExpression()
        {
            AssertQueryException<BadRequest>(UnescapedObject.With("foo", "bar"), "invalid expression", ArrayV.Empty);
        }

        [Test] public void TestUnboundVariable()
        {
            AssertQueryException<BadRequest>(Var("x"), "unbound variable", ArrayV.Empty);
        }

        [Test] public void TestInvalidArgument()
        {
            AssertQueryException<BadRequest>(Add(Arr(1, "two")), "invalid argument", ArrayV.Of("add", 1));
        }

        [Test] public async Task TestInstanceNotFound()
        {
            // Must be a reference to a real class or else we get InvalidExpression
            await client.Query(Create(Ref("classes"), Obj("name", "foofaws")));
            AssertQueryException<NotFound>(Get(Ref("classes/foofaws/123")), "instance not found", ArrayV.Empty);
        }

        [Test] public void TestValueNotFound()
        {
            AssertQueryException<NotFound>(Select("a", Obj()), "value not found", ArrayV.Empty);
        }

        [Test] public async Task TestInstanceAlreadyExists()
        {
            await client.Query(Create(Ref("classes"), Obj("name", "duplicates")));
            var @ref = (Ref) ((ObjectV) (await client.Query(Create(Ref("classes/duplicates"), Obj()))))["ref"];
            AssertQueryException<BadRequest>(Create(@ref, Obj()), "instance already exists", ArrayV.Of("create"));
        }
        #endregion

        [Test] public async Task TestDuplicateValue()
        {
            await client.Query(Create(Ref("classes"), Obj("name", "gerbils")));
            await client.Query(Create(Ref("indexes"), Obj(
                "name", "gerbils_by_x",
                "source", Ref("classes/gerbils"),
                "terms", Arr(Obj("path", "data.x")),
                "unique", true
            )));
            await client.Query(Create(Ref("classes/gerbils"), Obj("data", Obj("x", 1))));
        }

        void AssertException(FaunaException exception, string code, Expr position = null)
        {
            Assert.AreEqual(1, exception.Errors.Count());
            var error = exception.Errors.First();
            Assert.AreEqual(code, error.Code);
            Assert.AreEqual(position, error.Position);
        }

        void AssertQueryException<TException>(Expr query, string code, Expr position = null)
            where TException  : FaunaException
        {
            var exception = Assert.ThrowsAsync<TException>(async() => await client.Query(query));
            AssertException(exception, code, position);
        }
    }
}
