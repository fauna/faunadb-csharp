using System.Threading.Tasks;
using FaunaDB.Client.Exceptions;
using NUnit.Framework;
using Test;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Query;
using FaunaDB.Types;
using static FaunaDB.Query.Language;
using static FaunaDB.Types.Encoder;
using static FaunaDB.Types.Option;

namespace FaunaDB.Client.Test
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
            await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Obj("unique_filed", 1))));
            await testClient.Query(
                        CreateIndex(Obj(
                                    "name", EXISTS_COLLECTION_INDEX,
                                    "active", true,
                                    "source", Collection(EXISTS_COLLECTION),
                                    "terms", Arr(Obj("field", Arr("data", "unique_filed"))),
                                    "unique", true
                                )
                            )
                    );
        }

        [Test]
        public void InvalidRefTest()
        {
            Assert.ThrowsAsync<InvalidRef>(async () => await testClient.Query(Map(Paginate(Documents(Collection("aa"))), reference => Get(reference))));
        }

        [Test]
        public void TestHttpUnauthorized()
        {
            var client = GetClient(secret: "bad_key");
            Assert.ThrowsAsync<UnknownCode>(async () => await testClient.Query(Get(DbRef)));
        }

        [Test]
        public void InstnaceAlreadyExistTest()
        {
            Assert.ThrowsAsync<InstanceAlreadyExists>(async () => await testClient.Query(CreateCollection(Obj("name", EXISTS_COLLECTION))));
        }

        [Test]
        public void ValidationFailedTest()
        {
            Assert.ThrowsAsync<ValidationFailed>(async () => await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Arr("unique_filed", 1)))));
        }

        [Test]
        public void InstanceNotUniqueTest()
        {
            Assert.ThrowsAsync<InstanceNotUnique>(async () => await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("data", Obj("unique_filed", 1)))));
        }

        //not ready 
        [Test]
        public async Task FeatureNotAvailableTest()
        {
            //Assert.ThrowsAsync<FeatureNotAvailable>(async () => await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("wrong_parameter", Obj("unique_filed", 1)))));
            await testClient.Query(Create(Collection(EXISTS_COLLECTION), Obj("wrong_parameter", Obj("unique_filed", 1))));
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
        public void InvalidArgument()
        {
            Assert.ThrowsAsync<InvalidArgument>(async () => await testClient.Query(Delete(Ref(Collection(EXISTS_COLLECTION), "abcsvf"))));
        }
    }
}
