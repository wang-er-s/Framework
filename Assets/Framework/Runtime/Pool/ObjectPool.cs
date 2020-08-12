using System;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public class ObjectPool<T> : Pool<T>
    {
        private readonly Action<T> _resetMethod;

        public ObjectPool(IFactory<T> factory, Action<T> resetMethod = null, int initCount = 0)
        {
            Factory = factory;
            _resetMethod = resetMethod;
            for (int i = 0; i < initCount; i++)
            {
                CacheStack.Push(Factory.Create());
            }
        }

        public override void Free(T obj)
        {
            _resetMethod.InvokeGracefully(obj);
            CacheStack.Push(obj);
        }
    }
}
