using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD;
using AD.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace AD.UI.Wrap
{
    public static class WrapTool
    {

        public static BaseWrapper<T> GetWrapper<T>(T component) where T : Component
        {
            switch (component)
            {
                case Text text:
                    return (BaseWrapper<T>) getWrapper(nameof(Text), component);
                case Toggle toggle:
                    return (BaseWrapper<T>)getWrapper(nameof(Toggle), component);
                case InputField inputField:
                    return (BaseWrapper<T>)getWrapper(nameof(InputField), component);
                case Slider slider:
                    return (BaseWrapper<T>)getWrapper(nameof(Slider), component);
                case Image img:
                    return (BaseWrapper<T>)getWrapper(nameof(Image), component);
                case Button btn:
                    return (BaseWrapper<T>)getWrapper(nameof(Button), component);
                case View view:
                    return (BaseWrapper<T>)getWrapper(nameof(View), component);
            }
            Log.Error ($"没有找到{component.GetType ().Name}的包装器，自行添加");
            throw new NullReferenceException();
        }

        private static Object getWrapper(string componentName,Component component)
        {
            string typePath = $"{typeof(WrapTool).Namespace}.{componentName}Wrapper";
            Type type = Type.GetType(typePath);
            if (type == null)
            {
                Log.Error ($"没有找到{typePath}类，自行添加");
                throw new NullReferenceException();
            }
            return Activator.CreateInstance(type, args:component);
        }

    }
}
