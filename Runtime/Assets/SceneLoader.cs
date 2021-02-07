using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Framework.Assets
{
    public static class SceneLoader
    {
        public static IProgressResult<float,SceneInstance> LoadScene(string scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activeOnLoaded = true)
        {
            ProgressResult<float,SceneInstance> progressResult = new ProgressResult<float, SceneInstance>();
            loadScene(scene, loadSceneMode, progressResult,activeOnLoaded);
            return progressResult;
        }

        private static async void loadScene(string scene, LoadSceneMode loadSceneMode,
            IProgressPromise<float, SceneInstance> promise, bool activeOnLoaded)
        {
            var loader = Addressables.LoadSceneAsync(scene, loadSceneMode, activeOnLoaded);
            while (!loader.IsDone)
            {
                await Task.Yield();
                promise.UpdateProgress(loader.PercentComplete);
            }
            
            promise.UpdateProgress(1);
            Log.Msg("加载场景", scene, loader.Status, loader.Result);
            promise.SetResult(loader.Result);
        }
        
    }
}