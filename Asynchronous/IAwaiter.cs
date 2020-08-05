using System.Runtime.CompilerServices;

namespace Framework.Asynchronous
{
    public interface IAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>
        /// Returns <code>true</code> if the asynchronous operation is finished.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        void GetResult();
    }

    public interface IAwaiter<T> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// Returns <code>true</code> if the asynchronous operation is finished.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        /// <returns></returns>
        T GetResult();
    }
}