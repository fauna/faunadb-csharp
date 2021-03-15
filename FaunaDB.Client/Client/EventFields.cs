namespace FaunaDB.Client
{
    public class EventField
    {
        public string Value { get; }

        private EventField(string value)
        {
            Value = value;
        }
        
        public static EventField DocumentField => new EventField("document");
        
        public static EventField PrevField => new EventField("prev");

        public static EventField DiffField => new EventField("diff");

        public static EventField ActionField => new EventField("action");
    }
}
