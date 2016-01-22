using Newtonsoft.Json;

namespace FaunaDB.Values
{
    /// <summary>
    /// FaunaDB Set.
    /// </summary>
    /// <remarks>
    /// This represents a set returned as part of a response. This looks like <c>{"@set": set_query}</c>.
    /// For query sets see <see cref="Query"/>.
    /// </remarks>
    public sealed class SetRef : ValueWrap<SetRef, Query>
    {
        public SetRef(Query q) : base(q) {}

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@set", (Value) Val);
        }
    }
}
