using System.Collections.Generic;
using Framework;
using NSubstitute;
using NUnit.Framework;

namespace Tests
{
    public class 消息系统
    {

        private const string IntTag = "IntTag";
        private const string IntTag2 = "IntTag2";
        private const string TriggerTag = "TAG2";
        private const string ObjTag = "TAG3";

        public interface Receive
        {
            [Subscriber(IntTag, IntTag2)]
            void ReceiveInt(int val);

            [Subscriber(TriggerTag)]
            void ReceiveTrigger();

            [Subscriber(ObjTag)]
            void ReceiveObj(Receive obj);
        }

        [Test]
        public void 注册和广播()
        {
            var receive = Substitute.For<Receive>();
            Message.defaultEvent.Register(receive);
            Message.defaultEvent.Post(IntTag, 1);
            receive.Received().ReceiveInt(1);
            Message.defaultEvent.Post(IntTag2, 2);
            receive.Received().ReceiveInt(2);
            Message.defaultEvent.Post(TriggerTag);
            receive.Received().ReceiveTrigger();
            Message.defaultEvent.Post(ObjTag, receive);
            receive.Received().ReceiveObj(receive);
        }
    }
}