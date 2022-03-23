using System;
using Sirenix.OdinInspector;

namespace Framework.Editor.AssetsChecker
{
    using UnityEditor;

    [Serializable]
    [HideLabel]
    [HideInTables]
    public abstract class AssetRule : Rule
    {
        public abstract void Check(AssetImporter assetImporter);
    }
}