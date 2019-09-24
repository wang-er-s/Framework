using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public static class WrapTool
    {
        public static IBindData<T> GetWrapper<T>(Component component)
        {
            if (component is Text text)
            {
                return (IBindData<T>)new TextWrapper(text);
            }
            Log.E($"没有找到{component.name}这种类型，请自行添加");
            return null;
        }
    }
}
