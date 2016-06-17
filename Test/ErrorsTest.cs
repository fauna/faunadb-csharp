using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture] public class ErrorsTest : TestCase
    {
        [Test] public async Task TestRequestResult()
        {
            var err = await AssertU.Throws<BadRequest>(() => TestClient.Query(UnescapedObject.With("foo", "bar")));
            Assert.AreEqual(err.RequestResult.RequestContent, UnescapedObject.With("foo", "bar"));
        }

        [Test] public async Task TestInvalidResponse()
        {
            // Response must be valid JSON
            await AssertU.Throws<InvalidResponseException>(() => MockClient("I like fine wine").Query(Get(Ref(""))));
            // Response must have "resource"
            //todo: is this the right error to throw?
            await AssertU.Throws<KeyNotFoundException>(() => MockClient("{\"resoars\": 1}").Query(Get(Ref(""))));
        }

        #region HTTP errors
        [Test] public async Task TestHttpBadRequest()
        {
            await AssertU.Throws<BadRequest>(() => TestClient.Query(UnescapedObject.With("foo", "bar")));
        }

        [Test] public async Task TestHttpUnauthorized()
        {
            var client = GetClient(password: "bad_key");
            await AssertHttpException<Unauthorized>("unauthorized", () => client.Query(Get(DbRef)));
        }

        [Test] public async Task TestUnavailableError()
        {
            var client = MockClient("{\"errors\": [{\"code\": \"unavailable\", \"description\": \"on vacation\"}]}", HttpStatusCode.ServiceUnavailable);
            await AssertHttpException<UnavailableError>("unavailable", () => client.Query(Get(Ref(""))));
        }
        #endregion

        #region ErrorData
        [Test] public async Task TestInvalidExpression()
        {
            await AssertQueryException<BadRequest>(UnescapedObject.With("foo", "bar"), "invalid expression", ArrayV.Empty);
        }

        [Test] public async Task TestUnboundVariable()
        {
            await AssertQueryException<BadRequest>(Var("x"), "unbound variable", ArrayV.Empty);
        }

        [Test] public async Task TestInvalidArgument()
        {
            await AssertQueryException<BadRequest>(Add(Arr(1, "two")), "invalid argument", new ArrayV("add", 1));
        }

        [Test] public async Task TestInstanceNotFound()
        {
            // Must be a reference to a real class or else we get InvalidExpression
            await TestClient.Query(Create(Ref("classes"), Obj("name", "foofaws")));
            await AssertQueryException<NotFound>(Get(Ref("classes/foofaws/123")), "instance not found", ArrayV.Empty);
        }

        [Test] public async Task TestValueNotFound()
        {
            await AssertQueryException<NotFound>(Select("a", Obj()), "value not found", ArrayV.Empty);
        }

        [Test] public async Task TestInstanceAlreadyExists()
        {
            await TestClient.Query(Create(Ref("classes"), Obj("name", "duplicates")));
            var @ref = (Ref) ((ObjectV) (await TestClient.Query(Create(Ref("classes/duplicates"), Obj()))))["ref"];
            await AssertQueryException<BadRequest>(Create(@ref, Obj()), "instance already exists", new ArrayV("create"));
        }
        #endregion

        [Test] public async Task TestDuplicateValue()
        {
            await TestClient.Query(Create(Ref("classes"), Obj("name", "gerbils")));
            await TestClient.Query(Create(Ref("indexes"), Obj(
                "name", "gerbils_by_x",
                "source", Ref("classes/gerbils"),
                "terms", Arr(Obj("path", "data.x")),
                "unique", true
            )));
            await TestClient.Query(Create(Ref("classes/gerbils"), Obj("data", Obj("x", 1))));
        }

        async Task AssertHttpException<TException>(string code, Func<Task> action) where TException : FaunaException
        {
            var exception = await AssertU.Throws<TException>(action);
            AssertException(exception, code);
        }

        void AssertException(FaunaException exception, string code, Expr position = null)
        {
            Assert.AreEqual(1, exception.Errors.Count());
            var error = exception.Errors.First();
            Assert.AreEqual(code, error.Code);
            Assert.AreEqual(position, error.Position);
        }

        async Task AssertQueryException<TException>(Expr query, string code, Expr position = null)
            where TException  : FaunaException
        {
            var exception = await AssertU.Throws<TException>(() => TestClient.Query(query));
            AssertException(exception, code, position);
        }
    }
}
