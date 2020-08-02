using System.Collections.Generic;
using Framework.Pool.Factory;

namespace Framework.Pool
{
    public abstract class Pool<T> : IPool<T>
    {
        public int CurCount => CacheStack.Count;

        protected IFactory<T> Factory;

        protected readonly Stack<T> CacheStack = new Stack<T>();

        protected int MaxCount = 12;

        public abstract bool DeSpawn(T obj);

        public virtual T Spawn()
        {
            return CacheStack.Count == 0 ?
                Factory.Create() : CacheStack.Pop();
        }
    }
}
