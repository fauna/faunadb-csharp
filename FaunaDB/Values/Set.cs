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
    public sealed class Set : ValueWrap<Set, ObjectV>
    {
        public Set(ObjectV o) : base(o) {}

        override internal void WriteJson(JsonWriter writer)
        {
            writer.WriteObject("@set", Val);
        }
    }
}
