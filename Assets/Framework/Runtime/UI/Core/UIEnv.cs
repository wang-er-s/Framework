using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.UI.Core
{
    public static class UIEnv
    {
#if UNITY_EDITOR
        // relative to "Assets/"
        public static string UIPanelScriptFolder = "Assets/_Scripts/UI/Panel";
        public static string UIPrefabRootPath = "Assets/_StaticAssets/UI/";
#endif
    }
}