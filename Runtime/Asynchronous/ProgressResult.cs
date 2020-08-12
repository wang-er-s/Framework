namespace Framework.Asynchronous
{
    public class ProgressResult<TProgress> : AsyncResult, IProgressResult<TProgress>, IProgressPromise<TProgress>
    {
        private ProgressCallbackable<TProgress> _callbackable;

        public ProgressResult() : this(false)
        {
        }

        public ProgressResult(bool cancelable) : base(cancelable)
        {
        }

        /// <summary>
        /// The task's progress.
        /// </summary>
        public virtual TProgress Progress { get; protected set; }

        protected override void RaiseOnCallback()
        {
            base.RaiseOnCallback();
            _callbackable?.RaiseOnCallback();
        }

        protected virtual void RaiseOnProgressCallback(TProgress progress)
        {
            _callbackable?.RaiseOnProgressCallback(progress);
        }

        public new virtual IProgressCallbackable<TProgress> Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new ProgressCallbackable<TProgress>(this));
            }
        }

        public virtual void UpdateProgress(TProgress progress)
        {
            Progress = progress;
            RaiseOnProgressCallback(progress);
        }
    }

    public class ProgressResult<TProgress, TResult> : ProgressResult<TProgress>, IProgressResult<TProgress, TResult>,
        IProgressPromise<TProgress, TResult>
    {
        private Callbackable<TResult> _callbackable;
        private ProgressCallbackable<TProgress, TResult> _progressCallbackable;
        private Synchronizable<TResult> _synchronizable;

        public ProgressResult() : this(false)
        {
        }

        public ProgressResult(bool cancelable) : base(cancelable)
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
            _progressCallbackable?.RaiseOnCallback();
        }

        protected override void RaiseOnProgressCallback(TProgress progress)
        {
            base.RaiseOnProgressCallback(progress);
            _progressCallbackable?.RaiseOnProgressCallback(progress);
        }

        public new virtual IProgressCallbackable<TProgress, TResult> Callbackable()
        {
            lock (Lock)
            {
                return this._progressCallbackable ??
                       (this._progressCallbackable = new ProgressCallbackable<TProgress, TResult>(this));
            }
        }

        public new virtual ISynchronizable<TResult> Synchronized()
        {
            lock (Lock)
            {
                return this._synchronizable ?? (this._synchronizable = new Synchronizable<TResult>(this, this.Lock));
            }
        }

        ICallbackable<TResult> IAsyncResult<TResult>.Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new Callbackable<TResult>(this));
            }
        }
    }
}
