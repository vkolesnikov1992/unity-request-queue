using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    internal sealed class ApplicationEventLoop : MonoBehaviour
    {
        private IAppEntryPoint _entryPoint;
        private CancellationTokenSource _cancellation;

        [Inject]
        private void Construct(IAppEntryPoint entryPoint)
        {
            _entryPoint = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
            _cancellation = new CancellationTokenSource();
        }

        private void Start()
        {
            RunAsync().Forget(Debug.LogException);
        }

        private async UniTask RunAsync()
        {
            await _entryPoint.RunAsync(_cancellation.Token);
        }

        private void OnDestroy()
        {
            if (_cancellation == null)
            {
                return;
            }

            _cancellation.Cancel();
            _cancellation.Dispose();
            _cancellation = null;
        }
    }
}
