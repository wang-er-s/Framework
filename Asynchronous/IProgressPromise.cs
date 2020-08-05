namespace Framework.Asynchronous
{
    public interface IProgressPromise<TProgress> : IPromise
    {
        /// <summary>
        /// The task's progress.
        /// </summary>
        TProgress Progress { get; }

        /// <summary>
        /// Update progress
        /// </summary>
        /// <param name="progress"></param>
        void UpdateProgress(TProgress progress);
    }
    
    public interface IProgressPromise<TProgress, TResult> : IProgressPromise<TProgress>, IPromise<TResult>
    {
    }
}