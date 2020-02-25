using Framework.UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Services
{
    public class ChangeSceneService
    {
        private SceneViewLocator _sceneViewLocator;
        public ChangeSceneService()
        {
            Canvas canvas = SceneViewLocator.CreateCanvas();
            _sceneViewLocator = new SceneViewLocator(canvas);
        }

        public void ChangeScene(string sceneName)
        {
            //SceneManager.LoadSceneAsync(sceneName,)
        }
        
    }
}