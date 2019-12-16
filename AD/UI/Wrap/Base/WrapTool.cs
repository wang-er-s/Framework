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

        private static readonly Dictionary<Type,Type> SupportWrapperTypes = new Dictionary<Type, Type>()
        {
            {typeof(Text) , typeof(TextWrapper)},
            {typeof(Toggle),typeof(ToggleWrapper)},
            {typeof(InputField),typeof(InputFieldWrapper)},
            {typeof(Slider),typeof(SliderWrapper)},
            {typeof(Button),typeof(ButtonWrapper)},
            {typeof(View), typeof(ViewWrapper)},
            {typeof(Image), typeof(ImageWrapper)},
            {typeof(Dropdown), typeof(DropdownWrapper)},
        };
        
        private static readonly object[] Args = new object[1];

        public static BaseWrapper<T> GetWrapper<T>(T component) where T : Component
        {
            foreach (var type in SupportWrapperTypes)
            {
                if (type.Key.IsInstanceOfType(component))
                {
                    Args[0] = component;
                    return (BaseWrapper<T>) Activator.CreateInstance(type.Value, Args);
                }
            }
            Debug.LogError ($"没有找到{component.GetType ().Name}的包装器，自行添加");
            throw new NullReferenceException();
        }

    }
}
