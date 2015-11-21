using System;

using FaunaDB.Errors;
using FaunaDB.Values;

namespace FaunaDB
{
    /// <summary>
    /// FaunaDB Event.
    /// See the <see href="https://faunadb.com/documentation/queries#values">docs</see>.
    /// </summary>
    public struct Event : IEquatable<Event>
    {
        /// <summary>
        /// The Ref of the affected instance.
        /// </summary>
        public Ref Resource { get; }
        /// <summary>
        /// Microsecond UNIX timestamp at which the event occurred.
        /// </summary>
        public long Ts { get; }
        public EventType Action { get; }

        /// <summary>
        /// Use this on a value that you know represents an Event.
        /// </summary>
        public static explicit operator Event(Value v)
        {
            var obj = (ObjectV) v;
            return new Event((Ref) obj["resource"], (long) obj["ts"], GetType((string) obj["action"]));
        }

        static EventType GetType(string action)
        {
            switch (action)
            {
                case "create":
                    return EventType.Create;
                case "delete":
                    return EventType.Delete;
                default:
                    throw new InvalidResponseException(
                        string.Format("Expected event type to be \"create\" or \"delete\", not: {0}", action));
            }
        }

        public Event(Ref resource, long ts, EventType action)
        {
            Resource = resource;
            Ts = ts;
            Action = action;
        }

        public static implicit operator Value(Event e)
        {
            return new ObjectV("resource", e.Resource, "ts", e.Ts, "action", e.Action.Name());
        }

        #region boilerplate
        public override bool Equals(object obj)
        {
            return obj is Event && Equals((Event) obj);
        }

        public bool Equals(Event e)
        {
            return Resource == e.Resource && Ts == e.Ts && Action == e.Action;
        }

        public static bool operator ==(Event a, Event b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Event a, Event b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return HashUtil.Hash(Resource, Ts, Action);
        }

        public override string ToString()
        {
            return string.Format("Event({0}, {1}, {2})", Resource, Ts, Action.Name());
        }
        #endregion
    }

    /// <summary>
    /// Types of event.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Event that creates, changes, or resurrects an instance.
        /// </summary>
        Create,
        /// <summary>
        /// Event that removes an instance.
        /// </summary>
        Delete
    }

    public static class EventTypeUtil
    {
        /// <summary>
        /// <c>"create"</c> or <c>"delete"</c>/.
        /// </summary>
        public static string Name(this EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Create:
                    return "create";
                case EventType.Delete:
                    return "delete";
                default:
                    throw new InvalidValueException(string.Format("Bad EventType: {0}", eventType));
            }
        }
    }
}
