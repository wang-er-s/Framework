using AD.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using AD.UI.Wrap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Core
{

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class View : MonoBehaviour, IView
    {

        private List<View> subViews;

        private CanvasGroup canvasGroup;

        private ViewModel vm;
        public ViewModel VM
        {
            get { return vm; }
            set
            {
                if (vm == value)
                {
                    return;
                }
                vm = value;
                if (vm != null)
                    OnVmChange();
            }
        }

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            subViews = new List<View>();
        }

        #region 界面显示隐藏的调用和回调方法

        void IView.Create()
        {
            
        }

        void IView.Destroy()
        {
            OnDestroy();
            VM.OnDestroy();
            subViews.ForEach((subView) => subView.OnDestroy());
        }

        void IView.Show()
        {
            SetCanvas(true);
            VM.OnShow();
            OnShow();
            subViews.ForEach((subView) => subView.OnShow());
        }

        void IView.Hide()
        {
            SetCanvas(false);
            VM.OnHide();
            OnHide();
            subViews.ForEach((subView) => subView.OnHide());
        }
        
        protected virtual void OnCreate(){}

        protected virtual void OnShow(){}


        protected virtual void OnDestroy(){}

        protected virtual void OnHide(){}

        private void SetCanvas(bool visible)
        {
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = !visible;
        }

        #endregion

        protected void AddSubView(View view)
        {
            if (subViews.Contains(view)) return;
            subViews.Add(view);
        }

        protected abstract void OnVmChange();
        
    }

}