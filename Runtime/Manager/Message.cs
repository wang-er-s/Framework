using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tool;

namespace Framework.MessageCenter
{
    public class Message
    {
        private struct MessageEvent
        {
            public MethodInfo MethodInfo;
            public string Tag;
            public object Instance;

            public void Invoke(params object[] para)
            {
                MethodInfo.Invoke(Instance, para);
            }

            public MessageEvent(MessageEvent messageEvent) : this(messageEvent.MethodInfo, messageEvent.Instance, messageEvent.Tag)
            {
            }

            public MessageEvent(MethodInfo info, object instance, string tag)
            {
                MethodInfo = info;
                Instance = instance;
                Tag = tag;
            }
        }

        private Dictionary<string, List<MessageEvent>> _subscribeTag2Methods = new Dictionary<string, List<MessageEvent>>(0);
        private Dictionary<object, List<MessageEvent>> _subscribeInstance2Methods = new Dictionary<object, List<MessageEvent>>();
        private Dictionary<Type, List<MessageEvent>> _classType2Methods = new Dictionary<Type, List<MessageEvent>>();

        /// <summary>
        /// Default static JEvent
        /// 默认静态JEvent
        /// </summary>
        public static Message defaultEvent => _instance ?? (_instance = new Message());
        private static Message _instance = new Message();

        /// <summary>
        /// Post parameters to all subscibed methods
        /// 将参数广播到全部监听方法
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parameters"></param>
        public void Post(string tag, params object[] parameters)
        {
            if (!_subscribeTag2Methods.TryGetValue(tag, out var todo)) return;
            if (todo.Count == 0) return;
            foreach (var td in todo)
            {
                if(td.Tag != tag) continue;
                try
                {
                    td.Invoke(parameters);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        /// <summary>
        /// Unregister all subscribed methods in a type
        /// 取消注册某类型中全部被监听方法
        /// </summary>
        /// <param name="val"></param>
        public void Unregister<T>(T val) where T : class
        {
            var type = val.GetCLRType();
            if (!_classType2Methods.TryGetValue(type, out var methods))
            {
                return;
            }

            foreach (var method in methods)
            {
                UnregisterOneMethod(method.Tag, val);
            }
        }
        
        public void Unregister<T>(T val, MethodInfo method) where T : class
        {
            if (!_subscribeInstance2Methods.TryGetValue(val, out var methods))
            {
                return;
            }
            foreach (var _method in methods)
            {
                if (_method.MethodInfo == method)
                {
                    UnregisterOneMethod(_method.Tag, val);
                    break;
                }
            }
        }
        
        private void UnregisterOneMethod(string tag, object instance)
        {
            if (_subscribeTag2Methods.TryGetValue(tag, out var values))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].Instance != instance) continue;
                    values.RemoveAt(i);
                    break;
                }
                if (values.Count <= 0)
                {
                    _subscribeTag2Methods.Remove(tag);
                }
            }
            
            if (_subscribeInstance2Methods.TryGetValue(instance, out var events))
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].Tag != tag) continue;
                    events.RemoveAt(i);
                    break;
                }
                if (events.Count <= 0)
                {
                    _subscribeInstance2Methods.Remove(instance);
                }
            }
        }

        public void Register<T>(T val)
        {
            var type = val.GetCLRType();
            
            if (_subscribeInstance2Methods.ContainsKey(val))
            {
                Log.Error($"{type.FullName}已注册");
                return;
            }

            //如果没有缓存
            if (!_classType2Methods.TryGetValue(type, out var events))
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                events = new List<MessageEvent>();
                foreach (var method in methods)
                {
                    var methodAttr = method.GetCustomAttributes(typeof(SubscriberAttribute), false);
                    var HasAttr = methodAttr.Length > 0;
                    if (!HasAttr)
                    {
                        continue;
                    }
                    foreach (var tag in ((SubscriberAttribute) methodAttr[0]).Tags)
                    {
                        events.Add(new MessageEvent(method, val, tag));
                    }
                }
                _classType2Methods[type] = events;
            }

            foreach (var messageEvent in events)
            {
                var msgEvent = new MessageEvent(messageEvent) {Instance = val};
                Register(msgEvent);
            }
        }

        public void Register<T>(T val, MethodInfo method) where T : class
        {
            var methodAttr = method.GetCustomAttributes(typeof(SubscriberAttribute), false);
            var HasAttr = methodAttr.Length > 0;
            if (!HasAttr)
            {
                Log.Error("必须要有",nameof(SubscriberAttribute),"的标签");  
                return;
            }
            foreach (var tag in ((SubscriberAttribute) methodAttr[0]).Tags)
            {
                Register(new MessageEvent(method, val, tag));
            }
        }

        private void Register(MessageEvent messageEvent)
        {
            if (!_subscribeInstance2Methods.TryGetValue(messageEvent.Instance, out var instanceEvents))
            {
                instanceEvents = new List<MessageEvent>();
                _subscribeInstance2Methods[messageEvent.Instance] = instanceEvents;
            }
            instanceEvents.Add(messageEvent);

            if (!_subscribeTag2Methods.TryGetValue(messageEvent.Tag, out var paraTypeEvents))
            {
                paraTypeEvents = new List<MessageEvent>();
                _subscribeTag2Methods[messageEvent.Tag] = paraTypeEvents;
            }
            paraTypeEvents.Add(messageEvent);
        }

        public void Clear()
        {
            _subscribeInstance2Methods.Clear();
            _classType2Methods.Clear();
            _subscribeTag2Methods.Clear();
        }
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class SubscriberAttribute : Attribute
    {
        public string[] Tags { get; }
        public SubscriberAttribute(params string[] tags)
        {
            if (tags.Length <= 0)
            {
                Log.Error("必须添加至少一个标签");
            }
            Tags = tags;
        }
    }
}
