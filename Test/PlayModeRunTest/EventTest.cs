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

        class AModule : EventListener<AMsg> , EventListener<BMsg>
        {
            void EventListener<AMsg>.OnEvent(AMsg eventType)
            {
                Debug.Log(eventType.Name);
            }

            void EventListener<BMsg>.OnEvent(BMsg _event)
            {
                Debug.Log(_event.Name);
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
    }
}