using System;
using System.Threading;
using System.Collections;
using Framework.Execution;

namespace Framework.Asynchronous
{
    public class AsyncTask : IAsyncTask
    {
        private readonly Action _action;

        private Action _preCallbackOnMainThread;
        private Action _preCallbackOnWorkerThread;

        private Action _postCallbackOnMainThread;
        private Action _postCallbackOnWorkerThread;

        private Action<Exception> _errorCallbackOnMainThread;
        private Action<Exception> _errorCallbackOnWorkerThread;

        private Action _finishCallbackOnMainThread;
        private Action _finishCallbackOnWorkerThread;

        private int _running;
        private readonly AsyncResult _result;

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Action task, bool runOnMainThread = false)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            this._result = new AsyncResult();
            if (runOnMainThread)
            {
                this._action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(task, true);
                    this._result.SetResult();
                });
            }
            else
            {
                this._action = WrapAction(() =>
                {
                    task();
                    this._result.SetResult();
                });
            }
        }

        public AsyncTask(Action<IPromise> task, bool runOnMainThread = false, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this._result = new AsyncResult(!runOnMainThread && cancelable);
            if (runOnMainThread)
            {
                this._action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(_result), true);
                    this._result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this._action = WrapAction(() =>
                {
                    task(_result);
                    this._result.Synchronized().WaitForResult();
                });
            }
        }

        /// <summary>
        /// run on main thread
        /// </summary>
        public AsyncTask(IEnumerator task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            this._result = new AsyncResult(cancelable);
            this._action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task, _result);
                this._result.Synchronized().WaitForResult();
            });
        }

        public virtual object Result => this._result.Result;

        public virtual Exception Exception => this._result.Exception;

        public virtual bool IsDone => this._result.IsDone && this._running == 0;

        public virtual bool IsCancelled => this._result.IsCancelled;

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

        public virtual bool Cancel()
        {
            return this._result.Cancel();
        }

        public virtual ICallbackable Callbackable()
        {
            return _result.Callbackable();
        }

        public virtual ISynchronizable Synchronized()
        {
            return _result.Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IAsyncTask OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._preCallbackOnMainThread += callback;
            else
                this._preCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnPostExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._postCallbackOnMainThread += callback;
            else
                this._postCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._errorCallbackOnMainThread += callback;
            else
                this._errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this._finishCallbackOnMainThread += callback;
            else
                this._finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsyncNoReturn(() =>
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

        public IAsyncTask Start()
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
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            Executors.RunAsync(this._action);

            return this;
        }
    }

    public class AsyncTask<TResult> : IAsyncTask<TResult>
    {

        private Action action;

        private Action preCallbackOnMainThread;
        private Action preCallbackOnWorkerThread;

        private Action<TResult> postCallbackOnMainThread;
        private Action<TResult> postCallbackOnWorkerThread;

        private Action<Exception> errorCallbackOnMainThread;
        private Action<Exception> errorCallbackOnWorkerThread;

        private Action finishCallbackOnMainThread;
        private Action finishCallbackOnWorkerThread;

        private int running = 0;
        private AsyncResult<TResult> result;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Func<TResult> task, bool runOnMainThread = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>();

            if (runOnMainThread)
            {
                this.action = WrapAction(() => Executors.RunOnMainThread(task));
            }
            else
            {
                this.action = WrapAction(task);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="runOnMainThread"></param>
        public AsyncTask(Action<IPromise<TResult>> task, bool runOnMainThread = false, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>(!runOnMainThread && cancelable);

            if (runOnMainThread)
            {
                this.action = WrapAction(() =>
                {
                    Executors.RunOnMainThread(() => task(this.result));
                    return this.result.Synchronized().WaitForResult();
                });
            }
            else
            {
                this.action = WrapAction(() =>
                {
                    task(this.result);
                    return this.result.Synchronized().WaitForResult();
                });
            }
        }

        public AsyncTask(Func<IPromise<TResult>, IEnumerator> task, bool cancelable = false)
        {
            if (task == null)
                throw new ArgumentNullException();

            this.result = new AsyncResult<TResult>(cancelable);
            this.action = WrapAction(() =>
            {
                Executors.RunOnCoroutine(task(this.result), this.result);
                return this.result.Synchronized().WaitForResult();
            });
        }

        public virtual TResult Result => this.result.Result;

        object Asynchronous.IAsyncResult.Result => this.result.Result;

        public virtual Exception Exception => this.result.Exception;

        public virtual bool IsDone => this.result.IsDone && this.running == 0;

        public virtual bool IsCancelled => this.result.IsCancelled;

        protected virtual Action WrapAction(Func<TResult> action)
        {
            void WrapFunc()
            {
                try
                {
                    try
                    {
                        if (this.preCallbackOnWorkerThread != null) this.preCallbackOnWorkerThread();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    if (this.result.IsCancellationRequested)
                    {
                        this.result.SetCancelled();
                        return;
                    }

                    TResult obj = action();
                    this.result.SetResult(obj);
                }
                catch (Exception e)
                {
                    this.result.SetException(e);
                }
                finally
                {
                    try
                    {
                        if (this.Exception != null)
                        {
                            if (this.errorCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this.errorCallbackOnMainThread(this.Exception), true);

                            errorCallbackOnWorkerThread?.Invoke(this.Exception);
                        }
                        else
                        {
                            if (this.postCallbackOnMainThread != null)
                                Executors.RunOnMainThread(() => this.postCallbackOnMainThread(this.Result), true);

                            postCallbackOnWorkerThread?.Invoke(this.Result);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    try
                    {
                        if (this.finishCallbackOnMainThread != null)
                            Executors.RunOnMainThread(this.finishCallbackOnMainThread, true);

                        finishCallbackOnWorkerThread?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }

                    Interlocked.Exchange(ref this.running, 0);
                }
            }

            return WrapFunc;
        }

        public virtual bool Cancel()
        {
            return this.result.Cancel();
        }

        public virtual ICallbackable<TResult> Callbackable()
        {
            return result.Callbackable();
        }

        public virtual ISynchronizable<TResult> Synchronized()
        {
            return result.Synchronized();
        }

        ICallbackable IAsyncResult.Callbackable()
        {
            return (result as IAsyncResult).Callbackable();
        }

        ISynchronizable IAsyncResult.Synchronized()
        {
            return (result as IAsyncResult).Synchronized();
        }

        public virtual object WaitForDone()
        {
            return Executors.WaitWhile(() => !IsDone);
        }

        public IAsyncTask<TResult> OnPreExecute(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.preCallbackOnMainThread += callback;
            else
                this.preCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnPostExecute(Action<TResult> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.postCallbackOnMainThread += callback;
            else
                this.postCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnError(Action<Exception> callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.errorCallbackOnMainThread += callback;
            else
                this.errorCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> OnFinish(Action callback, bool runOnMainThread = true)
        {
            if (runOnMainThread)
                this.finishCallbackOnMainThread += callback;
            else
                this.finishCallbackOnWorkerThread += callback;
            return this;
        }

        public IAsyncTask<TResult> Start(int delay)
        {
            if (delay <= 0)
                return this.Start();

            Executors.RunAsyncNoReturn(() =>
            {
                Thread.Sleep(delay);
                if (this.IsDone || this.running == 1)
                    return;

                this.Start();
            });

            return this;
        }

        public IAsyncTask<TResult> Start()
        {
            if (this.IsDone)
            {
                Log.Warning("The task has been done!");
                return this;
            }

            if (Interlocked.CompareExchange(ref this.running, 1, 0) == 1)
            {
                Log.Warning("The task is running!");
                return this;
            }

            try
            {
                if (this.preCallbackOnMainThread != null)
                    Executors.RunOnMainThread(this.preCallbackOnMainThread, true);
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }

            Executors.RunAsync(this.action);

            return this;
        }
    }
}
