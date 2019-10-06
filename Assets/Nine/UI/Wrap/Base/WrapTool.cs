using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nine;
using Nine.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Assets.Nine.UI.Wrap
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
                case View<ViewModel> view:
                    return (BaseWrapper<T>)getWrapper(nameof(View<ViewModel>), component);
            }
            throw new NullReferenceException($"没有找到{component.GetType().Name}的包装器，自行添加");
        }

        private static Object getWrapper(string componentName,Component component)
        {
            Type type = Type.GetType($"Assets.Nine.UI.Wrap.{componentName}Wrapper");
            if (type == null)
            {
                throw new NullReferenceException($"没有找到{componentName}的包装器，自行添加");
            }
            return Activator.CreateInstance(type, args:component);
        }

    }
}
