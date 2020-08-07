using System;
using Framework.Execution;

namespace Framework.Net
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
