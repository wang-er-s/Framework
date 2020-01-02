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
            Debug.Log(path);
            Directory.CreateDirectory(path);
            Debug.Log(Thread.CurrentThread.ManagedThreadId);
            Task task = new Task(() =>
            {
                BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None,
                    BuildTarget.StandaloneWindows64);
                Debug.Log(Thread.CurrentThread.ManagedThreadId);
                Debug.Log("in");
            });
            task.ConfigureAwait(true);
            task.Start();
            while (!task.IsCompleted)
            {
            }
            Debug.Log("completed");
        }
    }
}
#endif
