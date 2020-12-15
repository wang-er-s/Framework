using System;

namespace Framework.UI.Wrap.Base
{
    public interface IFieldChangeCb<T>
    {
        Action<T> GetFieldChangeCb();
    }
}