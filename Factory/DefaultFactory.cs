/*
* Create by Soso
* Time : 2018-12-27-10 下午
*/

namespace Framework
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
