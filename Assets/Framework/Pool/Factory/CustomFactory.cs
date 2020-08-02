using System;

namespace Framework.Pool.Factory
{
    public class CustomFactory<T> : IFactory<T>
    {

        public CustomFactory(Func<T> func)
        {
            AllocMethod = func;
        }

        protected readonly Func<T> AllocMethod;

        public T Create()
        {
            return AllocMethod();
        }

        object IFactory.Create()
        {
            return Create();
        }
    }
}
