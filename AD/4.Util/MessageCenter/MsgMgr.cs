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
    public static class MsgMgr
    {
        static MsgMgr()
        {
            eventDispatcher = new EventDispatcher();
        }

        private static EventDispatcher eventDispatcher;
        
        public static void Register<T>(Action<T> listener)
        {
            eventDispatcher.Register(listener);
        }

        public static void UnRegister<T>(Action<T> listener)
        {
            eventDispatcher.UnRegister(listener);
        }

        public static void Clear()
        {
            eventDispatcher.Clear();
        }

        public static void SendMsg<T>(T msg)
        {
            eventDispatcher.SendMessage(msg);
        }

        public static void Register(string tag, Action listener)
        {
            eventDispatcher.Register(tag, listener);
        }

        public static void UnRegister(string tag, Action listener)
        {
            eventDispatcher.UnRegister(tag, listener);
        }

        public static void SendMsg(string tag)
        {
            eventDispatcher.SendMessage(tag);
        }
    }
}
