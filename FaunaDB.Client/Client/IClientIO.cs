using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaunaDB.Client
{
    /// <summary>
    /// Handles actual I/O for a <see cref="FaunaClient"/>.
    /// </summary>
    public interface IClientIO
    {
        IClientIO NewSessionClient(string secret);

        Task<RequestResult> DoRequest(
            HttpMethodKind method,
            string path,
            string data,
            IReadOnlyDictionary<string, string> query = null,
            TimeSpan? queryTimeout = null);
    }
}
