using System;
using System.Collections.Generic;

namespace Framework
{
    public static class Game
    {
        private static readonly Dictionary<Type, ISingleton> singletonTypes = new Dictionary<Type, ISingleton>();

        private static readonly Stack<ISingleton> singletons = new Stack<ISingleton>();
        private static readonly Queue<ISingleton> updates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> fixedUpdates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> rendererUpdates = new Queue<ISingleton>();
        private static readonly Queue<ISingleton> lateUpdates = new Queue<ISingleton>();
        private static readonly Queue<ETTask> frameFinishTask = new Queue<ETTask>();

        public static T AddSingleton<T>() where T : Singleton<T>, new()
        {
            T singleton = new T();
            AddSingleton(singleton);
            return singleton;
        }

        public static void AddSingleton(ISingleton singleton)
        {
            Type singletonType = singleton.GetType();
            if (singletonTypes.ContainsKey(singletonType))
            {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            singletonTypes.Add(singletonType, singleton);
            singletons.Push(singleton);

            singleton.Register();

            if (singleton is ISingletonAwake awake)
            {
                awake.Awake();
            }

            if (singleton is ISingletonUpdate)
            {
                updates.Enqueue(singleton);
            }
            
            if (singleton is ISingletonFixedUpdate)
            {
                fixedUpdates.Enqueue(singleton);
            } 
            
            if (singleton is ISingletonRendererUpdate)
            {
                rendererUpdates.Enqueue(singleton);
            }

            if (singleton is ISingletonLateUpdate)
            {
                lateUpdates.Enqueue(singleton);
            }
        }

        public static async ETTask WaitFrameFinish()
        {
            ETTask task = ETTask.Create(true);
            frameFinishTask.Enqueue(task);
            await task;
        }

        public static void Update(float deltaTime)
        {
            int count = updates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = updates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonUpdate update)
                {
                    continue;
                }

                updates.Enqueue(singleton);
                update.Update(deltaTime);
            }
        }
        
        public static void FixedUpdate(float deltaTime)
        {
            int count = fixedUpdates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = fixedUpdates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonFixedUpdate update)
                {
                    continue;
                }

                fixedUpdates.Enqueue(singleton);

                update.FixedUpdate(deltaTime);
            }
        } 
        
        public static void RendererUpdate(float deltaTime)
        {
            int count = rendererUpdates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = rendererUpdates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonRendererUpdate update)
                {
                    continue;
                }

                rendererUpdates.Enqueue(singleton);
                update.RendererUpdate(deltaTime);
            }
        }

        public static void LateUpdate(float deltaTime)
        {
            int count = lateUpdates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = lateUpdates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonLateUpdate lateUpdate)
                {
                    continue;
                }

                lateUpdates.Enqueue(singleton);
                lateUpdate.LateUpdate(deltaTime);
            }
        }

        public static void FrameFinishUpdate()
        {
            while (frameFinishTask.Count > 0)
            {
                ETTask task = frameFinishTask.Dequeue();
                task.SetResult();
            }
        }

        public static void Close()
        {
            // 顺序反过来清理
            while (singletons.Count > 0)
            {
                ISingleton iSingleton = singletons.Pop();
                iSingleton.Destroy();
            }

            singletonTypes.Clear();
        }
    }
}