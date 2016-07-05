using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaunaDB.Client
{
    /// <summary>
    /// Handles actual I/O for a Client. This can be changed for testing.
    /// </summary>
    public interface IClientIO
    {
        Task<RequestResult> DoRequest(HttpMethodKind method, string path, string data, IReadOnlyDictionary<string, string> query = null);
    }
}
