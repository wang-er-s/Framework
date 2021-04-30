using Framework.Asynchronous;

namespace Framework.Pool.Factory
{
    public interface IFactory<T> : IFactory
    {
        new T Create();
    }

    public interface IAsyncFactory<T> : IFactory
    {
        new AsyncResult<T> Create();
    }

    public interface IFactory
    {
        object Create();
    }
}
