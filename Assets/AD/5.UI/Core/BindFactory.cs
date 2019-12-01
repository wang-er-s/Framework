using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Core;
using UnityEngine;

namespace AD.UI.Core
{
    public class BindFactory<TView, TVm>
        where TView : View
        where TVm : ViewModel
    {
        private TView view;
        private TVm vm;

        public BindFactory (TView _view, TVm _vm)
        {
            view = _view;
            vm = _vm;
        }

        public BindField<TComponent, TData> Bind<TComponent, TData>
            (TComponent component, BindableProperty<TData> filed) where TComponent : Component
        {
            return new BindField<TComponent, TData>(component, filed);
        }

        public BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, BindableProperty<TData1>  field1, BindableProperty<TData2> field2,
         Func<TData1, TData2, TResult> wrapFunc) where TComponent : Component
        {
            return new BindField<TComponent, TData1, TData2, TResult>(component, field1, field2, wrapFunc);
        }

        public BindFunc<TComponent> BindCommand<TComponent>
            (TComponent component, Action command) where TComponent : Component
        {
            return new BindFunc<TComponent> (component, command);
        }

        public BindFuncWithPara<TComponent, TValue> BindCommand<TComponent, TValue>
            (TComponent component,  Action<TValue> command) where TComponent : Component
        {
            return new BindFuncWithPara<TComponent, TValue> (component, command);
        }

        public BindList<TItemVm> BindList<TItemVm>
            (BindableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            return new BindList<TItemVm> (list, views);
        }

        public BindIpairsView<TItemVm> BindIpairs<TItemVm>
            (BindableList<TItemVm> list, string pattern) where TItemVm : ViewModel
        {
            return new BindIpairsView<TItemVm> (ref list, pattern, view.transform);
        }
        
    }
}