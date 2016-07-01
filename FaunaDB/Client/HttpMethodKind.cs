using System;

namespace FaunaDB.Client
{

    /// <summary>
    /// A method used by the <see cref="Client"/>.
    /// </summary>
    public enum HttpMethodKind
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }

    public static class HttpMethodKindExtensions
    {
        /// <summary>
        /// All-caps name of the method.
        /// </summary>
        public static string Name(this HttpMethodKind method)
        {
            switch (method)
            {
                case HttpMethodKind.Get:
                    return "GET";
                case HttpMethodKind.Post:
                    return "POST";
                case HttpMethodKind.Put:
                    return "PUT";
                case HttpMethodKind.Patch:
                    return "PATCH";
                case HttpMethodKind.Delete:
                    return "DELETE";
                default:
                    throw new Exception($"Bad value: {method}");
            }
        }
    }
}
