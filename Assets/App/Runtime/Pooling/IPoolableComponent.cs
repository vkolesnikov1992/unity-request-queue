namespace UnityRequestQueue.Runtime.Pooling
{
    public interface IPoolableComponent
    {
        void OnRentFromPool();

        void OnReturnToPool();
    }
}
