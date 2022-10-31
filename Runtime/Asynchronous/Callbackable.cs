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

using System;

namespace Framework
{
    public interface ICallbackable
    {
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IAsyncResult> callback);
    }

    public interface ICallbackable<TResult>
    {
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IAsyncResult<TResult>> callback);
    }

    public interface IProgressCallbackable<TProgress>
    {
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IProgressResult<TProgress>> callback);

        /// <summary>
        /// Called when the progress update.
        /// </summary>
        /// <param name="callback"></param>
        void OnProgressCallback(Action<TProgress> callback);
    }

    public interface IProgressCallbackable<TProgress, TResult>
    {
        /// <summary>
        /// Called when the task is finished.
        /// </summary>
        /// <param name="callback"></param>
        void OnCallback(Action<IProgressResult<TProgress, TResult>> callback);

        /// <summary>
        /// Called when the progress update.
        /// </summary>
        /// <param name="callback"></param>
        void OnProgressCallback(Action<TProgress> callback);
    }

    internal class Callbackable : ICallbackable
    {

        private readonly IAsyncResult _result;
        private readonly object _lock = new object();
        private Action<IAsyncResult> _callback;

        public Callbackable(IAsyncResult result)
        {
            this._result = result;
        }

        public void RaiseOnCallback()
        {
            lock (_lock)
            {
                try
                {
                    if (this._callback == null)
                        return;

                    var list = this._callback.GetInvocationList();
                    this._callback = null;

                    foreach (var @delegate in list)
                    {
                        var action = (Action<IAsyncResult>) @delegate;
                        try
                        {
                            action(this._result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                }
            }
        }

        public void OnCallback(Action<IAsyncResult> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this._result.IsDone)
                {
                    try
                    {
                        callback(this._result);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this._callback += callback;
            }
        }
    }

    internal class Callbackable<TResult> : ICallbackable<TResult>
    {

        private IAsyncResult<TResult> result;
        private readonly object _lock = new object();
        private Action<IAsyncResult<TResult>> callback;

        public Callbackable(IAsyncResult<TResult> result)
        {
            this.result = result;
        }

        public void RaiseOnCallback()
        {
            lock (_lock)
            {
                try
                {
                    if (this.callback == null)
                        return;

                    var list = this.callback.GetInvocationList();
                    this.callback = null;

                    foreach (var @delegate in list)
                    {
                        var action = (Action<IAsyncResult<TResult>>) @delegate;
                        try
                        {
                            action(this.result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                }
            }
        }

        public void OnCallback(Action<IAsyncResult<TResult>> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this.result.IsDone)
                {
                    try
                    {
                        callback(this.result);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this.callback += callback;
            }
        }
    }

    internal class ProgressCallbackable<TProgress> : IProgressCallbackable<TProgress>
    {

        private readonly IProgressResult<TProgress> _result;
        private readonly object _lock = new object();
        private Action<IProgressResult<TProgress>> _callback;
        private Action<TProgress> _progressCallback;

        public ProgressCallbackable(IProgressResult<TProgress> result)
        {
            this._result = result;
        }

        public void RaiseOnCallback()
        {
            lock (_lock)
            {
                try
                {
                    if (this._callback == null)
                        return;

                    var list = this._callback.GetInvocationList();
                    this._callback = null;

                    foreach (var @delegate in list)
                    {
                        var action = (Action<IProgressResult<TProgress>>) @delegate;
                        try
                        {
                            action(this._result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                }
                finally
                {
                    this._progressCallback = null;
                }
            }
        }

        public void RaiseOnProgressCallback(TProgress progress)
        {
            lock (_lock)
            {
                try
                {
                    if (this._progressCallback == null)
                        return;

                    var list = this._progressCallback.GetInvocationList();
                    foreach (var @delegate in list)
                    {
                        var action = (Action<TProgress>) @delegate;
                        try
                        {
                            action(progress);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                }
            }
        }

        public void OnCallback(Action<IProgressResult<TProgress>> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this._result.IsDone)
                {
                    try
                    {
                        callback(this._result);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this._callback += callback;
            }
        }

        public void OnProgressCallback(Action<TProgress> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this._result.IsDone)
                {
                    try
                    {
                        callback(this._result.Progress);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this._progressCallback += callback;
            }
        }
    }

    internal class ProgressCallbackable<TProgress, TResult> : IProgressCallbackable<TProgress, TResult>
    {

        private readonly IProgressResult<TProgress, TResult> _result;
        private readonly object _lock = new object();
        private Action<IProgressResult<TProgress, TResult>> _callback;
        private Action<TProgress> _progressCallback;

        public ProgressCallbackable(IProgressResult<TProgress, TResult> result)
        {
            this._result = result;
        }

        public void RaiseOnCallback()
        {
            lock (_lock)
            {
                try
                {
                    if (this._callback == null)
                        return;

                    var list = this._callback.GetInvocationList();
                    this._callback = null;

                    foreach (var @delegate in list)
                    {
                        var action = (Action<IProgressResult<TProgress, TResult>>) @delegate;
                        try
                        {
                            action(this._result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    throw;
                }
                finally
                {
                    this._progressCallback = null;
                }
            }
        }

        public void RaiseOnProgressCallback(TProgress progress)
        {
            lock (_lock)
            {
                try
                {
                    if (this._progressCallback == null)
                        return;

                    var list = this._progressCallback.GetInvocationList();
                    foreach (var @delegate in list)
                    {
                        var action = (Action<TProgress>) @delegate;
                        try
                        {
                            action(progress);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                }
            }
        }

        public void OnCallback(Action<IProgressResult<TProgress, TResult>> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this._result.IsDone)
                {
                    try
                    {
                        callback(this._result);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this._callback += callback;
            }
        }

        public void OnProgressCallback(Action<TProgress> callback)
        {
            lock (_lock)
            {
                if (callback == null)
                    return;

                if (this._result.IsDone)
                {
                    try
                    {
                        callback(this._result.Progress);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Class[{GetType()}] callback exception.Error:{e}");
                    }

                    return;
                }

                this._progressCallback += callback;
            }
        }
    }
}