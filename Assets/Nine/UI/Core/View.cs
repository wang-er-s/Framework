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
    public abstract class View : MonoBehaviour, IView
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

        protected ViewModel data;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        #region 界面显示隐藏的调用和回调方法

        void IView.Create(ViewModel vm)
        {
            data = vm;
            OnCreate(vm);
        }

        void IView.Close()
        {
            OnClose();
        }

        void IView.Show()
        {
            data.OnShow();
            ShowAction?.Invoke();
            OnShow();
        }

        void IView.Hide()
        {
            HideAction?.Invoke();
            data.OnHide();
            OnHide();
        }


        protected virtual void OnCreate(ViewModel vm)
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

    }

}