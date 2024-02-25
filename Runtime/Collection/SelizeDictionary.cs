using System;
using System.Collections.Generic;
#if MongoDb
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class SerializeDictionary<TKey,TValue>
    {
#if MongoDb
        static SerializeDictionary()
        {
            MongoHelper.RegisterStruct<SerializeKeyValue<TKey, TValue>>();
        }
#endif
        
        [BoxGroup("Dictionary/添加item")]
        [PropertyOrder(1)]
        [InfoBox("有相同的key",InfoMessageType.Error, visibleIfMemberName: "HasSameKey")]
        [ShowInInspector]
        [HideIf(nameof(hasCustomAddFunc))]
        private TKey Key;
        
        [BoxGroup("Dictionary/添加item")]
        [PropertyOrder(2)]
        [ShowInInspector]
        [HideIf(nameof(hasCustomAddFunc))]
        private TValue Value;

        [BoxGroup("Dictionary/添加item")]
        [PropertyOrder(3)]
        [Button]
        private void Add()
        {
            if (CustomAddFunc != null)
            {
                CustomAddFunc(list);
            }
            else
            {
                if (HasSameKey) return;
                list.Add(new SerializeKeyValue<TKey, TValue>(Key, Value));
                Key = default;
                Value = default;
            }
        }

        private bool HasSameKey
        {
            get
            {
                foreach (var item in list)
                {
                    if (item.Key.Equals(Key)) return true;
                }

                return false;
            }
        }

        [BoxGroup("Dictionary", false)]
        [PropertyOrder(4)]
        [ShowInInspector]
        [SerializeField]
        [ListDrawerSettings(HideAddButton = true)]
#if MongoDb
        [BsonElement]
#endif
        [OnValueChanged(nameof(OnListChanged))]
        private List<SerializeKeyValue<TKey, TValue>> list = new List<SerializeKeyValue<TKey, TValue>>();

#if MongoDb
        [BsonIgnore]
#endif
        public IReadOnlyDictionary<TKey, TValue> Dic
        {
            get
            {
                if (dic == null)
                {
                    OnListChanged();
                }

                return dic;
            }
        }

        private Dictionary<TKey, TValue> dic;

        private bool hasCustomAddFunc => CustomAddFunc != null && CustomAddFunc.GetInvocationList().Length > 0;
        public event Action<List<SerializeKeyValue<TKey,TValue>>> CustomAddFunc;

        private void OnListChanged()
        {
            if(list == null) return;
            dic ??= new Dictionary<TKey, TValue>();
            dic.Clear();
            foreach (var value in list)
            {
                dic[value.Key] = value.Value;
            }
        }

        public bool AddData(TKey key, TValue value)
        {
            if (Dic.ContainsKey(key))
            {
                return false;
            }

            list.Add(new SerializeKeyValue<TKey, TValue>(key,value));
            dic[key] = value;
            return true;
        }

        public bool RemoveData(TKey key)
        {
            _ = Dic;
            if (!dic.Remove(key)) return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key.Equals(key))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public struct SerializeKeyValue<TKey, TValue>
    {
        [VerticalGroup()]
        [LabelWidth(50)]
        public TKey Key;
        [VerticalGroup()]
        public TValue Value;

        public SerializeKeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}