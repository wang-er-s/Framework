using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class ConfigManager : GameModuleWithAttribute<ConfigManager, ConfigAttribute, string>
    {
        private object[] @params = new object[1];
        private ProgressResult<float> asyncResult;
        public IProgressResult<float> LoadAsync => asyncResult;
        public bool Loaded { get; private set; }
        public static Func<string, string> CustomLoadPath;

        public override void OnStart()
        {
            asyncResult = new ProgressResult<float>();
            Executors.RunOnCoroutineNoReturn(Load(asyncResult));
            asyncResult.Callbackable().OnCallback(result =>
            {
                Loaded = true;
            });
        }

        IEnumerator Load(IProgressPromise<float> promise)
        {
            IRes res = Res.Create();
            int totalCount = ClassDataMap.Count;
            int index = 0;
            foreach (ClassData classData in GetAllClassData())
            {
                var path = (classData.Attribute as ConfigAttribute).Path;
                if (CustomLoadPath != null)
                    path = CustomLoadPath(path);
                var method = classData.Type.GetMethod("Load",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                var content = res.LoadAsset<TextAsset>(path).text;
                @params[0] = content;
                try
                {
                    method.Invoke(null, @params);
                }
                catch (Exception)
                {
                    Log.Error("加载", method.DeclaringType, "出错");
                    throw;
                }

                index++;
                promise.UpdateProgress(index * 1.0f / totalCount);
                yield return null;
            }
            promise.SetResult();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void Shutdown()
        {
        }

        public async Task AddConfig(Type type)
        {
            IRes res = Res.Create();
            CheckType(type);
            var classData = GetClassData(type);
            if (classData == null) return;
            var path = (classData.Attribute as ConfigAttribute).Path;
            if (CustomLoadPath != null)
                path = CustomLoadPath(path);
            var method = type.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var content = (await res.LoadAssetAsync<TextAsset>(path)).text;
            @params[0] = content;
            method.Invoke(null, @params);
            res.Dispose();
        }
    }
}