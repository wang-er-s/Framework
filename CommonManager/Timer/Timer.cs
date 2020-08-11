using System;
using UnityEngine;

namespace Framework
{
    public class Timer
    {
        /// <summary>
        /// How long the timer takes to complete from start to finish.
        /// </summary>
        private float _duration;

        private Action _onComplete;
        private Action<float> _onPerFrame;
        private Action<float> _onPerSecond;
        private Action<float> _onCountDown;

        private bool _isLoop;

        /// <summary>
        /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
        /// of the game(e.g. pausing, slow-mo), while game time is affected.
        /// </summary>
        private bool _usesRealTime;

        /// <summary>
        /// the GameObject bind to this timer.The timer will be destroyed On Update when the bind GameObject is null.
        /// </summary>
        private GameObject _autoDestroyOwner;
        
        internal bool IsCancelled { get; set; }

        private bool _hasAutoDestroyOwner;

        private float _startTime = 0f;
        private float _elapsedTimeTemp = 0f;
        private float _elapsedSecond = 0f;
        
        public static Timer CountDown(float duration,
            Action onComplete,
            Action<float> onCountDown)
        {
            return Register(duration, onComplete, null, null, onCountDown);
        }
        
        public static Timer Wait(float duration,
            Action onComplete)
        {
            return Register(duration, onComplete);
        }
        
        public static Timer RunPerFrame(Action<float> onPerFrame)
        {
            return Register(-1, null, onPerFrame);
        }
        
        public static Timer RunPerFrame(Action<float> onPerFrame, GameObject autoDestroyOwner)
        {
            return Register(-1,
                null,
                onPerFrame,
                null,
                null,
                false,
                false,
                autoDestroyOwner);
        }

        /// <summary>
        /// RunPerSecond function. onPerSecond will be fired in per second.
        /// The timer will be destoryed On Update when the bind gameobject is null.
        /// </summary>
        /// <param name="onPerSecond"></param>
        /// <param name="autoDestroyOwner"></param>
        /// <returns></returns>
        public static Timer RunPerSecond(Action<float> onPerSecond, GameObject autoDestroyOwner)
        {
            return Register(-1,
                null,
                null,
                onPerSecond,
                null,
                false,
                false,
                autoDestroyOwner);
        }

        public static Timer RunBySeconds(float duration, Action onComplete, GameObject autoDestroyOwner)
        {
            return Register(duration, onComplete, null, null, null, true, false, autoDestroyOwner);
        }

        public static Timer Register(float duration = 0,
            Action onComplete = null,
            Action<float> onPerFrame = null,
            Action<float> onPerSecond = null,
            Action<float> onCountDown = null,
            bool isLoop = false,
            bool useRealTime = false,
            GameObject autoDestroyOwner = null
        )
        {
            var timer = TimerManager.Instance.Get();
            timer.Reset();
            timer._duration = duration;
            timer._onComplete = onComplete;
            timer._onPerFrame = onPerFrame;
            timer._onPerSecond = onPerSecond;
            timer._onCountDown = onCountDown;
            timer._isLoop = isLoop;
            timer._usesRealTime = useRealTime;
            timer._hasAutoDestroyOwner = autoDestroyOwner != null;
            timer._autoDestroyOwner = autoDestroyOwner;
            timer._startTime = timer.GetWorldTime();
            timer._elapsedTimeTemp = 0f;
            timer._elapsedSecond = 0f;

            return timer;
        }

        public void Stop()
        {
            IsCancelled = true;
        }

        private float GetWorldTime()
        {
            return _usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }


        private float GetFireTime()
        {
            return _startTime + _duration;
        }
        
        public void Reset()
        {
            _duration = 0f;
            _onComplete = null;
            _onPerFrame = null;
            _onPerSecond = null;
            _onCountDown = null;
            _isLoop = false;
            _usesRealTime = false;
            _elapsedTimeTemp = 0f;
            _elapsedSecond = 0f;
            IsCancelled = false;
        }

        public void OnUpdate(float deltaTime)
        {
            if (IsCancelled) return;
            if (_hasAutoDestroyOwner && _autoDestroyOwner == null)
            {
                IsCancelled = true;
                return;
            }

            var worldTime = GetWorldTime();
            var fireTime = GetFireTime();

            if (_duration >= 0 && worldTime >= fireTime)
            {
                _onComplete?.Invoke();

                if (_isLoop)
                {
                    _startTime = worldTime;
                }
                else
                {
                    IsCancelled = true;
                    return;
                }
            }

            _onPerFrame?.Invoke(deltaTime);

            _elapsedTimeTemp += deltaTime;
            if (_elapsedTimeTemp >= 1f)
            {
                _elapsedTimeTemp -= 1;
                _elapsedSecond++;
                _onPerSecond?.Invoke(_elapsedSecond);

                if (_duration >= 0)
                {
                    var countDown = _duration - _elapsedSecond;
                    _onCountDown?.Invoke(countDown >= 1f ? countDown : 0f);
                }
            }
        }
    }
}