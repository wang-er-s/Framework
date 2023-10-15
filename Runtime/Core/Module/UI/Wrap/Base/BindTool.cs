using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

namespace Framework
{
    public static class BindTool
    {
        private static readonly Dictionary<Type, Type> supportWrapperTypes = new Dictionary<Type, Type>
        {
            {typeof(Text), typeof(TextWrapper)},
            {typeof(Toggle), typeof(ToggleWrapper)},
            {typeof(InputField), typeof(InputFieldWrapper)},
            {typeof(Slider), typeof(SliderWrapper)},
            {typeof(Button), typeof(ButtonWrapper)},
            {typeof(Image), typeof(ImageWrapper)},
            {typeof(Dropdown), typeof(DropdownWrapper)},
            {typeof(TextMeshProUGUI), typeof(TmpWrapper)},
            {typeof(TMP_InputField), typeof(TMPInputFieldWrapper)},
        };

        private static readonly object[] args = new object[2];
        private static ConstructorInfo defaultWrapperCon;

        public static IWrapper GetDefaultWrapper<T>(object container, T component)
        {
            args[1] = container;
            args[0] = component;
            Type wrapperType = null;
            foreach (var type in supportWrapperTypes)
            {
                if (type.Key.IsInstanceOfType(component))
                {
                    wrapperType = type.Value;
                }
            }

            if (wrapperType == null)
                wrapperType = typeof(DefaultWrapper);
            IWrapper wrapper = ReferencePool.Allocate(wrapperType) as IWrapper;
            wrapper.Init(component, container);
            return wrapper;
        }
    }
}