using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nine{

    public enum BundleType
    {
        None = 0,
        Shader,  // .shader or build-in shader with name
        Font,    // .ttf
        Texture, // .tga, .png, .jpg, .tif, .psd, .exr
        Sprite,
        Material,   // .mat
        Animation,  // .anim
        Controller, // .controller
        FBX,        // .fbx
        TextAsset,  // .txt, .bytes
        Prefab,     // .prefab
    }

    public class ResLoader
    {

        private static Dictionary<string, Object> cacheDic = new Dictionary<string, Object>();

        public static Object Load(string url, bool isCache = false)
        {
            return Load<Object>(url, isCache);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public static T Load<T>(string url, bool isCache = false) where T : Object
        {
            T obj;
            if (isCache)
            {
                if (cacheDic.ContainsKey(url))
                {
                    obj = cacheDic[url] as T;
                    return obj;
                }
                else
                {
                    obj = Resources.Load<T>(url);
                    if (obj == null)
                    {
                        Debug.LogError("你想加载的物体为空  " + url);
                        return null;
                    }
                    cacheDic.Add(url, obj);
                    return obj;
                }
            }
            obj = Resources.Load<T>(url);
            if (obj == null)
            {
                Debug.LogError("你想加载的物体为空 " + url);
                return null;
            }
            return obj;
        }

        public static GameObject LoadAndCreat(string url, bool isCache = false)
        {
            return LoadAndCreat<GameObject>(url, isCache);
        }

        /// <summary>
        /// 加载资源并实例化物体
        /// </summary>
        /// <returns>返回的是此物体的实例</returns>
        public static T LoadAndCreat<T>(string url, bool isCache = false) where T : Object
        {
            return GameObject.Instantiate(Load<T>(url, isCache));
        }

        /// <summary>
        /// 加载物体并设置其父物体
        /// </summary>
        /// <typeparam name="T">要加载物体的类型</typeparam>
        /// <param name="url"></param>
        /// <param name="transform">父物体</param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public static T LoadAndCreat<T>(string url, Transform transform, bool isCache = false) where T : Object
        {
            return GameObject.Instantiate(Load<T>(url, isCache), transform);
        }
        
        public static GameObject LoadAndCreat(string url, Transform transform, bool isCache = false)
        {
            return GameObject.Instantiate(Load<GameObject>(url, isCache), transform);
        }
        /// <summary>
        /// 加载GameObject并且设置父物体和局部坐标
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="transform">父物体</param>
        /// <param name="localPosition">局部坐标</param>
        /// <param name="isCache">是否加入缓存</param>
        /// <returns></returns>
        public static GameObject LoadAndCreat(string url, Transform transform, Vector3 localPosition, bool isCache = false)
        {
            GameObject obj = ResLoader.LoadAndCreat<GameObject>(url, transform, isCache);
            obj.transform.localPosition = localPosition;
            return obj;
        }

        /// <summary>
        ///  如果之前没缓存过，不要使用返回值，在回调方法里面使用异步加载的Object 
        /// </summary>
        /// <returns></returns>
        public static Object LoadAsync(MonoBehaviour mono, string url, bool isCache = false, Action<Object> action = null)
        {
            if (isCache)
            {
                if (cacheDic.ContainsKey(url))
                {
                    Object obj = cacheDic[url];
                    return obj;
                }
            }
            mono.StartCoroutine(LoadAsync(url,(Object)=>
            {
                action.InvokeGracefully(Object);
                if(isCache)
                    cacheDic.Add(url,Object);
            }));
            return null;
        }

        private static IEnumerator LoadAsync(string url, Action<Object> action)
        {
            Debug.Log("异步加载开始+"+url);
            ResourceRequest request = Resources.LoadAsync(url);
            yield return request;
            Debug.Log("异步加载完成+"+url);
            action.InvokeGracefully(request.asset);
        } 
        
        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void DisposeCache()
        {
            cacheDic.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}