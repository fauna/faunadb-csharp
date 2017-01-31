using System;
using System.Collections.Generic;
using FaunaDB.Errors;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    class ValueJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            ((Value)value).WriteJson(writer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            ValueReader.ReadValue(reader);

        public override bool CanConvert(Type objectType) =>
            typeof(Value).IsAssignableFrom(objectType);
    }

    class ValueReader
    {
        readonly JsonReader reader;

        public static Value ReadValue(JsonReader reader) =>
            new ValueReader(reader).ReadValue();

        ValueReader(JsonReader reader)
        {
            this.reader = reader;
        }

        Value ReadValue()
        {
            try
            {
                return HandleValue();
            }
            finally
            {
                Next();
            }
        }

        Value HandleValue()
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject();
                case JsonToken.StartArray:
                    return ReadArray();
                case JsonToken.Integer:
                    return LongV.Of((long) reader.Value);
                case JsonToken.Float:
                    return DoubleV.Of((double) reader.Value);
                case JsonToken.String:
                    return StringV.Of((string) reader.Value);
                case JsonToken.Boolean:
                    return BooleanV.Of((bool) reader.Value);
                case JsonToken.Null:
                    return NullV.Instance;
                default:
                    return Unexpected();
            }
        }

        JsonToken Next(JsonToken expect = JsonToken.None)
        {
            reader.Read();

            if (expect != JsonToken.None && expect != reader.TokenType)
                Unexpected(expect);

            return reader.TokenType;
        }

        Value ReadObject()
        {
            switch (Next())
            {
                case JsonToken.PropertyName:
                    switch ((string)reader.Value)
                    {
                        case "@obj":
                            return ReadEnclosedObject();
                        case "@set":
                            return new SetRefV(ReadEnclosedObject().Value);
                        case "@ref":
                            return new RefV(ReadEnclosedString());
                        case "@ts":
                            return new TimeV(ReadEnclosedString());
                        case "@date":
                            return new DateV(ReadEnclosedString());
                        case "@bytes":
                            return new BytesV(ReadEnclosedString());
                        default:
                            return ReadObjectBody();
                    }
                case JsonToken.EndObject:
                    return ObjectV.Empty;
                default:
                    return Unexpected();
            }
        }

        ObjectV ReadObjectBody()
        {
            if (reader.TokenType == JsonToken.EndObject)
                return ObjectV.Empty;

            return new ObjectV(Add =>
            {
                while (reader.TokenType != JsonToken.EndObject)
                    Add(ReadPropertyName(), ReadValue());
            });
        }

        ArrayV ReadArray()
        {
            if (Next() == JsonToken.EndArray)
                return ArrayV.Empty;

            return new ArrayV(Add =>
            {
                while (reader.TokenType != JsonToken.EndArray)
                    Add(ReadValue());
            });
        }

        string ReadPropertyName()
        {
            try
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    Unexpected(JsonToken.PropertyName);

                return (string)reader.Value;
            }
            finally
            {
                Next();
            }
        }

        string ReadEnclosedString()
        {
            try
            {
                Next(expect: JsonToken.String);
                return (string) reader.Value;
            }
            finally
            {
                Next(expect: JsonToken.EndObject);
            }
        }

        ObjectV ReadEnclosedObject()
        {
            try
            {
                Next(expect: JsonToken.StartObject);
                return (ObjectV)ReadObject();
            }
            finally
            {
                Next(expect: JsonToken.EndObject);
            }
        }

        Value Unexpected(JsonToken expected = JsonToken.None)
        {
            if (expected == JsonToken.None)
                throw new UnknowException($"Unexpected token {reader.TokenType.ToString()}");

            throw new UnknowException($"Expected {expected.ToString()} but received {reader.TokenType.ToString()}");
        }
    }

    static class JsonWriterExtensions
    {
        public static void WriteArray(this JsonWriter writer, IEnumerable<Expr> values)
        {
            writer.WriteStartArray();
            foreach (var value in values)
                value.WriteJson(writer);
            writer.WriteEndArray();
        }

        public static void WriteObject(this JsonWriter writer, string name, Expr value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(name);
            value.WriteJson(writer);
            writer.WriteEndObject();
        }

        public static void WriteObject<TValue>(this JsonWriter writer, IEnumerable<KeyValuePair<string, TValue>> props) where TValue : Expr
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
