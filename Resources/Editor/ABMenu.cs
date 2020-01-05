#if UNITY_EDITOR
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AD;
using UnityEditor;
using UnityEngine;

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
}
#endif
