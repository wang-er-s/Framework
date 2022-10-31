using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public static class CommonHelper
    {
        /// <summary>
        /// 资源清理和垃圾回收
        /// </summary>
        public static void ClearMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi =>
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;


        /// <summary>
        /// 获取当前运行的设备平台信息
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Device And Sysinfo:\r\n");
            stringBuilder.Append(string.Format("DeviceModel: {0} \r\n", SystemInfo.deviceModel));
            stringBuilder.Append(string.Format("DeviceName: {0} \r\n", SystemInfo.deviceName));
            stringBuilder.Append(string.Format("DeviceType: {0} \r\n", SystemInfo.deviceType));
            stringBuilder.Append(string.Format("BatteryLevel: {0} \r\n", SystemInfo.batteryLevel));
            stringBuilder.Append(string.Format("DeviceUniqueIdentifier: {0} \r\n", SystemInfo.deviceUniqueIdentifier));
            stringBuilder.Append(string.Format("SystemMemorySize: {0} \r\n", SystemInfo.systemMemorySize));
            stringBuilder.Append(string.Format("OperatingSystem: {0} \r\n", SystemInfo.operatingSystem));
            stringBuilder.Append(string.Format("GraphicsDeviceID: {0} \r\n", SystemInfo.graphicsDeviceID));
            stringBuilder.Append(string.Format("GraphicsDeviceName: {0} \r\n", SystemInfo.graphicsDeviceName));
            stringBuilder.Append(string.Format("GraphicsDeviceType: {0} \r\n", SystemInfo.graphicsDeviceType));
            stringBuilder.Append(string.Format("GraphicsDeviceVendor: {0} \r\n", SystemInfo.graphicsDeviceVendor));
            stringBuilder.Append(string.Format("GraphicsDeviceVendorID: {0} \r\n", SystemInfo.graphicsDeviceVendorID));
            stringBuilder.Append(string.Format("GraphicsDeviceVersion: {0} \r\n", SystemInfo.graphicsDeviceVersion));
            stringBuilder.Append(string.Format("GraphicsMemorySize: {0} \r\n", SystemInfo.graphicsMemorySize));
            stringBuilder.Append(string.Format("GraphicsMultiThreaded: {0} \r\n", SystemInfo.graphicsMultiThreaded));
            stringBuilder.Append(string.Format("SupportedRenderTargetCount: {0} \r\n",
                SystemInfo.supportedRenderTargetCount));
            return stringBuilder.ToString();
        }

        private static bool? isIpad;

        public static bool IsPad
        {
            get
            {
                if (isIpad != null) return isIpad.Value;
                string type = SystemInfo.deviceModel.ToLower().Trim();
                switch (type.Substring(0, 3))
                {
                    case "iph":
                        //iPhone机型
                        isIpad = false;
                        break;
                    case "ipa":
                        //iPad机型
                        isIpad = true;
                        break;
                    default:
                        //其他
                        isIpad = false;
                        break;
                }

                return isIpad.Value;
            }
        }

        /// <summary>
        /// 获取设备的电量
        /// </summary>
        /// <returns></returns>
        public static float GetBatteryLevel()
        {
            if (Application.isMobilePlatform)
            {
                return SystemInfo.batteryLevel;
            }

            return 1;
        }

        /// <summary>
        /// 获取设备的电池状态
        /// </summary>
        /// <returns></returns>
        public static BatteryStatus GetBatteryStatus()
        {
            return SystemInfo.batteryStatus;
        }

        /// <summary>
        /// 获取设备网络的状况
        /// </summary>
        /// <returns></returns>
        public static NetworkReachability GetNetworkStatus()
        {
            return Application.internetReachability;
        }
        
        public static long CurrentMillTime()
        {
            TimeSpan tss = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long millTime = Convert.ToInt64(tss.TotalMilliseconds);
            return millTime;
        }

        public static string FormatBytes(long bytes)
        {
            float kb = bytes / 1024f;
            if (kb < 1024)
            {
                return $"{kb.ToString(".0")}KB";
            }
            float mb = kb / 1024f;
            if (mb < 1024)
            {
                return $"{mb.ToString(".0")}MB";
            }
            
            float gb = mb / 1024f;
            return $"{gb.ToString(".0")}GB";
        }
        
        #region Editor

#if UNITY_EDITOR

        public static GameObject[] GetCurSceneRootObjs()
        {
            Scene curScene = SceneManager.GetActiveScene();
            return curScene.GetRootGameObjects();
        }

        public static T LoadScriptableAsset<T>(string assetPath, bool createDefault = true) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath + ".asset");
            if (asset == null && createDefault)
            {
                asset = CreateScriptableAsset<T>(assetPath);
            }

            return asset;
        }

        public static T CreateScriptableAsset<T>(string assetPath, T obj = null) where T : ScriptableObject
        {
            if (obj == null) obj = ScriptableObject.CreateInstance<T>();
            int pIndex = assetPath.LastIndexOfAny(new[] {'\\', '/'});
            if (pIndex > 0)
            {
                string parentFolderPath = assetPath.Substring(0, pIndex);
                if (!Directory.Exists(parentFolderPath))
                {
                    Directory.CreateDirectory(parentFolderPath);
                }
            }

            AssetDatabase.CreateAsset(obj, assetPath + ".asset");
            return obj;
        }

        public static void ReimportAsset<T>(bool includePackages = true) where T : UnityEngine.Object
        {
            string filter = string.Format("t:{0}", typeof(T)).Replace("UnityEngine.", "");
            string progressTitle = "reimport assets:" + filter;
            string[] searchFolders = includePackages ? null : new[] {"Assets"};
            string[] guids = AssetDatabase.FindAssets(filter, searchFolders);
            EditorUtility.DisplayProgressBar(progressTitle, "", 0);
            Debug.Log($"waiting for {guids.Length} objs to import");
            for (int i = 0; i < guids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar(progressTitle, assetPath, (float) i / guids.Length);
                AssetDatabase.ImportAsset(assetPath);
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("reimport done");
        }
#endif

        #endregion
        
    }
}