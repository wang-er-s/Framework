using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class CustomText : Text, IFieldChangeCb<bool>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>, IFieldChangeCb<long>, IFieldChangeCb<double>
    {
        [SerializeField]
        private string languageKey;

        public static event Func<string, string> GetLanguageStr;

        protected override void Awake()
        {
            base.Awake();
            if (!string.IsNullOrEmpty(languageKey))
            {
                if (Application.isPlaying)
                {
                    if (GetLanguageStr != null) text = GetLanguageStr(languageKey);
                }
                else
                {
                    text = languageKey;
                }
            }
        }
        
        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<int> IFieldChangeCb<int>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString(CultureInfo.InvariantCulture);
            };
        }

        Action<long> IFieldChangeCb<long>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString();
            };
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return b =>
            {
                text = b.ToString(CultureInfo.InvariantCulture);
            };
        }
    }
}