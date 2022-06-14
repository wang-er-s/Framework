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
            private Action<object, object> action;
            public string Tag;
            public object Instance;

            public void Invoke(params object[] para)
            {
                try
                {
                    action(Instance, para);
                }
                catch (Exception e)
                {
                    Log.Error("执行方法出错", Instance.GetType().Name, Tag, e);
                }
            }

            public MessageEvent(MessageEvent messageEvent, object instance)
            {
                this.action = messageEvent.action;
                this.Tag = messageEvent.Tag;
                Instance = instance;
            }

            public MessageEvent(MethodInfo info, object instance, string tag)
            {
                Instance = instance;
                Tag = tag;
                action = (ins, o) => info.Invoke(ins, o as object[]);
            }


            public MessageEvent(Action<object> action, object instance, string tag)
            {
                this.action = (ins, o) => action(o);
                Instance = instance;
                Tag = tag;
            }
        }

        // tag - methods
        private Dictionary<string, List<MessageEvent>> _subscribeTag2Methods =
            new Dictionary<string, List<MessageEvent>>(0);
        // instance - methods
        private Dictionary<object, List<MessageEvent>> _subscribeInstance2Methods =
            new Dictionary<object, List<MessageEvent>>();
        // type - methods
        private static Dictionary<Type, List<MessageEvent>> _classType2Methods =
            new Dictionary<Type, List<MessageEvent>>();

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
            var executeEvent = RecyclableList<MessageEvent>.Create();
            foreach (var td in todo)
            {
                if (td.Tag != tag) continue;
                try
                {
                    executeEvent.Add(td);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            foreach (var messageEvent in executeEvent)
            {
                messageEvent.Invoke(parameters);
            }
            executeEvent.Dispose();
        }

        /// <summary>
        /// Unregister all subscribed methods in a type
        /// 取消注册某类型中全部被监听方法
        /// </summary>
        /// <param name="val"></param>
        public void Unregister<T>(T val) where T : class
        {
            if (!_subscribeInstance2Methods.TryGetValue(val, out var methods))
            {
                return;
            }
            var tmpMethods = RecyclableList<MessageEvent>.Create();
            tmpMethods.AddRange(methods);
            foreach (var method in tmpMethods)
            {
                UnregisterOneMethod(method.Tag, val);
            }
            tmpMethods.Dispose();
        }

        /// <summary>
        /// 默认是一个instance同一个tag只注册一个方法
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="instance"></param>
        public void UnregisterOneMethod(string tag, object instance)
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
                var msgEvent = new MessageEvent(messageEvent, val);
                Register(msgEvent);
            }
        }

        public void Register<T>(T val, string tag, Action method) where T : class
        {
            Register(new MessageEvent(o => method(), val, tag));
        }

        public void Register<T, P>(T val, string tag, Action<P> method) where T : class
        {
            Register(new MessageEvent(o => method((P) (o as object[])[0]), val, tag));
        }

        public void Register<T, P, P2>(T val, string tag, Action<P, P2> method) where T : class
        {
            Register(new MessageEvent(o =>
            {
                object[] paras = o as object[];
                method((P) paras[0], (P2) paras[1]);
            }, val, tag));
        }

        public void Register<T, P, P2, P3>(T val, string tag, Action<P, P2, P3> method) where T : class
        {
            Register(new MessageEvent(o =>
            {
                object[] paras = o as object[];
                method((P) paras[0], (P2) paras[1], (P3) paras[2]);
            }, val, tag));
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

        public SubscriberAttribute(string tag)
        {
            Tags = new[] {tag};
        }

        public SubscriberAttribute(string tag1, string tag2)
        {
            Tags = new[] {tag1, tag2};
        }

        public SubscriberAttribute(string tag1, string tag2, string tag3)
        {
            Tags = new[] {tag1, tag2, tag3};
        }
    }
}