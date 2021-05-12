using System.Collections.Generic;
using FaunaDB.Collections;
using FaunaDB.Query;
using Newtonsoft.Json;

namespace FaunaDB.Types
{
    /// <summary>
    /// Represents a query value in the FaunaDB query language.
    /// <para>
    /// See <see href="https://fauna.com/documentation/queries#values-special_types">FaunaDB Special Types</see>.
    /// </para>
    /// </summary>
    public class QueryV : ScalarValue<IReadOnlyDictionary<string, Expr>>
    {
        internal QueryV(IReadOnlyDictionary<string, Expr> lambda)
            : base(lambda) { }

        protected internal override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("@query");
            writer.WriteObject(Value);
            writer.WriteEndObject();
        }

        public override bool Equals(Expr v)
        {
            var w = v as QueryV;
            return w != null && Value.DictEquals(w.Value);
        }
    }
}
