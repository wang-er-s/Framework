using System.Collections.Specialized;

namespace Framework
{
    public interface IBindDic
    {
        NotifyCollectionChangedEventHandler GetBindDicFunc();
    }
}