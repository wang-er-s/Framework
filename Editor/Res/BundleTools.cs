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
        

        public static void JenkinsBuildAssetBundle()
        {
            var reimport_shaders = string.Equals(Environment.GetEnvironmentVariable("REIMPORT_SHADERS"), "true",
                StringComparison.OrdinalIgnoreCase);
            if (reimport_shaders)
            {
                Utils.ReimportAsset<Shader>();
            }
            var clean = string.Equals(Environment.GetEnvironmentVariable("CLEAN_BUILD_ASSET_BUNDLE"), "true",
                StringComparison.OrdinalIgnoreCase);
            //指定平台
            var platformStr = Environment.GetEnvironmentVariable("BUILD_TARGET");
            var buildTarget = BuildTarget.NoTarget;
            if (!string.IsNullOrEmpty(platformStr))
            {
                if (platformStr.Equals("OSX", StringComparison.OrdinalIgnoreCase))
                {
                    buildTarget = BuildTarget.StandaloneOSX;
                }
            }
            
            var setting = Utils.LoadScriptableAsset<BundleSet>(BundleSet.ASSET_NAME);
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    setting.GenByPlatform(BuildTarget.StandaloneOSX,clean);
                    return;
            }
            setting.Gen(clean, true);
        }
    }
}