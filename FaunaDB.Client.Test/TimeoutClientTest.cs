using FaunaDB.Client;
using System.Diagnostics;
using FaunaDB.Types;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;

using static FaunaDB.Query.Language;

namespace Test
{
#if (NETFRAMEWORK || NETCOREAPP2_0 || NETCOREAPP3_0)

    [TestFixture]
    public class TimeoutClientTest
    {
        private DummyServer dummy;
        private FaunaClient client;

        [SetUp]
        public void SetUp()
        {
            dummy = new DummyServer();
            dummy.Start();

            client = new FaunaClient("secret", "http://127.0.0.1:9999", timeout: TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void TearDown()
        {
            dummy.Stop();
        }

        [Test]
        public void TestServerTimingOut()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Task<Value> task0 = client.Query(Now());

            Assert.ThrowsAsync<TimeoutException>(async () => await task0);

            watch.Stop();

            Assert.IsTrue(task0.IsCompleted, "the task0 is completed");
            Assert.IsTrue(task0.IsFaulted, "the task0 failed");

            StringAssert.Contains("The operation has timed out.", task0.Exception.Message);

            Assert.IsTrue(watch.Elapsed.TotalSeconds >= 10 && watch.Elapsed.TotalSeconds <= 12);
        }

        [Test]
        public async Task TestSimpleServerResponse()
        {
            dummy.AcceptResponses();

            Task<Value> task1 = client.Query(Add(41, 1));
            await task1;

            Assert.IsTrue(task1.IsCompleted, "the task1 is completed");
            Assert.IsFalse(task1.IsFaulted, "the task1 is Ok");
            Assert.AreEqual(42, task1.Result.To<long>().Value);
        }
    }

    class DummyServer
    {
        private TcpListener tcpListener;

        public void Start(int port = 9999)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            tcpListener = new TcpListener(address, port);
            tcpListener.Start();
        }

        public void AcceptResponses()
        {

            String response = @"HTTP/1.1 200 OK
X-Txn-Time: 1588276396485000
X-Read-Ops: 0
X-Write-Ops: 0
X-Query-Bytes-In: 15
X-Query-Bytes-Out: 19
X-FaunaDB-Build: dummy-server
connection: keep-alive
content-length: 19
content-type: application/json;charset=utf-8

{ ""resource"": 42 }
";

            Task cliTask = new Task(() =>
            {
                Socket cli = tcpListener.AcceptSocket();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
                cli.Send(msg);
                cli.Close();
            });

            cliTask.Start();
        }

        public void Stop() =>
            tcpListener.Stop();

    }

#endif
}