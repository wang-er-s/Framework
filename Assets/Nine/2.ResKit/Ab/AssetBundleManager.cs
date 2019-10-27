using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Nine
{
    public delegate void LoadProgress(string bundleName, float process);

    /// <summary>
    /// 加载资源完成时候的调用
    /// </summary>
    public delegate void LoadComplete();

    /// <summary>
    /// 加载assetbundle的回调
    /// </summary>
    public delegate void LoadAssetBundleCallback();


    public class AssetBundleManager : MonoBehaviour
    {

        #region 单例
        private static object lockObj = new object();

        private static AssetBundleManager instance;

        public static AssetBundleManager Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new GameObject("AssetBundleManager").AddComponent<AssetBundleManager>();
                        DontDestroyOnLoad(instance.gameObject);
                    }

                    return instance;
                }
            }
        }

        #endregion

        private void Awake()
        {
            instance = this;
            LoadABNameRecord();
            StartCoroutine(AssetBundleManifestLoader.Instance.Load());
        }

        private void OnDestroy()
        {
            nameSceneDict.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }



        /// <summary>
        /// 读取场景存储的AB包的配置文件
        /// </summary>
        private void LoadABNameRecord()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(PathUtil.GetAssetBundleOutPath());
            FileInfo[] records = directoryInfo.GetFiles();
            string sceneName = "";
            List<string> sceneAbName;
            foreach (FileInfo record in records)
            {
                if (!record.Extension.Equals(".config")) continue;
                using (FileStream fs = new FileStream(record.FullName, FileMode.Open))
                {
                    sceneAbName = new List<string>();
                    using (StreamReader sm = new StreamReader(fs))
                    {
                        string content = sm.ReadToEnd();
                        string[] strs = content.Split('\n');

                        foreach (string str in strs)
                        {
                            int temp = 0;
                            if (int.TryParse(str, out temp) || string.IsNullOrEmpty(str)) continue;
                            string[] sceneAb = Regex.Split(str, "--");
                            if (sceneAb.Length < 2) continue;
                            sceneAb[0] = sceneAb[0].Replace("\r", "");
                            sceneAb[1] = sceneAb[1].Replace("\r", "");
                            sceneName = sceneAb[0].ToLower();
                            sceneAbName.Add(sceneName + "/" + sceneAb[1].ToLower());
                        }
                        if (!nameSceneDict.ContainsKey(sceneName))
                        {
                            //如果没有这个场景 先创建这个场景 再读取一下 
                            OneSceneAssetBundles scene = new OneSceneAssetBundles(sceneName, sceneAbName);
                            nameSceneDict.Add(sceneName, scene);
                        }
                    }
                }

            }

        }

        /// <summary>
        /// 场景名 和 场景里面所有包的管理 的映射
        /// </summary>
        private Dictionary<string, OneSceneAssetBundles> nameSceneDict = new Dictionary<string, OneSceneAssetBundles>();

        /// <summary>
        /// 加载一个场景里面所有的AB包
        /// </summary>
        public void LoadOneSceneAssetBundle(string sceneName, LoadProgress lp, LoadAssetBundleCallback loadCompleteCallBack)
        {
            sceneName = sceneName.ToLower();
            if (!nameSceneDict.ContainsKey(sceneName))
            {
                Log.Error("请在读取配置表完成后再加载");
            }
            StartCoroutine(nameSceneDict[sceneName].LoadAllAssetBundle(lp, loadCompleteCallBack));
        }


        /// <summary>
        /// 加载资源包
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="folderName"></param>
        /// <param name="lp"></param>
        public void LoadAssetBundle(string sceneName, string folderName, LoadProgress lp, LoadAssetBundleCallback loadCompleteCallBack)
        {
            sceneName = sceneName.ToLower();
            if (!folderName.Contains(".assetbundle")) folderName = folderName + ".assetbundle";
            if (!nameSceneDict.ContainsKey(sceneName))
            {
                //如果没有这个场景 先创建这个场景 再读取一下 
                OneSceneAssetBundles scene = new OneSceneAssetBundles(sceneName);

                nameSceneDict.Add(sceneName, scene);
            }

            OneSceneAssetBundles sceneManager = nameSceneDict[sceneName];
            //开始加载
            StartCoroutine(sceneManager.Load(sceneName + "/" + folderName, lp, loadCompleteCallBack));
        }

        public T LoadAsset<T>(string sceneName, string folderName, string assetName, LoadProgress lp) where T : Object
        {
            sceneName = sceneName.ToLower();
            if (!folderName.Contains(".assetbundle")) folderName = folderName + ".assetbundle";
            if (!nameSceneDict.ContainsKey(sceneName))
            {
                LoadAssetBundle(sceneName, folderName, lp, () => LoadAsset<T>(sceneName, folderName, assetName, lp));
            }
            else if (!nameSceneDict[sceneName].IsLoadedAssetBundle(sceneName + "/" + folderName))
            {
                LoadAssetBundle(sceneName, folderName, lp, () => LoadAsset<T>(sceneName, folderName, assetName, lp));
            }

            return nameSceneDict[sceneName].LoadAsset(sceneName + "/" + folderName, assetName) as T;
        }


        /// <summary>
        /// 使用AssetsItem里面的字符串加载
        /// </summary>
        /// <param name="assetItem">使用AssetsItem里面的字符串加载</param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetItem, LoadProgress lp) where T : Object
        {
            string[] strs = assetItem.Split('-');
            string sceneName = strs[0];
            string folderName = strs[1];
            string assetName = strs[2];
            return LoadAsset<T>(sceneName, folderName, assetName, lp);
        }
    }
}