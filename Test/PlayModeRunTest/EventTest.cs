using System;
using Framework;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class 消息系统
    {
        struct AMsg
        {
            public string Name;
        }

        class AModule : EventListener<AMsg>
        {
            void EventListener<AMsg>.OnEvent(AMsg eventType)
            {
                Debug.Log(eventType.Name);
            }
        }
        
        [Test]
        public void 测试注册广播()
        {
            EventListener<AMsg> aModule = new AModule();
            //aModule.StartListening();
            EventManager.TriggerEvent(new AMsg {Name = "HH"});
            throw new Exception("aa");
        }
    }
}