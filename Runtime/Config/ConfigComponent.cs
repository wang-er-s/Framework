using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Config组件会扫描所有的有ConfigAttribute标签的配置,加载进来
    /// </summary>
    public class ConfigComponent : Singleton<ConfigComponent>
    {

        private readonly Dictionary<Type, ISingleton> allConfig = new Dictionary<Type, ISingleton>();
        private IRes res = Res.Create();
        private ProgressResult<float> loadProgress;
        public IProgressResult<float> LoadProgress => loadProgress;

        public override void Dispose()
        {
            foreach (var kv in this.allConfig)
            {
                kv.Value.Destroy();
            }
        }

        public object LoadOneConfig(Type configType)
        {
            this.allConfig.TryGetValue(configType, out ISingleton oneConfig);
            if (oneConfig != null)
            {
                oneConfig.Destroy();
            }

            string path = String.Empty;
            foreach (var item in EventSystem.Instance.GetTypesAndAttribute(typeof(ConfigAttribute)))
            {
                if (item.type == configType)
                {
                    path = (item.attribute as ConfigAttribute).Path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                Log.Error($"找不到{configType.Name}的配置表");
            }

            var oneConfigBytes = res.LoadAssetSync<TextAsset>(path).text;

            var method = configType.GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
            object category = Activator.CreateInstance(configType);
            method.Invoke(category, new[] { oneConfigBytes });
            ISingleton singleton = category as ISingleton;
            singleton.Register();

            this.allConfig[configType] = singleton;
            return category;
        }

        public void Load()
        {
            this.allConfig.Clear();
            var typeAndAttribute = EventSystem.Instance.GetTypesAndAttribute(typeof(ConfigAttribute));
            using RecyclableList<Task> recyclableListTasks = RecyclableList<Task>.Create();

            foreach ((BaseAttribute attribute, Type type) item in typeAndAttribute)
            {
                var oneConfigBytes =
                    res.LoadAssetSync<TextAsset>((item.attribute as ConfigAttribute).Path).text;
                LoadOneInThread(item.type, oneConfigBytes);
            }
        }

        public IProgressResult<float> LoadAsync()
        {
            loadProgress = ProgressResult<float>.Create(isFromPool: true);
            InternalLoadAsync(loadProgress);
            return loadProgress;
        }

        private async void InternalLoadAsync(IProgressPromise<float> promise)
        {
            this.allConfig.Clear();
            var typeAndAttribute = EventSystem.Instance.GetTypesAndAttribute(typeof(ConfigAttribute));
            float totalCount = typeAndAttribute.Count * 1.0f;
            foreach ((BaseAttribute attribute, Type type) item in typeAndAttribute)
            {
                var path = (item.attribute as ConfigAttribute).Path;
                var oneConfigBytes =
                    (await res.LoadAsset<TextAsset>(path)).text;
                //Task task = Task.Run(() =>
                //{
                LoadOneInThread(item.type, oneConfigBytes);
                promise.UpdateProgress(allConfig.Count / totalCount);
                //});
            }

            promise.SetResult();
        }

        private void LoadOneInThread(Type configType, byte[] oneConfigBytes)
        {
            var method = configType.GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
            object category = Activator.CreateInstance(configType);
            method.Invoke(category, new object[] { oneConfigBytes });

            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }
        
        private void LoadOneInThread(Type configType, string content)
        {
            var method = configType.GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
            object category = Activator.CreateInstance(configType);
            method.Invoke(category, new object[] { content });

            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }
        
    }
}