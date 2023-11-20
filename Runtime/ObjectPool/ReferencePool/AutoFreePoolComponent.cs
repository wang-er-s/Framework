using System.Collections.Generic;

namespace Framework
{
    public class AutoFreePoolComponent: Entity, IAwakeSystem, IDestroySystem
    {
        private HashSet<object> needFreeObj;
        public void Awake()
        {
            needFreeObj = ReferencePool.Allocate<HashSet<object>>();
        }

        public T Allocate<T>() where T : class
        {
            var obj = ReferencePool.Allocate<T>();
            needFreeObj.Add(obj);
            return obj;
        }

        public void Free(object obj)
        {
            ReferencePool.Free(obj);
            needFreeObj.Remove(obj);
        }

        public void OnDestroy()
        {
            foreach (object obj in needFreeObj)
            {
                ReferencePool.Free(obj);
            }
            needFreeObj.Clear();
            ReferencePool.Free(needFreeObj);
        }

    }

    public static class AutoFreePoolExtension
    {
        public static T Allocate<T>(this Entity entity) where T : class
        {
            var auto = entity.GetComponent<AutoFreePoolComponent>();
            if (auto == null)
                auto = entity.AddComponent<AutoFreePoolComponent>();
            return auto.Allocate<T>();
        }
        
        public static void Free(this Entity entity, object obj)
        {
            var auto = entity.GetComponent<AutoFreePoolComponent>();
            if (auto == null)
                auto = entity.AddComponent<AutoFreePoolComponent>();
            auto.Free(obj);
        } 
    }
}