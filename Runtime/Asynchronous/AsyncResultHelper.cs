namespace Framework
{
    public static class AsyncResultHelper
    {
        public static IAsyncResult<IAsyncResult> WaitAny(RecyclableList<IAsyncResult> results, bool autoStopOthers = false)
        {
            AsyncResult<IAsyncResult> asyncResult = AsyncResult<IAsyncResult>.Create();
            foreach (IAsyncResult result in results)
            {
                result.Callbackable().OnCallback(r =>
                {
                    if (autoStopOthers)
                    {
                        foreach (IAsyncResult other in results)
                        {
                            if (other != r && !other.IsDone)
                            {
                                other.Cancel();
                            }
                        }
                    }

                    asyncResult.SetResult(r);
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