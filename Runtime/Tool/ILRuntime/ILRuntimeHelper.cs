using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public class ILRuntimeHelper
    {
        public static ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        static MemoryStream m_hotfixDllMemoryStream;
        static MemoryStream m_hotfixPdbMemoryStream;
        
 
        public static IEnumerator LoadILRuntime(Action LoadedFinish)
        {
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
#else
            UnityWebRequest www = UnityWebRequest.Get("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
#endif
            yield return www.SendWebRequest();
            
            if (!string.IsNullOrEmpty(www.error))
                UnityEngine.Debug.LogError(www.error);
            byte[] dll = www.downloadHandler.data;
            www.Dispose();

            //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
#else
            www = UnityWebRequest.Get("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
#endif
            yield return www.SendWebRequest();
            
            if (!string.IsNullOrEmpty(www.error))
                Debug.LogError(www.error);
            byte[] pdb = www.downloadHandler.data;
            m_hotfixDllMemoryStream = new MemoryStream(dll);
            m_hotfixPdbMemoryStream = new MemoryStream(pdb);
            try
            {
                appdomain.LoadAssembly(m_hotfixDllMemoryStream, m_hotfixPdbMemoryStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("加载热更DLL失败，请确保已经通过VS打开Assets/Samples/ILRuntime/1.6/Demo/HotFix_Project/HotFix_Project.sln编译过热更DLL");
            }

            InitializeILRuntime();
 
            //用于ILRuntime Debug
            if (Application.isEditor)
                appdomain.DebugService.StartDebugService(56000);
 
            LoadedFinish?.Invoke();
        }
        
        static void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            ILRuntimeDelegateHelper.RegisterDelegate(appdomain);
            ILRuntimeAdapterHelper.RegisterCrossBindingAdaptor(appdomain);
            ILRuntimeRedirectHelper.RegisterMethodRedirection(appdomain);
        }
        
        public static void Dispose()
        {
            m_hotfixDllMemoryStream?.Dispose();
            m_hotfixPdbMemoryStream?.Dispose();
        }
    }
}