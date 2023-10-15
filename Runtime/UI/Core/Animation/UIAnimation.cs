using System;
using UnityEngine;

namespace Framework
{
    public enum AnimationType
    {
        EnterAnimation,
        ExitAnimation,
        ActivationAnimation,
        PassivationAnimation
    }

    public abstract class UIAnimation : MonoBehaviour, IAnimation
    {
        [SerializeField]
        private AnimationType animationType;

        public AnimationType AnimationType
        {
            get => this.animationType;
            set => this.animationType = value;
        }

        protected View View;

        private Action _onStart;
        private Action _onEnd;

        public IAnimation SetView(View view)
        {
            this.View = view;
            return this;
        }

        protected void OnStart()
        {
            try
            {
                if (_onStart != null)
                {
                    _onStart();
                    _onStart = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        protected void OnEnd()
        {
            try
            {
                if (_onEnd != null)
                {
                    _onEnd();
                    _onEnd = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public IAnimation OnStart(Action onStart)
        {
            _onStart = onStart;
            return this;
        }

        public IAnimation OnEnd(Action onEnd)
        {
            _onEnd = onEnd;
            return this;
        }


        public abstract IAnimation Play();
    }
}