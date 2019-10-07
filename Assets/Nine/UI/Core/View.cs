using Assets.Nine.UI.Core;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Assets.Nine.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nine.UI.Core
{

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View<T> : MonoBehaviour, IView<T> where T : ViewModel
    {
        /// <summary>
        /// 显示之后的回掉函数
        /// </summary>
        public Action ShowAction { get; set; }

        /// <summary>
        /// 隐藏之后的回掉函数
        /// </summary>
        public Action HideAction { get; set; }

        private CanvasGroup canvasGroup;

        private T _data;
        public T Data => _data ?? (_data = Activator.CreateInstance<T>());

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        #region 界面显示隐藏的调用和回调方法

        void IView<T>.Create(T vm)
        {
            _data = vm;
            OnCreate();
        }

        void IView<T>.Close()
        {
            OnClose();
        }

        void IView<T>.Show()
        {
            Data.OnShow();
            ShowAction?.Invoke();
            OnShow();
        }

        void IView<T>.Hide()
        {
            HideAction?.Invoke();
            Data.OnHide();
            OnHide();
        }


        protected virtual void OnCreate()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnHide()
        {
        }

        void OnDestroy()
        {
            OnClose();
        }
        #endregion

        #region 绑定的方法

        protected BindField<TComponent, TData> Bind<TComponent, TData>(TComponent component,  Expression<Func<T,TData>> filed ) where TComponent : Component
        {
            return new BindField<TComponent, TData>(component, GetBindPropertyByExpression(filed));
        }

        protected BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>(TComponent component, Expression<Func<T, TData1>> field1, Expression<Func<T, TData2>> field2,Func<TData1,TData2,TResult> wrapFunc) where TComponent : Component
        {
            return new BindField<TComponent, TData1, TData2, TResult>(component, GetBindPropertyByExpression(field1),
                GetBindPropertyByExpression(field2), wrapFunc);
        }

        protected BindFunc<TComponent> BindCommand<TComponent>(TComponent component,Func<T,Action> command ) where TComponent : Component
        {
            Action dataChanged = command?.Invoke(Data);
            return new BindFunc<TComponent>(component, dataChanged);
        }

        protected BindFuncWithPara<TComponent, TValue> BindCommand<TComponent, TValue>(TComponent component, Func<T, Action<TValue>> command  ) where TComponent : Component
        {
            Action<TValue> dataChanged = command?.Invoke(Data);
            return new BindFuncWithPara<TComponent, TValue>(component, dataChanged);
        }

        protected BindList<TVm> BindList<TVm>(View<TVm> view, BindableList<TVm> list) where TVm : ViewModel
        {
            return new BindList<TVm>(view, list);
        }

        private BindableProperty<TData> GetBindPropertyByExpression<TData>(Expression<Func<T, TData>> expression)
        {
            return Data.GetBindingAbleProperty<TData>((expression.Body as MemberExpression)?.Member.Name);
        }

        #endregion
    }

}