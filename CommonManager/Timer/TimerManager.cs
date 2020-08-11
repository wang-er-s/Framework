using System.Collections.Generic;
using Framework.CommonManager;
using Framework.Pool;
using Framework.Pool.Factory;

namespace Framework
{
    public class TimerManager : IManager
    {
        public void Init(params object[] para)
        {
            Instance = this;
        }

        private readonly List<Timer> _timers = new List<Timer>();
        private readonly ObjectPool<Timer> _objectPool = new ObjectPool<Timer>(new DefaultFactory<Timer>());
        public static TimerManager Instance;
        

        public Timer Get()
        {
            var timer = _objectPool.Allocate();
            _timers.Add(timer);

            return timer;
        }


        public void Update(float deltaTime)
        {
            RemoveTimer();

            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                _timers[i].OnUpdate(deltaTime);
            }

            RemoveTimer();
        }

        public void RemoveTimer()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer.IsCancelled)
                {
                    timer.Reset();
                    _timers.RemoveAt(i);
                    _objectPool.Free(timer);
                }
            }
        }

    }
}