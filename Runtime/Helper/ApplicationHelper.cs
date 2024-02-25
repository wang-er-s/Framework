using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 职责类似Unity的Application
    /// </summary>
    public static class ApplicationHelper
    {
        #region 路径相关
#if UNITY_EDITOR
        static  ApplicationHelper()
        {
            Init();
        }

        private static void Init()
        {
            ProjectRoot = Application.dataPath.Replace("/Assets", "");
            AssetsRoot = Application.dataPath;
            Library = ProjectRoot + "/Library";
        }
        /// <summary>
        /// 项目根目录
        /// Assets的同级目录
        /// </summary>
        public static string ProjectRoot { get; private set; }
        public static string AssetsRoot { get; private set; }
        /// <summary>
        /// Library
        /// </summary>
        public static string Library { get; private set; }

        public static string GetPlatformPath(UnityEditor.BuildTarget platform)
        {
            switch (platform)
            {
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return "OSX";
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return "Windows";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.iOS:
                    return "iOS";
            }

            return "";
        }
#endif
        
        /// <summary>
        /// 平台资源的父路径
        /// </summary>
        public static string GetPlatformPath(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
            }

            return "";
        }
        
       
        #endregion

        public static bool IsMobile => Application.platform == RuntimePlatform.Android ||
                                       Application.platform == RuntimePlatform.IPhonePlayer;

        public static bool IsEditor => Application.platform == RuntimePlatform.WindowsEditor ||
                                       Application.platform == RuntimePlatform.LinuxEditor ||
                                       Application.platform == RuntimePlatform.OSXEditor;

        public static bool IsPC => Application.platform == RuntimePlatform.WindowsPlayer ||
                                   Application.platform == RuntimePlatform.OSXPlayer;

        public static bool IsOnUi
        {
            get
            {
#if UNITY_EDITOR
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
#endif
#if UNITY_ANDROID
                if (Input.touchCount <= 0) return false;
                Touch touch = Input.touches[0];
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId);
#endif
            }
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
    }
}