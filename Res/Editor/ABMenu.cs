
using System;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AD;
using UnityEditor;

namespace AD
{
    public class ABMenu
    {
        [MenuItem("Tools/打包")]
        private static void PackBundle()
        {
            string path = ResPath.EditorAssetBundleFullPath;
            Directory.CreateDirectory(path);
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);
        }
    }

    public static class AssetsMenuItem
    {
        private const string KMarkAssetsWithDir = "Assets/AssetBundles/按目录标记";
        private const string KMarkAssetsWithFile = "Assets/AssetBundles/按文件标记";
        private const string KMarkAssetsWithName = "Assets/AssetBundles/按名称标记";
        private const string KBuildManifest = "Assets/AssetBundles/生成配置";
        private const string KBuildAssetBundles = "Assets/AssetBundles/生成资源包";
        private const string KBuildPlayer = "Assets/AssetBundles/生成播放器";
        private const string KCopyPath = "Assets/复制路径";
        private const string KMarkAssets = "标记资源";
        private const string KCopyToStreamingAssets = "Assets/AssetBundles/拷贝到StreamingAssets";
        public static string assetRootPath;

        public static string TrimedAssetBundleName(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetRootPath))
                return assetBundleName;
            var name = assetBundleName.Replace(assetRootPath, "");
            if (name.Length > 0 && (name[0] == '/' || name[0] == '\\'))
            {
                name = name.Substring(1);
            }
            return name;
        }

        [MenuItem(KCopyToStreamingAssets)]
        private static void CopyAssetBundles()
        {
            BuildScript.CopyAssetBundlesTo(Path.Combine(ResPath.StreamingPath, Configs.BundlesDirName));
            AssetDatabase.Refresh();
        }

        [MenuItem(KMarkAssetsWithDir)]
        private static void MarkAssetsWithDir()
        {
            try
            {
                var settings = BuildScript.GetSettings();
                assetRootPath = Configs.EditorResourcesDir;
                var assetsManifest = BuildScript.GetManifest();
                var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
                for (var i = 0; i < assets.Length; i++)
                {
                    var asset = assets[i];
                    var path = AssetDatabase.GetAssetPath(asset);
                    if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                        continue;
                    if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                        break;
                    var assetBundleName = TrimedAssetBundleName(Path.GetDirectoryName(path).Replace("\\", "/"));
                    if (string.IsNullOrEmpty(assetBundleName))
                    {
                        assetBundleName = "root";
                    }
                    BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
                }
                EditorUtility.SetDirty(assetsManifest);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem(KMarkAssetsWithFile)]
        private static void MarkAssetsWithFile()
        {
            try
            {
                var settings = BuildScript.GetSettings();
                assetRootPath = Configs.EditorResourcesDir;
                var assetsManifest = BuildScript.GetManifest();
                var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
                for (var i = 0; i < assets.Length; i++)
                {
                    var asset = assets[i];
                    var path = AssetDatabase.GetAssetPath(asset);
                    if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                        continue;
                    if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                        break;

                    var dir = Path.GetDirectoryName(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    if (dir == null)
                        continue;
                    dir = dir.Replace("\\", "/") + "/";
                    if (name == null)
                        continue;

                    var assetBundleName = TrimedAssetBundleName(Path.Combine(dir, name));
                    BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
                }
                EditorUtility.SetDirty(assetsManifest);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem(KMarkAssetsWithName)]
        private static void MarkAssetsWithName()
        {
            var settings = BuildScript.GetSettings();
            assetRootPath = Configs.EditorResourcesDir;
            var assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            var assetsManifest = BuildScript.GetManifest();
            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var path = AssetDatabase.GetAssetPath(asset);
                if (Directory.Exists(path) || path.EndsWith(".cs", System.StringComparison.CurrentCulture))
                    continue;
                if (EditorUtility.DisplayCancelableProgressBar(KMarkAssets, path, i * 1f / assets.Length))
                    break;
                var assetBundleName = Path.GetFileNameWithoutExtension(path);
                BuildScript.SetAssetBundleNameAndVariant(path, assetBundleName.ToLower(), null);
            }
            EditorUtility.SetDirty(assetsManifest);
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(KBuildAssetBundles)]
        private static void BuildAssetBundles()
        {
            BuildScript.BuildManifest();
            BuildScript.BuildAssetBundles();
        }
    }
}
#endif
