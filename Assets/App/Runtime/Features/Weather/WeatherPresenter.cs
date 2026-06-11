using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.Weather
{
    public sealed class WeatherPresenter : NetworkTabPresenterBase<WeatherView, WeatherParameters, WeatherModel>
    {
        public WeatherPresenter(IRequestQueue requestQueue)
            : base(requestQueue)
        {
        }

        public override TabId TabId => TabId.Weather;

        protected override WeatherParameters DefaultParameters => WeatherParameters.Default;

        protected override UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
