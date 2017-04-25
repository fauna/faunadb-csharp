using System;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// A FaunaDB ref type.
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class RefV : Value
    {
        public string Id { get; }
        public RefV Class { get; }
        public RefV Database { get; }

        public RefV(string id, RefV @class = null, RefV database = null)
        {
            Id = id;
            Class = @class;
            Database = database;
        }

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

            return $"{GetType().Name}(id = \"{Id}\"{cls}{db})";
        }
    }

    /// <summary>
    /// A Database reference. Terse version for <c>new RefV(id, BuiltIn.DATABASES, database)</c>
    /// </summary>
    public class DatabaseV : RefV
    {
        public DatabaseV(string id, DatabaseV database = null) : base(id, BuiltIn.DATABASES, database)
        { }

        public static DatabaseV Of(string id, DatabaseV database = null) =>
            new DatabaseV(id, database);
    }

    /// <summary>
    /// A Class reference. Terse version for <c>new RefV(id, BuiltIn.CLASSES, database)</c>
    /// </summary>
    public class ClassV : RefV
    {
        public ClassV(string id, DatabaseV database = null) : base(id, BuiltIn.CLASSES, database)
        { }

        public static ClassV Of(string id, DatabaseV database = null) =>
            new ClassV(id, database);
    }

    /// <summary>
    /// An Index reference. Terse version for <c>new RefV(id, BuiltIn.INDEXES, database)</c>
    /// </summary>
    public class IndexV : RefV
    {
        public IndexV(string id, DatabaseV database = null) : base(id, BuiltIn.INDEXES, database)
        { }

        public static IndexV Of(string id, DatabaseV database = null) =>
            new IndexV(id, database);
    }

    /// <summary>
    /// A Function reference. Terse version for <c>new RefV(id, BuiltIn.FUNCTIONS, database)</c>
    /// </summary>
    public class FunctionV : RefV
    {
        public FunctionV(string id, DatabaseV database = null) : base(id, BuiltIn.FUNCTIONS, database)
        { }

        public static FunctionV Of(string id, DatabaseV database = null) =>
            new FunctionV(id, database);
    }

    /// <summary>
    /// A Key reference. Terse version for <c>new RefV(id, BuiltIn.KEYS, database)</c>
    /// </summary>
    public class KeyV : RefV
    {
        public KeyV(string id, DatabaseV database = null) : base(id, BuiltIn.KEYS, database)
        { }

        public static KeyV Of(string id, DatabaseV database = null) =>
            new KeyV(id, database);
    }

    public static class BuiltIn
    {
        public static readonly RefV CLASSES = new RefV("classes");
        public static readonly RefV INDEXES = new RefV("indexes");
        public static readonly RefV DATABASES = new RefV("databases");
        public static readonly RefV KEYS = new RefV("keys");
        public static readonly RefV FUNCTIONS = new RefV("functions");

        internal static RefV FromName(string name)
        {
            switch (name)
            {
                case "classes": return CLASSES;
                case "indexes": return INDEXES;
                case "databases": return DATABASES;
                case "keys": return KEYS;
                case "functions": return FUNCTIONS;
            }

            return new RefV(name);
        }
    }
}
