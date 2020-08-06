using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents a null value in the FaunaDB query language.
    /// </summary>
    public sealed class NullV : Value
    {
        public static readonly Value Instance = new NullV();

        internal NullV() {}

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteNull();
        }

        #region boilerplate
        public override bool Equals(Expr v) =>
            object.ReferenceEquals(this, v);

        public override string ToString() =>
            "null";

        protected override int HashCode() =>
            0;
        #endregion
    }
}
