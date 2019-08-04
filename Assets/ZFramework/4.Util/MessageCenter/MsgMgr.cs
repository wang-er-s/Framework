/*
* Create by Soso
* Time : 2019-01-01-03 下午
*/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SF
{
    public class MsgMgr : Singleton<MsgMgr>
    {
        private MsgMgr() { }

        public delegate void EventListenerDelegate(Message evt);

        private Dictionary<int, EventListenerDelegate> events = new Dictionary<int, EventListenerDelegate>();

        public void AddListener(int type, EventListenerDelegate listener)
        {
            if (listener == null)
            {
                Log.E("AddListener: listener不能为空");
                return;
            }

            EventListenerDelegate myListener = null;
            events.TryGetValue(type, out myListener);
            events[type] = (EventListenerDelegate)Delegate.Combine(myListener, listener);
        }


        public void RemoveListener(int type, EventListenerDelegate listener)
        {
            if (listener == null)
            {
                Log.E("RemoveListener: listener不能为空");
                return;
            }

            events[type] = (EventListenerDelegate)Delegate.Remove(events[type], listener);
        }

        public void Clear()
        {
            events.Clear();
        }

        public void SendMessage(Message evt)
        {
            EventListenerDelegate listenerDelegate;
            if (events.TryGetValue(evt.Type, out listenerDelegate))
            {
                try
                {
                    listenerDelegate?.Invoke(evt);
                }
                catch (System.Exception e)
                {
                    Log.E("SendMessage:" + evt.Type.ToString() + e.Message + e.StackTrace, e);
                }
            }
        }

        public void SendMessage(int type, params System.Object[] param)
        {
            EventListenerDelegate listenerDelegate;
            if (events.TryGetValue(type, out listenerDelegate))
            {
                Message evt = new Message(type, param);
                try
                {
                    listenerDelegate?.Invoke(evt);
                }
                catch (System.Exception e)
                {
                    Log.E("SendMessage:" + evt.Type.ToString() + e.Message + e.StackTrace, e);
                }
            }
        }

    }
}
