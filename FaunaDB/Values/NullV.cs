using Newtonsoft.Json;

namespace FaunaDB.Values
{
    class NullV : Value
    {
        public static readonly NullV Instance = new NullV();

        NullV() {}

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteNull();
        }

        #region boilerplate
        public override bool Equals(Value v)
        {
            return object.ReferenceEquals(this, v);
        }

        public override string ToString()
        {
            return "Value.Null";
        }

        protected override int HashCode()
        {
            return 0;
        }
        #endregion
    }
}
