using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Framework;
using Framework.Assets;
using Framework.Asynchronous;
using UnityEngine;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

public class ConfigManager : ManagerBase<ConfigManager, ConfigAttribute, string>
{
    private object[] @params = new object[1];
    private MulAsyncResult asyncResult;
    public IAsyncResult LoadAsync => asyncResult;
    public event Action LoadedCb;
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
                lock (@params)
                {
                    @params[0] = result.Result.text;
                    try
                    {
                        method.Invoke(null, @params);
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
                LoadedCb?.Invoke();
                Loaded = true;
            });
        });
    }

    public async Task AddConfig(Type type)
    {
        CheckType(type);
        var classData = GetClassData(type);
        if(classData == null) return;
        var path = (classData.Attribute as ConfigAttribute).Path;
        if (CustomLoadPath != null)
            path = CustomLoadPath(path);
        var method = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var content = (await Res.Default.LoadAssetAsync<TextAsset>(path)).text;
        @params[0] = content;
        method.Invoke(null, @params);
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