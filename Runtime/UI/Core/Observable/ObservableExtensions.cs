using System.Collections.Specialized;
using Framework;

public static class ObservableExtensions
{
    public static IAsyncResult<T> WaitAdd<T>(this ObservableList<T> list)
    {
        AsyncResult<T> result = AsyncResult<T>.Create();
        void Listener(NotifyCollectionChangedAction action, T val, int index)
        {
            result.SetResult(val);
            list.RemoveListener(Listener);
        }

        list.AddListenerWithoutCall(Listener);
        return result;
    }
}