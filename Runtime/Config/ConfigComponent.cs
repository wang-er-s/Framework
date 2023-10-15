using System;
using System.Collections.Generic;
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

            byte[] oneConfigBytes = res.LoadAssetSync<TextAsset>(path).bytes;

            object category = SerializeHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);
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
                    res.LoadAssetSync<TextAsset>((item.attribute as ConfigAttribute).Path).bytes;
                LoadOneInThread(item.type, oneConfigBytes);
            }
        }

        public IProgressResult<float> LoadAsync()
        {
            ProgressResult<float> result = ProgressResult<float>.Create(isFromPool: true);
            InternalLoadAsync(result);
            return result;
        }

        private async void InternalLoadAsync(IProgressPromise<float> promise)
        {
            this.allConfig.Clear();
            var typeAndAttribute = EventSystem.Instance.GetTypesAndAttribute(typeof(ConfigAttribute));
            using RecyclableList<Task> recyclableListTasks = RecyclableList<Task>.Create();
            float totalCount = typeAndAttribute.Count * 1.0f;
            foreach ((BaseAttribute attribute, Type type) item in typeAndAttribute)
            {
                var path = (item.attribute as ConfigAttribute).Path;
                var oneConfigBytes =
                    (await res.LoadAsset<TextAsset>(path)).text;
                Task task = Task.Run(() =>
                {
                    LoadOneInThread(item.type, oneConfigBytes);
                    promise.UpdateProgress(allConfig.Count / totalCount);
                });
                recyclableListTasks.Add(task);
            }

            await Task.WhenAll(recyclableListTasks.ToArray());
            promise.SetResult();
        }

        private void LoadOneInThread(Type configType, byte[] oneConfigBytes)
        {
            object category = SerializeHelper.Deserialize(configType, oneConfigBytes, 0, oneConfigBytes.Length);

            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }
        
        private void LoadOneInThread(Type configType, string content)
        {
            object category = SerializeHelper.Deserialize(configType, content);

            lock (this)
            {
                ISingleton singleton = category as ISingleton;
                singleton.Register();
                this.allConfig[configType] = singleton;
            }
        }
        
    }
}