using System;

namespace Framework
{
    public interface IFieldChangeCb<T>
    {
        Action<T> GetFieldChangeCb();
    }
}