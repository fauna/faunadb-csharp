using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaunaDB.Errors
{
    public class QueryError
    {
        public IReadOnlyList<string> Positions { get; private set; }

        public string Code { get; private set; }

        public string Description { get; private set; }

        public IReadOnlyList<ValidationFailure> Failures { get; private set; }

        [JsonConstructor]
        public QueryError(IReadOnlyList<string> positions, string code, string description, IReadOnlyList<ValidationFailure> failures, IReadOnlyList<ValidationFailure> cause = null)
        {
            Positions = positions ?? new List<string>();
            Code = code;
            Description = description;
            Failures = failures ?? cause ?? new List<ValidationFailure>();
        }
    }
}