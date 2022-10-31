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
    public class Progress<T> : IProgress<T>
    {
        private readonly bool _runOnMainThread;
        private readonly Action<T> _handler;

        public Progress() : this(null, true)
        {
        }

        public Progress(Action<T> handler) : this(handler, true)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
        }

        public Progress(Action<T> handler, bool runOnMainThread)
        {
            this._handler = handler;
            this._runOnMainThread = runOnMainThread;
        }

        public event EventHandler<T> ProgressChanged;

        protected virtual void OnReport(T value)
        {
            try
            {
                Action<T> handler = this._handler;
                EventHandler<T> changedEvent = ProgressChanged;
                if (handler != null || changedEvent != null)
                {
                    if (_runOnMainThread)
                        Executors.RunOnMainThread(() => { RaiseProgressChanged(value); });
                    else
                        RaiseProgressChanged(value);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        void IProgress<T>.Report(T value)
        {
            OnReport(value);
        }

        private void RaiseProgressChanged(T value)
        {
            Action<T> handler = this._handler;
            EventHandler<T> progressChanged = this.ProgressChanged;

            handler?.Invoke(value);
            progressChanged?.Invoke(this, value);
        }
    }
}
