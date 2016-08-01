using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaunaDB.Errors
{
    public class ValidationFailure
    {
        public IReadOnlyList<string> Field { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }

        [JsonConstructor]
        public ValidationFailure(IReadOnlyList<string> field, string code, string description)
        {
            Field = field ?? new List<string>();
            Code = code;
            Description = description;
        }
    }
}