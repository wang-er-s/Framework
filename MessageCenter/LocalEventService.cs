using System;
using System.Collections.Generic;

namespace Framework
{
    public class LocalEventService
    {
        interface ICallBackBase
        {
        }
        
        
        class MulCallbackContainer<T> : ICallBackBase
        {
            public string Tag;
            public Action<T> Action;

            public MulCallbackContainer(string tag, Action<T> action)
            {
                Tag = tag;
                Action = action;
            }

            public void Trigger(string tag, T value)
            {
                if(tag != Tag ) return;
                Action(value);
            }
        }
        
        private static Dictionary<Type, List<ICallBackBase>> _subscribersDic = new Dictionary<Type, List<ICallBackBase>>();

        internal void Register<T>(string tag, Action<T> cb)
        {
            var type = typeof(T);

            if (!_subscribersDic.TryGetValue(type, out var list))
            {
                list = new List<ICallBackBase>();
                _subscribersDic.Add(type, list);
            }
            if (!CallBackExists(list, tag, cb))
            {
                ICallBackBase callBackBase = new MulCallbackContainer<T>(tag, cb);
                list.Add(callBackBase);
            }
            else
            {
                this.Warning("repeated register....");
            }
        }

        private bool CallBackExists<T>(List<ICallBackBase> list, string tag, Action<T> cb)
        {
            return GetCallBackBase(list, tag, cb) != null;
        }

        private ICallBackBase GetCallBackBase<T>(List<ICallBackBase> list, string tag, Action<T> cb)
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
        
        internal void UnRegister<T>(string tag, Action<T> cb)
        {
            var type = typeof(T);

            if (!_subscribersDic.TryGetValue(type, out var list))
            {
                this.Warning("try unRegister empty....");
                return;
            }
            var callBackBase = GetCallBackBase(list, tag, cb);
            if (cb == null)
            {
                this.Warning("try unRegister empty....");
                return;
            }
            list.Remove(callBackBase);
        }

        public void TriggerEvent<T>(string tag, T value)
        {
            var type = typeof(T);

            if (!_subscribersDic.TryGetValue(type, out var list)) return;
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (MulCallbackContainer<T> callbackContainer in list)
            {
                callbackContainer.Trigger(tag, value);
            }
        }
        
        public void TriggerEvent<T>(T value)
        {
            TriggerEvent(string.Empty ,value);
        }
    }


    public interface IEventBroadcaster
    {
        LocalEventService LocalEventService { get; }
    }
    
    public static class BroadcasterExtension
    {
        public static void Register<T>(this IEventBroadcaster broadcaster,string tag, Action<T> cb)
        {
            broadcaster.LocalEventService.Register(tag, cb);
        }
        
        public static void Register<T>(this IEventBroadcaster broadcaster, Action<T> cb)
        {
            broadcaster.LocalEventService.Register(string.Empty, cb);
        }

        public static void UnRegister<T>(this IEventBroadcaster broadcaster, string tag, Action<T> cb)
        {
            broadcaster.LocalEventService.UnRegister(tag, cb);
        }

        public static void UnRegister<T>(this IEventBroadcaster broadcaster, Action<T> cb)
        {
            broadcaster.UnRegister(string.Empty, cb);
        }
        
    }
}