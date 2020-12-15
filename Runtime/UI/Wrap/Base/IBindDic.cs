using System.Collections.Specialized;

namespace Framework.UI.Wrap.Base
{
    public interface IBindDic
    {
        NotifyCollectionChangedEventHandler GetBindDicFunc();
    }
}