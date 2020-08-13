using System;
using System.Collections.Generic;
using Framework.UI.Core;
using UnityEngine.UI;

namespace Framework.UI.Wrap.Base
{
    public static class BindTool
    {
        private static readonly Dictionary<Type, Type> supportWrapperTypes = new Dictionary<Type, Type>()
        {
            {typeof(Text), typeof(TextWrapper)},
            {typeof(Toggle), typeof(ToggleWrapper)},
            {typeof(InputField), typeof(InputFieldWrapper)},
            {typeof(Slider), typeof(SliderWrapper)},
            {typeof(Button), typeof(ButtonWrapper)},
            {typeof(View), typeof(ViewWrapper)},
            {typeof(Image), typeof(ImageWrapper)},
            {typeof(Dropdown), typeof(DropdownWrapper)}
        };

        private static readonly object[] args = new object[1];

        public static object GetDefaultWrapper<T>(T component)
        {
            foreach (var type in supportWrapperTypes)
                if (type.Key.IsInstanceOfType(component))
                {
                    args[0] = component;
                    return Activator.CreateInstance(type.Value, args);
                }
            return component;
        }
    }
}