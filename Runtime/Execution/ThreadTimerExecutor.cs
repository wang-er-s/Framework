using System;
using System.Collections.Generic;
using System.Threading;
using Framework.Asynchronous;
using JetBrains.Annotations;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

namespace Framework.Execution
{
    public class ThreadTimerExecutor : AbstractExecutor, ITimerExecutor
    {
        private readonly IComparer<IDelayTask> _comparer = new ComparerImpl<IDelayTask>();
        private readonly List<IDelayTask> _queue = new List<IDelayTask>();
        private readonly object _lock = new object();
        private bool _running;

        public ThreadTimerExecutor()
        {
        }

        public void Start()
        {
            if (this._running)
                return;

            this._running = true;
            Executors.RunAsync(() =>
            {
                while (_running)
                {
                    IDelayTask task;
                    lock (_lock)
                    {
                        if (_queue.Count <= 0)
                        {
                            Monitor.Wait(_lock);
                            continue;
                        }

                        task = _queue[0];
                        if (task.Delay.Ticks > 0)
                        {
                            Monitor.Wait(_lock, task.Delay);
                            continue;
                        }

                        _queue.RemoveAt(0);
                    }

                    task.Run();
                }
            });
        }

        public void Stop()
        {
            if (!this._running)
                return;

            lock (_lock)
            {
                this._running = false;
                Monitor.PulseAll(_lock);
            }

            List<IDelayTask> list = new List<IDelayTask>(this._queue);
            foreach (IDelayTask task in list)
            {
                if (task != null && !task.IsDone)
                    task.Cancel();
            }

            this._queue.Clear();
        }

        private void Add(IDelayTask task)
        {

            lock (_lock)
            {
                _queue.Add(task);
                _queue.Sort(_comparer);
                Monitor.PulseAll(_lock);
            }
        }

        private bool Remove(IDelayTask task)
        {
            lock (_lock)
            {
                if (_queue.Remove(task))
                {
                    _queue.Sort(_comparer);
                    Monitor.PulseAll(_lock);
                    return true;
                }
            }

            return false;
        }

        protected virtual void Check()
        {
            if (!this._running)
                throw new Exception("The ScheduledExecutor isn't started.");
        }

        public virtual Asynchronous.IAsyncResult Delay(Action command, long delay)
        {
            return Delay(command, GetTimeSpan(delay).Value);
        }

        public virtual Asynchronous.IAsyncResult Delay(Action command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask(this, command, delay);
        }

        public virtual IAsyncResult<TResult> Delay<TResult>(Func<TResult> command, long delay)
        {
            return Delay(command, GetTimeSpan(delay).Value);
        }

        public virtual IAsyncResult<TResult> Delay<TResult>(Func<TResult> command, TimeSpan delay)
        {
            this.Check();
            return new OneTimeDelayTask<TResult>(this, command, delay);
        }

        public virtual Asynchronous.IAsyncResult FixedRate(Action command, long initialDelay, long period)
        {
            return FixedRate(command, GetTimeSpan(initialDelay).Value, GetTimeSpan(period).Value);
        }

        public virtual Asynchronous.IAsyncResult FixedRate(Action command, TimeSpan initialDelay,
            TimeSpan period)
        {
            this.Check();
            return new FixedRateDelayTask(this, command, initialDelay, period);
        }

        public IAsyncResult FixedRateAtDuration(Action<long> fixedUpdateCommand, [CanBeNull] Action endCommand = null, long? initialDelay = null, long? period = null,
            long? duration = null)
        {
            return this.FixedRateAtDuration(fixedUpdateCommand, endCommand, GetTimeSpan(initialDelay),
                GetTimeSpan(period), GetTimeSpan(duration));
        }

        public IAsyncResult FixedRateAtDuration(Action<long> fixedUpdateCommand, [CanBeNull] Action endCommand = null, TimeSpan? initialDelay = null, TimeSpan? period = null,
            TimeSpan? duration = null)
        {
            Check();
            return new DurationFixedRateTask(this, fixedUpdateCommand, endCommand, initialDelay, period, duration);
        }
        
        private TimeSpan? GetTimeSpan(long? milliseconds)
        {
            if(milliseconds == null) return null;
            return new TimeSpan(milliseconds.Value * TimeSpan.TicksPerMillisecond);
        }

        public virtual void Dispose()
        {
            this.Stop();
        }

        interface IDelayTask : Asynchronous.IAsyncResult
        {
            TimeSpan Delay { get; }

            void Run();
        }

        class OneTimeDelayTask : AsyncResult, IDelayTask
        {
            private readonly long _startTime;
            private TimeSpan _delay;
            private readonly Action _wrappedAction;
            private readonly ThreadTimerExecutor _executor;

            public OneTimeDelayTask(ThreadTimerExecutor executor, Action command, TimeSpan delay) : base(true)
            {
                this._startTime = DateTime.Now.Ticks;
                this._delay = delay;
                this._executor = executor;
                this._wrappedAction = () =>
                {
                    try
                    {
                        if (this.IsDone)
                        {
                            return;
                        }

                        if (this.IsCancellationRequested)
                        {
                            this.SetCancelled();
                        }
                        else
                        {
                            command();
                            this.SetResult();
                        }
                    }
                    catch (Exception e)
                    {
                        this.SetException(e);
                    }
                };
                this._executor.Add(this);
            }

