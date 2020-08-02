using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public enum UILevel
    {
        None = -1, //留给内部Element的空值
        Bg = -1, //背景层UI
        Common = 0, //普通层UI
        Pop = 1, //弹出层UI
        Toast = 2, //对话框层UI
        Guide = 3 //新手引导层
    }

    public class SceneViewLocator : IUIViewLocator
    {
        private Dictionary<string, View> existUI = new Dictionary<string, View>();

        private Transform bgTrans;
        private Transform commonTrans;
        private Transform popTrans;
        private Transform toastTrans;
        private Transform guideTrans;

        public Canvas Canvas { get; private set; }

        public Func<string, GameObject> LoadResFunc { get; set; }

        public SceneViewLocator(Canvas canvas = null)
        {
            Canvas = canvas == null ? Object.FindObjectOfType<Canvas>() : canvas;
            if (Canvas == null) Canvas = CreateCanvas();
            bgTrans = Canvas.transform.Find("Bg");
            commonTrans = Canvas.transform.Find("Common");
            popTrans = Canvas.transform.Find("Pop");
            toastTrans = Canvas.transform.Find("Toast");
            guideTrans = Canvas.transform.Find("Guide");
        }

        public View Load(string path, ViewModel viewModel)
        {
            var view = CreateUI(path);
            view.SetVM(viewModel);
            return view;
        }

        private View CreateUI(string panelName)
        {
            var loadGo = UIEnv.LoadPrefabFunc(panelName);
            var view = loadGo.GetComponent<View>();
            var uiLevel = view.UILevel;
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
            loadGo.transform.SetParent(par, false);
            return view;
        }

        public static Canvas CreateCanvas()
        {
            var canvas = Resources.Load<Canvas>("Canvas");
            return canvas;
        }
    }
}