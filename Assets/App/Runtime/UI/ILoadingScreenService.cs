namespace UnityRequestQueue.Runtime.UI
{
    public interface ILoadingScreenService
    {
        bool IsVisible { get; }

        LoadingScreenHandle Show(string message = null);

        void Hide(LoadingScreenHandle handle);

        void HideAll();

        void SetMessage(string message);
    }
}
