/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

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
        
        private static IProgressResult<TProgress> voidResult;
        
        /// <summary>
        /// 返回一个完成的IProgressResult<float>
        /// </summary>
        /// <returns></returns>
        public new static IProgressResult<TProgress> Void()
        {
            if (voidResult == null)
            {
                var result = new ProgressResult<TProgress>();
                result.SetResult();
                voidResult = result;
            }
            return voidResult;
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
        
        private static IProgressResult<TProgress, TResult> voidResult;
        
        /// <summary>
        /// 返回一个完成的IProgressResult<float>
        /// </summary>
        /// <returns></returns>
        public new static IProgressResult<TProgress, TResult> Void()
        {
            if (voidResult == null)
            {
                var result = new ProgressResult<TProgress, TResult>();
                result.SetResult();
                voidResult = result;
            }
            return voidResult;
        }
    }
}
