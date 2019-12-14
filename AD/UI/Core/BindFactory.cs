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

        private List<IInitBind> binds;

        public BindFactory(TView _view, TVm _vm)
        {
            view = _view;
            vm = _vm;
            binds = new List<IInitBind>();
        }

        public BindField<TComponent, TData> Bind<TComponent, TData>
            (TComponent component, BindableProperty<TData> filed) where TComponent : Component
        {
            var bind = new BindField<TComponent, TData>(component, filed);
            binds.Add(bind);
            return bind;
        }

        public BindField<TComponent, TData> TwoWayBind<TComponent, TData>
            (TComponent component, BindableProperty<TData> filed) where TComponent : Component
        {
            var bind = new BindField<TComponent, TData>(component, filed, BindType.TwoWay);
            binds.Add(bind);
            return bind;
        }

        public BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>
        (TComponent component, BindableProperty<TData1> field1, BindableProperty<TData2> field2,
            Func<TData1, TData2, TResult> wrapFunc) where TComponent : Component
        {
            var bind = new BindField<TComponent, TData1, TData2, TResult>(component, field1, field2, wrapFunc);
            binds.Add(bind);
            return bind;
        }

        public void BindData<TData>(BindableProperty<TData> property, Action<TData> cb)
        {
            Core.BindData.Bind(property, cb);
        }

        public BindFunc<TComponent> BindCommand<TComponent>
            (TComponent component, Action command) where TComponent : Component
        {
            var bind = new BindFunc<TComponent>(component, command);
            binds.Add(bind);
            return bind;
        }

        public BindFuncWithPara<TComponent, TValue> BindCommand<TComponent, TValue>
            (TComponent component, Action<TValue> command) where TComponent : Component
        {
            var bind = new BindFuncWithPara<TComponent, TValue>(component, command);
            binds.Add(bind);
            return bind;
        }

        public BindList<TItemVm> BindList<TItemVm>
            (BindableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            var bind = new BindList<TItemVm>(list, views);
            binds.Add(bind);
            return bind;
        }

        public BindIpairsView<TItemVm> BindIpairs<TItemVm>
            (BindableList<TItemVm> list, string pattern) where TItemVm : ViewModel
        {
            var bind = new BindIpairsView<TItemVm>(ref list, pattern, view.transform);
            binds.Add(bind);
            return bind;
        }

        public void InitBind()
        {
            foreach (var initBind in binds)
            {
                initBind.InitBind();
            }
        }

    }
}