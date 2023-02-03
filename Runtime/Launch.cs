using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework;
using UnityEngine;

public class Launch : MonoBehaviour
{
    private async void Start()
    {
        Res.DefaultResType = typeof(YooRes);
        using var res = Res.Create();
        await res.Init();
        BetterStreamingAssets.Initialize();
        Vibration.Init();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        int memory = Mathf.RoundToInt(SystemInfo.systemMemorySize / 1024f);
        if (memory <= 3)
        {
            ScalableBufferManager.ResizeBuffers(0.7f,0.7f);
        }else if (memory <= 5)
        {
            ScalableBufferManager.ResizeBuffers(0.8f,0.8f);
        }
        ParseTypes();
    }

    private void ParseTypes()
    {
        //编辑器环境下 寻找dll
        List<Type> allTypes = new List<Type>();
        allTypes.AddRange(GetType().Assembly.GetTypes());
        allTypes.AddRange(typeof(UIManager).Assembly.GetTypes());

        var gameModuleWithAttributes = new List<IGameModuleWithAttribute>();
        var gameModules = new List<IGameModule>();
        List<Type> managerAttributeTypes = new List<Type>();
        List<Type> gameStartBeforeManager = new();
        List<Type> gameStartAfterManager = new();

        //寻找所有的管理器
        allTypes = allTypes.Distinct().ToList();
        foreach (var t in allTypes)
        {
            if (t != null && typeof(IGameModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            {
                var i = t.GetProperty("Ins", BindingFlags.Public | BindingFlags.Static)
                    ?.GetValue(null, null) as IGameModule;
                if (i == null)
                {
                    this.Error($"{t} 找不到Ins的public static属性");
                    continue;
                }
                if (i is IGameModuleWithAttribute gameModuleWithAttribute)
                {
                    gameModuleWithAttributes.Add(gameModuleWithAttribute);
                }
                gameModules.Add(i);
                continue;
            }

            if (t.GetCustomAttributes(typeof(ManagerAttribute), false).Length > 0)
            {
                managerAttributeTypes.Add(t);
                continue;
            }
            
            if (t.GetCustomAttributes(typeof(GameStartBeforeManagerAttribute), false).Length > 0)
            {
                gameStartBeforeManager.Add(t);
                continue;
            }
            
            if (t.GetCustomAttributes(typeof(GameStartAfterManagerAttribute), false).Length > 0)
            {
                gameStartAfterManager.Add(t);
                continue;
            }
        }
        //类型注册
        foreach (var t in managerAttributeTypes)
        {
            foreach (var iMgr in gameModuleWithAttributes)
            {
                iMgr.CheckType(t);
            }
        }
        
        FrameworkEngine.AddModule(gameModules);

        foreach (var type in gameStartBeforeManager)
        {
            var gameStartMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).First((
                info => info.GetCustomAttributes(typeof(GameStartBeforeManagerAttribute), false).Length > 0));
            gameStartMethod.Invoke(null, null); 
        }

        //管理器初始化
        foreach (var m in gameModules)
        {
            m.Init();
        }

        //所有管理器开始工作
        foreach (var m in gameModules)
        {
            m.OnStart();
        }
        
        foreach (var type in gameStartAfterManager)
        {
            var gameStartMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).First((
                info => info.GetCustomAttributes(typeof(GameStartAfterManagerAttribute), false).Length > 0));
            gameStartMethod.Invoke(null, null); 
        }
        
        ProcedureManager.Ins.ChangeProcedure(-1);
    }
}