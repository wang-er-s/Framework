#if UNITY_EDITOR
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Framework.Assets
{
    public class EditorRes : Assets.Res
    {
        public override string DownloadURL { get; set; }
        protected override void LoadScene(IProgressPromise<float, Scene> promise, string path, LoadSceneMode loadSceneMode)
        {
            SceneManager.LoadScene(path, loadSceneMode);
            var scene = SceneManager.GetSceneByName(path);            
            promise.SetResult(scene);
        }

        public override void Release()
        {
            
        }

        public override T LoadAsset<T>(string key)
        {
            return AssetDatabase.LoadAssetAtPath<T>(key);
        }

        public override Task<string> CheckDownloadSize(string key)
        {
            throw new System.NotImplementedException();
        }

        public override Task<IProgressResult<DownloadProgress>> DownloadAssets(string key)
        {
            throw new System.NotImplementedException();
        }

        protected override void loadAssetAsync<T>(string key, IProgressPromise<float, T> promise)
        {
            var go = AssetDatabase.LoadAssetAtPath<T>(key);
            promise.SetResult(go);
        }
    }
}
#endif