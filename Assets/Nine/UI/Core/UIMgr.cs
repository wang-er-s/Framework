

using System;
using System.Collections.Generic;
using Nine;
using Nine.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using View = Nine.UI.Core.IView<Nine.UI.Core.ViewModel>;

namespace Nine.UI.Core
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

        private Dictionary<string, View> existUI;

        private Transform bgTrans;
        private Transform commonTrans;
        private Transform popTrans;
        private Transform toastTrans;
        private Transform guideTrans;

        public Camera UICamera { get; private set; }
        public Canvas Canvas { get; private set; }
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;

        void Awake()
        {
            Ins = this;
            existUI = new Dictionary<string, View>();
        }

        public static void Init()
        {

        }

        public void SetResolution(int width, int height)
        {
            canvasScaler.referenceResolution = new UnityEngine.Vector2(width, height);
        }

        public void SetMatchOnWidthOrHeight(float heightPercent)
        {
            canvasScaler.matchWidthOrHeight = heightPercent;
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
                panel.Close();
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
            panel.Close();
        }

        public void CreateListItem(View<ViewModel> view , ViewModel vm, int index)
        {
            GameObject go = Instantiate(view.gameObject, view.transform.parent);
            go.Show();
            go.transform.SetSiblingIndex(index);
            View v = go.GetComponent<View<ViewModel>>();
            v.Create(vm);
        }

        private View CreateUI(string panelName)
        {
            GameObject go;
            //TODO 有Assetbundle后再修改
            go = Instantiate(Resources.Load<GameObject>(panelName));
            return go.GetComponent<View>();
        }

    }

}