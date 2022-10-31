// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Threading.Tasks;
// 
// // // using UnityEngine;
// using VEngine;
// using Logger = VEngine.Logger;
//
// public class Launch : MonoBehaviour
// {
//
// 	private bool useHotfix;
// 	private bool debug = false;
// 	public ManifestInfo[] ManifestInfos;
// 	
// 	private void Awake()
// 	{
// 		useHotfix = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig.UseHotFix;
// 		CheckDownload();
// 	}
// 	
// 	private async void CheckDownload()
// 	{
// 		Logger.Loggable = false;
// 		Versions.addressableByName = path => true;
// 		await Versions.InitializeAsync(ManifestInfos);
// 		var uiRoot = GameObject.Find("UIRoot").transform;
// 		var checkWindow = new Window_CheckRes();
// 		checkWindow.SetVm(new Window_CheckResVM(InitGame));
// 		var checkTrans = new GameObject(nameof(Window_CheckRes)).transform;
// 		checkTrans.SetParent(uiRoot, false);
// 		checkWindow.SetGameObject(checkTrans.gameObject);
// 	}
//
// 	private async void InitGame()
// 	{
// 		var ilrTypes = await LoadHotFix();
// 		ParseTypes(ilrTypes.Distinct());
// 		if (debug) return;
// 		var logo = UIManager.Ins.Canvas.transform.Find("Logo");
// 		if (logo != null)
// 			Destroy(logo.gameObject);
// 	}
//
// 	private void Start()
// 	{
// 		GMCommand.Init();
// 	}
//
// 	private void ParseTypes(IEnumerable<Type> ilrTypes)
// 	{
// 		//编辑器环境下 寻找dll
// 		List<Type> allTypes = new List<Type>();
// 		allTypes.AddRange(GetType().Assembly.GetTypes());
// 		allTypes.AddRange(typeof(IManager).Assembly.GetTypes());
//
// 		var mgrs = new List<IManager>();
// 		List<Type> managerAttributeTypes = new List<Type>();
//
// 		//寻找所有的管理器
// 		allTypes = allTypes.Distinct().ToList();
// 		foreach (var t in allTypes)
// 		{
// 			if (t != null
// 			    && t.BaseType != null
// 			    && t.BaseType.FullName != null
// 			    && t.BaseType.FullName.Contains(".ManagerBase`3"))
// 			{
// 				var i = t.BaseType.GetProperty("Ins", BindingFlags.Public | BindingFlags.Static)
// 					?.GetValue(null, null) as IManager;
// 				mgrs.Add(i);
// 				continue;
// 			}
// 			//非热更dll需要查找adaptor
// 			if (ILRuntimeAdapterHelper.AddAdaptor(t))
// 			{
// 				continue;
// 			}
// 			if (t.GetCustomAttributes(typeof(ManagerAttribute),false).Length > 0)
// 			{
// 				managerAttributeTypes.Add(t);
// 				continue;
// 			}
// 			//找到热更注册的方法
// 			if (t.GetCustomAttribute(typeof(HotfixInitAttribute), false) != null)
// 			{
// 				hotfixInitMethod.Add(t.GetMethods().First(info => info.GetCustomAttribute(typeof(HotfixInitAttribute)) != null));
// 			}
// 			
// 		}
// 		
// 		//查找完adaptor才能注册各种适配
// 		if (useHotfix)
// 			InitializeILRuntime();
// 		
// 		HotfixEnterGame();
// 		if (debug) return;
// 		foreach (var t in ilrTypes)
// 		{
// 			if (t != null
// 			    && t.BaseType != null
// 			    && t.BaseType.FullName != null
// 			    && t.BaseType.FullName.Contains(".ManagerBase`3"))
// 			{
// 				var i = t.BaseType.GetProperty("Ins", BindingFlags.Public | BindingFlags.Static)
// 					?.GetValue(null, null) as IManager;
// 				mgrs.Add(i);
// 				continue;
// 			}
// 			if (t.GetCustomAttributes(false).Length > 0)
// 			{
// 				managerAttributeTypes.Add(t);
// 			}
// 		}
//
// 		//类型注册
// 		foreach (var t in managerAttributeTypes)
// 		{
// 			foreach (var iMgr in mgrs)
// 			{
// 				iMgr.CheckType(t);
// 			}
// 		}
//
// 		//管理器初始化
// 		foreach (var m in mgrs)
// 		{
// 			m.Init();
// 		}
//
// 		//所有管理器开始工作
// 		foreach (var m in mgrs)
// 		{
// 			m.Start();
// 		}
// 	}
//
// 	private async Task<List<Type>> LoadHotFix()
// 	{
// 		List<Type> result = new List<Type>();
// 		Log.Msg("use hotfix :", useHotfix);
// 		if (!useHotfix)
// 		{
// 			result.AddRange(GetComponent($"HotfixEntry").GetType().Assembly.DefinedTypes);
// 		}
// 		else
// 		{
// 			await ILRuntimeHelper.LoadILRuntime();
// 			result.AddRange(ILRuntimeHelper.GetHotfixTypes());
// 		}
// 		return result;
// 	}
// 	
// 	private void HotfixEnterGame()
// 	{
// 		if (useHotfix)
// 		{
// 			ILRuntimeHelper.Appdomain.Invoke("HotfixEntry", "RunGame", null, null);
// 		}
// 		else
// 		{
// 			GetComponent($"HotfixEntry").GetType().GetMethod("RunGame", BindingFlags.Static | BindingFlags.Public)
// 				.Invoke(null, null);
// 		}
// 	}
//
// 	private List<MethodInfo> hotfixInitMethod = new List<MethodInfo>();
// 	
// 	private void InitializeILRuntime()
// 	{
// 		var appdomain = ILRuntimeHelper.Appdomain;
// 		ILRuntimeHelper.InitializeILRuntime(appdomain);
// 		var para = new object[] {appdomain};
// 		foreach (var methodInfo in hotfixInitMethod)
// 		{
// 			methodInfo.Invoke(null, para);
// 		}
// 	}
//
// }