using System;
using Framework;
using NSubstitute;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    public class 消息系统
    {

        [SetUp]
        public void SetUp()
        {
            IsCall = false;
        }
        
        [Test]
        public void 基本广播()
        {
            //两种触发方式 --静态触发
            var bMsgEventListener = Substitute.For<EventListener<BMsg>>();
            bMsgEventListener.StartListening();
            BMsg.Trigger("HH");
            bMsgEventListener.Received().OnEvent(Arg.Any<BMsg>());
            
            //实例触发
            var aMsgEventListener = Substitute.For<EventListener<AMsg>>();
            aMsgEventListener.StartListening();
            var aMsg = new AMsg();
            EventManager.TriggerEvent(aMsg);
            aMsgEventListener.Received().OnEvent(aMsg);
        }

        private const string TAG = "TAG";
        private const string TAG2 = "TAG2";
        
        [Test]
        public void 复用广播()
        {
            MulEventListener<CMsg> aModule = Substitute.For<MulEventListener<CMsg>>();
            aModule.StartListening(TAG);
            aModule.StartListening(TAG2);
            CMsg.Trigger(TAG, "name");
            aModule.Received().OnEvent(Arg.Any<CMsg>(), TAG);
            CMsg.Trigger(TAG2, "name2");
            aModule.Received().OnEvent(Arg.Any<CMsg>(), TAG2);
        }

        [Test]
        public void 局部事件()
        {
            IEventBroadcaster eventBroadcaster = Substitute.For<IEventBroadcaster>();
            eventBroadcaster.LocalEventService.Returns(new LocalEventService());
            eventBroadcaster.Register<AMsg>(AMsgCB);
            eventBroadcaster.LocalEventService.TriggerEvent(new AMsg());
            Assert.IsTrue(IsCall);
        }

        private bool IsCall = false;
        private void AMsgCB(AMsg obj)
        {
            IsCall = true;
        }
    }



    public class AMsg
    {
        public string Name;
    }
        
    public class BMsg
    {
        public string Name;
        static readonly BMsg e = new BMsg();
        public static void Trigger(string newName)
        {
            e.Name = newName;
            EventManager.TriggerEvent(e);
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

