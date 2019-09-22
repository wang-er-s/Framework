using Assets.SF.UI.Core;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

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

        public T Data => ViewModelProperty.Value ?? (ViewModelProperty.Value = new T());

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
            Data.OnShow();
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
            Data.OnHide();
        }
       
        /// <summary>
        /// 当gameObject将被销毁时，这个方法被调用
        /// </summary>
        public virtual void OnDestroy()
        {
            if (Data.IsShow)
            {
                Close(true);
            }
            Data.OnDestory();
            ViewModelProperty.OnValueChanged = null;
        }

        #endregion

        #region 绑定的方法

        protected BindField<TComponent,TData> Bind<TComponent, TData>(TComponent component,BindableProperty<TData> field)
        {
            return new BindField<TComponent, TData>(component, field);
        }
        
        protected BindFunc<TComponent> Bind<TComponent>(TComponent component, Action dataChanged)
        {
            return new BindFunc<TComponent>(component, dataChanged);
        }

        protected BindFuncWithPara<TComponent, TValue> Bind<TComponent, TValue>(TComponent component, Action<TValue> dataChanged)
        {
            return new BindFuncWithPara<TComponent, TValue>(component, dataChanged);
        }

        #endregion
    }

    public class BindFuncWithPara<TComponent, TValue>
    {
        private TComponent component;
        private UnityEvent<TValue> dataChange;

        public BindFuncWithPara(TComponent component, Action<TValue> dataChange)
        {
            this.component = component;
            this.dataChange = dataChange;
        }

        public BindFuncWithPara<TComponent, TValue> For()
    }
}