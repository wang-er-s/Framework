using System.Collections.Specialized;
using Framework.UI.Wrap.Base;

namespace Framework.UI.Core.Bind
{
    // public class BindDic<TComponent,TKey, TValue> : BaseBind
    // {
    //     private readonly TComponent _component;
    //     private readonly ObservableDictionary<TKey, TValue> _dictionary;
    //     private IBindDic _bindDic;
    //
    //     public BindDic( TComponent view, ObservableDictionary<TKey, TValue> dictionary)
    //     {
    //         _component = view;
    //         _dictionary = dictionary;
    //         InitEvent();
    //         InitCpntValue();
    //     }
    //
    //     private void InitCpntValue()
    //     {
    //         _bindDic.GetBindDicFunc();
    //         foreach (var value in _dictionary)
    //         {
    //             _bindDic.GetBindDicFunc()( this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
    //         }
    //     }
    //
    //     private void InitEvent()
    //     {
    //         var bind = BindTool.GetDefaultWrapper(_component);
    //         _bindDic = _bindDic ?? _component as IBindDic ?? bind as IBindDic;
    //         Log.Assert(_bindDic != null);
    //         _dictionary.CollectionChanged += _bindDic.GetBindDicFunc();
    //     }
    //
    //     public override void ClearBind()
    //     {
    //         _dictionary.ClearListener();
    //     }
    // }
}