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
        public static IBindData<T> GetBindData<T>(Component component)
        {
            switch (component)
            {
                case Text text:
                    return (IBindData<T>) new TextWrapper(text);
                case Toggle toggle:
                    return (IBindData<T>) new ToggleWrapper(toggle);
                case InputField inputField:
                    return (IBindData<T>) new InputFieldWrapper(inputField);
                case Slider slider:
                    return (IBindData<T>) new SliderWrapper(slider);
            }
            Log.E($"没有找到{component.name}这种类型，请自行添加");
            return null;
        }

        public static IBindCommand<T> GetBindCommand<T>(Component component)
        {
            switch (component)
            {
                case Toggle toggle:
                    return (IBindCommand<T>)new ToggleWrapper(toggle);
                case InputField inputField:
                    return (IBindCommand<T>)new InputFieldWrapper(inputField);
                case Slider slider:
                    return (IBindCommand<T>)new SliderWrapper(slider);
            }
            Log.E($"没有找到{component.name}这种类型，请自行添加");
            return null;
        }

        public static IBindCommand GetBindCommand(Component component)
        {
            switch (component)
            {
                case Button button:
                    return (IBindCommand) new ButtonWrapper(button);
            }
            Log.E($"没有找到{component.name}这种类型，请自行添加");
            return null;
        }

    }
}
