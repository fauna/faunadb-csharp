using Newtonsoft.Json;

namespace FaunaDB.Values
{
    /// <summary>
    /// Type of Value.Null.
    /// </summary>
    public class NullV : Value
    {
        public static readonly NullV Instance = new NullV();

        NullV() {}

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteNull();
        }

        #region boilerplate
        public override bool Equals(Value v) =>
            object.ReferenceEquals(this, v);

        public override string ToString() =>
            "Value.Null";

        protected override int HashCode() =>
            0;
        #endregion
    }
}
