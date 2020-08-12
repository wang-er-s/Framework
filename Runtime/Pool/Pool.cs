using System.Collections.Generic;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount => CacheStack.Count;

        protected IFactory<T> Factory;

        protected readonly Stack<T> CacheStack = new Stack<T>();

        public abstract void Free(T obj);

        public virtual T Allocate()
        {
            return CacheStack.Count == 0 ?
                Factory.Create() : CacheStack.Pop();
        }
    }
}
