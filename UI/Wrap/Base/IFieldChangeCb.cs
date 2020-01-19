using System;

namespace Framework.UI.Wrap
{
    public interface IFieldChangeCb<T>
    {
        Action<T> GetFieldChangeCb();
    }
}
