using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using ILRuntime.Mono.Cecil.Pdb;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Framework
{
    public class ILRuntimeHelper
    {
        public static AppDomain Appdomain;
        private static MemoryStream fs;
        private static MemoryStream pdb;
        
        public static async Task LoadILRuntime()
        {
	        Appdomain = new AppDomain();
			
	        pdb = null;
	        var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
	        //开发模式
	        if (ilrConfig.UseHotFix)
	        {
		        string prefix = String.Empty;
		        if (Application.platform == RuntimePlatform.OSXEditor ||
		            Application.platform == RuntimePlatform.WindowsEditor)
		        {
			        prefix = "file:///";
		        }

		        var dllPath = $"{prefix}{Application.streamingAssetsPath}/{ilrConfig.DllName}.dll";
		        var pdbPath = $"{prefix}{Application.streamingAssetsPath}/{ilrConfig.DllName}.pdb";
		        UnityWebRequest www = UnityWebRequest.Get(dllPath);
		        await www.SendWebRequest();
		        if (www.isHttpError | www.isNetworkError)
		        {
			        Log.Error(www.error);
		        }
		        fs = new MemoryStream(www.downloadHandler.data);
		        if (ilrConfig.UsePbd)
		        {
			        www = UnityWebRequest.Get(pdbPath);
			        await www.SendWebRequest();
			        if (www.isHttpError | www.isNetworkError)
			        {
				        Log.Error(www.error);
			        }
			        else
			        {
				        pdb = new MemoryStream(www.downloadHandler.data);
			        }
		        }
	        }

	        try
	        {
		        Appdomain.LoadAssembly(fs, ilrConfig.UsePbd ? pdb : null, new PdbReaderProvider());
	        }
	        catch (Exception e)
	        {
		        Log.Error("加载热更DLL错误：\n" , e.Message);
	        }

#if UNITY_EDITOR
	        Appdomain.DebugService.StartDebugService(56000);
#endif

	        GameLoop.Ins.OnApplicationQuitEvent += Dispose;
        }

        public static void InitializeILRuntime(AppDomain appDomain)
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            ILRuntimeDelegateHelper.RegisterDelegate(appDomain);
            ILRuntimeAdapterHelper.RegisterCrossBindingAdaptor(appDomain);
            ILRuntimeRedirectHelper.RegisterMethodRedirection(appDomain);
            ILRuntimeValueTypeBinderHelper.Register(appDomain);
            ILRuntimeGenericHelper.RegisterGenericFunc(appDomain);
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);

            //初始化CLR绑定请放在初始化的最后一步！！
            //请在生成了绑定代码后解除下面这行的注释
            // if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXPlayer)
	           //  ILRuntime.Runtime.CLRBinding.CLRBindingUtils.Initialize(appDomain);
        }
        
        private static Dictionary<string,Type> hotfixType = null;
        
        public static IEnumerable<Type> GetHotfixTypes()
        {
	        if (hotfixType == null)
	        {
		        hotfixType = new Dictionary<string, Type>();
		        foreach (var v in Appdomain.LoadedTypes)
		        {
			        hotfixType.Add(v.Key,v.Value.ReflectionType);
		        }
	        }

	        return hotfixType.Values;
        }

        public static Type GetType(string name)
        {
	        if (hotfixType == null)
		        GetHotfixTypes();
	        return hotfixType.TryGetValue(name, out var type) ? type : null;
        }

        public static void Dispose()
        {
	        fs?.Close();
            fs?.Dispose();
            pdb?.Close();
            pdb?.Dispose();
        }
    }
}