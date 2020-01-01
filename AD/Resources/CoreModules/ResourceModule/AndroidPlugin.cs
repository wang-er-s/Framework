using System;
using UnityEngine;
using System.Collections;

namespace AD
{
    /// <summary>
    /// AD's Android Plugins is a jar.
    /// 
    /// Load sync from android asset folder.
    /// Unity3D's WWW class can only load async from asset folder, disgusting.
    /// </summary>
    public class ADAndroidPlugin
    {
#if !KENGINE_DLL && UNITY_ANDROID
        private static AndroidJavaClass _helper;

        /// <summary>
        /// Get AndroidHelper from Java jar
        /// </summary>
        private static AndroidJavaClass AndroidHelper
        {
            get
            {
                if (_helper != null) return _helper;

                _helper = new AndroidJavaClass("com.github.AD.AndroidHelper");

                if (_helper == null)
                    ErrorNotSupport();

                return _helper;
            }
        }
#endif

        private static void ErrorNotSupport()
        {
            throw new Exception("[ADAndroidPlugin.cs]Error on Android Plugin. Check if AD.Android.jar file exist in your Plugins/Android/libs? AD DLL mode also not support.");
        }

        /// <summary>
        /// Check if path exist in asset folder?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAssetExists(string path)
        {
#if !KENGINE_DLL && UNITY_ANDROID
            return AndroidHelper.CallStatic<bool>("isAssetExists", path);
#else
            ErrorNotSupport();
            return false;
#endif
        }

        /// <summary>
        /// Call from java get asset file bytes and convert to string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAssetString(string path)
        {
#if !KENGINE_DLL && UNITY_ANDROID
            return AndroidHelper.CallStatic<string>("getAssetString", path);
#else
            ErrorNotSupport();
            return null;
#endif
        }

        /// <summary>
        /// Call from java get asset file bytes
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetAssetBytes(string path)
        {
#if !KENGINE_DLL && UNITY_ANDROID
            return AndroidHelper.CallStatic<byte[]>("getAssetBytes", path);
#else
            ErrorNotSupport();
            return null;
#endif
        }
    }
}

