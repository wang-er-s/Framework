using System;

namespace Framework
{
	public interface IPool<T> : IDisposable
	{
        T Allocate();

        void Free(T obj);
	}
	
	public interface IPoolWithKey<TKey,T> : IDisposable
	{
		T Allocate(TKey key);

		void Free(TKey key, T obj);
	}

	public interface IAsyncPool<T> : IDisposable
	{
		AsyncResult<T> Allocate();
		void Free(T obj);
	}
}
