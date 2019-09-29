using Assets.SF.UI.Core;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Assets.SF.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SF.UI.Core
{

    
     [RequireComponent(typeof(CanvasGroup))]
    public abstract class UnityGuiView<T>:MonoBehaviour,IView<T> where T:ViewModelBase,new()
    {
        /// <summary>
        /// 显示之后的回掉函数
        /// </summary>
        public Action ShowAction { get; set; }
        /// <summary>
        /// 隐藏之后的回掉函数
        /// </summary>
        public Action HideAction { get; set; }

        private T _data;
        public T Data => _data ?? (_data = new T());


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
        #endregion

        #region 绑定的方法

        protected BindField<TComponent, TData> Bind<TComponent, TData>(Component component,  Expression<Func<T,TData>> filed ) where TComponent : Component
        {
            return new BindField<TComponent, TData>(WrapTool. component, GetBindPropertyByExpression(filed));
        }

        protected BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>(TComponent component, Expression<Func<T, TData1>> field1, Expression<Func<T, TData2>> field2,Func<TData1,TData2,TResult> wrapFunc) where TComponent : Component
        {
            return new BindField<TComponent, TData1, TData2, TResult>(component, GetBindPropertyByExpression(field1),
                GetBindPropertyByExpression(field2), wrapFunc);
        }

        protected BindFunc<TComponent> Bind<TComponent>(TComponent component, Action dataChanged) where TComponent : Component
        {
            return new BindFunc<TComponent>(component, dataChanged);
        }

        protected BindFuncWithPara<TComponent, TValue> BindCommand<TComponent, TValue>(TComponent component, Action<TValue> dataChanged) where TComponent : Component
        {
            return new BindFuncWithPara<TComponent, TValue>(component, dataChanged);
        }

        private BindingAbleProperty<TData> GetBindPropertyByExpression<TData>(Expression<Func<T, TData>> expression)
        {
            return Data.GetBindingAbleProperty<TData>((expression.Body as MemberExpression)?.Member.Name);
        }

        #endregion
    }

}