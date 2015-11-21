using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaunaDB.Values
{
    static class JsonU
    {
        public static void WriteArray(this JsonWriter writer, IEnumerable<Value> vals)
        {
            writer.WriteStartArray();
            foreach (var _ in vals)
                _.WriteJson(writer);
            writer.WriteEndArray();
        }

        public static void WriteObject(this JsonWriter writer, string name, Value value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(name);
            value.WriteJson(writer);
            writer.WriteEndObject();
        }

        public static void WriteObject(this JsonWriter writer, IEnumerable<KeyValuePair<string, Value>> props)
        {
            writer.WriteStartObject();
            foreach (var kv in props)
            {
                writer.WritePropertyName(kv.Key);
                kv.Value.WriteJson(writer);
            }
            writer.WriteEndObject();
        }
    }
}
