using System;

namespace AD.UI.Wrap
{
    public interface IFieldChangeCb<T>
    {
        Action<T> GetFieldChangeCb();
    }
}
