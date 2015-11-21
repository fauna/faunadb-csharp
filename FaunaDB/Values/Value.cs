using Newtonsoft.Json;
using System;
using System.Collections.Immutable;

using FaunaDB.Errors;

namespace FaunaDB.Values
{
    [JsonConverter(typeof(ValueJsonConverter))]
    public abstract class Value : IEquatable<Value>
    {
        /// <summary>
        /// <c>null</c> is not an acceptable value. Use Value.Null instead.
        /// </summary>
        public static Value Null { get { return NullV.Instance; } }

        /// <summary>
        /// Convert to a JSON string.
        /// </summary>
        /// <param name="pretty">If true, output with helpful whitespace.</param>
        public string ToJson(bool pretty = false)
        {
            return JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// Read a Value from JSON.
        /// </summary>
        /// <exception cref="Errors.InvalidResponseException"/>
        //todo: Should we convert invalid Value downcasts and missing field exceptions to InvalidResponseException?
        public static Value FromJson(string json)
        {
            // We handle dates ourselves. Don't want them automatically parsed.
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            try
            {
                return JsonConvert.DeserializeObject<Value>(json, settings);
            }
            catch (JsonReaderException j)
            {
                throw new InvalidResponseException(string.Format("Bad JSON: {0}", j));
            }
        }

        internal abstract void WriteJson(JsonWriter writer);

        #region implicit conversions
        public static implicit operator Value(ImmutableArray<Value> values)
        {
            return new ArrayV(values);
        }

        public static implicit operator Value(ImmutableDictionary<string, Value> d)
        {
            return new ObjectV(d);
        }

        public static implicit operator Value(bool b)
        {
            return new BoolV(b);
        }

        public static implicit operator Value(double d)
        {
            return new DoubleV(d);
        }

        public static implicit operator Value(long l)
        {
            return new LongV(l);
        }

        public static implicit operator Value(int i)
        {
            return new LongV(i);
        }

        public static implicit operator Value(string s)
        {
            return s == null ? null : new StringV(s);
        }

        public static implicit operator Value(EventType e)
        {
            return e.Name();
        }
        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator ImmutableDictionary<string, Value>(Value v)
        {
            return ((ObjectV) v).Val;
        }

        public static explicit operator ImmutableArray<Value>(Value v)
        {
            return ((ArrayV) v).Val;
        }

        public static explicit operator bool(Value v)
        {
            return ((BoolV) v).Val;
        }

        public static explicit operator double(Value v)
        {
            return ((DoubleV) v).Val;
        }

        public static explicit operator long(Value v)
        {
            return ((LongV) v).Val;
        }

        public static explicit operator string(Value v)
        {
            return ((StringV) v).Val;
        }
        #endregion

        #region boilerplate
        public override bool Equals(object obj)
        {
            var v = obj as Value;
            return v != null && Equals(v);
        }

        public abstract bool Equals(Value v);

        public static bool operator ==(Value a, Value b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Value a, Value b)
        {
            return !object.Equals(a, b);
        }

        public override int GetHashCode()
        {
            return HashCode();
        }

        // Force subclasses to implement hash code.
        protected abstract int HashCode();
        #endregion
    }
}
