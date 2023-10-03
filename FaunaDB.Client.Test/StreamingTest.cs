using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Client;
using FaunaDB.Errors;
using FaunaDB.Types;
using NUnit.Framework;

using static FaunaDB.Query.Language;
using static Test.ClientTest;

namespace Test
{
    public class StreamingTest : TestCase
    {
        [OneTimeSetUp]
        public new void SetUp()
        {
            SetUpAsync().Wait();
        }

        private async Task SetUpAsync()
        {
            await adminClient.Query(CreateCollection(Obj("name", "streams_test")));
        }

        [Test]
        public void TestThatStreamFailsIfTargetDoesNotExist()
        {
            AsyncTestDelegate doc = async () => { await adminClient.Stream(Get(Ref(Collection("streams_test"), "1234"))); };

            var ex = Assert.ThrowsAsync<NotFound>(doc);
            Assert.AreEqual("instance not found: Document not found.", ex.Message);
            AssertErrors(ex, code: "instance not found", description: "Document not found.");
        }

        [Test]
        public void TestStreamFailsIfIncorrectValuePassedToStreamMethod()
        {
            AsyncTestDelegate doc = async () => { await adminClient.Stream(Collection("streams_test")); };
            var ex = Assert.ThrowsAsync<BadRequest>(doc);
            Assert.AreEqual("invalid argument: Expected a Document Ref or Version, or a Set Ref, got Collection Ref.", ex.Message);
            AssertErrors(ex, code: "invalid argument", description: "Expected a Document Ref or Version, or a Set Ref, got Collection Ref.");
        }

        [Test]
        public void TestStreamFailsIfQueryIsNotReadOnly()
        {
            AsyncTestDelegate doc = async () => { await adminClient.Stream(CreateCollection(Collection("streams_test"))); };
            var ex = Assert.ThrowsAsync<BadRequest>(doc);
            Assert.AreEqual("invalid expression: Write effect in read-only query expression.", ex.Message);
            AssertErrors(ex, code: "invalid expression", description: "Write effect in read-only query expression.");
        }

        [Test]
        public async Task TestStreamEventsOnDocumentReferenceWithDocumentFieldByDefault()
        {
            Value createdInstance = await adminClient.Query(
                    Create(await RandomCollection(),
                        Obj("credentials",
                            Obj("password", "abcdefg"))));

            var docRef = createdInstance.At("ref");

            var provider = await adminClient.Stream(docRef);

            var done = new TaskCompletionSource<object>();

            List<Value> events = new List<Value>();

            var monitor = new StreamingEventMonitor(
                value =>
                {
                    events.Add(value);
                    if (events.Count == 4)
                    {
                        provider.Complete();
                    }
                    else
                    {
                        provider.RequestData();
                    }
                },
                ex => { done.SetException(ex); },
                () => { done.SetResult(null); }
            );

            // subscribe to data provider
            monitor.Subscribe(provider);

            // push 3 updates
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue1"))));
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue2"))));
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue3"))));

            // blocking until we receive all the events
            await done.Task;

            // clear the subscription
            monitor.Unsubscribe();

            Value startEvent = events[0];
            Assert.AreEqual("start", startEvent.At("type").To<string>().Value);

            Value e1 = events[1];
            Assert.AreEqual("version", e1.At("type").To<string>().Value);
            Assert.AreEqual("testValue1", e1.At("event", "document", "data", "testField").To<string>().Value);

            Value e2 = events[2];
            Assert.AreEqual("version", e1.At("type").To<string>().Value);
            Assert.AreEqual("testValue2", e2.At("event", "document", "data", "testField").To<string>().Value);

            Value e3 = events[3];
            Assert.AreEqual("version", e1.At("type").To<string>().Value);
            Assert.AreEqual("testValue3", e3.At("event", "document", "data", "testField").To<string>().Value);
        }

