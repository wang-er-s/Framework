using Framework.Asynchronous;

namespace Framework.Pool
{
	public interface IPool<T>
	{
        T Allocate();

        void Free(T obj);

	}

	public interface IAsyncPool<T>
	{
		AsyncResult<T> Allocate();
		void Free(T obj);
	}
}
