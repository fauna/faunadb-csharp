using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    public struct RefID
    {
        public string Id { get; }
        public RefV Collection { get; }
        public RefV Database { get; }

        public RefID(string id, RefV collection, RefV database)
        {
            this.Id = id;
            this.Collection = collection;
            this.Database = database;
        }
    }

    /// <summary>
    /// A FaunaDB ref type.
    /// <para>
    /// See <see href="https://app.fauna.com/documentation/reference/queryapi#special-type">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class RefV : ScalarValue<RefID>
    {
        public string Id { get { return Value.Id; } }
        public RefV Collection { get { return Value.Collection; } }
        public RefV Database { get { return Value.Database; } }

        public RefV(string id, RefV collection = null, RefV database = null)
            : base(new RefID(id, collection, database))
        { }

        public static RefV Of(string id, RefV collection = null, RefV database = null) =>
            new RefV(id, collection, database);

        protected internal override void WriteJson(JsonWriter writer)
        {
            var props = UnescapedObject.With(
                "id", Id,
                "collection", Collection,
                "database", Database
            );

            writer.WriteObject("@ref", props);
        }

        public override bool Equals(Expr v)
        {
            var other = v as RefV;

            return other != null &&
                Id == other.Id &&
                Collection == other.Collection &&
                Database == other.Database;
        }

        protected override int HashCode() =>
            new { Id, Collection, Database }.GetHashCode();

        public override string ToString()
        {
            var cls = Collection != null ? $", collection={Collection}" : string.Empty;
            var db = Database != null ? $", database={Database}" : string.Empty;

            return $"Ref(id=\"{Id}\"{cls}{db})";
        }
    }

    public static class Native
    {
        public static readonly RefV ACCESS_PROVIDERS = new RefV("access_providers"); 
        public static readonly RefV COLLECTIONS = new RefV("collections");
        public static readonly RefV INDEXES = new RefV("indexes");
        public static readonly RefV DATABASES = new RefV("databases");
        public static readonly RefV KEYS = new RefV("keys");
        public static readonly RefV FUNCTIONS = new RefV("functions");
        public static readonly RefV TOKENS = new RefV("tokens");
        public static readonly RefV CREDENTIALS = new RefV("credentials");
        public static readonly RefV ROLES = new RefV("roles");

        internal static RefV FromName(string name)
        {
            switch (name)
            {
                case "access_providers": return ACCESS_PROVIDERS;
                case "collections": return COLLECTIONS;
                case "indexes": return INDEXES;
                case "databases": return DATABASES;
                case "keys": return KEYS;
                case "functions": return FUNCTIONS;
                case "tokens": return TOKENS;
                case "credentials": return CREDENTIALS;
                case "roles": return ROLES;
            }

            return new RefV(name);
        }
    }
}
