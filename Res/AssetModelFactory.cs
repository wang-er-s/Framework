using System.Collections.Generic;
using System.IO;

namespace Framework
{
    public static class AssetModelFactory
    {
        private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

        public static string GetAssetPath(string res)
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
            else
            {
                string path = res;
                //if (path.EndsWith(".prefab"))
                //    path = path.Substring(0, path.Length - 7);
                path = string.Format("{0}{1}", ResNameRedirect.GetRedirectName(path.ToLower()),BundleConfig.bundleFileExt);
                cache.Add(res, path);
                return path;
            }
        }
        public static AssetModel CreateModel(string res = null)
        {
            return new AssetModel(GetAssetPath(res));
        }

        public static AssetModel CreateEmpty()
        {
            return new EmptyAssetModel();
        }
    }
}