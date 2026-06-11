using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class DogBreedsPresenter : NetworkTabPresenterBase<DogBreedsView, DogBreedsParameters, DogBreedsModel>
    {
        public DogBreedsPresenter(IRequestQueue requestQueue)
            : base(requestQueue)
        {
        }

        public override TabId TabId => TabId.DogBreeds;

        protected override DogBreedsParameters DefaultParameters => DogBreedsParameters.Default;

        protected override UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
