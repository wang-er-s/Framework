using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public static class UIEnv
    {
#if UNITY_EDITOR
        // relative to "Assets/"
        public static string UIPanelScriptFolder = "Assets/Scripts/UI/Panel";
        public static string UIPrefabRootPath = "Resources";
#endif
        public static Func<string, Sprite> LoadSpriteFunc;
        public static Func<string, GameObject> LoadPrefabFunc;
    }
}