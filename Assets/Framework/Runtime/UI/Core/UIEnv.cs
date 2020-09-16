using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public static class UIEnv
    {
        public static Func<string, Sprite> LoadSpriteFunc = Resources.Load<Sprite>;
        public static Func<string, GameObject> LoadPrefabFunc = Resources.Load<GameObject>;
    }
}