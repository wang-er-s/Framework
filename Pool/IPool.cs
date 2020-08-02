namespace Framework.Pool
{
	public interface IPool<T>
	{
        T Spawn();

        bool DeSpawn(T obj);

	}
}
