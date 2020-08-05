namespace Framework.Asynchronous
{
    public interface IProgressResult<TProgress> : IAsyncResult
    {
        /// <summary>
        /// The task's progress.
        /// </summary>
        TProgress Progress { get; }

        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        new IProgressCallbackable<TProgress> Callbackable();
    }
    
    public interface IProgressResult<TProgress, TResult> : IAsyncResult<TResult>, IProgressResult<TProgress>
    {
        /// <summary>
        /// Gets a callbackable object.
        /// </summary>
        /// <returns></returns>
        new IProgressCallbackable<TProgress, TResult> Callbackable();
    }
}