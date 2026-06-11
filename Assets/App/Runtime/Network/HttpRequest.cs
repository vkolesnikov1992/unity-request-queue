using System;
using System.Collections.Generic;

namespace UnityRequestQueue.Runtime.Network
{
    public sealed class HttpRequest
    {
        private static readonly IReadOnlyDictionary<string, string> SEmptyHeaders =
            new Dictionary<string, string>();

        public HttpRequest(
            string method,
            string url,
            IReadOnlyDictionary<string, string> headers = null,
            byte[] body = null,
            string contentType = null)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("HTTP method is required.", nameof(method));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL is required.", nameof(url));
            }

            Method = method;
            Url = url;
            Headers = headers ?? SEmptyHeaders;
            Body = body;
            ContentType = contentType;
        }

        public string Method { get; }

        public string Url { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public byte[] Body { get; }

        public string ContentType { get; }

        public static HttpRequest Get(string url, IReadOnlyDictionary<string, string> headers = null)
        {
            return new HttpRequest("GET", url, headers);
        }
    }
}
