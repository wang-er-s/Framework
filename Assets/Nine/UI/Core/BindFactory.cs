using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nine.UI.Core;
using UnityEngine;

namespace Assets.Nine.UI.Core
{
    public class BindFactory<TView,TVm> 
        where TView: View
        where TVm : ViewModel
    {

        private TView view;
        private TVm vm;

        public BindFactory(TView _view, TVm _vm)
        {
            view = _view;
            vm = _vm;
        }

        public BindField<TComponent, TData> Bind<TComponent, TData>(TComponent component, Expression<Func<TVm, TData>> filed) where TComponent : Component
        {
            return new BindField<TComponent, TData>(component, GetBindPropertyByExpression(filed));
        }

        public BindField<TComponent, TData1, TData2, TResult> Bind<TComponent, TData1, TData2, TResult>(TComponent component, Expression<Func<TVm, TData1>> field1, Expression<Func<TVm, TData2>> field2, Func<TData1, TData2, TResult> wrapFunc) where TComponent : Component
        {
            return new BindField<TComponent, TData1, TData2, TResult>(component, GetBindPropertyByExpression(field1),
                GetBindPropertyByExpression(field2), wrapFunc);
        }

        public BindFunc<TComponent> BindCommand<TComponent>(TComponent component, Func<TVm, Action> command) where TComponent : Component
        {
            Action dataChanged = command?.Invoke(vm);
            return new BindFunc<TComponent>(component, dataChanged);
        }

        public BindFuncWithPara<TComponent, TValue> BindCommand<TComponent, TValue>(TComponent component, Func<TVm, Action<TValue>> command) where TComponent : Component
        {
            Action<TValue> dataChanged = command?.Invoke(vm);
            return new BindFuncWithPara<TComponent, TValue>(component, dataChanged);
        }

        public BindList<TVm> BindList<TVm>(View view, BindableList<TVm> list) where TVm : ViewModel
        {
            return new BindList<TVm>(view, list);
        }

        private BindableProperty<TData> GetBindPropertyByExpression<TData>(Expression<Func<TVm, TData>> expression)
        {
            return vm.GetBindingAbleProperty<TData>((expression.Body as MemberExpression)?.Member.Name);
        }
    }
}
