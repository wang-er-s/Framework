using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public abstract class UIPanel : MonoBehaviour,IPanel
    {
        private PanelLoader mPanelLoader = null;
        public CanvasGroup canvasGroup;
        public Transform Transform { get { return transform; } }
        public UIPanelInfo PanelInfo { get; set; }
        public virtual UIMoveType MoveType { get { return UIMoveType.Fixed; } } 
        protected IUIData mUIData;

        public static UIPanel Load(string panelName, string assetBundleName = null)
        {
            var panelLoader = new PanelLoader();
            //var panelPrefab = assetBundleName.IsNullOrEmpty()
            //    ? panelLoader.LoadPanelPrefab(panelName)
            //    : panelLoader.LoadPanelPrefab(assetBundleName, panelName);
            GameObject obj = ResLoader.LoadAndCreat("UIPrefab/" + panelName);
            var retScript = obj.GetComponent<UIPanel>();
            retScript.mPanelLoader = panelLoader;
            return retScript;
        }

        protected void OnBeforeDestroy ()
        {
            ClearUIComponents ();
        }

        protected virtual void ClearUIComponents () { }

        void IPanel.Init ( IUIData uiData )
        {
            mUIData = uiData;
            InitUI ( uiData );
            RegisterUIEvent ();
            //mSubPanelInfos.ForEach(subPanelInfo => UIMgr.OpenPanel(subPanelInfo.PanelName, subPanelInfo.Level));
        }

        protected virtual void InitUI ( IUIData uiData ) { }

        protected virtual void RegisterUIEvent () { }


        private void OnDestroy () { }

        void IPanel.Show()
        {
            gameObject.SetActive (true);
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            OnShow ();
        }

        void IPanel.Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            OnHide();
        }
        
        void IPanel.Close (bool destroyed)
        {
            PanelInfo.UIData = mUIData;
			
            OnClose();
            if (destroyed)
            {
                Destroy(gameObject);
            }

            mPanelLoader.Unload();
            mPanelLoader = null;
            mUIData      = null;
			
        }

        protected virtual void Update ()
        {
            if (canvasGroup.alpha >= 1)
            {
                OnUpdate();
            }
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }


        protected abstract void OnUpdate();

        protected virtual void OnClose (){}


    }

    
    /// <summary>
    /// 每个UIbehaviour对应的Data
    /// </summary>
    public interface IUIData { }

    public class UIPanelData : IUIData { }

    public class UIPanelInfo
    {
        public IUIData UIData;

        public UILevel Level;

        public string AssetBundleName;

        public string PanelName;
    }

}
