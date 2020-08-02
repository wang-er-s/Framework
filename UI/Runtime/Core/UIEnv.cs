using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public static class UIEnv
    {
        public static Func<string, Sprite> LoadSpriteFunc;
        public static Func<string, GameObject> LoadPrefabFunc;
    }
}