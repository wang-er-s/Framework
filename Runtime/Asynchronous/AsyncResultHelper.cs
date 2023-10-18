namespace Framework
{
    public static class AsyncResultHelper
    {
        public static IAsyncResult WaitAny(RecyclableList<IAsyncResult> results)
        {
            AsyncResult asyncResult = AsyncResult.Create(isFromPool: true);
            foreach (IAsyncResult result in results)
            {
                result.Callbackable().OnCallback((r) =>
                {
                    asyncResult.SetResult();
                });
            }
            return asyncResult;
        }
        
        public static IAsyncResult WaitAll(params IAsyncResult[] results)
        {
            MulAsyncResult mulProgressResult = MulAsyncResult.Create(allProgress: results);
            AsyncResult asyncResult = AsyncResult.Create();
            return asyncResult;
        } 
    }
}