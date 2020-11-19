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
        private Dictionary<string, View> _existUi = new Dictionary<string, View>();

        private readonly Transform _bgTrans;
        private readonly Transform _commonTrans;
        private readonly Transform _popTrans;
        private readonly Transform _toastTrans;
        private readonly Transform _guideTrans;

        public Canvas Canvas { get; private set; }

        public Func<string, GameObject> LoadResFunc { get; set; }

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

        public T Load<T>(string path, ViewModel viewModel) where T : View
        {
            var view = CreateUI(path) as T;
            view.SetVm(viewModel);
            return view;
        }

        private View CreateUI(string panelName)
        {
            var loadGo = UIEnv.LoadPrefabFunc(panelName);
            loadGo = Object.Instantiate(loadGo);
            var view = loadGo.GetComponent<View>();
            var uiLevel = view.UILevel;
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