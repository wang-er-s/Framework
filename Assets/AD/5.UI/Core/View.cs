using AD.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AD.AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Core
{

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour, IView
    {

        public Action ShowAction { get; set; }

        public Action HideAction { get; set; }

        private List<View> subViews;

        private CanvasGroup canvasGroup;

        public ViewModel data { get; private set; }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            subViews = new List<View> ();
        }

        private void Start ()
        {
            ((IView)this).Create (CreateVM ());
        }

        #region 界面显示隐藏的调用和回调方法

        void IView.Create (ViewModel vm)
        {
            if ( data != null ) return;
            data = vm ?? CreateVM ();
            data.OnCreate ();
            OnCreate ();
            subViews.ForEach ((subView) => subView.OnCreate ());
        }

        void IView.Destroy()
        {
            OnDestroy();
            data.OnDestroy();
            subViews.ForEach ((subView) => subView.OnDestroy ());
        }

        void IView.Show()
        {
            SetCanvas (true);
            data.OnShow();
            ShowAction?.Invoke();
            OnShow();
            subViews.ForEach ((subView) => subView.OnShow ());
        }

        void IView.Hide()
        {
            SetCanvas(false);
            HideAction?.Invoke();
            data.OnHide();
            OnHide();
            subViews.ForEach ((subView) => subView.OnHide ());
        }


        protected abstract void OnCreate ();

        protected virtual void OnShow()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnHide()
        {
        }

        private void SetCanvas (bool visible)
        {
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = !visible;
        }
        
        #endregion

        protected abstract ViewModel CreateVM ();

        protected void AddSubView (View view)
        {
            subViews.Add(view);
        }
        

    }

}