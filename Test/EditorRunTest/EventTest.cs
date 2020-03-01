using System;
using Framework;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class 消息系统
    {
        class AMsg
        {
            public string Name;
        }
        
        class BMsg
        {
            public string Name;
            static readonly BMsg e = new BMsg();
            public static void Trigger(string newName)
            {
                e.Name = newName;
                EventManager.TriggerEvent(e);
            }
        }

        class AModule : EventListener<AMsg> , EventListener<BMsg>, MulEventListener<CMsg>
        {
            void EventListener<AMsg>.OnEvent(AMsg eventType)
            {
                Debug.Log(eventType.Name);
            }

            void EventListener<BMsg>.OnEvent(BMsg _event)
            {
                Debug.Log(_event.Name);
            }

            void MulEventListener<CMsg>.OnEvent(CMsg value)
            {
                Debug.Log(value.Name);
            }
        }
        
        [Test]
        public void 测试注册广播()
        {
            EventListener<AMsg> aModule = new AModule();
            aModule.StartListening();
            EventListener<BMsg> bMsg = (EventListener<BMsg>) aModule;
            bMsg.StartListening();
            EventManager.TriggerEvent(new AMsg {Name = "HH"});
            BMsg.Trigger("BB");
        }

        [Test]
        public void 复用广播()
        {
            MulEventListener<CMsg> aModule = new AModule();
            aModule.StartListening("TAG");
            CMsg.Trigger("TAG", "name");
        }
    }

    public interface MulEventListener<T>
    {
        void OnEvent(T value);
    }

    public static class MulEventListenerExtension
    {
        public static void StartListening<T>(this MulEventListener<T> listener, string tag)
        {
            
        }
    }

    public class CMsg
    {
        public string Name;
        private static CMsg _e = new CMsg();

        public static void Trigger(string tag, string name)
        {
            
        }
    }
}