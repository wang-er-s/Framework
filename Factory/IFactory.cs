namespace Framework
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
