using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Framework.Assets
{
    public static class SceneLoader
    {
        public static IProgressResult<float,Scene> LoadScene(string scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            ProgressResult<float,Scene> progressResult = new ProgressResult<float, Scene>();
            loadScene(scene, loadSceneMode, progressResult);
            return progressResult;
        }

        private static async void loadScene(string scene, LoadSceneMode loadSceneMode,
            IProgressPromise<float, Scene> promise)
        {
            var loader = Addressables.LoadSceneAsync(scene, loadSceneMode);
            while (!loader.IsDone)
            {
                await Task.Yield();
                promise.UpdateProgress(loader.PercentComplete);
            }
            promise.UpdateProgress(1);
            promise.SetResult(loader.Result.Scene);
        }
        
    }
}