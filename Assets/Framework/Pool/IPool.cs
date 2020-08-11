namespace Framework.Pool
{
	public interface IPool<T>
	{
        T Allocate();

        void Free(T obj);

	}
}
