using Framework.UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Services
{
    public class ChangeSceneService
    {
        private UIManager _uiManager;
        public ChangeSceneService()
        {
            Canvas canvas = UIManager.CreateCanvas();
            _uiManager = new UIManager(canvas);
        }

        public void ChangeScene(string sceneName)
        {
            //SceneManager.LoadSceneAsync(sceneName,)
        }
        
    }
}