            public virtual TimeSpan Delay => new TimeSpan(_startTime + _delay.Ticks - DateTime.Now.Ticks);

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this._executor.Remove(this))
                    return false;

                this.CancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    Executors.RunAsync(() => this._wrappedAction());
                }
                catch (Exception)
                {
                }
            }
        }

        class OneTimeDelayTask<TResult> : AsyncResult<TResult>, IDelayTask
        {
            private readonly long _startTime;
            private TimeSpan _delay;
            private readonly Action _wrappedAction;
            private readonly ThreadTimerExecutor _executor;

            public OneTimeDelayTask(ThreadTimerExecutor executor, Func<TResult> command, TimeSpan delay)
            {
                this._startTime = DateTime.Now.Ticks;
                this._delay = delay;
                this._executor = executor;
                this._wrappedAction = () =>
                {
                    try
                    {
                        if (this.IsDone)
                        {
                            return;
                        }

                        if (this.IsCancellationRequested)
                        {
                            this.SetCancelled();
                        }
                        else
                        {
                            this.SetResult(command());
                        }
                    }
                    catch (Exception e)
                    {
                        this.SetException(e);
                    }
                };
                this._executor.Add(this);
            }

            public virtual TimeSpan Delay => new TimeSpan(_startTime + _delay.Ticks - DateTime.Now.Ticks);

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                if (!this._executor.Remove(this))
                    return false;

                this.CancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    Executors.RunAsync(() => this._wrappedAction());
                }
                catch (Exception)
                {
                }
            }
        }

        class FixedRateDelayTask : AsyncResult, IDelayTask
        {
            private readonly long _startTime;
            private TimeSpan _initialDelay;
            private TimeSpan _period;
            private readonly ThreadTimerExecutor _executor;
            private readonly Action _wrappedAction;
            private int _count = 0;

            public FixedRateDelayTask(ThreadTimerExecutor executor, Action command, TimeSpan initialDelay,
                TimeSpan period) : base()
            {
                this._startTime = DateTime.Now.Ticks;
                this._initialDelay = initialDelay;
                this._period = period;
                this._executor = executor;

                this._wrappedAction = () =>
                {
                    try
                    {
                        if (this.IsDone)
                            return;

                        if (this.IsCancellationRequested)
                        {
                            this.SetCancelled();
                        }
                        else
                        {
                            Interlocked.Increment(ref _count);
                            this._executor.Add(this);
                            command();
                        }
                    }
                    catch (Exception)
                    {
                    }
                };
                this._executor.Add(this);
            }

            public virtual TimeSpan Delay => new TimeSpan(_startTime + _initialDelay.Ticks + _period.Ticks * _count - DateTime.Now.Ticks);

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;

                this._executor.Remove(this);
                this.CancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    Executors.RunAsync(() => this._wrappedAction());
                }
                catch (Exception)
                {
                }
            }
        }
        
        class DurationFixedRateTask : AsyncResult, IDelayTask
        {
            private readonly long _startTime;
            private TimeSpan _initialDelay;
            private TimeSpan _period;
            private TimeSpan _timer;
            private readonly ThreadTimerExecutor _executor;
            private int _count;
            private readonly Action _wrappedAction;
            private readonly Action _endCommand;

            public DurationFixedRateTask(ThreadTimerExecutor executor, Action<long> fixedUpdateCommand,
                Action endCommand, TimeSpan? initialDelay, TimeSpan? period, TimeSpan? duration) : base()
            {
                this._startTime = DateTime.Now.Ticks;
                this._initialDelay = initialDelay ?? TimeSpan.Zero;
                this._period = period ?? TimeSpan.FromMilliseconds(10);
                this._executor = executor;
                this._endCommand = endCommand;
                this._executor.Add(this);

                this._wrappedAction = () =>
                {
                    try
                    {
                        if (this.IsDone)
                            return;

                        if (this.IsCancellationRequested)
                        {
                            this.SetCancelled();
                        }
                        else
                        {
                            if (_count >= 1)
                                _timer += _period;
                            if (duration.HasValue && _timer >= duration)
                            {
                                Cancel();
                            }
                            fixedUpdateCommand((long) _timer.TotalMilliseconds);
                            this._executor.Add(this);
                            Interlocked.Increment(ref _count);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                    }
                };
            }

            public virtual TimeSpan Delay =>
                new TimeSpan(_startTime + _initialDelay.Ticks + _period.Ticks * _count - DateTime.Now.Ticks);

            public override bool Cancel()
            {
                if (this.IsDone)
                    return false;
                _endCommand();
                this._executor.Remove(this);
                this.CancellationRequested = true;
                this.SetCancelled();
                return true;
            }

            public virtual void Run()
            {
                try
                {
                    Executors.RunAsync(() => this._wrappedAction());
                }
                catch (Exception)
                {
                }
            }
        }

        class ComparerImpl<T> : IComparer<T> where T : IDelayTask
        {
            public int Compare(T x, T y)
            {
                if (x.Delay.Ticks == y.Delay.Ticks)
                    return 0;

                return x.Delay.Ticks > y.Delay.Ticks ? 1 : -1;
            }
        }
    }
}
