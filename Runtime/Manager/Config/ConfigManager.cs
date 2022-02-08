using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Framework;
using Framework.Assets;
using Framework.Asynchronous;
using UnityEngine;

public class ConfigManager : ManagerBase<ConfigManager, ConfigAttribute, string>
{
    private object[] _objects = new object[1];
    private MulAsyncResult asyncResult;
    private Action _loadedCb;
    public bool Loaded { get; private set; }
    public static Func<string, string> CustomLoadPath;
    
    public override void Start()
    {
        asyncResult = new MulAsyncResult();
        foreach (ClassData classData in GetAllClassDatas())
        {
            var path = (classData.Attribute as ConfigAttribute).Path;
            if (CustomLoadPath != null)
                path = CustomLoadPath(path);
            var method = classData.Type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var progress = Res.Default.LoadAssetAsync<TextAsset>(path);
            progress.Callbackable().OnCallback(result =>
            {
                lock (_objects)
                {
                    _objects[0] = result.Result.text;
                    try
                    {
                        method.Invoke(null, _objects);
                    }
                    catch (Exception)
                    {
                        Log.Error("加载",method.DeclaringType,"出错");
                        throw;
                    }
                }
            });
            asyncResult.AddAsyncResult(progress);
        }
        asyncResult.Callbackable().OnCallback(result =>
        {
            Timer.RegisterFrame(() =>
            {
                Loaded = true;
                _loadedCb?.Invoke();
            });
        });
    }

    public async Task AddConfig(Type type)
    {
        var configAttribute = type.GetCustomAttribute(typeof(ConfigAttribute), false);
        if(configAttribute == null) return;
        var path = (configAttribute as ConfigAttribute).Path;
        if (CustomLoadPath != null)
            path = CustomLoadPath(path);
        var method = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var content = (await Res.Default.LoadAssetAsync<TextAsset>(path)).text;
        _objects[0] = content;
        method.Invoke(null, _objects);
    }

    public void AddLoadedCallback(Action action)
    {
        _loadedCb += action;
    }

#if UNITY_EDITOR && ILRUNTIME
    public void EditorLoad()
    {
        List<Type> allTypes = new List<Type>();
        var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            var ilrAsm = AssemblyManager.GetAssembly(ilrConfig.DllName);
            allTypes.AddRange(ilrAsm.DefinedTypes);
        }
        foreach (var type in allTypes)
        {
            CheckType(type);
        }
    }
#endif
}