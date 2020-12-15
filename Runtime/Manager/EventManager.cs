using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.MessageCenter
{
    public class Message
    {
        private struct MessageEvent
        {
            public MethodInfo MethodInfo;
            public string Tag;
            public string ParaStr;
            public object Instance;

            public void Invoke(params object[] para)
            {
                if (para.GetType() == TriggerType.Type)
                {
                    MethodInfo.Invoke(Instance, null);
                    return;
                }
                MethodInfo.Invoke(Instance, para);
            }

            public MessageEvent(MessageEvent messageEvent) : this(messageEvent.MethodInfo, messageEvent.Instance, messageEvent.Tag, messageEvent.ParaStr)
            {
            }

            public MessageEvent(MethodInfo info, object instance, string tag, string paraStr)
            {
                MethodInfo = info;
                Instance = instance;
                Tag = tag;
                ParaStr = paraStr;
            }
        }
        
        private struct TriggerType
        {
            public static TriggerType Trigger = new TriggerType();
            public static Type Type = typeof(TriggerType);
        }

        private Dictionary<string, List<MessageEvent>> _subscribeParaType2Methods = new Dictionary<string, List<MessageEvent>>(0);
        private Dictionary<object, List<MessageEvent>> _subscribeInstance2Methods = new Dictionary<object, List<MessageEvent>>();
        private Dictionary<Type, List<MessageEvent>> _classType2Methods = new Dictionary<Type, List<MessageEvent>>();

        /// <summary>
        /// Default static JEvent
        /// 默认静态JEvent
        /// </summary>
        public static Message defaultEvent
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Message();
                }
                return _instance;
            }
        }
        private static Message _instance = new Message();

        /// <summary>
        /// Post parameters to all subscibed methods
        /// 将参数广播到全部监听方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        public void Post(string tag, params object[] parameters)
        {
            var paraStr = string.Join(",", parameters.Select(para => para.GetType()));
            if (!_subscribeParaType2Methods.TryGetValue(paraStr, out var todo)) return;
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
            var type = val.GetType();
            if (!_classType2Methods.TryGetValue(type, out var methods))
            {
                return;
            }

            foreach (var method in methods)
            {
                UnregisterOneMethod(method.ParaStr, val);
            }
        }
        
        public void Unregister<T>(T val, MethodInfo _method) where T : class
        {
            if (!_subscribeInstance2Methods.TryGetValue(val, out var methods))
            {
                return;
            }
            foreach (var method in methods)
            {
                if (method.MethodInfo == _method)
                {
                    UnregisterOneMethod(method.ParaStr, val);
                    break;
                }
            }
        }
        
        private void UnregisterOneMethod(string paraStr, object instance)
        {
            if (_subscribeParaType2Methods.TryGetValue(paraStr, out var values))
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].Instance != instance) continue;
                    values.RemoveAt(i);
                    break;
                }
                if (values.Count <= 0)
                {
                    _subscribeParaType2Methods.Remove(paraStr);
                }
            }
            
            if (_subscribeInstance2Methods.TryGetValue(instance, out var events))
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].ParaStr != paraStr) continue;
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
            var type = typeof(T);
            
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
                    var paras = method.GetParameters();
                    string paraStr = string.Join(",", paras.Select((para => para.ParameterType)));
                    events.Add(new MessageEvent(method, val, ((SubscriberAttribute) methodAttr[0]).Tag, paraStr));
                }
                _classType2Methods[type] = events;
            }

            foreach (var messageEvent in events)
            {
                var msgEvent = new MessageEvent(messageEvent) {Instance = val};
                Register(msgEvent);
            }
        }

        public void Register<T, ParaT>(T val, MethodInfo method) where T : class
        {
            var paraStr = string.Join(",", method.GetParameters().Select((para) => para.ParameterType));
            var methodAttr = method.GetCustomAttributes(typeof(SubscriberAttribute), false);
            var HasAttr = methodAttr.Length > 0;
            if (!HasAttr)
            {
                Log.Error("必须要有",nameof(SubscriberAttribute),"的标签");  
                return;
            }
            var msgEvent = new MessageEvent(method, val, ((SubscriberAttribute) methodAttr[0]).Tag, paraStr);
            Register(msgEvent);
        }

        private void Register(MessageEvent messageEvent)
        {
            if (!_subscribeInstance2Methods.TryGetValue(messageEvent.Instance, out var instanceEvents))
            {
                instanceEvents = new List<MessageEvent>();
                _subscribeInstance2Methods[messageEvent.Instance] = instanceEvents;
            }
            instanceEvents.Add(messageEvent);

            if (!_subscribeParaType2Methods.TryGetValue(messageEvent.ParaStr, out var paraTypeEvents))
            {
                paraTypeEvents = new List<MessageEvent>();
                _subscribeParaType2Methods[messageEvent.ParaStr] = paraTypeEvents;
            }
            paraTypeEvents.Add(messageEvent);
        }
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class SubscriberAttribute : Attribute
    {
        public string Tag { get; private set; }
        public SubscriberAttribute(string tag = null)
        {
            Tag = tag ?? string.Empty;
        }
    }
}
