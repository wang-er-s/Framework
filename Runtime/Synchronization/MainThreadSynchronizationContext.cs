using System;
using System.Threading;

namespace Framework
{
    public class MainThreadSynchronizationContext : Singleton<MainThreadSynchronizationContext>, ISingletonUpdate
    {
        private readonly ThreadSynchronizationContext threadSynchronizationContext = new ThreadSynchronizationContext();

        public MainThreadSynchronizationContext()
        {
            SynchronizationContext.SetSynchronizationContext(this.threadSynchronizationContext);
        }

        public void Update(float deltaTime)
        {
            this.threadSynchronizationContext.Update();
        }

        public void Post(SendOrPostCallback callback, object state)
        {
            this.Post(() => callback(state));
        }

        public void Post(Action action)
        {
            this.threadSynchronizationContext.Post(action);
        }
    }
}