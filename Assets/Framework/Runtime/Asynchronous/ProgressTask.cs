using System;
using System.Threading;
using System.Collections;
using Framework.Execution;

namespace Framework.Asynchronous
{
    public class ProgressTask<TProgress> : IProgressTask<TProgress>
    {
        private readonly Action _action;

        private Action _preCallbackOnMainThread;
        private Action _preCallbackOnWorkerThread;

        private Action _postCallbackOnMainThread;
        private Action _postCallbackOnWorkerThread;

        private Action<TProgress> _progressCallbackOnMainThread;
        private Action<TProgress> _progressCallbackOnWorkerThread;

        private Action<Exception> _errorCallbackOnMainThread;
        private Action<Exception> _errorCallbackOnWorkerThread;

        private Action _finishCallbackOnMainThread;
        private Action _finishCallbackOnWorkerThread;

        private int _running;
        private readonly ProgressResult<TProgress> _result;

        public ProgressTask(Action<IProgressPromise<TProgress>> task, bool runOnMainThread = false,
            bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this._result = new ProgressResult<TProgress>(!runOnMainThread && cancelable);
            this._result.Callbackable().OnProgressCallback(OnProgressChanged);
            if (runOnMainThread)
            {
                this._action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(_result), true);
                    _result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this._action = WrapAction(() =>
                {
                    task(_result);
                    _result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// run on main thread.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="cancelable"></param>
        public ProgressTask(Func<IProgressPromise<TProgress>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this._result = new ProgressResult<TProgress>(cancelable);
            this._result.Callbackable().OnProgressCallback(OnProgressChanged);
            this._action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task(this._result), this._result);
                this._result.Synchronized().WaitForResult();
            });
        }

        public virtual object Result => this._result.Result;

        public virtual Exception Exception => this._result.Exception;

        public virtual bool IsDone => this._result.IsDone && this._running == 0;

        public virtual bool IsCancelled => this._result.IsCancelled;

        public virtual TProgress Progress => this._result.Progress;

