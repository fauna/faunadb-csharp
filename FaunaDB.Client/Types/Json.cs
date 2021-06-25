using System;
using System.Collections.Generic;
using System.Reflection;
using FaunaDB.Errors;
using FaunaDB.Query;
using Newtonsoft.Json;

using static System.Linq.Enumerable;

namespace FaunaDB.Types
{
    internal class ValueJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            ((Value)value).WriteJson(writer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            ValueReader.ReadValue(reader);

        public override bool CanConvert(Type objectType) =>
            typeof(Value).IsAssignableFrom(objectType);
    }

    internal class ValueReader
    {
        private readonly JsonReader reader;

        public static Value ReadValue(JsonReader reader) =>
            new ValueReader(reader).ReadValue();

        private ValueReader(JsonReader reader)
        {
            this.reader = reader;
        }

        private Value ReadValue()
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

        private Value HandleValue()
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return ReadObject();
                case JsonToken.StartArray:
                    return ReadArray();
                case JsonToken.Integer:
                    return LongV.Of((long)reader.Value);
                case JsonToken.Float:
                    return DoubleV.Of((double)reader.Value);
                case JsonToken.String:
                    return StringV.Of((string)reader.Value);
                case JsonToken.Boolean:
                    return BooleanV.Of((bool)reader.Value);
                case JsonToken.Null:
                    return NullV.Instance;
                default:
                    return Unexpected();
            }
        }

        private JsonToken Next(JsonToken expect = JsonToken.None)
        {
            reader.Read();

            if (expect != JsonToken.None && expect != reader.TokenType)
            {
                Unexpected(expect);
            }

            return reader.TokenType;
        }

        private Value ReadObject()
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
                            return RefParser.Parse(ReadEnclosedObject());
                        case "@ts":
                            return new TimeV(ReadEnclosedString());
                        case "@date":
                            return new DateV(ReadEnclosedString());
                        case "@bytes":
                            return new BytesV(ReadEnclosedString());
                        case "@query":
                            return new QueryV(ReadEnclosedLambda());
                        default:
                            return ReadObjectBody();
                    }

                case JsonToken.EndObject:
                    return ObjectV.Empty;
                default:
                    return Unexpected();
            }
        }

        private ObjectV ReadObjectBody()
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return ObjectV.Empty;
            }

            return new ObjectV(Add =>
            {
                while (reader.TokenType != JsonToken.EndObject)
                {
                    Add(ReadPropertyName(), ReadValue());
                }
            });
        }

        private ArrayV ReadArray()
        {
            if (Next() == JsonToken.EndArray)
            {
                return ArrayV.Empty;
            }

            return new ArrayV(Add =>
            {
                while (reader.TokenType != JsonToken.EndArray)
                {
                    Add(ReadValue());
                }
            });
        }

        private string ReadPropertyName()
        {
            try
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    Unexpected(JsonToken.PropertyName);
                }

                return (string)reader.Value;
            }
            finally
            {
                Next();
            }
        }

        private string ReadEnclosedString()
        {
            try
            {
                Next(expect: JsonToken.String);
                return (string)reader.Value;
            }
            finally
            {
                Next(expect: JsonToken.EndObject);
            }
        }

        private ObjectV ReadEnclosedObject()
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

        private IReadOnlyDictionary<string, Expr> ReadEnclosedLambda()
        {
            return ReadEnclosedObject().Value.ToDictionary(
                key => key.Key,
                value => Unwrap(value.Value)
            );
        }

        private static Expr Unwrap(Expr expr)
        {
            var obj = expr as ObjectV;
            if (obj != null)
            {
                var values = new Dictionary<string, Expr>();

                foreach (var kv in obj.Value)
                {
                    values.Add(kv.Key, Unwrap(kv.Value));
                }

                return new UnescapedObject(values);
            }

            var arr = expr as ArrayV;
            if (arr != null)
            {
                var values = new List<Expr>();

                foreach (var value in arr.Value)
                {
                    values.Add(Unwrap(value));
                }

                return new UnescapedArray(values);
            }

            return expr;
        }

        private Value Unexpected(JsonToken expected = JsonToken.None)
        {
            if (expected == JsonToken.None)
            {
                throw new UnknowException($"Unexpected token {reader.TokenType.ToString()}");
            }

            throw new UnknowException($"Expected {expected.ToString()} but received {reader.TokenType.ToString()}");
        }
    }

    internal static class JsonWriterExtensions
    {
        public static void WriteArray(this JsonWriter writer, IEnumerable<Expr> values)
        {
            writer.WriteStartArray();
            foreach (var value in values)
            {
                if (value == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    value.WriteJson(writer);
                }
            }

            writer.WriteEndArray();
        }

        public static void WriteObject(this JsonWriter writer, string name, Expr value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(name);
            value.WriteJson(writer);
            writer.WriteEndObject();
        }

        public static void WriteObject<TValue>(this JsonWriter writer, IEnumerable<KeyValuePair<string, TValue>> props)
            where TValue : Expr
        {
            writer.WriteStartObject();
            foreach (var kv in props)
            {
                writer.WritePropertyName(kv.Key);
                if (kv.Value == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    kv.Value.WriteJson(writer);
                }
            }

            writer.WriteEndObject();
        }
    }
}
