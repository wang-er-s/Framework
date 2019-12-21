using System;
using Microsoft.VisualBasic;

namespace AD.UI.Core
{
    public static class BindData
    {
        public static void Bind<TData>(IBindableField<TData> property, Action<TData> cb)
        {
            property.AddListener(cb);
        }
        
    }
}