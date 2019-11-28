/*
* Create by Soso
* Time : 2019-01-01-03 下午
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AD
{
    public class MsgMgr : PrivateSingleton<MsgMgr>
    {
        private MsgMgr()
        {
            _eventDispatcher = new EventDispatcher();
        }

        private EventDispatcher _eventDispatcher;

        public static void Register<T>(Action<T> listener)
        {
            Instance._eventDispatcher.Register(listener);
        }

        public static void UnRegister<T>(Action<T> listener)
        {
            Instance._eventDispatcher.UnRegister(listener);
        }

        public static void Clear()
        {
            Instance._eventDispatcher.Clear();
        }

        public static void SendMsg<T>(T msg)
        {
            Instance._eventDispatcher.SendMessage(msg);
        }

        public static void Register(int tag, Action listener)
        {
            Instance._eventDispatcher.Register(tag, listener);
        }

        public static void UnRegister(int tag, Action listener)
        {
            Instance._eventDispatcher.UnRegister(tag, listener);
        }

        public static void SendMsg(int tag)
        {
            Instance._eventDispatcher.SendMessage(tag);
        }
    }
}
