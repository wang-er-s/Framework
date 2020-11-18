using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Framework.Assets
{
    public class SceneLoader
    {
        public static async void LoadScene(string scene, IProgressPromise<float> progressPromise = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var loader = Addressables.LoadSceneAsync(scene, loadSceneMode);
            while (!loader.IsDone)
            {
                await Task.Yield();
                progressPromise?.UpdateProgress(loader.PercentComplete);
            }

            progressPromise?.SetResult();
        }
    }
}