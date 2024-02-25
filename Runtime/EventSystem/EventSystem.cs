using System;
using System.Collections.Generic;

namespace Framework
{
    public class EventSystem : Singleton<EventSystem>, ISingletonUpdate, ISingletonLateUpdate, ISingletonRendererUpdate,
        ISingletonFixedUpdate
    {
        private class EventInfo
        {
            public IEvent IEvent { get; }
            public SceneType SceneType { get; }

            public EventInfo(IEvent iEvent, SceneType sceneType)
            {
                this.IEvent = iEvent;
                this.SceneType = sceneType;
            }
        }

        private readonly Dictionary<string, Type> allTypes = new();

        private readonly UnOrderMultiMapSet<Type, Type> attribute2Types = new();

        private readonly UnOrderMultiMapSet<Type, (BaseAttribute attribute, Type type)> attribute2TypesAndAttribute =
            new();

        private readonly Dictionary<Type, List<EventInfo>> allEvents = new();

        private Dictionary<Type, Dictionary<int, object>> allInvokes = new();

        private readonly Queue<long>[] queues = new Queue<long>[(int)InstanceQueueIndex.Max];

        private Queue<long> startQueue = new();

        public EventSystem()
        {
            for (int i = 0; i < this.queues.Length; i++)
            {
                this.queues[i] = new Queue<long>();
            }
        }

        public void Add(Dictionary<string, Type> addTypes)
        {
            foreach ((string fullName, Type type) in addTypes)
            {
                this.allTypes[fullName] = type;

                if (type.IsAbstract)
                {
                    continue;
                }

                // 记录所有的有BaseAttribute标记的的类型
                object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                foreach (object o in objects)
                {
                    this.attribute2Types.Add(o.GetType(), type);
                    this.attribute2TypesAndAttribute.Add(o.GetType(), (o as BaseAttribute, type));
                }
            }

        }

        /// <summary>
        /// 需要在所有程序集的类都加入完后才能初始化
        /// </summary>
        public void InitType()
        {

            this.allEvents.Clear();
            foreach (Type type in attribute2Types[typeof(EventAttribute)])
            {
                IEvent obj = Activator.CreateInstance(type) as IEvent;
                if (obj == null)
                {
                    throw new Exception($"type not is AEvent: {type.Name}");
                }

                object[] attrs = type.GetCustomAttributes(typeof(EventAttribute), false);
                foreach (object attr in attrs)
                {
                    EventAttribute eventAttribute = attr as EventAttribute;

                    Type eventType = obj.Type;

                    EventInfo eventInfo = new(obj, eventAttribute.SceneType);

                    if (!this.allEvents.ContainsKey(eventType))
                    {
                        this.allEvents.Add(eventType, new List<EventInfo>());
                    }

                    this.allEvents[eventType].Add(eventInfo);
                }
            }

            foreach (Type type in attribute2Types[typeof(InvokeAttribute)])
            {
                object obj = Activator.CreateInstance(type);
                IInvoke iInvoke = obj as IInvoke;
                if (iInvoke == null)
                {
                    throw new Exception($"type not is callback: {type.Name}");
                }

                object[] attrs = type.GetCustomAttributes(typeof(InvokeAttribute), false);
                foreach (object attr in attrs)
                {
                    if (!this.allInvokes.TryGetValue(iInvoke.Type, out var dict))
                    {
                        dict = new Dictionary<int, object>();
                        this.allInvokes.Add(iInvoke.Type, dict);
                    }

                    InvokeAttribute invokeAttribute = attr as InvokeAttribute;

                    try
                    {
                        dict.Add(invokeAttribute.Type, obj);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"action type duplicate: {iInvoke.Type.Name} {invokeAttribute.Type}", e);
                    }
                }
            }
        }

        public HashSet<Type> GetTypes(Type systemAttributeType)
        {
            if (!this.attribute2Types.TryGetValue(systemAttributeType, out var result))
            {
                result = new HashSet<Type>();
            }

            return result;
        }

        public HashSet<(BaseAttribute attribute, Type type)> GetTypesAndAttribute(Type systemAttributeType)
        {
            if (!this.attribute2TypesAndAttribute.TryGetValue(systemAttributeType, out var result))
            {
                result = new HashSet<(BaseAttribute attribute, Type type)>();
            }

            return result;
        }

        public Dictionary<string, Type> GetTypes()
        {
            return allTypes;
        }

        public Type GetType(string typeName)
        {
            return this.allTypes[typeName];
        }

        public void RegisterSystem(Entity component)
        {
            Type type = component.GetType();

            foreach (KeyValuePair<Type, InstanceQueueIndex> instanceQueueIndex in InstanceQueueMap.InstanceQueueMapDic)
            {
                if (instanceQueueIndex.Key.IsAssignableFrom(type))
                {
                    this.queues[(int)instanceQueueIndex.Value].Enqueue(component.Id);
                }
            }
        }

        public Queue<long> GetQueueByIndex(InstanceQueueIndex index)
        {
            return queues[(int)index];
        }

        public void Deserialize(Entity component)
        {
            if (component is IDeserializeSystem deserializeSystem)
            {
                deserializeSystem.OnDeserialize();
            }
        }

        // GetComponentSystem
        public void GetComponent(Entity entity, Entity component)
        {
            if (entity is IGetComponentSystem getSystem)
            {
                getSystem.OnGetComponent(component);
            }
        }

        // AddComponentSystem
        public void AddComponent(Entity entity, Entity component)
        {
            if (entity is IAddComponentSystem addComponentSystem)
            {
                addComponentSystem.OnAddComponent(component);
            }
        }

