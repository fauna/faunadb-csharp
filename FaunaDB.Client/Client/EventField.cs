namespace FaunaDB.Client
{
    public class EventField
    {
        private EventField(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static EventField DocumentField => new EventField("document");

        public static EventField PrevField => new EventField("prev");

        public static EventField DiffField => new EventField("diff");

        public static EventField ActionField => new EventField("action");
    }
}
