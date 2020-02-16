
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

        public T Load<T>(string path, UILevel uiLevel = UILevel.Common, ViewModel viewModel = null) where T : IView
        {
            return (T) Load(path, uiLevel, viewModel);
        }

        public IView Load(string path, UILevel uiLevel = UILevel.Common, ViewModel viewModel = null)
        {
            IView view = CreateUI(path, uiLevel);
            view.SetUIManager(this);
            view.SetVM(viewModel);
            return view;
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
                //Object.Destroy(panel);
            }

            existUI.Clear();
        }

        public void HideAllUI()
        {
            existUI.Values.ForEach(panel => panel.Hide());
        }

        public void CreateListItem(Transform view , ViewModel vm, int index)
        {
            GameObject go = Object.Instantiate(view.gameObject, view.parent);
            go.transform.SetSiblingIndex(index);
            IView v = go.GetComponent<IView>();
            v.SetVM(vm);
            v.Show();
        }

        private IView CreateUI(string panelName,UILevel uiLevel )
        {
            Transform par;
            switch (uiLevel)
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
                    throw new ArgumentOutOfRangeException(nameof(uiLevel), uiLevel, null);
            }
            var loadGo = LoadResFunc == null ? Resources.Load<GameObject>(panelName) : LoadResFunc(panelName);
            loadGo.transform.SetParent(par, false);
            return loadGo.GetComponent<IView>();
        }

        public static Canvas CreateCanvas()
        {
            var canvas = Resources.Load<Canvas>("Canvas");
            return canvas;
        }
    }
}