        public void Awake(Entity component)
        {
            if (component is IAwakeSystem awakeSystem)
            {
                awakeSystem.Awake();
            }
        }

        public void Start(Entity entity)
        {
            if (entity is IStartSystem startSystem)
            {
                startQueue.Enqueue(entity.Id);
            }
        }

        public void Awake<P1>(Entity component, P1 p1)
        {
            if (component is IAwakeSystem<P1> aAwakeSystem)
            {
                aAwakeSystem.Awake(p1);
            }
        }

        public void Awake<P1, P2>(Entity component, P1 p1, P2 p2)
        {
            if (component is IAwakeSystem<P1, P2> aAwakeSystem)
            {
                aAwakeSystem.Awake(p1, p2);
            }
        }

        public void Awake<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
        {
            if (component is IAwakeSystem<P1, P2, P3> aAwakeSystem)
            {
                aAwakeSystem.Awake(p1, p2, p3);
            }
        }

        public void Awake<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            if (component is IAwakeSystem<P1, P2, P3, P4> aAwakeSystem)
            {
                aAwakeSystem.Awake(p1, p2, p3, p4);
            }
        }

        public void Destroy(Entity component)
        {

            if (component is IDestroySystem iDestroySystem)
            {
                iDestroySystem.OnDestroy();
            }
        }

        public void Update(float deltaTime)
        {
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.Update];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (component is IUpdateSystem iUpdateSystem)
                {
                    iUpdateSystem.Update(deltaTime);
                }
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.FixedUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (component is IFixedUpdateSystem iUpdateSystem)
                {
                    iUpdateSystem.FixedUpdate(deltaTime);
                }
            }

        }

        public void RendererUpdate(float deltaTime)
        {
            while (startQueue.Count > 0)
            {
                (Root.Instance.Get(startQueue.Dequeue()) as IStartSystem)?.Start();
            }

            Queue<long> queue = this.queues[(int)InstanceQueueIndex.RendererUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (component is IRendererUpdateSystem iUpdateSystem)
                {
                    iUpdateSystem.RenderUpdate(deltaTime);
                }
            }

        }

        public void LateUpdate(float deltaTime)
        {
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.LateUpdate];
            int count = queue.Count;
            while (count-- > 0)
            {
                long instanceId = queue.Dequeue();
                Entity component = Root.Instance.Get(instanceId);
                if (component == null)
                {
                    continue;
                }

                if (component.IsDisposed)
                {
                    continue;
                }

                queue.Enqueue(instanceId);

                if (component is ILateUpdateSystem iLateUpdateSystem)
                {
                    iLateUpdateSystem.LateUpdate(deltaTime);
                }
            }
        }


        public async ETTask PublishAsync<T>(Scene scene, T a) where T : struct
        {
            List<EventInfo> iEvents;
            if (!this.allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return;
            }

            using RecyclableList<ETTask> recyclableList = RecyclableList<ETTask>.Create();

            foreach (EventInfo eventInfo in iEvents)
            {
                if (!(eventInfo.IEvent is AEvent<T> aEvent))
                {
                    Log.Error($"event error: {eventInfo.IEvent.GetType().Name}");
                    continue;
                }

                recyclableList.Add(aEvent.Handle(scene, a));
            }

            try
            {
                await ETTaskHelper.WaitAll(recyclableList);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Publish<T>(Scene scene, T a) where T : struct
        {
            List<EventInfo> iEvents;
            if (!this.allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return;
            }

            SceneType sceneType = scene.SceneType;
            foreach (EventInfo eventInfo in iEvents)
            {
                if (eventInfo.SceneType != SceneType.Root && eventInfo.SceneType != sceneType) continue;
                if (!(eventInfo.IEvent is AEvent<T> aEvent))
                {
                    Log.Error($"event error: {eventInfo.IEvent.GetType().Name}");
                    continue;
                }

                aEvent.Handle(scene, a).Coroutine();
            }
        }

        // Invoke跟Publish的区别(特别注意)
        // Invoke类似函数，必须有被调用方，否则异常，调用者跟被调用者属于同一模块，比如MoveComponent中的Timer计时器，调用跟被调用的代码均属于移动模块
        // 既然Invoke跟函数一样，那么为什么不使用函数呢? 因为有时候不方便直接调用，比如Config加载，在客户端跟服务端加载方式不一样。比如TimerComponent需要根据Id分发
        // Invoke用在定时器到时间后调用某个函数，且必须有接收方
        // 注意，不要把Invoke当函数使用，这样会造成代码可读性降低，能用函数不要用Invoke
        // publish是事件，抛出去可以没人订阅，调用者跟被调用者属于两个模块，比如任务系统需要知道道具使用的信息，则订阅道具使用事件
        public void Invoke<A>(int type, A args) where A : struct
        {
            if (!this.allInvokes.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error: {typeof(A).Name}");
            }

            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error: {typeof(A).Name} {type}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error, not AInvokeHandler: {typeof(A).Name} {type}");
            }

            aInvokeHandler.Handle(args);
        }

        public T Invoke<A, T>(int type, A args) where A : struct
        {
            if (!this.allInvokes.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error: {typeof(A).Name}");
            }

            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error: {typeof(A).Name} {type}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A, T>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error, not AInvokeHandler: {typeof(T).Name} {type}");
            }

            return aInvokeHandler.Handle(args);
        }

        public void Invoke<A>(A args) where A : struct
        {
            Invoke(0, args);
        }

        public T Invoke<A, T>(A args) where A : struct
        {
            return Invoke<A, T>(0, args);
        }
    }
}