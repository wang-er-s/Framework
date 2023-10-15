using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 引用池。
    /// </summary>
    public static class ReferencePool
    {
        private static readonly Dictionary<Type, Queue<object>> pool = new Dictionary<Type, Queue<object>>();

        public static T Allocate<T>() where T : class
        {
            return Allocate(typeof(T)) as T;
        }

        public static object Allocate(Type type)
        {
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                return Activator.CreateInstance(type, true);
            }

            if (queue.Count == 0)
            {
                return Activator.CreateInstance(type, true);
            }

            return queue.Dequeue();
        }

        public static void Free(object obj)
        {
            if (obj == null) return;
            Type type = obj.GetType();
            Queue<object> queue = null;
            if (!pool.TryGetValue(type, out queue))
            {
                queue = new Queue<object>();
                pool.Add(type, queue);
            }

            // 一种对象最大为1000个
            if (queue.Count > 1000)
            {
                return;
            }

            if (obj is IReference reference)
            {
                reference.Clear();
            }

            queue.Enqueue(obj);
        }

        public static void Release()
        {
            pool.Clear();
        }
    }
}
