using System;
using Framework.Asynchronous;

namespace Framework.Pool.Factory
{
    public class CustomAsyncFactory<T> : IAsyncFactory<T>
    {
        private Func<AsyncResult<T>> allocMethod;
        
        public CustomAsyncFactory(Func<AsyncResult<T>> allocMethod)
        {
            this.allocMethod = allocMethod;
        }
        
        public AsyncResult<T> Create()
        {
            return allocMethod();
        }

        object IFactory.Create()
        {
            return Create();
        }
    }
}