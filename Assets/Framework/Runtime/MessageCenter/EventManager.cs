using System;
using System.Collections.Generic;

namespace Framework.MessageCenter
{

    public static class EventManager
    {
        private static readonly Dictionary<Type, List<IEventListenerBase>> subscribersDic;
        private static readonly Dictionary<Type, List<MulEventListenerContainer>> mulSubscribersDic;

        static EventManager()
        {
            subscribersDic = new Dictionary<Type, List<IEventListenerBase>>();
            mulSubscribersDic = new Dictionary<Type, List<MulEventListenerContainer>>();
        }

        #region EventListener

        public static void Register<T>(IEventListener<T> listener) 
        {
            Type eventType = typeof(T);

            if (!subscribersDic.ContainsKey(eventType))
                subscribersDic[eventType] = new List<IEventListenerBase>();

            if (!SubscriptionExists(eventType, listener))
                subscribersDic[eventType].Add(listener);
        }

        public static void UnRegister<T>(IEventListener<T> listener) 
        {
            Type eventType = typeof(T);

            if (!subscribersDic.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<IEventListenerBase> subscriberList = subscribersDic[eventType];
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
                        subscribersDic.Remove(eventType);

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

        public static void TriggerEvent<T>(T newEvent)
        {
            List<IEventListenerBase> list;
            if (!subscribersDic.TryGetValue(typeof(T), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as IEventListener<T>)?.OnEvent(newEvent);
            }
        }

        private static bool SubscriptionExists(Type type, IEventListenerBase receiver)
        {
            List<IEventListenerBase> receivers;

            if (!subscribersDic.TryGetValue(type, out receivers)) return false;

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

        public static void Register<T>(IMulEventListener<T> listener, string tag)
        {
            Type eventType = typeof(T);

            if (!mulSubscribersDic.ContainsKey(eventType))
                mulSubscribersDic[eventType] = new List<MulEventListenerContainer>();

            if (!MulSubscriptionExists(eventType, listener, tag))
                mulSubscribersDic[eventType].Add(new MulEventListenerContainer(tag, listener));
        }



        public static void UnRegister<T>(IMulEventListener<T> listener, string tag)
        {
            Type eventType = typeof(T);

            if (!mulSubscribersDic.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<MulEventListenerContainer> mulSubscriberList = mulSubscribersDic[eventType];
#pragma warning disable 219
            bool listenerFound = false;
#pragma warning restore 219

            for (int i = 0; i < mulSubscriberList.Count; i++)
            {
                if (mulSubscriberList[i].Tag != tag || mulSubscriberList[i].MulEventListener != listener) continue;
                mulSubscriberList.Remove(mulSubscriberList[i]);
                listenerFound = true;

                if (mulSubscriberList.Count == 0)
                    subscribersDic.Remove(eventType);

                return;
            }

#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
        }

        public static void TriggerEvent<T>(T newEvent, string tag) 
        {
            List<MulEventListenerContainer> list;
            if (!mulSubscribersDic.TryGetValue(typeof(T), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Tag.Equals(tag)) continue;
                (list[i].MulEventListener as IMulEventListener<T>)?.OnEvent(newEvent, tag);
            }
        }

        private static bool MulSubscriptionExists(Type type, IEventListenerBase receiver, string tag)
        {
            List<MulEventListenerContainer> receivers;

            if (!mulSubscribersDic.TryGetValue(type, out receivers)) return false;

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
            subscribersDic.Clear();
            mulSubscribersDic.Clear();
        }

    }

    public static class EventRegister
    {
        public delegate void Delegate<T>(T eventType);

        public static void StartListening<T>(this IEventListener<T> caller) 
        {
            EventManager.Register(caller);
        }

        public static void StopListening<T>(this IEventListener<T> caller) 
        {
            EventManager.UnRegister(caller);
        }

        public static void StartListening<T>(this IMulEventListener<T> caller, string tag) 
        {
            EventManager.Register(caller, tag);
        }

        public static void StopListening<T>(this IMulEventListener<T> caller, string tag) 
        {
            EventManager.UnRegister(caller, tag);
        }
    }

    public interface IEventListenerBase
    {
    }

    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T @event);
    }

    public class MulEventListenerContainer
    {
        public readonly string Tag;
        public readonly IEventListenerBase MulEventListener;

        public MulEventListenerContainer(string tag, IEventListenerBase eventListenerBase)
        {
            Tag = tag;
            MulEventListener = eventListenerBase;
        }
    }

    public interface IMulEventListener<T> : IEventListenerBase
    {
        void OnEvent(T @event, string tag);
    }
}
