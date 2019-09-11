using SF.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.SF.UI.Core
{
    public class BindField<TComponent, TData>
    {
        private TComponent component;
        private Action<TData> ValueChangeEvent;

        public BindField(TComponent component, BindableProperty<TData> field)
        {
            this.component = component;
            field.AddChangeEvent((value) => ValueChangeEvent(value));
        }

        public void For(Action<TData> dataChanged)
        {
            ValueChangeEvent = dataChanged;
        }
    }
}
