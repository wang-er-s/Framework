using System;
using System.Reflection;
using UnityEngine;

namespace SF.UI.Core
{

    
     [RequireComponent(typeof(CanvasGroup))]
    public abstract class UnityGuiView<T>:MonoBehaviour,IView<T> where T:ViewModelBase,new()
    {
        public readonly BindableProperty<T> ViewModelProperty = new BindableProperty<T>();
        /// <summary>
        /// 显示之后的回掉函数
        /// </summary>
        public Action ShowAction { get; set; }
        /// <summary>
        /// 隐藏之后的回掉函数
        /// </summary>
        public Action HideAction { get; set; }

        public T BindingContext => ViewModelProperty.Value ?? (ViewModelProperty.Value = new T());

        #region 界面显示隐藏的调用和回调方法

        public void Create(bool immediate = false, Action<Transform> action = null)
        {
            if(!immediate)
                action?.Invoke(transform);
            OnInitialize();
            OnCreate();
        }

        public void Close(bool immediate = false, Action<Transform> action = null)
        {
            if(!immediate)
                action?.Invoke(transform);
            OnClose();
        }

        /// <summary>
        /// 初始化View，当BindingContext改变时执行
        /// </summary>
        protected abstract void OnInitialize();
        
        
        private void OnCreate()
        {
            //立即显示
            transform.localScale = Vector3.one;
            GetComponent<CanvasGroup>().alpha = 1;
            OnShow();
        }
        
        public virtual void OnShow()
        {
            BindingContext.OnShow();
            ShowAction?.Invoke();
        }

        private void OnClose()
        {
            OnHide();
            transform.localScale = Vector3.zero;
            GetComponent<CanvasGroup>().alpha = 0;
        }

        /// <summary>
        /// alpha 1->0时
        /// </summary>
        public virtual void OnHide()
        {
            //回掉函数
            HideAction?.Invoke();
            BindingContext.OnHide();
        }
       
        /// <summary>
        /// 当gameObject将被销毁时，这个方法被调用
        /// </summary>
        public virtual void OnDestroy()
        {
            if (BindingContext.IsShow)
            {
                Close(true);
            }
            BindingContext.OnDestory();
            ViewModelProperty.OnValueChanged = null;
        }

        #endregion

        #region 绑定的方法

        protected void Bind<TProperty>(UnityEngine.Object component,params BindableProperty<TProperty>[] property,
            BindableProperty<TProperty>.ValueChangedHandler decorator = null)
        {
            if (property.OnValueChanged == null)
                property.OnValueChanged = decorator;
            else
                property.OnValueChanged += decorator;
        }
        
        protected void TwoWayBind 

        #endregion
    }
}