        protected virtual Action WrapAction(Action action)
        {
            void WrapFunc()
            {
                try
                {
                    try
                    {
                        _preCallbackOnWorkerThread?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    if (this._result.IsCancellationRequested)
                    {
                        this._result.SetCancelled();
                        return;
                    }

                    action();
                }
                catch (Exception e)
                {
                    this._result.SetException(e);
                }
                finally
                {
                    try
                    {
                        if (this.Exception != null)
                        {
                            if (this._errorCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this._errorCallbackOnMainThread(this.Exception), true);

                            _errorCallbackOnWorkerThread?.Invoke(this.Exception);
                        }
                        else
                        {
                            if (this._postCallbackOnMainThread != null)
                                Executors.RunOnMainThread(this._postCallbackOnMainThread, true);

                            _postCallbackOnWorkerThread?.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    try
                    {
                        if (this._finishCallbackOnMainThread != null)
                            Executors.RunOnMainThread(this._finishCallbackOnMainThread, true);

                        _finishCallbackOnWorkerThread?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    Interlocked.Exchange(ref this._running, 0);
                }
            }

            return WrapFunc;
        }

        protected virtual IEnumerator DoUpdateProgressOnMainThread()
        {
            while (!_result.IsDone)
            {
                try
                {
                    _progressCallbackOnMainThread?.Invoke(this._result.Progress);
                }
                catch (Exception e)
                {
                    Log.Warning(e);
                }

                yield return null;
            }
        }

        protected virtual void OnProgressChanged(TProgress progress)
        {
            try
            {
                if (this._result.IsDone || this._progressCallbackOnWorkerThread == null)
                    return;

                this._progressCallbackOnWorkerThread(progress);
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }

        public virtual bool Cancel()
        {
            return this._result.Cancel();
        }

        public virtual IProgressCallbackable<TProgress> Callbackable()
        {
            return _result.Callbackable();
        }

        ICallbackable IAsyncResult.Callbackable()
        {
            return (_result as IAsyncResult).Callbackable();
        }

        public virtual ISynchronizable Synchronized()
        {
            return _result.Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IProgressTask<TProgress> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._preCallbackOnMainThread += callback;
            else
                this._preCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnPostExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._postCallbackOnMainThread += callback;
            else
                this._postCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._errorCallbackOnMainThread += callback;
            else
                this._errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnProgressUpdate(Action<TProgress> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._progressCallbackOnMainThread += callback;
            else
                this._progressCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._finishCallbackOnMainThread += callback;
            else
                this._finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress> Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsync(() =>
            {
#if NETFX_CORE
                Task.Delay(delay).Wait();
#else
                Thread.Sleep(delay);
#endif
                if (this.IsDone || this._running == 1)
                    return;

                this.Start();
            });
            return this;
        }

        public IProgressTask<TProgress> Start()
        {
            if (this.IsDone)
            {
                Log.Warning("The task has been done!");
                return this;
            }

            if (Interlocked.CompareExchange(ref this._running, 1, 0) == 1)
            {
                Log.Warning("The task is running!");
                return this;
            }

            try
            {
                if (this._preCallbackOnMainThread != null)
                    Executors.RunOnMainThread(this._preCallbackOnMainThread, true);

                if (this._progressCallbackOnMainThread != null)
                    Executors.RunOnCoroutineNoReturn(DoUpdateProgressOnMainThread());
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            Executors.RunAsync(this._action);

            return this;
        }


    }

    public class ProgressTask<TProgress, TResult> : IProgressTask<TProgress, TResult>
    {
        private readonly Action _action;

        private Action _preCallbackOnMainThread;
        private Action _preCallbackOnWorkerThread;

        private Action<TResult> _postCallbackOnMainThread;
        private Action<TResult> _postCallbackOnWorkerThread;

        private Action<TProgress> _progressCallbackOnMainThread;
        private Action<TProgress> _progressCallbackOnWorkerThread;

        private Action<Exception> _errorCallbackOnMainThread;
        private Action<Exception> _errorCallbackOnWorkerThread;

        private Action _finishCallbackOnMainThread;
        private Action _finishCallbackOnWorkerThread;

        private int _running;
        private readonly ProgressResult<TProgress, TResult> _result;

        public ProgressTask(Action<IProgressPromise<TProgress, TResult>> task, bool runOnMainThread,
            bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this._result = new ProgressResult<TProgress, TResult>(!runOnMainThread && cancelable);
            this._result.Callbackable().OnProgressCallback(OnProgressChanged);

            if (runOnMainThread)
            {
                this._action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(_result), true);
                    return this._result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this._action = WrapAction(() =>
                {
                    task(_result);
                    return this._result.Synchronized().WaitForResult();
                });
            }
        }

        public ProgressTask(Func<IProgressPromise<TProgress, TResult>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this._result = new ProgressResult<TProgress, TResult>(cancelable);
            this._result.Callbackable().OnProgressCallback(OnProgressChanged);
            this._action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task(this._result), this._result);
                return this._result.Synchronized().WaitForResult();
            });
        }

        public virtual TResult Result => this._result.Result;

        object Asynchronous.IAsyncResult.Result => this._result.Result;

        public virtual Exception Exception => this._result.Exception;

        public virtual bool IsDone => this._result.IsDone && this._running == 0;

        public virtual bool IsCancelled => this._result.IsCancelled;

        public virtual TProgress Progress => this._result.Progress;

        protected virtual Action WrapAction(Func<TResult> action)
        {
            void WrapAction()
            {
                try
                {
                    try
                    {
                        if (this._preCallbackOnWorkerThread != null) this._preCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    if (this._result.IsCancellationRequested)
                    {
                        this._result.SetCancelled();
                        return;
                    }

                    TResult obj = action();
                    this._result.SetResult(obj);
                }
                catch (Exception e)
                {
                    this._result.SetException(e);
                }
                finally
                {
                    try
                    {
                        if (this.Exception != null)
                        {
                            if (this._errorCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this._errorCallbackOnMainThread(this.Exception), true);

                            _errorCallbackOnWorkerThread?.Invoke(this.Exception);
                        }
                        else
                        {
                            if (this._postCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this._postCallbackOnMainThread(this.Result), true);

                            _postCallbackOnWorkerThread?.Invoke(this.Result);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    try
                    {
                        if (this._finishCallbackOnMainThread != null)
                            Executors.RunOnMainThread(this._finishCallbackOnMainThread, true);

                        _finishCallbackOnWorkerThread?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    Interlocked.Exchange(ref this._running, 0);
                }
            }

            return WrapAction;
        }

        protected virtual IEnumerator DoUpdateProgressOnMainThread()
        {
            while (!_result.IsDone)
            {
                try
                {
                    _progressCallbackOnMainThread?.Invoke(this._result.Progress);
                }
                catch (Exception e)
                {
                    Log.Warning(e);
                }

                yield return null;
            }
        }

        protected virtual void OnProgressChanged(TProgress progress)
        {
            try
            {
                if (this._result.IsDone || this._progressCallbackOnWorkerThread == null)
                    return;

                this._progressCallbackOnWorkerThread(progress);
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
        }

        public virtual bool Cancel()
        {
            return this._result.Cancel();
        }

        public virtual IProgressCallbackable<TProgress, TResult> Callbackable()
        {
            return _result.Callbackable();
        }

        public virtual ISynchronizable<TResult> Synchronized()
        {
            return _result.Synchronized();
        }

        ICallbackable IAsyncResult.Callbackable()
        {

            return (_result as IAsyncResult).Callbackable();
        }

        ICallbackable<TResult> IAsyncResult<TResult>.Callbackable()
        {
            return (_result as IAsyncResult<TResult>).Callbackable();
        }

        IProgressCallbackable<TProgress> IProgressResult<TProgress>.Callbackable()
        {
            return (_result as IProgressResult<TProgress>).Callbackable();
        }

        ISynchronizable IAsyncResult.Synchronized()
        {
            return (_result as IAsyncResult).Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IProgressTask<TProgress, TResult> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._preCallbackOnMainThread += callback;
            else
                this._preCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._postCallbackOnMainThread += callback;
            else
                this._postCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._errorCallbackOnMainThread += callback;
            else
                this._errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnProgressUpdate(Action<TProgress> callback,
            bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._progressCallbackOnMainThread += callback;
            else
                this._progressCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._finishCallbackOnMainThread += callback;
            else
                this._finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IProgressTask<TProgress, TResult> Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsync(() =>
            {
                Thread.Sleep(delay);
                if (this.IsDone || this._running == 1)
                    return;

                this.Start();
            });
            return this;
        }

        public IProgressTask<TProgress, TResult> Start()
        {
            if (this.IsDone)
            {
                Log.Warning("The task has been done!");
                return this;
            }

            if (Interlocked.CompareExchange(ref this._running, 1, 0) == 1)
            {
                Log.Warning("The task is running!");
                return this;
            }

            try
            {
                if (this._preCallbackOnMainThread != null)
                    Executors.RunOnMainThread(this._preCallbackOnMainThread, true);

                if (this._progressCallbackOnMainThread != null)
                    Executors.RunOnCoroutineNoReturn(DoUpdateProgressOnMainThread());
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            Executors.RunAsync(this._action);
            return this;
        }
    }
}
