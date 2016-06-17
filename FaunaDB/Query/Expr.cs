using FaunaDB.Errors;
using FaunaDB.Types;
using FaunaDB.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FaunaDB.Query
{
    [JsonConverter(typeof(ValueJsonConverter))]
    public abstract class Expr : IEquatable<Expr>
    {
        internal abstract void WriteJson(JsonWriter writer);

        /// <summary>
        /// Convert to a JSON string.
        /// </summary>
        /// <param name="pretty">If true, output with helpful whitespace.</param>
        public string ToJson(bool pretty = false) =>
            JsonConvert.SerializeObject(this, pretty ? Formatting.Indented : Formatting.None);

        /// <summary>
        /// Read a Value from JSON.
        /// </summary>
        /// <exception cref="Errors.InvalidResponseException"/>
        //todo: Should we convert invalid Value downcasts and missing field exceptions to InvalidResponseException?
        public static Expr FromJson(string json)
        {
            // We handle dates ourselves. Don't want them automatically parsed.
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            try
            {
                return JsonConvert.DeserializeObject<Expr>(json, settings);
            }
            catch (JsonReaderException j)
            {
                throw new InvalidResponseException($"Bad JSON: {j}");
            }
        }


        #region implicit conversions
        public static implicit operator Expr(List<Expr> values) =>
            values == null ? NullV.Instance : ArrayV.FromEnumerable(values);

        public static implicit operator Expr(OrderedDictionary<string, Expr> values) =>
            values == null ? NullV.Instance : new ObjectV(values);

        public static implicit operator Expr(bool b) =>
            BooleanV.Of(b);

        public static implicit operator Expr(double d) =>
            new DoubleV(d);

        public static implicit operator Expr(long l) =>
            new LongV(l);

        public static implicit operator Expr(int i) =>
            new LongV(i);

        public static implicit operator Expr(string s) =>
            s == null ? NullV.Instance : new StringV(s);

        #endregion

        #region explicit (downcasting) conversions
        public static explicit operator List<Expr>(Expr v) =>
            ((ArrayV) v).Value;

        public static explicit operator OrderedDictionary<string, Expr>(Expr v) =>
            ((ObjectV)v).Value;

        public static explicit operator bool(Expr v) =>
            ((BooleanV) v).Value;

        public static explicit operator double(Expr v) =>
            ((DoubleV) v).Value;

        public static explicit operator long(Expr v) =>
            ((LongV) v).Value;

        public static explicit operator string(Expr v) =>
            ((StringV) v).Value;
        #endregion

        #region boilerplate
        public override bool Equals(object obj)
        {
            var v = obj as Expr;
            return v != null && Equals(v);
        }

        public abstract bool Equals(Expr v);

        public static bool operator ==(Expr a, Expr b) =>
            object.Equals(a, b);

        public static bool operator !=(Expr a, Expr b) =>
            !object.Equals(a, b);

        public override int GetHashCode() =>
            HashCode();

        // Force subclasses to implement hash code.
        protected abstract int HashCode();
        #endregion

        #region Operators
        public static Expr operator !(Expr a) =>
            Language.Not(a);

        public static Expr operator +(Expr a, Expr b) =>
            Language.Add(a, b);

        public static Expr operator -(Expr a, Expr b) =>
            Language.Subtract(a, b);

        public static Expr operator *(Expr a, Expr b) =>
            Language.Multiply(a, b);

        public static Expr operator /(Expr a, Expr b) =>
            Language.Divide(a, b);

        public static Expr operator %(Expr a, Expr b) =>
            Language.Modulo(a, b);

        public static Expr operator &(Expr a, Expr b) =>
            Language.And(a, b);

        public static Expr operator |(Expr a, Expr b) =>
            Language.Or(a, b);

        public static Expr operator <(Expr a, Expr b) =>
            Language.LT(a, b);

        public static Expr operator <=(Expr a, Expr b) =>
            Language.LTE(a, b);

        public static Expr operator >(Expr a, Expr b) =>
            Language.GT(a, b);

        public static Expr operator >=(Expr a, Expr b) =>
            Language.GTE(a, b);

        #endregion
    }
}
