using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.SF.UI.Core
{
    public class BindFunc<TComponent>
    {

        private TComponent component;

        public BindFunc(TComponent component, Action dataChanged)
        {
            this.component = component;
            //this.func = func;
        }

    }
}
