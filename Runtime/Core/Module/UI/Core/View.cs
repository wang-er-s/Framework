using System;
using UnityEngine;

namespace Framework
{
    public abstract class View : Entity, IAwakeSystem, IDestroySystem
    {
        public GameObject GameObject { get; private set; }
        public ViewModel ViewModel { get; private set; }
        public CanvasGroup CanvasGroup;
        protected UIBindFactory Binding;
        protected UIAnimation EnterAnim;
        protected UIAnimation ExitAnim;
        public ResComponent ResComponent { get; private set; }
        public event Action<View> VisibilityChanged;

        public virtual void Awake()
        {
            Binding = ReferencePool.Allocate<UIBindFactory>();
            Binding.Init(this);
            AddComponent<ResComponent>();
        }

        public virtual void OnDestroy()
        {
        }

        public virtual float Alpha
        {
            get { return this.GameObject != null ? this.CanvasGroup.alpha : 0f; }
            set
            {
                if (this.GameObject != null) this.CanvasGroup.alpha = value;
            }
        }

        public virtual bool Interactable
        {
            get
            {
                if (this.GameObject == null)
                    return false;

                return this.CanvasGroup.interactable;
            }
            set
            {
                if (this.GameObject == null)
                    return;

                this.CanvasGroup.interactable = value;
            }
        }

        public void SetVm(ViewModel vm)
        {
            if (vm == null || ViewModel == vm) return;
            ViewModel = vm;
            if (ViewModel != null)
            {
                Binding.Reset();
                OnVmChange();
            }
        }

        protected abstract void OnVmChange();

        public void SetGameObject(GameObject obj)
        {
            GameObject = obj;
            CanvasGroup = GameObject.GetOrAddComponent<CanvasGroup>();
            Create();
        }

        protected virtual void Create()
        {
            foreach (UIAnimation uiAnimation in GameObject.GetComponents<UIAnimation>())
            {
                if (uiAnimation.AnimationType == AnimationType.EnterAnimation)
                {
                    EnterAnim = uiAnimation;
                }
                else if (uiAnimation.AnimationType == AnimationType.ExitAnimation)
                {
                    ExitAnim = uiAnimation;
                }
            }

            OnCreated();
        }

        protected virtual void OnEnable()
        {
            OnVisibilityChanged();
            RaiseVisibilityChanged();
        }

        protected virtual void OnDisable()
        {
            OnVisibilityChanged();
            RaiseVisibilityChanged();
        }

        protected virtual void OnCreated()
        {
        }

        protected virtual void OnVisibilityChanged()
        {
        }

        protected void RaiseVisibilityChanged()
        {
            try
            {
                VisibilityChanged?.Invoke(this);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 
        
        public virtual bool Visibility
        {
            get => this.GameObject != null && this.GameObject.activeSelf;
            set
            {
                if (this.GameObject == null)
                    return;

                if (this.GameObject.activeSelf == value)
                    return;

                this.GameObject.SetActive(value);
                if (value)
                    OnEnable();
                else
                    OnDisable();
            }
        }
    }
}