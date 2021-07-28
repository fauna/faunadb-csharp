using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FaunaDB.Client
{
    internal class Builder
    {
        internal AuthenticationHeaderValue AuthHeader;
        internal LastSeen LastSeen;
        internal HttpClient Client;
        internal string Secret;
        internal Uri Endpoint;
        internal TimeSpan? Timeout;
        internal IReadOnlyDictionary<string, string> CustomHeaders;
        internal Version HttpVersion;

        internal Builder SetLastSeen(LastSeen lastSeen)
        {
            LastSeen = lastSeen;
            return this;
        }

        internal Builder SetClient(HttpClient client)
        {
            Client = client;
            return this;
        }

        internal Builder SetSecret(string secret)
        {
            Secret = secret;
            return this;
        }

        internal Builder SetEndpoint(Uri endpoint)
        {
            Endpoint = endpoint;
            return this;
        }

        internal Builder SetTimeout(TimeSpan? timeout)
        {
            Timeout = timeout;
            return this;
        }

        internal Builder SetCustomHeaders(IReadOnlyDictionary<string, string> customHeaders)
        {
            CustomHeaders = customHeaders;
            return this;
        }

        internal Builder SetHttpVersion(Version httpVersion)
        {
            HttpVersion = httpVersion;
            return this;
        }

        internal DefaultClientIO Build()
        {
            return new DefaultClientIO(this);
        }
    }
}
