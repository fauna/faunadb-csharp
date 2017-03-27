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

        Value ConsumeValue()
        {
            var value = ReadValue();
            Consume(token: JsonToken.None);
            return value;
        }

        Value ReadValue()
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject();
                case JsonToken.StartArray:
                    return ReadArray();
                case JsonToken.Integer:
                    return LongV.Of((long)Expect(token: JsonToken.Integer));
                case JsonToken.Float:
                    return DoubleV.Of((double)Expect(token: JsonToken.Float));
                case JsonToken.String:
                    return StringV.Of((string)Expect(token: JsonToken.String));
                case JsonToken.Boolean:
                    return BooleanV.Of((bool)Expect(token: JsonToken.Boolean));
                case JsonToken.Null:
                    return NullV.Instance;
                default:
                    return Unexpected();
            }
        }

        Value ReadObject()
        {
            Consume(token: JsonToken.StartObject);

            switch (reader.TokenType)
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
                    Add(ReadPropertyName(), ConsumeValue());
            });
        }

        ArrayV ReadArray()
        {
            Consume(token: JsonToken.StartArray);

            if (reader.TokenType == JsonToken.EndArray)
                return ArrayV.Empty;

            return new ArrayV(Add =>
            {
                while (reader.TokenType != JsonToken.EndArray)
                    Add(ConsumeValue());
            });
        }

        string ReadPropertyName()
        {
            if (reader.TokenType != JsonToken.PropertyName)
                Unexpected(JsonToken.PropertyName);

            return (string)Consume(token: JsonToken.PropertyName);
        }

        string ReadEnclosedString()
        {
            Consume(token: JsonToken.PropertyName);
            return (string)Consume(token: JsonToken.String);
        }

        ObjectV ReadEnclosedObject()
        {
            Consume(token: JsonToken.PropertyName);
            var obj = (ObjectV)ReadObject();
            Consume(token: JsonToken.EndObject);
            return obj;
        }

        object Expect(JsonToken token)
        {
            if (token != JsonToken.None && token != reader.TokenType)
                Unexpected(token);

            return reader.Value;
        }

        object Consume(JsonToken token)
        {
            if (token != JsonToken.None && token != reader.TokenType)
                Unexpected(token);

            var value = reader.Value;
            reader.Read();
            return value;
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
