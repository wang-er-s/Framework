using System.Collections.Generic;
using System.IO;

namespace Framework
{
    public static class AssetModelFactory
    {
        private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

        private static string GetAssetPath(string res)
        {
            if (string.IsNullOrEmpty(res))
                return res;
#if UNITY_EDITOR
            if (!AppEnv.UseBundleInEditor)
            {
                if(File.Exists(BundleConfig.ProjectPath + res))
                    return res;
                return null;
            }
#endif
            if (cache.ContainsKey(res))
                return cache[res];
            string path = res;
            path = $"{ResNameRedirect.GetRedirectName(path.ToLower())}{BundleConfig.bundleFileExt}";
            cache.Add(res, path);
            return path;
        }
        public static AssetModel CreateModel(string res = null)
        {
            return new AssetModel(GetAssetPath(res));
        }
    }
}