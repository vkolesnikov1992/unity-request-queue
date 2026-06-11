using System.Collections.Generic;

namespace UnityRequestQueue.Runtime.Network
{
    public sealed class HttpResponse
    {
        private static readonly IReadOnlyDictionary<string, string> SEmptyHeaders =
            new Dictionary<string, string>();

        public HttpResponse(
            long statusCode,
            string text,
            byte[] data,
            IReadOnlyDictionary<string, string> headers,
            string error)
        {
            StatusCode = statusCode;
            Text = text;
            Data = data;
            Headers = headers ?? SEmptyHeaders;
            Error = error;
        }

        public long StatusCode { get; }

        public string Text { get; }

        public byte[] Data { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }

        public string Error { get; }

        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode <= 299;
    }
}
