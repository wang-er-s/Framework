using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoUtil.Util.Tools
{
    public static class Utils
    {
        public static GameObject[] GetCurSceneRootObjs()
        {
            Scene curScene = SceneManager.GetActiveScene();
            return curScene.GetRootGameObjects();
        }
    }
}