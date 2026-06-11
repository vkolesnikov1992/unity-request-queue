using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Network
{
    [Preserve]
    public sealed class UnityWebRequestHttpClient : IHttpClient
    {
        public async UniTask<HttpResponse> SendAsync(
            HttpRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var unityRequest = CreateRequest(request);
            await using (cancellationToken.Register(unityRequest.Abort))
            {
                await unityRequest.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                return new HttpResponse(
                    unityRequest.responseCode,
                    unityRequest.downloadHandler?.text,
                    unityRequest.downloadHandler?.data,
                    unityRequest.GetResponseHeaders(),
                    unityRequest.error);
            }
        }

        private static UnityWebRequest CreateRequest(HttpRequest request)
        {
            var unityRequest = new UnityWebRequest(request.Url, request.Method)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            if (request.Body != null)
            {
                unityRequest.uploadHandler = new UploadHandlerRaw(request.Body);

                if (!string.IsNullOrWhiteSpace(request.ContentType))
                {
                    unityRequest.uploadHandler.contentType = request.ContentType;
                }
            }

            foreach (var header in request.Headers)
            {
                unityRequest.SetRequestHeader(header.Key, header.Value);
            }

            return unityRequest;
        }
    }
}
