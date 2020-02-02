
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

    public class UIManager
    {
        
        private Dictionary<string, IView> existUI = new Dictionary<string, IView>();

        private Transform bgTrans;
        private Transform commonTrans;
        private Transform popTrans;
        private Transform toastTrans;
        private Transform guideTrans;
 
        public Camera UICamera { get; private set; }
        public Canvas Canvas { get; private set; }

        public Func<string,GameObject> LoadResFunc { get; set; }

        public UIManager(Canvas canvas = null)
        {
            Canvas = canvas == null ? Object.FindObjectOfType<Canvas>() : canvas;
            bgTrans = Canvas.transform.Find("Bg");
            commonTrans = Canvas.transform.Find("Common");
            popTrans = Canvas.transform.Find("Pop");
            toastTrans = Canvas.transform.Find("Toast");
            guideTrans = Canvas.transform.Find("Guide");
        }

        public IView Create(string uiBehaviourName, UILevel canvasLevel = UILevel.Common, ViewModel vm = null)
        {
            if (!existUI.TryGetValue(uiBehaviourName, out var panel))
            {
                panel = CreateUI(uiBehaviourName, canvasLevel);
            }
            panel.ViewModel = vm;
            panel.Create();
            return panel;
        }

        public void ShowUI(string panelName)
        {
            if (!existUI.TryGetValue(panelName, out var panel)) return;
            panel.Show();
        }

        public void HideUI(string panelName)
        {
            if (!existUI.TryGetValue(panelName, out var panel)) return;
            panel.Hide();
        }

        public void CloseAllUI()
        {
            foreach (var panel in existUI.Values)
            {
                panel.Destroy();
            }

            existUI.Clear();
        }

        public void HideAllUI()
        {
            existUI.Values.ForEach(panel => panel.Hide());
        }

        public void CloseUI(string panelName)
        {
            if (! existUI.TryGetValue(panelName, out var panel)) return;
            panel.Destroy();
        }

        public void CreateListItem(Transform view , ViewModel vm, int index)
        {
            GameObject go = Object.Instantiate(view.gameObject, view.parent);
            go.Show();
            go.transform.SetSiblingIndex(index);
            IView v = go.GetComponent<IView>();
            v.ViewModel = vm;
        }

        private IView CreateUI(string panelName,UILevel canvasLevel )
        {
            Transform par;
            switch (canvasLevel)
            {
                case UILevel.Bg:
                    par = bgTrans;
                    break;
                case UILevel.Common:
                    par = commonTrans;
                    break;
                case UILevel.Pop:
                    par = popTrans;
                    break;
                case UILevel.Toast:
                    par = toastTrans;
                    break;
                case UILevel.Guide:
                    par = guideTrans;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(canvasLevel), canvasLevel, null);
            }
            var loadGo = LoadResFunc == null ? Resources.Load<GameObject>(panelName) : LoadResFunc(panelName);
            GameObject go = Object.Instantiate(loadGo, par);
            return go.GetComponent<IView>();
        }
    }
}