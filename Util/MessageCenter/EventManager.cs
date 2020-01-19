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
    /// <summary>
    /// MMGameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
    /// </summary>
    public struct MMGameEvent
    {
        public string EventName;
        public MMGameEvent(string newName)
        {
            EventName = newName;
        }
        static MMGameEvent e;
        public static void Trigger(string newName)
        {
            e.EventName = newName;
            EventManager.TriggerEvent(e);
        }
    }
    
    public static class EventManager
    {
        private static Dictionary<Type, List<EventListenerBase>> subscribersList;
        
        static EventManager()
        {
            subscribersList = new Dictionary<Type, List<EventListenerBase>>();
        }
        
        public static void Register<T>(EventListener<T> listener) where T : struct
        {
            Type eventType = typeof( T );

            if( !subscribersList.ContainsKey( eventType ) )
                subscribersList[eventType] = new List<EventListenerBase>();

            if( !SubscriptionExists( eventType, listener ) )
                subscribersList[eventType].Add( listener );
        }

        public static void UnRegister<T>(EventListener<T> listener) where T : struct
        {
            Type eventType = typeof( T );

            if( !subscribersList.ContainsKey( eventType ) )
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<EventListenerBase> subscriberList = subscribersList[eventType];
            bool listenerFound;
            listenerFound = false;

            if (listenerFound)
            {
				
            }

            for (int i = 0; i<subscriberList.Count; i++)
            {
                if( subscriberList[i] == listener )
                {
                    subscriberList.Remove( subscriberList[i] );
                    listenerFound = true;

                    if( subscriberList.Count == 0 )
                        subscribersList.Remove( eventType );

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
        
        public static void TriggerEvent<T>( T newEvent ) where T : struct
        {
            List<EventListenerBase> list;
            if( !subscribersList.TryGetValue( typeof( T ), out list ) )
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i=0; i<list.Count; i++)
            {
                ( list[i] as EventListener<T> ).OnEvent( newEvent );
            }
        }

        public static void Clear()
        {
            subscribersList.Clear();
        }
        
        private static bool SubscriptionExists( Type type, EventListenerBase receiver )
        {
            List<EventListenerBase> receivers;

            if( !subscribersList.TryGetValue( type, out receivers ) ) return false;

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
    
    /// <summary>
    /// Static class that allows any class to start or stop listening to events
    /// </summary>
    public static class EventRegister
    {
        public delegate void Delegate<T>( T eventType );

        public static void StartListening<T>( this EventListener<T> caller ) where T : struct
        {
            EventManager.Register( caller );
        }

        public static void StopListening<T>( this EventListener<EventType> caller ) where T : struct
        {
            EventManager.UnRegister( caller );
        }
    }
    
    /// <summary>
    /// Event listener basic interface
    /// </summary>
    public interface EventListenerBase { };

    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    public interface EventListener<T> : EventListenerBase
    {
        void OnEvent( T _event );
    }
}
