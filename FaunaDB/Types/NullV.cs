using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Type of Value.Null.
    /// </summary>
    public class NullV : Value
    {
        public static readonly Value Instance = new NullV();

        NullV() {}

        protected override void WriteJson(JsonWriter writer)
        {
            writer.WriteNull();
        }

        #region boilerplate
        public override bool Equals(Expr v) =>
            object.ReferenceEquals(this, v);

        public override string ToString() =>
            "NullV";

        protected override int HashCode() =>
            0;
        #endregion
    }
}
