using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AD
{
    public class LoadTest : MonoBehaviour
    {

        private void Start()
        {
            //StartCoroutine ( Load ( "scene1/building.assetbundle", "Cube" ) );
            AssetBundleManager.Instance.LoadAssetBundle("game", "prefab.assetbundle", Print, null);
            //AssetBundleManager.Instance.LoadAsset ( "scene1", "building.assetbundle", "Cube", null );
            //string[] strs =  AssetBundleManifestLoader.Instance.GetDependencies("building.assetbundle");
            //AssetBundleManager.Instance.LoadOneSceneAssetBundle ( "Game", Print, ( )=>print("加载完成" ) );
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                AssetBundleManager.Instance.LoadOneSceneAssetBundle("Game", Print, () => print("加载完成"));
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Instantiate ( AssetBundleManager.Instance.LoadAsset<GameObject> ( AssetsItem.GamePrefabSphere, null ) );
            }
        }

        private void Print(string bundleName, float process)
        {
            print(bundleName + "  " + process);
        }

        private IEnumerator Load(string bundleName, string assetName)
        {
            yield return null;
            //Debug.Log(PathUtil.GetAssetBundleOutPath() + "/" + bundleName);
            //WWW www = new WWW(PathUtil.GetAssetBundleOutPath() + "/" + bundleName);
            //yield return www;
            //AssetBundle ab = www.assetBundle;
            ////资源加载
            //Object obj1 = ab.LoadAsset(assetName);

            //场景加载
            /*if (ab.isStreamedSceneAssetBundle) {
                string[] scenePaths = ab.GetAllScenePaths();
                string   sceneName  = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                SceneManager.LoadScene(sceneName);
            }*/

        }

    }
}