using System.Collections.Specialized;
using Framework;

public static class ObservableExtensions
{
    public static IAsyncResult<T> WaitAdd<T>(this ObservableList<T> list)
    {
        AsyncResult<T> result = AsyncResult<T>.Create();
        UnRegister unRegister = default;
        unRegister = list.AddListener((action, val, index) =>
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                result.SetResult(val);
                unRegister.Invoke();
            }
        });
        return result;
    }
}