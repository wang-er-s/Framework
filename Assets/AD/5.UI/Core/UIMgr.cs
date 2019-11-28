

using System;
using System.Collections.Generic;
using AD;
using AD.UI.Core;
using UnityEngine;
using UnityEngine.UI;

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

    public class UIMgr : MonoBehaviour
    {
        public static UIMgr Ins;

        private Dictionary<string, IView> existUI;

        private Transform bgTrans;
        private Transform commonTrans;
        private Transform popTrans;
        private Transform toastTrans;
        private Transform guideTrans;

        public Camera UICamera { get; private set; }
        public Canvas Canvas { get; private set; }
        private GraphicRaycaster graphicRaycaster;

        void Awake()
        {
            Ins = this;
            existUI = new Dictionary<string, IView>();
        }

        public static void Init()
        {

        }

        public void Create(string uiBehaviourName, UILevel canvasLevel)
        {

            if (!existUI.TryGetValue(uiBehaviourName, out var panel))
            {
                panel = CreateUI(uiBehaviourName);
            }

            panel.Create(null);
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
            if(! existUI.TryGetValue(panelName, out var panel)) return;
            panel.Destroy();
        }

        public void CreateListItem(Transform view , ViewModel vm, int index)
        {
            GameObject go = Instantiate(view.gameObject, view.parent);
            go.Show();
            go.transform.SetSiblingIndex(index);
            IView v = go.GetComponent<IView>();
            v.Create(vm);
        }

        private IView CreateUI(string panelName)
        {
            GameObject go;
            //TODO 有Assetbundle后再修改
            go = Instantiate(Resources.Load<GameObject>(panelName));
            return go.GetComponent<IView>();
        }

    }

}