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

            void MulEventListener<CMsg>.OnEvent(CMsg value, string tag)
            {
                if (tag == TAG)
                    this.Msg("tag1111", value.Name);
                else
                    this.Msg("tag2222", value.Name);
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

        private const string TAG = "TAG";
        private const string TAG2 = "TAG2";
        
        [Test]
        public void 复用广播()
        {
            MulEventListener<CMsg> aModule = new AModule();
            aModule.StartListening(TAG);
            aModule.StartListening(TAG2);
            CMsg.Trigger(TAG, "name");
            CMsg.Trigger(TAG2, "name2");
        }
    }

    public class CMsg
    {
        public string Name;
        private static CMsg _e = new CMsg();

        public static void Trigger(string tag, string name)
        {
            _e.Name = name;
            EventManager.TriggerEvent(_e, tag);
        }
    }
}