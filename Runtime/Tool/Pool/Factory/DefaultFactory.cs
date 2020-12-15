namespace Framework.Pool.Factory
{
    public class DefaultFactory<T> : IFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }

        object IFactory.Create()
        {
            return Create();
        }
    }
}
