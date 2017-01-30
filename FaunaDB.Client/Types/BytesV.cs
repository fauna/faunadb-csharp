using System;
using System.Linq;
using System.Text;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// A FaunaDB bytes type.
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class BytesV : Value
    {
        byte[] _Value;

        public byte[] Value
        {
            get
            {
                return (byte[])_Value.Clone();
            }
        }

        internal BytesV(string base64)
        {
            _Value = FromUrlSafeBase64(base64);
        }

        public BytesV(params byte[] value)
        {
            _Value = value;
        }

        public override bool Equals(Expr v)
        {
            var other = v as BytesV;
            return other != null && _Value.SequenceEqual(other._Value);
        }

        protected override int HashCode() =>
            _Value.GetHashCode();

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@bytes", ToUrlSafeBase64(_Value));
        }

        public override string ToString()
        {
            var buffer = new StringBuilder(_Value.Length * 4);

            buffer.Append("ByteV(");

            for (int i = 0; i < _Value.Length; i++)
            {
                buffer.AppendFormat("0x{0:x2}", _Value[i]);

                if (i < _Value.Length - 1)
                    buffer.Append(", ");
            }

            buffer.Append(")");

            return buffer.ToString();
        }

        static byte[] FromUrlSafeBase64(string urlSafe)
        {
            var base64 = urlSafe.Replace('_', '/').Replace('-', '+');

            return Convert.FromBase64String(base64);
        }

        static string ToUrlSafeBase64(byte[] value)
        {
            return Convert.ToBase64String(value)
                          .Replace('+', '-').Replace('/', '_');
        }
    }
}
