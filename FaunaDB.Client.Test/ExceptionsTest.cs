using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using NUnit.Framework;
using static FaunaDB.Query.Language;

namespace Test
{
    [TestFixture]
    public class ExceptionsTest : TestCase
    {
        private const string EXISTS_COLLECTION = "TestExistCollection";
        private const string EXISTS_COLLECTION_INDEX = "TestExistCollectionUniqueIndex";
        private FaunaClient testClient;

        [OneTimeSetUp]
        public void SetUpCollection()
        {
            SetUpCollectionAsync().Wait();
        }

        public async Task SetUpCollectionAsync()
        {
            testClient = client;
            bool collectionExist = (await client.Query(Exists(Collection(EXISTS_COLLECTION)))).To<bool>().Value;
            if (collectionExist)
            {
                return;
            }

            await testClient.Query(CreateCollection(Obj("name", EXISTS_COLLECTION)));

            await testClient.Query(
                        CreateIndex(Obj(
                                    "name", EXISTS_COLLECTION_INDEX,
                                    "active", true,
                                    "source", Collection(EXISTS_COLLECTION),
                                    "terms", Arr(Obj("field", Arr("data", "unique_field"))),
                                    "unique", true
                                )
                            )
                    );
            await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Obj("unique_field", 1))));
        }

        [Test]
        public void InvalidRefTest()
        {
            Assert.ThrowsAsync<InvalidRef>(async () => await testClient.Query(Map(Paginate(Documents(Collection("aa"))), reference => Get(reference))));
        }

        [Test]
        public void InstnaceAlreadyExistTest()
        {
            Assert.ThrowsAsync<InstanceAlreadyExists>(async () => await testClient.Query(CreateCollection(Obj("name", EXISTS_COLLECTION))));
        }

        [Test]
        public void ValidationFailedTest()
        {
            Assert.ThrowsAsync<ValidationFailed>(async () => await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Arr("unique_field", 1)))));
        }

        [Test]
        public void InstanceNotUniqueTest()
        {
            Assert.ThrowsAsync<InstanceNotUnique>(async () => await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Obj("unique_field", 1)))));
        }

        [Test]
        public void ValueNotFoundTest()
        {
            Assert.ThrowsAsync<ValueNotFound>(async () => await testClient.Query(Select(Arr("some_value", 1), Obj("a", "b"))));
        }

        [Test]
        public void InstanceNotFoundTest()
        {
            Assert.ThrowsAsync<InstanceNotFound>(async () => await testClient.Query(Delete(Ref(Collection(EXISTS_COLLECTION), "306546882291695649"))));
        }

        [Test]
        public void AuthenticationFailedTest()
        {
            Assert.ThrowsAsync<AuthenticationFailed>(async () => await testClient.Query(Login(Match(Index(EXISTS_COLLECTION_INDEX), "somename@mail.com"), Obj("password", "some_pasword"))));
        }

        [Test]
        public void InvalidArgumentTest()
        {
            Assert.ThrowsAsync<InvalidArgument>(async () => await testClient.Query(Delete(Ref(Collection(EXISTS_COLLECTION), "abcsvf"))));
        }

        [Test]
        public void InvalidExpressionTest()
        {
            Assert.ThrowsAsync<InvalidExpression>(async () => await adminClient.Stream(CreateCollection(Collection("spells"))));
        }

        [Test]
        public void TransactionAbortedTest()
        {
            Assert.ThrowsAsync<TransactionAborted>(async () => await testClient.Query(Abort("a message")));
        }

        [Test]
        public async Task CallErrorTest()
        {
            await testClient.Query(CreateFunction(Obj("name", "increment", "body", Query(Lambda("x", Divide(Var("x"), LongV.Of(0)))))));
            Assert.ThrowsAsync<CallError>(async () => await testClient.Query(Call(Function("increment"), 10)));
        }

        [Test]
        public async Task PermissionDeniedTest()
        {
            var key = await rootClient.Query(CreateKey(Obj("database", DbRef, "role", "client")));
            FaunaClient client = GetClient(key.Get(SECRET_FIELD));
            Assert.ThrowsAsync<PermissionDenied>(async () => await testClient.Query(Paginate(Databases())));
        }

        [Test]
        public async Task StackOverflowTest()
        {
            await testClient.Query(CreateFunction(Obj("name", "StackOverflowFunc", "body", Query(Lambda("x", Call(Function("StackOverflowFunc"), Var("x")))))));
            Assert.ThrowsAsync<StackOverflow>(async () => await testClient.Query(Call(Function("StackOverflowFunc"), 10)));
        }
    }
}
