using Framework.UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Services
{
    public class ChangeSceneService
    {
        private UIManager uiManager;
        public ChangeSceneService()
        {
            Canvas canvas = UIManager.CreateCanvas();
            uiManager = new UIManager(canvas);
        }

        public void ChangeScene(string sceneName)
        {
            //SceneManager.LoadSceneAsync(sceneName,)
        }
        
    }
}