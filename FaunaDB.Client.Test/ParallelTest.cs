using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Types;
using NUnit.Framework;
using static FaunaDB.Query.Language;

namespace Test
{
    public class ParallelTest : TestCase
 {
        private const int IN_PARALLEL = 200;
        private const int MAX_ATTEMPTS = 5;
        private const string COLLECTION_NAME = "ParallelTestCollection";

        [OneTimeSetUp]
        public void SetUpCollection()
        {
            SetUpCollectionAsync().Wait();
        }

        public async Task SetUpCollectionAsync()
        {
            var client = new FaunaClient(secret: faunaSecret, endpoint: faunaEndpoint);
            bool collectionExist = (await client.Query(Exists(Collection(COLLECTION_NAME)))).To<bool>().Value;

            // create collection and fill documents
            if (!collectionExist)
            {
                await client.Query(CreateCollection(Obj("name", COLLECTION_NAME)));
                for (int i = 0; i < 10; i++)
                {
                    await client.Query(
                            Create(
                                    Collection(COLLECTION_NAME),
                                    Obj("data", FaunaDB.Types.Encoder.Encode(new SampleDocument() { Id = i + 1, Text = $"Document {i + 1}" }))
                                )
                        );
                }
            }
        }

        [Test]
        public void ParallelQueriesTest()
        {
            Random random = new Random();
            ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
            var clients = GetClientsPool();
#if !NETCOREAPP2_1
            Func<FaunaClient, Task> query = async (client) =>
            {
                try
                {
                    await client.Query(Map(Paginate(Documents(Collection(COLLECTION_NAME))), reference => Get(reference)));
                    await client.Query(Sum(Arr(1, 2, 3.5, 0.25)));
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            };

#endif
#if NETCOREAPP2_1
            Action<FaunaClient> queryCore21 = (client) =>
            {
                try
                {
                    Task.Run(async () => await client.Query(Map(Paginate(Documents(Collection(COLLECTION_NAME))), reference => Get(reference))));
                    Task.Run(async () => await client.Query(Sum(Arr(1, 2, 3.5, 0.25))));
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            };
#endif
            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                List<Task> tasks = new List<Task>();
                for (int j = 0; j < IN_PARALLEL; j++)
                {
#if !NETCOREAPP2_1
                    tasks.Add(Task.Run(async () => await query(clients[random.Next(0, clients.Count - 1)])));
#else
                    tasks.Add(Task.Run(() => queryCore21(clients[random.Next(0, clients.Count - 1)])));
#endif
                }

                Task.WaitAll(tasks.ToArray());
            }

            Assert.IsFalse(exceptions.Any(), $"Exceptions occured. Details:{Environment.NewLine}{PrintExceptions(exceptions)}");
        }

        private IList<FaunaClient> GetClientsPool()
        {
            List<FaunaClient> clients = new List<FaunaClient>();
            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                clients.Add(new FaunaClient(secret: faunaSecret, endpoint: faunaEndpoint));
            }

            return clients;
        }

        private string PrintExceptions(IEnumerable<Exception> exceptions)
        {
            StringBuilder sb = new StringBuilder();
            Func<Exception, string> printException = (exception) =>
                $"Exception: {exception.Message}{Environment.NewLine}Stack trace:{exception.StackTrace}";
            foreach (Exception exception in exceptions.Take<Exception>(MAX_ATTEMPTS))
            {
                string message = printException(exception);
                if (exception.InnerException != null)
                {
                    message += $"Inner exception:{Environment.NewLine}" + printException(exception.InnerException);
                }

                sb.AppendLine(message);
            }

            return sb.ToString();
        }

        private class SampleDocument
        {
            [FaunaField("id")]
            public int Id { get; set; }

            [FaunaField("text")]
            public string Text { get; set; }
        }
    }
}
