using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    public struct RefID
    {
        public string Id { get; }
        public RefV Class { get; }
        public RefV Database { get; }

        public RefID(string id, RefV @class, RefV database)
        {
            this.Id = id;
            this.Class = @class;
            this.Database = database;
        }
    }

    /// <summary>
    /// A FaunaDB ref type.
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class RefV : ScalarValue<RefID>
    {
        public string Id { get { return Value.Id; } }
        public RefV Class { get { return Value.Class; } }
        public RefV Database { get { return Value.Database; } }

        public RefV(string id, RefV @class = null, RefV database = null)
            : base(new RefID(id, @class, database))
        { }

        public static RefV Of(string id, RefV @class = null, RefV database = null) =>
            new RefV(id, @class, database);

        protected internal override void WriteJson(JsonWriter writer)
        {
            var props = UnescapedObject.With(
                "id", Id,
                "class", Class,
                "database", Database
            );

            writer.WriteObject("@ref", props);
        }

        public override bool Equals(Expr v)
        {
            var other = v as RefV;

            return other != null &&
                Id == other.Id &&
                Class == other.Class &&
                Database == other.Database;
        }

        protected override int HashCode() =>
            new { Id, Class, Database }.GetHashCode();

        public override string ToString()
        {
            var cls = Class != null ? $", class = {Class}" : string.Empty;
            var db = Database != null ? $", database = {Database}" : string.Empty;

            return $"RefV(id = \"{Id}\"{cls}{db})";
        }
    }

    public static class Native
    {
        public static readonly RefV CLASSES = new RefV("classes");
        public static readonly RefV INDEXES = new RefV("indexes");
        public static readonly RefV DATABASES = new RefV("databases");
        public static readonly RefV KEYS = new RefV("keys");
        public static readonly RefV FUNCTIONS = new RefV("functions");
        public static readonly RefV TOKENS = new RefV("tokens");
        public static readonly RefV CREDENTIALS = new RefV("credentials");

        internal static RefV FromName(string name)
        {
            switch (name)
            {
                case "classes": return CLASSES;
                case "indexes": return INDEXES;
                case "databases": return DATABASES;
                case "keys": return KEYS;
                case "functions": return FUNCTIONS;
                case "tokens": return TOKENS;
                case "credentials": return CREDENTIALS;
            }

            return new RefV(name);
        }
    }
}
