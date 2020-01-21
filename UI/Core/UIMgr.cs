
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
#if SLUA_SUPPORT
	using SLua;
#endif
    public enum UILevel
    {
        Bg = -1, //背景层UI
        Common = 0, //普通层UI
        Pop = 1, //弹出层UI
        Toast = 2, //对话框层UI
        Guide = 3, //新手引导层
    }

    public class UIMgr
    {
        private static UIMgr _instance;
        
        private Dictionary<string, IView> existUI = new Dictionary<string, IView>();

        private Transform bgTrans;
        private Transform commonTrans;
        private Transform popTrans;
        private Transform toastTrans;
        private Transform guideTrans;
 
        public Camera UICamera { get; private set; }
        public Canvas Canvas { get; private set; }

        public Func<string,GameObject> LoadResFunc { get; set; }

        public static void Init(UIMgr uiMgr = null ,Canvas canvas = null)
        {
            if (uiMgr != null)
                _instance = uiMgr;
            if (_instance == null)
                _instance = new UIMgr();
            _instance.Canvas = canvas == null ? Object.FindObjectOfType<Canvas>() : canvas;
            _instance.bgTrans = _instance.Canvas.transform.Find("Bg");
            _instance.commonTrans = _instance.Canvas.transform.Find("Common");
            _instance.popTrans = _instance.Canvas.transform.Find("Pop");
            _instance.toastTrans = _instance.Canvas.transform.Find("Toast");
            _instance.guideTrans = _instance.Canvas.transform.Find("Guide");
        }

        public static IView Create(string uiBehaviourName, UILevel canvasLevel = UILevel.Common, ViewModel vm = null)
        {
            if (!_instance.existUI.TryGetValue(uiBehaviourName, out var panel))
            {
                panel = CreateUI(uiBehaviourName, canvasLevel);
            }
            panel.ViewModel = vm;
            panel.Create();
            return panel;
        }

        public static void ShowUI(string panelName)
        {
            if (!_instance.existUI.TryGetValue(panelName, out var panel)) return;
            panel.Show();
        }

        public static void HideUI(string panelName)
        {
            if (!_instance.existUI.TryGetValue(panelName, out var panel)) return;
            panel.Hide();
        }

        public static void CloseAllUI()
        {
            foreach (var panel in _instance.existUI.Values)
            {
                panel.Destroy();
            }

            _instance.existUI.Clear();
        }

        public static void HideAllUI()
        {
            _instance.existUI.Values.ForEach(panel => panel.Hide());
        }

        public static void CloseUI(string panelName)
        {
            if (! _instance.existUI.TryGetValue(panelName, out var panel)) return;
            panel.Destroy();
        }

        public static void CreateListItem(Transform view , ViewModel vm, int index)
        {
            GameObject go = Object.Instantiate(view.gameObject, view.parent);
            go.Show();
            go.transform.SetSiblingIndex(index);
            IView v = go.GetComponent<IView>();
            v.ViewModel = vm;
        }

        private static IView CreateUI(string panelName,UILevel canvasLevel )
        {
            Transform par;
            switch (canvasLevel)
            {
                case UILevel.Bg:
                    par = _instance.bgTrans;
                    break;
                case UILevel.Common:
                    par = _instance.commonTrans;
                    break;
                case UILevel.Pop:
                    par = _instance.popTrans;
                    break;
                case UILevel.Toast:
                    par = _instance.toastTrans;
                    break;
                case UILevel.Guide:
                    par = _instance.guideTrans;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(canvasLevel), canvasLevel, null);
            }
            var loadGo = _instance.LoadResFunc == null ? Resources.Load<GameObject>(panelName) : _instance.LoadResFunc(panelName);
            GameObject go = Object.Instantiate(loadGo, par);
            return go.GetComponent<IView>();
        }
    }
}