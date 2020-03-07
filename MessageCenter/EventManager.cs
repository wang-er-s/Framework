/*
* Create by Soso
* Time : 2019-01-01-03 下午
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{

    public static class EventManager
    {
        private static Dictionary<Type, List<EventListenerBase>> _subscribersDic;
        private static Dictionary<Type, List<MulEventListenerContainer>> _mulSubscribersDic;

        static EventManager()
        {
            _subscribersDic = new Dictionary<Type, List<EventListenerBase>>();
            _mulSubscribersDic = new Dictionary<Type, List<MulEventListenerContainer>>();
        }

        #region EventListener

        public static void Register<T>(EventListener<T> listener) where T : class
        {
            Type eventType = typeof(T);

            if (!_subscribersDic.ContainsKey(eventType))
                _subscribersDic[eventType] = new List<EventListenerBase>();

            if (!SubscriptionExists(eventType, listener))
                _subscribersDic[eventType].Add(listener);
        }

        public static void UnRegister<T>(EventListener<T> listener) where T : class
        {
            Type eventType = typeof(T);

            if (!_subscribersDic.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<EventListenerBase> subscriberList = _subscribersDic[eventType];
#pragma warning disable 219
            bool listenerFound = false;
#pragma warning restore 219

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)
                {
                    subscriberList.Remove(subscriberList[i]);
                    listenerFound = true;

                    if (subscriberList.Count == 0)
                        _subscribersDic.Remove(eventType);

                    return;
                }
            }

#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
        }

        public static void TriggerEvent<T>(T newEvent) where T : class
        {
            List<EventListenerBase> list;
            if (!_subscribersDic.TryGetValue(typeof(T), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as EventListener<T>)?.OnEvent(newEvent);
            }
        }

        private static bool SubscriptionExists(Type type, EventListenerBase receiver)
        {
            List<EventListenerBase> receivers;

            if (!_subscribersDic.TryGetValue(type, out receivers)) return false;

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        #endregion

        #region MulEventListener

        public static void Register<T>(MulEventListener<T> listener, string tag) where T : class
        {
            Type eventType = typeof(T);

            if (!_mulSubscribersDic.ContainsKey(eventType))
                _mulSubscribersDic[eventType] = new List<MulEventListenerContainer>();

            if (!MulSubscriptionExists(eventType, listener, tag))
                _mulSubscribersDic[eventType].Add(new MulEventListenerContainer(tag, listener));
        }



        public static void UnRegister<T>(MulEventListener<T> listener, string tag) where T : class
        {
            Type eventType = typeof(T);

            if (!_mulSubscribersDic.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<MulEventListenerContainer> mulSubscriberList = _mulSubscribersDic[eventType];
#pragma warning disable 219
            bool listenerFound = false;
#pragma warning restore 219

            for (int i = 0; i < mulSubscriberList.Count; i++)
            {
                if (mulSubscriberList[i].Tag != tag || mulSubscriberList[i].MulEventListener != listener) continue;
                mulSubscriberList.Remove(mulSubscriberList[i]);
                listenerFound = true;

                if (mulSubscriberList.Count == 0)
                    _subscribersDic.Remove(eventType);

                return;
            }

#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
        }

        public static void TriggerEvent<T>(T newEvent, string tag) where T : class
        {
            List<MulEventListenerContainer> list;
            if (!_mulSubscribersDic.TryGetValue(typeof(T), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Tag.Equals(tag)) continue;
                (list[i].MulEventListener as MulEventListener<T>)?.OnEvent(newEvent, tag);
            }
        }

        private static bool MulSubscriptionExists(Type type, EventListenerBase receiver, string tag)
        {
            List<MulEventListenerContainer> receivers;

            if (!_mulSubscribersDic.TryGetValue(type, out receivers)) return false;

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i].MulEventListener == receiver && receivers[i].Tag == tag)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        #endregion

        public static void Clear()
        {
            _subscribersDic.Clear();
            _mulSubscribersDic.Clear();
        }

    }

    public static class EventRegister
    {
        public delegate void Delegate<T>(T eventType);

        public static void StartListening<T>(this EventListener<T> caller) where T : class
        {
            EventManager.Register(caller);
        }

        public static void StopListening<T>(this EventListener<T> caller) where T : class
        {
            EventManager.UnRegister(caller);
        }

        public static void StartListening<T>(this MulEventListener<T> caller, string tag) where T : class
        {
            EventManager.Register(caller, tag);
        }

        public static void StopListening<T>(this MulEventListener<T> caller, string tag) where T : class
        {
            EventManager.UnRegister(caller, tag);
        }
    }

    public interface EventListenerBase
    {
    }

    public interface EventListener<T> : EventListenerBase
    {
        void OnEvent(T _event);
    }

    public class MulEventListenerContainer
    {
        public string Tag;
        public EventListenerBase MulEventListener;

        public MulEventListenerContainer(string tag, EventListenerBase eventListenerBase)
        {
            Tag = tag;
            MulEventListener = eventListenerBase;
        }
    }

    public interface MulEventListener<T> : EventListenerBase
    {
        void OnEvent(T _event, string tag);
    }
}
