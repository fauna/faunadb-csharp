using NUnit.Framework;
using System;
using System.Collections.Generic;
using FaunaDB.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;
using System.Collections.Concurrent;
using FaunaDB.Client;

namespace Test
{
    public class ParallelTest: TestCase
    {
        private const int IN_PARALLEL = 1000;
        private const int MAX_ATTEMPTS = 10;
        private const string COLLECTION_NAME = "ParallelTestCollection";
        
        [OneTimeSetUp]
        public void SetUpCollection()
        {
            SetUpCollectionAsync().Wait();
        }
        public async Task SetUpCollectionAsync()
        {
            var client = new FaunaClient(secret: faunaSecret, endpoint: faunaEndpoint);
            Value exist = await client.Query(Exists(Collection(COLLECTION_NAME)));
            
            //create collection and fill documents
            if (!(bool)exist)
            {
                await client.Query(CreateCollection(Obj("name", COLLECTION_NAME)));
                for (int i = 0; i < 10; i++)
                {
                    await client.Query(
                            Create(
                                    Collection(COLLECTION_NAME),
                                    Obj("data", FaunaDB.Types.Encoder.Encode(new CollectionDocument() { Id = i+1, Text=$"Document {i+1}" }))
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
            
            Func<FaunaClient, Task> query = async (client) =>
            {
                try
                {
                    Value value = await client.Query(Map(Paginate(Documents(Collection(COLLECTION_NAME))), reference => Get(reference)));
                    value = await client.Query(Sum(Arr(1, 2, 3.5, 0.25)));
                }
                catch(Exception e)
                {
                    exceptions.Add(e);
                }
            };

            for (int i = 0; i < MAX_ATTEMPTS; i++)
            {
                List<Task> tasks = new List<Task>();
                for (int j = 0; j < IN_PARALLEL; j++)
                {
                    tasks.Add(Task.Run(async () =>await query(clients[random.Next(0, clients.Count-1)])));
                }
                Task.WaitAll(tasks.ToArray());
                Task.Delay(500);
            }

            Assert.IsFalse(exceptions.Any(), $"Exceptions occured. Details:{Environment.NewLine}{PrintExceptions(exceptions)}");
        }

        private IList<FaunaClient> GetClientsPool()
        {
            List<FaunaClient> clients = new List<FaunaClient>();
            for (int i = 0; i < 10; i++)
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
            foreach (Exception exception in exceptions)
            {
                string message = printException(exception);
                if (exception.InnerException != null)
                    message += $"Inner exception:{Environment.NewLine}" + printException(exception.InnerException);
                sb.AppendLine(message);
            }
            return sb.ToString();
        }

        class CollectionDocument
        {
            [FaunaField("id")]
            public int Id;
            [FaunaField("text")]
            public string Text;
        }
    }
}
