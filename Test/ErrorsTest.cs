using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using FaunaDB;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Values;
using static FaunaDB.Query;

namespace Test
{
    [TestFixture] public class ErrorsTest : TestCase
    {
        [Test] public async void TestRequestResult()
        {
            var err = await AssertU.Throws<BadRequest>(() => TestClient.Query((Query) new ObjectV("foo", "bar")));
            Assert.AreEqual(err.RequestResult.RequestContent, new ObjectV("foo", "bar"));
        }

        [Test] public async void TestInvalidResponse()
        {
            // Response must be valid JSON
            await AssertU.Throws<InvalidResponseException>(() => MockClient("I like fine wine").Get(""));
            // Response must have "resource"
            //todo: is this the right error to throw?
            await AssertU.Throws<KeyNotFoundException>(() => MockClient("{\"resoars\": 1}").Get(""));
        }

        #region HTTP errors
        [Test] public async void TestHttpBadRequest()
        {
            await AssertU.Throws<BadRequest>(() => TestClient.Query((Query) new ObjectV("foo", "bar")));
        }

        [Test] public async void TestHttpUnauthorized()
        {
            var client = GetClient(password: "bad_key");
            await AssertHttpException<Unauthorized>("unauthorized", () => client.Get(DbRef));
        }

        [Test] public async void TestHttpPermissionDenied()
        {
            await AssertHttpException<PermissionDenied>("permission denied", () => TestClient.Get("databases"));
        }

        [Test] public async void TestHttpNotFound()
        {
            await AssertHttpException<NotFound>("not found", () => TestClient.Get("classes/not_found"));
        }

        [Test] public async void TestHttpMethodNotAllowed()
        {
            await AssertHttpException<MethodNotAllowed>("method not allowed", () => TestClient.Delete("classes"));
        }

        [Test] public async void TestInternalError()
        {
            await AssertHttpException<InternalError>("internal server error", () => TestClient.Get("error"));
        }

        [Test] public async void TestUnavailableError()
        {
            var client = MockClient("{\"errors\": [{\"code\": \"unavailable\", \"description\": \"on vacation\"}]}", HttpStatusCode.ServiceUnavailable);
            await AssertHttpException<UnavailableError>("unavailable", () => client.Get(""));
        }
        #endregion

        #region ErrorData
        [Test] public async void TestInvalidExpression()
        {
            await AssertQueryException<BadRequest>((Query) new ObjectV("foo", "bar"), "invalid expression", ArrayV.Empty);
        }

        [Test] public async void TestUnboundVariable()
        {
            await AssertQueryException<BadRequest>(Var("x"), "unbound variable", ArrayV.Empty);
        }

        [Test] public async void TestInvalidArgument()
        {
            await AssertQueryException<BadRequest>(Add(new ArrayV(1, "two")), "invalid argument", new ArrayV("add", 1));
        }

        [Test] public async void TestInstanceNotFound()
        {
            // Must be a reference to a real class or else we get InvalidExpression
            await TestClient.Post("classes", new ObjectV("name", "foofaws"));
            await AssertQueryException<NotFound>(Get(new Ref("classes/foofaws/123")), "instance not found", ArrayV.Empty);
        }

        [Test] public async void TestValueNotFound()
        {
            await AssertQueryException<NotFound>(Select("a", QueryObject(ObjectV.Empty)), "value not found", ArrayV.Empty);
        }

        [Test] public async void TestInstanceAlreadyExists()
        {
            await TestClient.Post("classes", new ObjectV("name", "duplicates"));
            var @ref = (Ref) ((ObjectV) (await TestClient.Post("classes/duplicates", ObjectV.Empty)))["ref"];
            await AssertQueryException<BadRequest>(Create(@ref, QueryObject(ObjectV.Empty)), "instance already exists", new ArrayV("create"));
        }
        #endregion

        #region InvalidData
        [Test] public async void TestInvalidType()
        {
            await AssertInvalidData("classes", new ObjectV("name", 123), "invalid type", new ArrayV("name"));
        }

        [Test] public async void TestValueRequired()
        {
            await AssertInvalidData("classes", ObjectV.Empty, "value required", new ArrayV("name"));
        }

        [Test] public async void TestDuplicateValue()
        {
            await TestClient.Post("classes", new ObjectV("name", "gerbils"));
            await TestClient.Post("indexes", new ObjectV(
                "name", "gerbils_by_x",
                "source", new Ref("classes/gerbils"),
                "terms", new ArrayV(new ObjectV("path", "data.x")),
                "unique", true
            ));
            await TestClient.Post("classes/gerbils", new ObjectV("data", new ObjectV("x", 1)));
        }

        async Task AssertInvalidData(string className, ObjectV data, string code, ArrayV field)
        {
            var exception = await AssertU.Throws<BadRequest>(() => TestClient.Post(className, data));
            AssertException(exception, "validation failed", ArrayV.Empty);
            var failures = ((ValidationFailed) exception.Errors.First()).Failures;
            Assert.AreEqual(1, failures.Count());
            var failure = failures.First();
            Assert.AreEqual(code, failure.Code);
            Assert.AreEqual(field, failure.Field);
        }
        #endregion

        [Test] public void TestToString()
        {
            var err = new ErrorData("code", "desc", null);
            Assert.AreEqual(err.ToString(), "ErrorData(code, desc, null)");

            var failure = new Failure("code", "desc", new ArrayV("a", "b"));
            var vf = new ValidationFailed("vf_desc", new ArrayV("vf"), (new[] { failure }).ToImmutableArray());
            Assert.AreEqual(
                "ValidationFailed(vf_desc, ArrayV(StringV(vf)), [Failure(code, desc, ArrayV(StringV(a), StringV(b)))])",
                vf.ToString());
        }

        async Task AssertHttpException<TException>(string code, Func<Task> action) where TException : FaunaException
        {
            var exception = await AssertU.Throws<TException>(action);
            AssertException(exception, code);
        }

        void AssertException(FaunaException exception, string code, ArrayV position = null)
        {
            Assert.AreEqual(1, exception.Errors.Count());
            var error = exception.Errors.First();
            Assert.AreEqual(code, error.Code);
            Assert.AreEqual(position, error.Position);
        }

        async Task AssertQueryException<TException>(Query query, string code, ArrayV position = null)
            where TException  : FaunaException
        {
            var exception = await AssertU.Throws<TException>(() => TestClient.Query(query));
            AssertException(exception, code, position);
        }
    }
}
