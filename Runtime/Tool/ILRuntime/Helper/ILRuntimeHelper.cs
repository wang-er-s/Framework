using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Assets;
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
        
        public static async void LoadILRuntime()
        {
	        Appdomain = new AppDomain();
			
	        pdb = null;
	        var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
	        //开发模式
	        if (ilrConfig.UseHotFix)
	        {
		        var dllPath = Path.Combine(Application.streamingAssetsPath, $"{ilrConfig.DllName}.bytes");
		        var pdbPath = Path.Combine(Application.streamingAssetsPath, $"{ilrConfig.DllName}.pdb");
		        // UnityWebRequest www = new UnityWebRequest(dllPath);
		        // await www.SendWebRequest();
		        // if (www.isHttpError | www.isNetworkError)
		        // {
			       //  Log.Error(www.error);
		        // }
		        fs = new MemoryStream(File.ReadAllBytes(dllPath));
		        // www = new UnityWebRequest(pdbPath);
		        // await www.SendWebRequest();
		        // if (www.isHttpError | www.isNetworkError)
		        // {
			       //  Log.Error(www.error);
		        // }
		        pdb = new MemoryStream(File.ReadAllBytes(pdbPath));
	        }

	        try
	        {
		        Appdomain.LoadAssembly(fs, ilrConfig.UsePbd ? pdb : null, new PdbReaderProvider());
	        }
	        catch (Exception e)
	        {
		        Log.Error("加载热更DLL错误：\n" , e.Message);
		        Log.Error("可能是DLL和PDB版本不一致，可能DLL是Release，如果是Release出包，请取消UsePdb选项，本次已跳过使用PDB");
	        }

#if UNITY_EDITOR
	        Appdomain.DebugService.StartDebugService(56000);
#endif

	        GameLoop.Ins.OnApplicationQuitEvent += Dispose;
        }

        public static void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            Appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            ILRuntimeDelegateHelper.RegisterDelegate(Appdomain);
            ILRuntimeAdapterHelper.RegisterCrossBindingAdaptor(Appdomain);
            ILRuntimeRedirectHelper.RegisterMethodRedirection(Appdomain);
            ILRuntimeValueTypeBinderHelper.Register(Appdomain);
            ILRuntimeGenericHelper.RegisterGenericFunc();
            
            //初始化CLR绑定请放在初始化的最后一步！！
            //初始化CLR绑定请放在初始化的最后一步！！
            //初始化CLR绑定请放在初始化的最后一步！！

            //请在生成了绑定代码后解除下面这行的注释
            //请在生成了绑定代码后解除下面这行的注释
            //请在生成了绑定代码后解除下面这行的注释
            //ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
        }
        
        private static List<Type> hotfixTypeList = null;
        
        public static List<Type> GetHotfixTypes()
        {
	        if (hotfixTypeList == null)
	        {
		        hotfixTypeList = new List<Type>();
		        var values = Appdomain.LoadedTypes.Values.ToList();
		        foreach (var v in values)
		        {
			        hotfixTypeList.Add(v.ReflectionType);
		        }
	        }

	        return hotfixTypeList;
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