/*
* Create by Soso
* Time : 2019-01-01-03 下午
*/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class GameEvent
    {
        public string EventName;
        public GameEvent(string newName)
        {
            EventName = newName;
        }
        static readonly GameEvent e = new GameEvent(string.Empty);
        public static void Trigger(string newName)
        {
            e.EventName = newName;
            EventManager.TriggerEvent(e);
        }
    }

    public static class EventManager
    {
        private static Dictionary<Type, List<EventListenerBase>> _subscribersList;
        
        static EventManager()
        {
            _subscribersList = new Dictionary<Type, List<EventListenerBase>>();
        }
        
        public static void Register<T>(EventListener<T> listener) where T : class
        {
            Type eventType = typeof( T );

            if( !_subscribersList.ContainsKey( eventType ) )
                _subscribersList[eventType] = new List<EventListenerBase>();

            if( !SubscriptionExists( eventType, listener ) )
                _subscribersList[eventType].Add( listener );
        }

        public static void UnRegister<T>(EventListener<T> listener) where T : class
        {
            Type eventType = typeof( T );

            if( !_subscribersList.ContainsKey( eventType ) )
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<EventListenerBase> subscriberList = _subscribersList[eventType];
            bool listenerFound = false;

            for (int i = 0; i<subscriberList.Count; i++)
            {
                if( subscriberList[i] == listener )
                {
                    subscriberList.Remove( subscriberList[i] );
                    listenerFound = true;

                    if( subscriberList.Count == 0 )
                        _subscribersList.Remove( eventType );

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
        
        public static void TriggerEvent<T>( T newEvent ) where T : class
        {
            List<EventListenerBase> list;
            if( !_subscribersList.TryGetValue( typeof( T ), out list ) )
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i=0; i<list.Count; i++)
            {
                ( list[i] as EventListener<T> )?.OnEvent( newEvent );
            }
        }

        public static void Clear()
        {
            _subscribersList.Clear();
        }
        
        private static bool SubscriptionExists( Type type, EventListenerBase receiver )
        {
            List<EventListenerBase> receivers;

            if( !_subscribersList.TryGetValue( type, out receivers ) ) return false;

            bool exists = false;

            for (int i=0; i<receivers.Count; i++)
            {
                if( receivers[i] == receiver )
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
        
    }
    
    public static class EventRegister
    {
        public delegate void Delegate<T>( T eventType );

        public static void StartListening<T>( this EventListener<T> caller ) where T : class
        {
            EventManager.Register( caller );
        }

        public static void StopListening<T>( this EventListener<T> caller ) where T : class
        {
            EventManager.UnRegister( caller );
        }
    }
    
    public interface EventListenerBase { };

    public interface EventListener<T> : EventListenerBase
    {
        void OnEvent( T _event );
    }
}
