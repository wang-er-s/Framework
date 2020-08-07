using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Framework.Asynchronous
{
    public class CoroutineAwaiter : IAwaiter
    {
        protected readonly object Lock = new object();
        protected bool Done = false;
        protected Exception Exception;
        protected Action Continuation;

        public bool IsCompleted => this.Done;

        public void GetResult()
        {
            lock (Lock)
            {
                if (!Done)
                    throw new Exception("The task is not finished yet");
            }

            if (Exception != null)
                ExceptionDispatchInfo.Capture(Exception).Throw();
        }

        public void SetResult(Exception exception)
        {
            lock (Lock)
            {
                if (Done)
                    return;

                this.Exception = exception;
                this.Done = true;
                try
                {
                    Continuation?.Invoke();
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.Continuation = null;
                }
            }
        }

        public void OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion) this).UnsafeOnCompleted(continuation);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            lock (Lock)
            {
                if (this.Done)
                {
                    continuation();
                }
                else
                {
                    this.Continuation += continuation;
                }
            }
        }
    }

    public class CoroutineAwaiter<T> : CoroutineAwaiter, IAwaiter<T>
    {
        protected T Result;

        public CoroutineAwaiter()
        {
        }

        public new T GetResult()
        {
            lock (Lock)
            {
                if (!Done)
                    throw new Exception("The task is not finished yet");
            }

            if (Exception != null)
                ExceptionDispatchInfo.Capture(Exception).Throw();

            return Result;
        }

        public void SetResult(T result, Exception exception)
        {
            lock (Lock)
            {
                if (Done)
                    return;

                this.Result = result;
                this.Exception = exception;
                this.Done = true;
                try
                {
                    Continuation?.Invoke();
                }
                catch (Exception)
                {
                }
                finally
                {
                    this.Continuation = null;
                }
            }
        }
    }

    public struct AsyncOperationAwaiter : IAwaiter
    {
        private readonly AsyncOperation _asyncOperation;
        private Action<AsyncOperation> _continuationAction;

        public AsyncOperationAwaiter(AsyncOperation asyncOperation)
        {
            this._asyncOperation = asyncOperation;
            this._continuationAction = null;
        }

        public bool IsCompleted => _asyncOperation.isDone;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (_continuationAction != null)
                _asyncOperation.completed -= _continuationAction;
            _continuationAction = null;
        }

        public void OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion) this).UnsafeOnCompleted(continuation);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            if (_asyncOperation.isDone)
            {
                continuation();
            }
            else
            {
                _continuationAction = (ao) => { continuation(); };
                _asyncOperation.completed += _continuationAction;
            }
        }
    }

    public struct AsyncOperationAwaiter<T, TResult> : IAwaiter<TResult>
        where T : AsyncOperation
    {
        private readonly T _asyncOperation;
        private readonly Func<T, TResult> _getter;
        private Action<AsyncOperation> _continuationAction;

        public AsyncOperationAwaiter(T asyncOperation, Func<T, TResult> getter)
        {
            this._asyncOperation = asyncOperation ?? throw new ArgumentNullException(nameof(asyncOperation));
            this._getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this._continuationAction = null;
        }

        public bool IsCompleted => _asyncOperation.isDone;

        public TResult GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (_continuationAction != null)
            {
                _asyncOperation.completed -= _continuationAction;
                _continuationAction = null;
            }

            return _getter(_asyncOperation);
        }

        public void OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion) this).UnsafeOnCompleted(continuation);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            if (_asyncOperation.isDone)
            {
                continuation();
            }
            else
            {
                _continuationAction = (ao) => { continuation(); };
                _asyncOperation.completed += _continuationAction;
            }
        }
    }

    public struct AsyncResultAwaiter<T> : IAwaiter where T : IAsyncResult
    {
        private T _asyncResult;

        public AsyncResultAwaiter(T asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));
            this._asyncResult = asyncResult;
        }

        public bool IsCompleted => _asyncResult.IsDone;

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (_asyncResult.Exception != null)
                ExceptionDispatchInfo.Capture(_asyncResult.Exception).Throw();
        }

        public void OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion) this).UnsafeOnCompleted(continuation);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));
            _asyncResult.Callbackable().OnCallback((ar) => { continuation(); });
        }
    }

    public struct AsyncResultAwaiter<T, TResult> : IAwaiter<TResult>
        where T : IAsyncResult<TResult>
    {
        private T _asyncResult;

        public AsyncResultAwaiter(T asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));
            this._asyncResult = asyncResult;
        }

        public bool IsCompleted => _asyncResult.IsDone;

        public TResult GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");

            if (_asyncResult.Exception != null)
                ExceptionDispatchInfo.Capture(_asyncResult.Exception).Throw();

            return this._asyncResult.Result;
        }

        public void OnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion) this).UnsafeOnCompleted(continuation);
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));
            _asyncResult.Callbackable().OnCallback((ar) => { continuation(); });
        }
    }
}