using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.EventSystems;

namespace ZFramework
{
      
    public enum UILevel
    {
        AlwayBottom = -3,        //如果不想区分太复杂那最底层的UI请使用这个
        Bg = -2,                 //背景层UI
        AnimationUnderPage = -1, //动画层
        Common = 0,              //普通层UI
        AnimationOnPage = 1,     // 动画层
        PopUI = 2,               //弹出层UI
        Guide = 3,               //新手引导层
        Const = 4,               //持续存在层UI
        Toast = 5,               //对话框层UI
        Forward = 6,             //最高UI层用来放置UI特效和模型
    }

    public enum UIMoveType
    {
        Move,
        Fixed
    }
    
    public class UIManager : MonoSingleton<UIManager>
    {

        #region 固定不动的UI的canvas
        [SerializeField] private Transform fBgTrans;
        [SerializeField] private Transform fAnimationUnderTrans;
        [SerializeField] private Transform fCommonTrans;
        [SerializeField] private Transform fAnimationOnTrans;
        [SerializeField] private Transform fPopUITrans;
        [SerializeField] private Transform fConstTrans;
        [SerializeField] private Transform fToastTrans;
        [SerializeField] private Transform fForwardTrans;
		
        [SerializeField] private Camera fUICamera;
        [SerializeField] private Canvas fCanvas;
        [SerializeField] private CanvasScaler fCanvasScaler;
        #endregion

        public int AA = 0;

        /// <summary>
        /// 存储所有实例化的面板身上的IPanel组件
        /// </summary>
        private Dictionary<string,IPanel> existPanelDic;
        /// <summary>
        /// 存储所有显示的面板上的IPanel组件
        /// </summary>
        private Dictionary<string, IPanel> showPanelDic;


        private void Awake ()
        {
            existPanelDic      = new Dictionary<string, IPanel> ();
            showPanelDic  = new Dictionary<string, IPanel> ();
            DontDestroyOnLoad(gameObject);
        }
        
        public void SetResolution(int width, int height)
        {
            fCanvasScaler.referenceResolution = new UnityEngine.Vector2(width, height);
        }

        public void SetMatchOnWidthOrHeight(float heightPercent)
        {
            fCanvasScaler.matchWidthOrHeight = heightPercent;
        }

        /// <summary>
        /// 隐藏除此之外的Panel，如果不赋值则清空所有
        /// </summary>
        /// <param name="uiName"></param>
        public void HideOtherPanel ( string uiName = "" )
        {
            foreach ( KeyValuePair<string, IPanel> item in showPanelDic )
            {
                if ( item.Key != uiName )
                    item.Value.Hide ();
            }
            IPanel panel = null;
            showPanelDic.TryGetValue ( uiName, out panel );
            showPanelDic.Clear ();
            if ( panel != null )
               showPanelDic.Add(uiName,panel);
        }

        /// <summary>
        /// 将UI加入到已显示面板的字典中
        /// </summary>
        /// <param name="uiName"></param>
        public void ShowUI ( string uiName ,UILevel canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = "")
        {
            //如果显示面板字典里面有当前面板则返回
            if ( showPanelDic.ContainsKey ( uiName ) ) return;
            IPanel basePanel = GetPanel ( uiName, canvasLevel, assetBundleName, uiData );
            showPanelDic.Add ( uiName, basePanel );
            basePanel.Show();
        }

        /// <summary>
        /// 将UI隐藏
        /// </summary>
        /// <param name="uiName"></param>
        public void HideUI ( string uiName )
        {
            //如果显示面板字典里面没有当前面板则返回
            if ( !showPanelDic.ContainsKey ( uiName ) ) return;
            IPanel basePanel = GetPanel ( uiName );
            showPanelDic.Remove ( uiName );
            basePanel.Hide();
        }

        public void DestoryUI ( string uiName )
        {
            IPanel panel = null;
            if ( showPanelDic.TryGetValue ( uiName, out panel ) )
                showPanelDic.Remove ( uiName );
            if ( existPanelDic.TryGetValue ( uiName, out panel ) )
                existPanelDic.Remove ( uiName );
            if (panel != null)
                panel.Close();
        }
        
