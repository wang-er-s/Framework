using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public enum UILevel
    {
        //这个在第一个
        None,
        Bg,
        Common,
        Pop,
        Toast,
        Guide,
        FullScreen,

        //这个放到最下边
        Max,
    }

    public abstract class Window : View
    {
        private List<View> _subViews = new List<View>();
        protected UIAnimation ActivationAnim;
        protected UIAnimation PassivationAnim;
        private bool created = false;
        private bool dismissed = false;
        private bool activated = false;
        public event Action<Window> ActivatedChanged;
        public event Action<Window> OnDismissed;
        public event Action<Window, WindowStateEventArgs> StateChanged;
        
        private WindowState state;
        public WindowState State
        {
            get => state;
            internal set
            {
                if(state == value) return;
                var old = this.state;
                state = value;
                RaiseStateChanged(old, state);
            }
        }

        public bool Activated
        {
            get => this.activated;
            protected set
            {
                if (activated == value)
                    return;

                activated = value;
                OnActivatedChanged();
                RaiseActivatedChanged();
            }
        }

        protected virtual void OnActivatedChanged()
        {
            this.Interactable = this.Activated;
        }

        protected override void Create()
        {
            State = WindowState.CREATE_BEGIN;
            foreach (UIAnimation uiAnimation in GameObject.GetComponents<UIAnimation>())
            {
                if (uiAnimation.AnimationType == AnimationType.ActivationAnimation)
                {
                    ActivationAnim = uiAnimation;
                }
                else if (uiAnimation.AnimationType == AnimationType.PassivationAnimation)
                {
                    PassivationAnim = uiAnimation;
                }
            }

            Visibility = false;
            Interactable = Visibility;
            base.Create();
            created = true;
            State = WindowState.CREATE_END;
        }
        
        public IAsyncResult Show(bool ignoreAnimation = false)
        {
            if (this.dismissed)
                throw new InvalidOperationException("The window has been destroyed");

            if (this.Activated)
                return AsyncResult.Void();

            if (Visibility)
                return AsyncResult.Void();
            
            UIComponent.Instance.ShowSort(this);
            return DoShow(ignoreAnimation);
        }

        protected virtual IAsyncResult DoShow(bool ignoreAnimation = false)
        {
            AsyncResult result = AsyncResult.Create();
            try
            {
                if (!this.created)
                    this.Create();

                this.OnShow();
                this.Visibility = true;
                this.State = WindowState.VISIBLE;
                if (!ignoreAnimation && this.EnterAnim != null)
                {
                    this.EnterAnim.SetView(this).OnStart(() => { this.State = WindowState.ENTER_ANIMATION_BEGIN; }).OnEnd(() =>
                    {
                        this.State = WindowState.ENTER_ANIMATION_END;
                        result.SetResult();
                        UIComponent.Instance.ActiveWindow(this, ignoreAnimation);
                    }).Play();
                }
                else
                {
                    result.SetResult();
                    UIComponent.Instance.ActiveWindow(this, ignoreAnimation);
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
                Log.Warning($"The window named {this.GetType().Name} failed to open!Error:{e}");
            }

            return result;
        }

        /// <summary>
        /// Called before the start of the display animation.
        /// </summary>
        protected virtual void OnShow()
        {
        }
        
        public IAsyncResult Hide(bool ignoreAnimation = false)
        {
            if (!this.created)
                throw new InvalidOperationException("The window has not been created.");

            if (this.dismissed)
                throw new InvalidOperationException("The window has been destroyed");

            if (!this.Visibility)
                return AsyncResult.Void();

            UIComponent.Instance.HideSort(this);
            return DoHide(ignoreAnimation);
        }

        protected virtual IAsyncResult DoHide(bool ignoreAnimation = false)
        {
            AsyncResult result = AsyncResult.Create();
            try
            {
                void __doHide()
                {
                    if (!ignoreAnimation && this.ExitAnim != null)
                    {
                        this.ExitAnim.SetView(this).OnStart(() => { this.State = WindowState.EXIT_ANIMATION_BEGIN; }).OnEnd(() =>
                        {
                            this.State = WindowState.EXIT_ANIMATION_END;
                            this.Visibility = false;
                            this.State = WindowState.INVISIBLE;
                            this.OnHide();
                            result.SetResult();
                            UIComponent.Instance.PassivateWindow(this, ignoreAnimation);
                        }).Play();
                    }
                    else
                    {
                        this.Visibility = false;
                        this.State = WindowState.INVISIBLE;
                        this.OnHide();
                        result.SetResult();
                        UIComponent.Instance.PassivateWindow(this, ignoreAnimation);
                    }
                }

                if (Activated)
                {
                    Passivate(ignoreAnimation).Callbackable().OnCallback(_ =>
                    {
                        __doHide();
                    });
                }
                else
                {
                    __doHide();
                }

            }
            catch (Exception e)
            {
                result.SetException(e);
                    Log.Warning("The window named \"{0}\" failed to hide!Error:{1}", GetType().Name, e);
            }
            return result;
        }
        
        /// <summary>
        /// Called at the end of the hidden animation.
        /// </summary>
        protected virtual void OnHide()
        {
        } 
        
        /// <summary>
        /// Activate
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncResult Activate(bool ignoreAnimation)
        {
            AsyncResult result = AsyncResult.Create();
            try
            {
                if (!this.Visibility)
                {
                    result.SetException(new InvalidOperationException("The window is not visible."));
                    return result;
                }

                if (this.Activated)
                {
                    result.SetResult();
                    return result;
                }

                if (!ignoreAnimation && this.ActivationAnim != null)
                {
                    this.ActivationAnim.SetView(this).OnStart(() =>
                    {
                        this.State = WindowState.ACTIVATION_ANIMATION_BEGIN;
                    }).OnEnd(() =>
                    {
                        this.State = WindowState.ACTIVATION_ANIMATION_END;
                        this.Activated = true;
                        this.State = WindowState.ACTIVATED;
                        result.SetResult();
                    }).Play();
                }
                else
                {
                    this.Activated = true;
                    this.State = WindowState.ACTIVATED;
                    result.SetResult();
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return result;
        }
        
        /// <summary>
        /// Passivate
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncResult Passivate(bool ignoreAnimation)
        {
            AsyncResult result = AsyncResult.Create();
            try
            {
                if (!this.Visibility)
                {
                    result.SetException(new InvalidOperationException("The window is not visible."));
                    return result;
                }

                if (!this.Activated)
                {
                    result.SetResult();
                    return result;
                }

                this.Activated = false;
                this.State = WindowState.PASSIVATED;

                if (!ignoreAnimation && this.PassivationAnim != null)
                {
                    this.PassivationAnim.SetView(this).OnStart(() =>
                    {
                        this.State = WindowState.PASSIVATION_ANIMATION_BEGIN;
                    }).OnEnd(() =>
                    {
                        this.State = WindowState.PASSIVATION_ANIMATION_END;
                        result.SetResult();
                    }).Play();
                }
                else
                {
                    result.SetResult();
                }
            }
            catch (Exception e)
            {
                result.SetException(e);
            }
            return result;
        }
 
        protected void RaiseStateChanged(WindowState oldState, WindowState newState)
        {
            try
            {
                WindowStateEventArgs eventArgs = new WindowStateEventArgs(this, oldState, newState);
                StateChanged?.Invoke(this, eventArgs);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        
        protected void RaiseActivatedChanged()
        {
            try
            {
                ActivatedChanged?.Invoke(this);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 
        
        protected void RaiseOnDismissed()
        {
            try
            {
                OnDismissed?.Invoke(this);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        } 

        public IProgressResult<float, T> AddSubView<T>(ViewModel viewModel = null) where T : View
        {
            var progressResult = this.RootScene().GetComponent<UIComponent>().CreateSubViewAsync<T>(viewModel);
            progressResult.Callbackable().OnCallback((result => AddSubView(result.Result)));
            return progressResult;
        }

        public void RemoveSubView(View view)
        {
            _subViews.TryRemove(view);
        }

        public void AddSubView(View view)
        {
            view.GameObject.transform.SetParent(GameObject.transform, false);
            _subViews.Add(view);
        }

        protected T GetSubView<T>() where T : View
        {
            foreach (var subView in _subViews)
            {
                if (subView is T view)
                    return view;
            }

            return null;
        }

        protected void Close()
        {
            this.RootScene().GetComponent<UIComponent>().Close(this);
        }

        public virtual UILevel UILevel { get; } = UILevel.Common;

        public override void OnDestroy()
        {
            base.OnDestroy();
            Binding.Clear();
            OnDismissed?.Invoke(this);
            for (int i = 0; i < _subViews.Count; i++)
            {
                _subViews[i].Dispose();
            }

            ViewModel?.OnViewDestroy();
        }
    }

    public class WindowStateEventArgs : IReference
    {
        public WindowStateEventArgs(Window window, WindowState oldState, WindowState newState)
        {
            this.Window = window;
            this.OldState = oldState;
            this.State = newState;
        }

        public WindowState OldState { get; }

        public WindowState State { get; }

        public Window Window { get; private set; }

        public void Clear()
        {
            Window = null;
        }
    }

    public enum WindowState
    {
        NONE,
        CREATE_BEGIN,
        CREATE_END,
        ENTER_ANIMATION_BEGIN,
        VISIBLE,
        ENTER_ANIMATION_END,
        ACTIVATION_ANIMATION_BEGIN,
        ACTIVATED,
        ACTIVATION_ANIMATION_END,
        PASSIVATION_ANIMATION_BEGIN,
        PASSIVATED,
        PASSIVATION_ANIMATION_END,
        EXIT_ANIMATION_BEGIN,
        INVISIBLE,
        EXIT_ANIMATION_END,
        DISMISS_BEGIN,
        DISMISS_END
    } 
}