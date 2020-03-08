
using System;
using System.Collections.Generic;
using Plugins.XAsset;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
#if SLUA_SUPPORT
	using SLua;
#endif
    public enum UILevel
    {
        None = -1, //留给内部Element的空值
        Bg = -1, //背景层UI
        Common = 0, //普通层UI
        Pop = 1, //弹出层UI
        Toast = 2, //对话框层UI
        Guide = 3, //新手引导层
    }

    public class SceneViewLocator : IUIViewLocator
    {
        
        private Dictionary<string, View> existUI = new Dictionary<string, View>();

        private Transform _bgTrans;
        private Transform _commonTrans;
        private Transform _popTrans;
        private Transform _toastTrans;
        private Transform _guideTrans;
        
        public Canvas Canvas { get; private set; }

        public Func<string, GameObject> LoadResFunc { get; set; } = Assets.LoadIns;

        public SceneViewLocator(Canvas canvas = null)
        {
            Canvas = canvas == null ? Object.FindObjectOfType<Canvas>() : canvas;
            if (Canvas == null) Canvas = CreateCanvas();
            _bgTrans = Canvas.transform.Find("Bg");
            _commonTrans = Canvas.transform.Find("Common");
            _popTrans = Canvas.transform.Find("Pop");
            _toastTrans = Canvas.transform.Find("Toast");
            _guideTrans = Canvas.transform.Find("Guide");
        }

        public View Load(string path, ViewModel viewModel)
        {
            View view = CreateUI(path);
            view.SetVM(viewModel);
            return view;
        }

        private View CreateUI(string panelName)
        {
            var loadGo = LoadResFunc == null
                ? Object.Instantiate(Resources.Load<GameObject>(panelName))
                : LoadResFunc(panelName);
            View view = loadGo.GetComponent<View>();
            UILevel uiLevel = view.UILevel;
            Transform par;
            switch (uiLevel)
            {
                case UILevel.Bg:
                    par = _bgTrans;
                    break;
                case UILevel.Common:
                    par = _commonTrans;
                    break;
                case UILevel.Pop:
                    par = _popTrans;
                    break;
                case UILevel.Toast:
                    par = _toastTrans;
                    break;
                case UILevel.Guide:
                    par = _guideTrans;
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