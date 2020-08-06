using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Execution
{
    /// <summary>
    /// Interceptable enumerator
    /// </summary>
    public class InterceptableEnumerator : IEnumerator
    {
        private object _current;
        private readonly Stack<IEnumerator> _stack = new Stack<IEnumerator>();

        private Action<Exception> _onException;
        private Action _onFinally;
        private Func<bool> _hasNext;

        public InterceptableEnumerator(IEnumerator routine)
        {
            this._stack.Push(routine);
        }

        public object Current => this._current;

        public bool MoveNext()
        {
            try
            {
                if (!this.HasNext())
                {
                    this.OnFinally();
                    return false;
                }

                if (_stack.Count <= 0)
                {
                    this.OnFinally();
                    return false;
                }

                IEnumerator ie = _stack.Peek();
                bool hasNext = ie.MoveNext();
                if (!hasNext)
                {
                    this._stack.Pop();
                    return MoveNext();
                }

                this._current = ie.Current;
                if (this._current is IEnumerator current)
                {
                    _stack.Push(current);
                    return MoveNext();
                }

                if (this._current is Coroutine)
                    Log.Warning(
                        "The Enumerator's results contains the 'UnityEngine.Coroutine' type,If occurs an exception,it can't be catched.It is recommended to use 'yield return routine',rather than 'yield return StartCoroutine(routine)'.");

                return true;
            }
            catch (Exception e)
            {
                this.OnException(e);
                this.OnFinally();
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private void OnException(Exception e)
        {
            try
            {
                if (this._onException == null)
                    return;

                foreach (var @delegate in this._onException.GetInvocationList())
                {
                    var action = (Action<Exception>) @delegate;
                    try
                    {
                        action(e);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnFinally()
        {
            try
            {
                if (this._onFinally == null)
                    return;

                foreach (var @delegate in this._onFinally.GetInvocationList())
                {
                    var action = (Action) @delegate;
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private bool HasNext()
        {
            return _hasNext == null || _hasNext();
        }

        /// <summary>
        /// Register a condition code block.
        /// </summary>
        /// <param name="hasNext"></param>
        public virtual void RegisterConditionBlock(Func<bool> hasNext)
        {
            this._hasNext = hasNext;
        }

        /// <summary>
        /// Register a code block, when an exception occurs it will be executed.
        /// </summary>
        /// <param name="onException"></param>
        public virtual void RegisterCatchBlock(Action<Exception> onException)
        {
            this._onException += onException;
        }

        /// <summary>
        /// Register a code block, when the end of the operation is executed.
        /// </summary>
        /// <param name="onFinally"></param>
        public virtual void RegisterFinallyBlock(Action onFinally)
        {
            this._onFinally += onFinally;
        }
    }
}
