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
using System.Threading;

namespace Framework.Asynchronous
{
    public interface ISynchronizable
    {
        /// <summary>
        ///  Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        bool WaitForDone();

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        object WaitForResult(int millisecondsTimeout = 0);

        /// <summary>
        ///  Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        object WaitForResult(TimeSpan timeout);
    }

    public interface ISynchronizable<TResult> : ISynchronizable
    {
        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        new TResult WaitForResult(int millisecondsTimeout = 0);

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        new TResult WaitForResult(TimeSpan timeout);
    }

    internal class Synchronizable : ISynchronizable
    {
        private readonly IAsyncResult _result;
        private readonly object _lock;

        public Synchronizable(IAsyncResult result, object @lock)
        {
            this._result = result;
            this._lock = @lock;
        }

        /// <summary>
        /// Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        public bool WaitForDone()
        {
            if (_result.IsDone)
                return _result.IsDone;

            lock (_lock)
            {
                if (!_result.IsDone)
                    Monitor.Wait(_lock);
            }

            return _result.IsDone;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public object WaitForResult(int millisecondsTimeout = 0)
        {
            if (_result.IsDone)
            {
                if (_result.Exception != null)
                    throw _result.Exception;

                return _result.Result;
            }

            lock (_lock)
            {
                if (!_result.IsDone)
                {
                    if (millisecondsTimeout > 0)
                        Monitor.Wait(_lock, millisecondsTimeout);
                    else
                        Monitor.Wait(_lock);
                }
            }

            if (!_result.IsDone)
                throw new TimeoutException();

            if (_result.Exception != null)
                throw _result.Exception;

            return _result.Result;
        }

        /// <summary>
        ///  Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public object WaitForResult(TimeSpan timeout)
        {
            if (_result.IsDone)
            {
                if (_result.Exception != null)
                    throw _result.Exception;

                return _result.Result;
            }

            lock (_lock)
            {
                if (!_result.IsDone)
                {
                    Monitor.Wait(_lock, timeout);
                }
            }

            if (!_result.IsDone)
                throw new TimeoutException();

            if (_result.Exception != null)
                throw _result.Exception;

            return _result.Result;
        }
    }

    internal class Synchronizable<TResult> : ISynchronizable<TResult>
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Synchronizable<TResult>));

        private readonly IAsyncResult<TResult> _result;
        private readonly object _lock;

        public Synchronizable(IAsyncResult<TResult> result, object @lock)
        {
            this._result = result;
            this._lock = @lock;
        }

        /// <summary>
        /// Wait for done,will block the current thread.
        /// </summary>
        /// <returns></returns>
        public bool WaitForDone()
        {
            if (_result.IsDone)
                return _result.IsDone;

            lock (_lock)
            {
                if (!_result.IsDone)
                    Monitor.Wait(_lock);
            }

            return _result.IsDone;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public TResult WaitForResult(int millisecondsTimeout = 0)
        {
            if (_result.IsDone)
            {
                if (_result.Exception != null)
                    throw _result.Exception;

                return _result.Result;
            }

            lock (_lock)
            {
                if (!_result.IsDone)
                {
                    if (millisecondsTimeout > 0)
                        Monitor.Wait(_lock, millisecondsTimeout);
                    else
                        Monitor.Wait(_lock);
                }
            }

            if (!_result.IsDone)
                throw new TimeoutException();

            if (_result.Exception != null)
                throw _result.Exception;

            return _result.Result;
        }

        /// <summary>
        /// Wait for the result,will block the current thread.
        /// </summary>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public TResult WaitForResult(TimeSpan timeout)
        {
            if (_result.IsDone)
            {
                if (_result.Exception != null)
                    throw _result.Exception;

                return _result.Result;
            }

            lock (_lock)
            {
                if (!_result.IsDone)
                {
                    Monitor.Wait(_lock, timeout);
                }
            }

            if (!_result.IsDone)
                throw new TimeoutException();

            if (_result.Exception != null)
                throw _result.Exception;

            return _result.Result;
        }

        object ISynchronizable.WaitForResult(int millisecondsTimeout)
        {
            return WaitForResult(millisecondsTimeout);
        }

        object ISynchronizable.WaitForResult(TimeSpan timeout)
        {
            return WaitForResult(timeout);
        }
    }
}
