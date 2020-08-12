using System;
using System.Collections.Generic;

namespace Framework.MessageCenter
{
    public static class LocalEventService
    {
        interface ICallBackBase
        {
        }

        class MulCallbackContainer<T> : ICallBackBase
        {
            public readonly string Tag;
            public readonly Action<T> Action;

            public MulCallbackContainer(string tag, Action<T> action)
            {
                Tag = tag;
                Action = action;
            }

            public void Trigger(string tag, T value)
            {
                if (tag != Tag) return;
                Action(value);
            }
        }

        private static readonly Dictionary<IEventBroadcaster, Dictionary<Type, List<ICallBackBase>>> subscribersDic =
            new Dictionary<IEventBroadcaster, Dictionary<Type, List<ICallBackBase>>>();

        internal static void Register<T>(IEventBroadcaster eventBroadcaster, string tag, Action<T> cb)
        {
            var type = typeof(T);

            if (!subscribersDic.TryGetValue(eventBroadcaster, out var dic))
            {
                dic = new Dictionary<Type, List<ICallBackBase>>();
                subscribersDic.Add(eventBroadcaster, dic);
            }

            if (!dic.TryGetValue(type, out var list))
            {
                list = new List<ICallBackBase>();
                dic.Add(type, list);
            }

            if (!CallBackExists(list, tag, cb))
            {
                ICallBackBase callBackBase = new MulCallbackContainer<T>(tag, cb);
                list.Add(callBackBase);
            }
            else
            {
                Log.Warning("repeated register....");
            }
        }

        private static bool CallBackExists<T>(List<ICallBackBase> list, string tag, Action<T> cb)
        {
            return GetCallBackBase(list, tag, cb) != null;
        }

        private static ICallBackBase GetCallBackBase<T>(List<ICallBackBase> list, string tag, Action<T> cb)
        {
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (MulCallbackContainer<T> callbackContainer in list)
            {
                if (callbackContainer.Tag == tag && callbackContainer.Action == cb)
                {
                    return callbackContainer;
                }
            }

            return null;
        }

        internal static void UnRegister<T>(IEventBroadcaster eventBroadcaster, string tag, Action<T> cb)
        {
            var type = typeof(T);
            if (!subscribersDic.TryGetValue(eventBroadcaster, out var dic))
            {
                Log.Warning("try unRegister empty....");
                return;
            }

            if (!dic.TryGetValue(type, out var list))
            {
                Log.Warning("try unRegister empty....");
                return;
            }

            var callBackBase = GetCallBackBase(list, tag, cb);
            if (cb == null)
            {
                Log.Warning("try unRegister empty....");
                return;
            }

            list.Remove(callBackBase);
        }

        public static void TriggerEvent<T>(IEventBroadcaster eventBroadcaster, string tag, T value)
        {
            var type = typeof(T);

            if (!subscribersDic.TryGetValue(eventBroadcaster, out var dic)) return;
            if (!dic.TryGetValue(type, out var list)) return;
            foreach (var callBackBase in list)
            {
                var callbackContainer = (MulCallbackContainer<T>) callBackBase;
                callbackContainer.Trigger(tag, value);
            }
        }

        public static void Clear(IEventBroadcaster eventBroadcaster)
        {
            if (subscribersDic.ContainsKey(eventBroadcaster))
                subscribersDic.Remove(eventBroadcaster);
        }
    }


    public interface IEventBroadcaster
    {
    }

    public static class BroadcasterExtension
    {
        public static void Register<T>(this IEventBroadcaster broadcaster, string tag, Action<T> cb)
        {
            LocalEventService.Register(broadcaster, tag, cb);
        }

        public static void Register<T>(this IEventBroadcaster broadcaster, Action<T> cb)
        {
            broadcaster.Register(string.Empty, cb);
        }

        public static void UnRegister<T>(this IEventBroadcaster broadcaster, string tag, Action<T> cb)
        {
            LocalEventService.UnRegister(broadcaster, tag, cb);
        }

        public static void UnRegister<T>(this IEventBroadcaster broadcaster, Action<T> cb)
        {
            broadcaster.UnRegister(string.Empty, cb);
        }

        public static void TriggerEvent<T>(this IEventBroadcaster broadcaster, string tag, T value)
        {
            LocalEventService.TriggerEvent(broadcaster, tag, value);
        }

        public static void TriggerEvent<T>(this IEventBroadcaster broadcaster, T value)
        {
            broadcaster.TriggerEvent(string.Empty, value);
        }
        
        
    }
}