using System;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public class SimpleObjectPool<T> : Pool<T>
    {
        private readonly Action<T> _resetMethod;

        public SimpleObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null, int initCount = 0, int? maxCount = null)
        {
            Factory = new CustomFactory<T>(factoryMethod);
            _resetMethod = resetMethod;
            MaxCount = maxCount ?? MaxCount;
            for (int i = 0; i < initCount; i++)
            {
                CacheStack.Push(Factory.Create());
            }
        }

        public override bool DeSpawn(T obj)
        {
            _resetMethod.InvokeGracefully(obj);
            CacheStack.Push(obj);
            return true;
        }
    }
}
