using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Features.Core;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerPresenter : TabPresenterBase<ClickerView, ClickerParameters, ClickerModel>
    {
        public override TabId TabId => TabId.Clicker;

        protected override UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
