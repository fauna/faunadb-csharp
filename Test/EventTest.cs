using NUnit.Framework;

using FaunaDB;
using FaunaDB.Values;

namespace Test
{
    [TestFixture] public class EventTest : TestCase
    {
        [Test] public void TestEvent()
        {
            Ref @ref = new Ref("classes", "frogs", "123");
            var @event = new Event(@ref, 123, EventType.Create);
            const string jsonEvent = "{\"ts\":123,\"action\":\"create\",\"resource\":{\"@ref\":\"classes/frogs/123\"}}";
            Assert.AreEqual(@event, (Event) Value.FromJson(jsonEvent));
            Assert.AreEqual(jsonEvent, ((Value) @event).ToJson());
            Assert.AreEqual("Event(Ref(classes/frogs/123), 123, create)", @event.ToString());
        }
    }
}
