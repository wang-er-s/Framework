using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AD
{
    /// <summary>
    /// Load www, A wrapper of WWW.
    /// </summary>
    public class WWWLoader : AbstractResourceLoader
    {
        // 前几项用于监控器
        private static IEnumerator CachedWWWLoaderMonitorCoroutine; // 专门监控WWW的协程
        private const int MAX_WWW_COUNT = 15; // 同时进行的最大Www加载个数，超过的排队等待
        private static int WWWLoadingCount = 0; // 有多少个WWW正在运作, 有上限的

        private static readonly Stack<WWWLoader> WWWLoadersStack = new Stack<WWWLoader>();
        // WWWLoader的加载是后进先出! 有一个协程全局自我管理. 后来涌入的优先加载！

        public static event Action<string> WWWFinishCallback;

        public float BeginLoadTime;
        public float FinishLoadTime;
        public UnityWebRequest Www;

        public ulong Size
        {
            get { return Www.downloadedBytes; }
        }

        public float LoadSpeed
        {
            get
            {
                if (!IsCompleted)
                    return 0;
                return Size / (FinishLoadTime - BeginLoadTime);
            }
        }

        //public int DownloadedSize { get { return Www != null ? Www.bytesDownloaded : 0; } }

        /// <summary>
        /// Use this to directly load WWW by Callback or Coroutine, pass a full URL.
        /// A wrapper of Unity's WWW class.
        /// </summary>
        public static WWWLoader Load(string url, LoaderDelgate callback = null)
        {
            var wwwLoader = AutoNew<WWWLoader>(url, callback);
            return wwwLoader;
        }

        protected override void Init(string url, params object[] args)
        {
            base.Init(url, args);
            WWWLoadersStack.Push(this); // 不执行开始加载，由www监控器协程控制

            if (CachedWWWLoaderMonitorCoroutine == null)
            {
                CachedWWWLoaderMonitorCoroutine = WWWLoaderMonitorCoroutine();
                ResourceModule.Ins.StartCoroutine(CachedWWWLoaderMonitorCoroutine);
            }
        }

        protected void StartLoad()
        {
            ResourceModule.Ins.StartCoroutine(CoLoad(Url)); //开启协程加载Assetbundle，执行Callback
        }

        /// <summary>
        /// 协程加载Assetbundle，加载完后执行callback
        /// </summary>
        /// <param name="url">资源的url</param>
        /// <param name="callback"></param>
        /// <param name="callbackArgs"></param>
        /// <returns></returns>
        private IEnumerator CoLoad(string url)
        {
#if UNITY_2017_1_OR_NEWER
            //在Unity2017.1.1下，路径中包含两种分隔符(/和\),仅限windows平台
            //比如：C:\Code\KSFramework\Product/Bundles/Windows/ui/login.prefab.k)会报: UriFormatException: Invalid URI: Invalid port number
            //此处对路径处理成Unity标准路径格式：C:/Code/KSFramework/Product/Bundles/Windows/ui/login.prefab.k
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            url = KTool.FormatToAssetUrl(url);
#endif
#endif
            ResourceModule.LogRequest("WWW", url);
            System.DateTime beginTime = System.DateTime.Now;

            // 潜规则：不用LoadFromCache~它只能用在.assetBundle
            Www = new UnityWebRequest(url);
            BeginLoadTime = Time.time;
            WWWLoadingCount++;

            while (!Www.isDone)
            {
                Progress = Www.downloadProgress;
                yield return null;
            }

            yield return Www;
            WWWLoadingCount--;
            Progress = 1;
            if (IsReadyDisposed)
            {
                Debugger.Error("[KWWWLoader]Too early release: {0}", url);
                OnFinish(null);
                yield break;
            }
            if (!string.IsNullOrEmpty(Www.error))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    // TODO: Android下的错误可能是因为文件不存在!
                }
                
                Debugger.Error("[KWWWLoader:Error]{0} {1}", Www.error, url);

                OnFinish(null);
                yield break;
            }
            else
            {
                ResourceModule.LogLoadTime("WWW", url, beginTime);
                if (WWWFinishCallback != null)
                    WWWFinishCallback(url);

                Desc = string.Format("{0}K", Www.downloadedBytes / 1024f);
                OnFinish(Www);
            }

            // 预防WWW加载器永不反初始化, 造成内存泄露~
            if (Application.isEditor)
            {
                while (GetCount<WWWLoader>() > 0)
                    yield return null;

                yield return new WaitForSeconds(5f);

                while (Debug.isDebugBuild && !IsReadyDisposed)
                {
                    Debugger.Error("[KWWWLoader]Not Disposed Yet! : {0}", this.Url);
                    yield return null;
                }
            }
        }

        protected override void OnFinish(object resultObj)
        {
            FinishLoadTime = Time.time;
            base.OnFinish(resultObj);
        }

        protected override void DoDispose()
        {
            base.DoDispose();

            Www.Dispose();
            Www = null;
        }


        /// <summary>
        /// 监视器协程
        /// 超过最大WWWLoader时，挂起~
        /// 后来的新loader会被优先加载
        /// </summary>
        /// <returns></returns>
        protected static IEnumerator WWWLoaderMonitorCoroutine()
        {
            //yield return new WaitForEndOfFrame(); // 第一次等待本帧结束
            yield return null;

            while (WWWLoadersStack.Count > 0)
            {
                if (ResourceModule.LoadByQueue)
                {
                    while (GetCount<WWWLoader>() != 0)
                        yield return null;
                }
                while (WWWLoadingCount >= MAX_WWW_COUNT)
                {
                    yield return null;
                }

                var wwwLoader = WWWLoadersStack.Pop();
                wwwLoader.StartLoad();
            }

            ResourceModule.Ins.StopCoroutine(CachedWWWLoaderMonitorCoroutine);
            CachedWWWLoaderMonitorCoroutine = null;
        }
    }
}