using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Framework
{
    public class CustomTextMeshPro : TextMeshProUGUI, IFieldChangeCb<bool>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>, IFieldChangeCb<long>, IFieldChangeCb<double>
    {
        [SerializeField]
        private Color32 customOutLineColor;
        [SerializeField, Range(0f, 1f)]
        private float customOutLineWidth;
        [SerializeField]
        private string languageKey;
        [SerializeField]
        private bool shadow;
        [SerializeField] private Color shadowColor = new Color(0, 0, 0, 1f);
        [SerializeField, Range(-1f, 1f)]
        private float offsetX = 0;
        [SerializeField, Range(-1f, 1f)]
        private float offsetY = 0.5f;
        [SerializeField, Range(-1f, 1f)]
        private float dilate = 0.1f;
        [SerializeField, Range(0f, 1f)]
        private float softness = 0;
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");
        private static readonly int underlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
        private static readonly int underlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
        private static readonly int underlayDilate = Shader.PropertyToID("_UnderlayDilate");
        private static readonly int underlaySoftness = Shader.PropertyToID("_UnderlaySoftness");

        public static event Func<string, string> GetLanguageStr;

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                Message.defaultEvent.Register(this, "Language", () =>
                {
                    if (GetLanguageStr != null && !string.IsNullOrEmpty(languageKey))
                        text = GetLanguageStr(languageKey);
                });
                if (GetLanguageStr != null && !string.IsNullOrEmpty(languageKey)) text = GetLanguageStr(languageKey);
            }
            else
            {
                if (!string.IsNullOrEmpty(languageKey))
                    text = languageKey;
            }

            ShowOutline();
            ShowShadow();
        }
        
        public void ShowShadow()
        {
            if (m_fontMaterial == null)
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
            if (shadow)
            {
                m_fontMaterial.EnableKeyword("UNDERLAY_ON");
                m_fontMaterial.SetColor(underlayColor, shadowColor);
                m_fontMaterial.SetFloat(underlayOffsetX, offsetX);
                m_fontMaterial.SetFloat(underlayOffsetY, -offsetY);
                m_fontMaterial.SetFloat(underlayDilate, dilate);
                m_fontMaterial.SetFloat(underlaySoftness, softness);
            }
            else
            {
                m_fontMaterial.DisableKeyword("UNDERLAY_ON");
            }
        }

        public void ShowOutline()
        {
            if (m_fontMaterial == null)
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
            if (customOutLineColor.a != 0 && customOutLineWidth != 0)
            {
                m_fontMaterial.EnableKeyword("OUTLINE_ON");
                SetOutlineColor(customOutLineColor);
                SetOutlineThickness(customOutLineWidth);
            }
            else
            {
                m_fontMaterial.DisableKeyword("OUTLINE_ON");
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.defaultEvent.Unregister(this);
        }
    }
}