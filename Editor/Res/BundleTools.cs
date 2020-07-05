using System;
using SoUtil.Util.Tools;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class BundleTools
    {
        [MenuItem("Game/Bundle/Setting",false,1)]
        private static void SelectBundleSet()
        {
            Selection.activeObject = Utils.LoadScriptableAsset<BundleSet>(BundleSet.ASSET_NAME);
        }
    }
}