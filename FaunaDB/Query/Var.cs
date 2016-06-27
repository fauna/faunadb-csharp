using Newtonsoft.Json;

namespace FaunaDB.Query
{
    public class Var : Expr
    {
        private string name;

        public Var(string name)
        {
            this.name = name;
        }

        public override bool Equals(Expr expr)
        {
            Var other = expr as Var;
            return other != null && name.Equals(other.name);
        }

        protected override int HashCode() =>
            name.GetHashCode();

        internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("var");
            writer.WriteValue(name);
            writer.WriteEndObject();
        }

        public override string ToString() =>
            $"Var({name})";
    }
}
