namespace Framework.Pool.Factory
{
    public interface IFactory<T> : IFactory
    {
        new T Create();
    }

    public interface IFactory
    {
        object Create();
    }
}
