

using System;
using System.Collections.Generic;
using AD;
using AD.ResKit;
using AD.UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AD.UI.Core
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
        private static Dictionary<string, IView> existUI = new Dictionary<string, IView>();

        private static Transform bgTrans;
        private static Transform commonTrans;
        private static Transform popTrans;
        private static Transform toastTrans;
        private static Transform guideTrans;
 
        public static Camera UICamera { get; private set; }
        public static Canvas Canvas { get; private set; }

        static UIMgr()
        {
            Canvas = Object.FindObjectOfType<Canvas>();
            bgTrans = Canvas.transform.Find("Bg");
            commonTrans = Canvas.transform.Find("Common");
            popTrans = Canvas.transform.Find("Pop");
            toastTrans = Canvas.transform.Find("Toast");
            guideTrans = Canvas.transform.Find("Guide");
        }

        public static void Create(string uiBehaviourName, UILevel canvasLevel = UILevel.Common)
        {

            if (!existUI.TryGetValue(uiBehaviourName, out var panel))
            {
                panel = CreateUI(uiBehaviourName, canvasLevel);
            }
            panel.Create(null);
        }

        public static void ShowUI(string panelName)
        {
            if (!existUI.TryGetValue(panelName, out var panel)) return;
            panel.Show();
        }

        public static void HideUI(string panelName)
        {
            if (!existUI.TryGetValue(panelName, out var panel)) return;
            panel.Hide();
        }

        public static void CloseAllUI()
        {
            foreach (var panel in existUI.Values)
            {
                panel.Destroy();
            }

            existUI.Clear();
        }

        public static void HideAllUI()
        {
            existUI.Values.ForEach(panel => panel.Hide());
        }

        public static void CloseUI(string panelName)
        {
            if(! existUI.TryGetValue(panelName, out var panel)) return;
            panel.Destroy();
        }

        public static void CreateListItem(Transform view , ViewModel vm, int index)
        {
            GameObject go = Object.Instantiate(view.gameObject, view.parent);
            go.Show();
            go.transform.SetSiblingIndex(index);
            IView v = go.GetComponent<IView>();
            v.Create(vm);
        }

        private static IView CreateUI(string panelName,UILevel canvasLevel )
        {
            Transform par = commonTrans;
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
            GameObject go = Object.Instantiate(ResMgr.Load<GameObject>(panelName), par);
            return go.GetComponent<IView>();
        }

    }

}