        [Test]
        public async Task TeststreamEventsOnDocumentReferenceWithOptInFields()
        {
            Value createdInstance = await adminClient.Query(
                Create(await RandomCollection(),
                    Obj("data",
                        Obj("testField", "testValue0"))));

            var docRef = createdInstance.At("ref");

            var fields = new List<EventField>
            {
                EventField.ActionField,
                EventField.DiffField,
                EventField.DocumentField,
                EventField.PrevField,
            };

            var provider = await adminClient.Stream(docRef, fields);

            var done = new TaskCompletionSource<object>();

            List<Value> events = new List<Value>();

            var monitor = new StreamingEventMonitor(
                value =>
                {
                    events.Add(value);
                    if (events.Count == 4)
                    {
                        provider.Complete();
                    }
                    else
                    {
                        provider.RequestData();
                    }
                },
                ex => { done.SetException(ex); },
                () => { done.SetResult(null); }
            );

            // subscribe to data provider
            monitor.Subscribe(provider);

            // push 3 updates
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue1"))));
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue2"))));
            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "testValue3"))));

            // blocking until we receive all the events
            await done.Task;

            // clear the subscription
            monitor.Unsubscribe();

            Value startEvent = events[0];
            Assert.AreEqual("start", startEvent.At("type").To<string>().Value);

            Value e1 = events[1];
            Assert.AreEqual("version", e1.At("type").To<string>().Value);
            Assert.AreEqual("update", e1.At("event", "action").To<string>().Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue1")),
                ((ObjectV)e1.At("event", "diff", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue1")),
                ((ObjectV)e1.At("event", "document", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue0")),
                ((ObjectV)e1.At("event", "prev", "data")).Value);

            Value e2 = events[2];
            Assert.AreEqual("version", e2.At("type").To<string>().Value);
            Assert.AreEqual("update", e2.At("event", "action").To<string>().Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue2")),
                ((ObjectV)e2.At("event", "diff", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue2")),
                ((ObjectV)e2.At("event", "document", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue1")),
                ((ObjectV)e2.At("event", "prev", "data")).Value);

            Value e3 = events[3];
            Assert.AreEqual("version", e3.At("type").To<string>().Value);
            Assert.AreEqual("update", e3.At("event", "action").To<string>().Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue3")),
                ((ObjectV)e3.At("event", "diff", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue3")),
                ((ObjectV)e3.At("event", "document", "data")).Value);
            Assert.AreEqual(
                FaunaDB.Collections.ImmutableDictionary.Of("testField", StringV.Of("testValue2")),
                ((ObjectV)e3.At("event", "prev", "data")).Value);
        }

        [Test]
        public async Task TestStreamHandlesLossOfAuthorization()
        {
            await adminClient.Query(
                CreateCollection(Obj("name", "streamed-things-auth"))
            );

            Value createdInstance = await adminClient.Query(
                Create(Collection("streamed-things-auth"),
                    Obj("credentials",
                        Obj("password", "abcdefg"))));

            var docRef = createdInstance.At("ref");

            // new key + client
            Value newKey = await adminClient.Query(CreateKey(Obj("role", "server-readonly")));
            FaunaClient streamingClient = adminClient.NewSessionClient(newKey.At("secret").To<string>().Value);

            var provider = await streamingClient.Stream(docRef);

            var done = new TaskCompletionSource<object>();

            List<Value> events = new List<Value>();

            var monitor = new StreamingEventMonitor(
                async value =>
                {
                    if (events.Count == 0)
                    {
                        try
                        {
                            // update doc on `start` event
                            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "afterStart"))));

                            // delete key
                            await adminClient.Query(Delete(newKey.At("ref").To<RefV>().Value));

                            // push an update to force auth revalidation.
                            await adminClient.Query(Update(docRef, Obj("data", Obj("testField", "afterKeyDelete"))));
                        }
                        catch (Exception ex)
                        {
                            done.SetException(ex);
                        }
                    }

                    // capture element
                    events.Add(value);

                    // ask for more elements
                    provider.RequestData();
                },
                ex => { done.SetException(ex); },
                () => { done.SetResult(null); }
            );

            // subscribe to data provider
            monitor.Subscribe(provider);

            // wrapping an asynchronous call
            AsyncTestDelegate res = async () => await done.Task;

            // blocking until we get an exception
            var exception = Assert.ThrowsAsync<StreamingException>(res);

            // clear the subscription
            monitor.Unsubscribe();

            // validating exception message
            Assert.AreEqual("permission denied: Authorization lost during stream evaluation.", exception.Message);
            AssertErrors(exception, code: "permission denied", description: "Authorization lost during stream evaluation.");
        }
    }
}
