using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 职责类似Unity的Application
    /// </summary>
    public static class FApplication
    {
        #region 路径相关

        static  FApplication()
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
        /// Assets的上层目录
        /// </summary>
        public static string ProjectRoot { get; private set; }
        public static string AssetsRoot { get; private set; }
        /// <summary>
        /// Library
        /// </summary>
        public static string Library { get; private set; }

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
    }
}