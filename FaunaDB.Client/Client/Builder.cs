using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FaunaDB.Client
{
    internal class Builder
    {
        internal AuthenticationHeaderValue AuthHeader { get; private set; }

        internal LastSeen LastSeen { get; private set; }

        internal HttpClient Client { get; private set; }

        internal string Secret { get; private set; }

        internal Uri Endpoint { get; private set; }

        internal TimeSpan? Timeout { get; private set; }

        internal IReadOnlyDictionary<string, string> CustomHeaders { get; private set; }

        internal Version HttpVersion { get; private set; }

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

        internal Builder SetAuthHeader(AuthenticationHeaderValue authHeader)
        {
            AuthHeader = authHeader;
            return this;
        }

        internal DefaultClientIO Build()
        {
            return new DefaultClientIO(this);
        }
    }
}
