using System;

namespace AD
{
    /// <summary>
    /// 父类EventDispatcher
    /// </summary>
    public class ParentEventDispatcher
    {
        private readonly EventDispatcher eventDispatcher;
        
        public ParentEventDispatcher()
        {
            eventDispatcher = new EventDispatcher();
        }
        
        public void Register<T>(Action<T> listener)
        {
            eventDispatcher.Register(listener);
        }

        public void Register(string tag, Action listener)
        {
            eventDispatcher.Register(tag, listener);
        }

        public void UnRegister<T>(Action<T> listener)
        {
            eventDispatcher.UnRegister(listener);
        }

        public void UnRegister(string tag, Action listener)
        {
            eventDispatcher.UnRegister(tag, listener);
        }

        public void SendMsg<T>(T msg)
        {
            eventDispatcher.SendMessage(msg);
        }

        public void SendMsg(string tag)
        {
            eventDispatcher.SendMessage(tag);
        }
        
    }
}