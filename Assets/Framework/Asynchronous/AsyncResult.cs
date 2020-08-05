using System;
using System.Threading;
using Framework.Execution;

namespace Framework.Asynchronous
{
    public class AsyncResult : IAsyncResult, IPromise
    {

        private bool _done;
        private object _result;
        private Exception _exception;

        private bool _cancelled;
        protected readonly bool Cancelable;
        protected bool CancellationRequested;

        protected readonly object Lock = new object();

        private Synchronizable _synchronizable;
        private Callbackable _callbackable;

        public AsyncResult() : this(false)
        {
        }

        public AsyncResult(bool cancelable)
        {
            this.Cancelable = cancelable;
        }

        /// <summary>
        /// Exception
        /// </summary>
        public virtual Exception Exception => this._exception;

        /// <summary>
        /// Returns  "true" if this task finished.
        /// </summary>
        public virtual bool IsDone => this._done;

        /// <summary>
        /// The execution result
        /// </summary>
        public virtual object Result => this._result;

        public virtual bool IsCancellationRequested => this.CancellationRequested;

        /// <summary>
        /// Returns "true" if this task was cancelled before it completed normally.
        /// </summary>
        public virtual bool IsCancelled => this._cancelled;

        public virtual void SetException(string error)
        {
            if (this._done)
                return;

            var exception = new Exception(string.IsNullOrEmpty(error) ? "unknown error!" : error);
            SetException(exception);
        }

        public virtual void SetException(Exception exception)
        {
            lock (Lock)
            {
                if (this._done)
                    return;

                this._exception = exception;
                this._done = true;
                Monitor.PulseAll(Lock);
            }

            this.RaiseOnCallback();
        }

        public virtual void SetResult(object result = null)
        {
            lock (Lock)
            {
                if (this._done)
                    return;

                this._result = result;
                this._done = true;
                Monitor.PulseAll(Lock);
            }

            this.RaiseOnCallback();
        }

        public virtual void SetCancelled()
        {
            lock (Lock)
            {
                if (!this.Cancelable || this._done)
                    return;

                this._cancelled = true;
                this._exception = new OperationCanceledException();
                this._done = true;
                Monitor.PulseAll(Lock);
            }

            this.RaiseOnCallback();
        }

        /// <summary>
        /// Attempts to cancel execution of this task.  This attempt will 
        /// fail if the task has already completed, has already been cancelled,
        /// or could not be cancelled for some other reason.If successful,
        /// and this task has not started when "Cancel" is called,
        /// this task should never run. 
        /// </summary>
        /// <exception cref="NotSupportedException">If not supported, throw an exception.</exception>
        /// <returns></returns>
        public virtual bool Cancel()
        {
            if (!this.Cancelable)
                throw new NotSupportedException();

            if (this.IsDone)
                return false;

            this.CancellationRequested = true;
            this.SetCancelled();
            return true;
        }

        protected virtual void RaiseOnCallback()
        {
            _callbackable?.RaiseOnCallback();
        }

        public virtual ICallbackable Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new Callbackable(this));
            }
        }

        public virtual ISynchronizable Synchronized()
        {
            lock (Lock)
            {
                return this._synchronizable ?? (this._synchronizable = new Synchronizable(this, this.Lock));
            }
        }

        /// <summary>
        /// Wait for the result,suspends the coroutine.
        /// eg:
        /// IAsyncResult result;
        /// yiled return result.WaitForDone();
        /// </summary>
        /// <returns></returns>
        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }
    }

    public class AsyncResult<TResult> : AsyncResult, IAsyncResult<TResult>, IPromise<TResult>
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(AsyncResult<TResult>));

        private Synchronizable<TResult> _synchronizable;
        private Callbackable<TResult> _callbackable;

        public AsyncResult() : this(false)
        {
        }

        public AsyncResult(bool cancelable) : base(cancelable)
        {
        }

        /// <summary>
        /// The execution result
        /// </summary>
        public new virtual TResult Result
        {
            get
            {
                var result = base.Result;
                return result != null ? (TResult) result : default(TResult);
            }
        }

        public virtual void SetResult(TResult result)
        {
            base.SetResult(result);
        }

        protected override void RaiseOnCallback()
        {
            base.RaiseOnCallback();
            _callbackable?.RaiseOnCallback();
        }

        public new virtual ICallbackable<TResult> Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new Callbackable<TResult>(this));
            }
        }

        public new virtual ISynchronizable<TResult> Synchronized()
        {
            lock (Lock)
            {
                return this._synchronizable ?? (this._synchronizable = new Synchronizable<TResult>(this, this.Lock));
            }
        }
    }
}