        public UIPanel GetUIPanel(string uiName)
        {
            IPanel retIuiPanel = null;
            if (existPanelDic.TryGetValue(uiName, out retIuiPanel))
            {
                return retIuiPanel as UIPanel;
            }

            return null;
        }
        
        public T ShowUI<T>(UILevel canvasLevel = UILevel.Common, IUIData uiData = null, string assetBundleName = null) where T : UIPanel
        {
            string uiName = typeof ( T ).ToString ();

            if (!existPanelDic.ContainsKey(uiName))
            {
                GetPanel ( uiName, canvasLevel, assetBundleName, uiData );
            }

            existPanelDic[uiName].Show();
            return existPanelDic[uiName] as T;
        }


        /// <summary>
        /// 根据面板类型得到实例化的面板
        /// </summary>
        private IPanel GetPanel(string uiPanelName, UILevel level = UILevel.Common, string assetBundleName = "",
            IUIData uiData = null)
        {
            if (string.IsNullOrEmpty(uiPanelName)) return null;
            IPanel panel;
            if (existPanelDic.TryGetValue(uiPanelName, out panel))
            {
                return panel;
            }
            panel = UIPanel.Load(uiPanelName, assetBundleName);

            switch (level)
            {
                case UILevel.Bg:
                    panel.Transform.SetParent(fBgTrans, false);
                    break;
                case UILevel.AnimationUnderPage:
                    panel.Transform.SetParent(fAnimationUnderTrans, false);
                    break;
                case UILevel.Common:
                    panel.Transform.SetParent(fCommonTrans, false);
                    break;
                case UILevel.AnimationOnPage:
                    panel.Transform.SetParent(fAnimationOnTrans, false);
                    break;
                case UILevel.PopUI:
                    panel.Transform.SetParent(fPopUITrans, false);
                    break;
                case UILevel.Const:
                    panel.Transform.SetParent(fConstTrans, false);
                    break;
                case UILevel.Toast:
                    panel.Transform.SetParent(fToastTrans, false);
                    break;
                case UILevel.Forward:
                    panel.Transform.SetParent(fForwardTrans, false);
                    break;
            }

            panel.PanelInfo = new UIPanelInfo()
                {PanelName = uiPanelName, AssetBundleName = assetBundleName, Level = level, UIData = uiData};
            panel.Init();
            existPanelDic.Add(uiPanelName, panel);
            return panel;
        }

    }

    public static class UIMgr
    {
        public static void SetResolution ( int width, int height )
        {
            UIManager.Instance.SetResolution ( width, height );
        }

        public static void SetMatchOnWidthOrHeight ( float heightPercent )
        {
            UIManager.Instance.SetMatchOnWidthOrHeight ( heightPercent );
        }

        public static void HideOtherPanel ( string uiName = "" )
        {
            UIManager.Instance.HideOtherPanel(uiName);
        }

        public static void ShowUI ( string uiName, UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
                                    string assetBundleName = "" )
        {
            UIManager.Instance.ShowUI(uiName,canvasLevel,uiData,assetBundleName);
        }

        public static void HideUI ( string uiName )
        {
            UIManager.Instance.HideUI ( uiName );
        }

        public static void DestoryUI ( string uiName )
        {
            UIManager.Instance.DestoryUI(uiName);
        }

        public static UIPanel GetUIPanel ( string uiName )
        {
            return UIManager.Instance.GetUIPanel ( uiName );
        }

        public static T ShowUI<T> ( UILevel canvasLevel = UILevel.Common, IUIData uiData = null,
                                    string assetBundleName = null ) where T : UIPanel
        {
            return UIManager.Instance.ShowUI<T> ( canvasLevel, uiData, assetBundleName );
        }

        public static void HideUI<T> ()where T : UIPanel
        {
            UIManager.Instance.HideUI(typeof(T).ToString());
        }

        public static T GetUI<T> ()where T : UIPanel
        {
            return UIManager.Instance.GetUIPanel ( typeof ( T ).ToString () ) as T;
        }
    }
}