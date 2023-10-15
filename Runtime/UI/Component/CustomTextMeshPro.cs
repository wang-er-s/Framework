using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Framework
{
    public class CustomTextMeshPro : TextMeshProUGUI, IFieldChangeCb<bool>, IFieldChangeCb<string>, IFieldChangeCb<int>, IFieldChangeCb<float>, IFieldChangeCb<long>,
        IFieldChangeCb<double>
    {
        [SerializeField] private Color32 customOutLineColor;
        [SerializeField, Range(0f, 1f)] private float customOutLineWidth;
        [SerializeField] private string languageKey;

        [SerializeField] private bool shadow;
        [SerializeField] private Color shadowColor = new Color(0, 0, 0, 1f);
        [SerializeField, Range(-1f, 1f)] private float offsetX = 0;
        [SerializeField, Range(-1f, 1f)] private float offsetY = 0.5f;
        [SerializeField, Range(-1f, 1f)] private float dilate = 0.1f;
        [SerializeField, Range(0f, 1f)] private float softness = 0;
        [SerializeField, Range(-1f, 1f)] private float faceDilate = 0;
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");
        private static readonly int underlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
        private static readonly int underlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
        private static readonly int underlayDilate = Shader.PropertyToID("_UnderlayDilate");
        private static readonly int underlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
        private static readonly int FaceDilate = Shader.PropertyToID("_FaceDilate");

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
                CheckNewMat();
            Refresh();
        }

        public void Refresh()
        {
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_sharedMaterial = m_fontMaterial;
            }

            m_fontMaterial.SetFloat(FaceDilate, faceDilate);
            ShowShadow();
            ShowOutline();
        }

        private void ShowShadow()
        {
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
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
                m_fontMaterial.DisableKeyword("UNDERLAY_ON");
#endif
            }
        }

        private void ShowOutline()
        {
            if (customOutLineColor.a > 0 && customOutLineWidth > 0)
            {
                m_fontMaterial.EnableKeyword("OUTLINE_ON");
            }

            var lineColor = customOutLineColor;
            lineColor.r = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.r / 255f) * 255);
            lineColor.g = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.g / 255f) * 255);
            lineColor.b = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.b / 255f) * 255);
            lineColor.a = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.a / 255f) * 255);
            outlineColor = lineColor;
            outlineWidth = customOutLineWidth;
        }

        private static Dictionary<long, Material> hash2Mat = new Dictionary<long, Material>();

        private void CheckNewMat()
        {
            long hash = color.GetHashCode();
            hash += faceDilate.GetHashCode();
            if (shadow)
            {
                hash += shadowColor.GetHashCode() + offsetX.GetHashCode() + offsetY.GetHashCode() +
                        dilate.GetHashCode() +
                        softness.GetHashCode();
            }

            if (outlineColor.a > 0 && customOutLineWidth > 0)
            {
                hash += customOutLineColor.GetHashCode() + customOutLineWidth.GetHashCode();
            }

            if (!hash2Mat.TryGetValue(hash, out var mat))
            {
                mat = CreateMaterialInstance(m_sharedMaterial);
                hash2Mat[hash] = mat;
            }

            m_fontMaterial = mat;
            m_sharedMaterial = m_fontMaterial;
        }

        Action<bool> IFieldChangeCb<bool>.GetFieldChangeCb()
        {
            return b => { text = b.ToString(); };
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return b => { text = b; };
        }

        Action<int> IFieldChangeCb<int>.GetFieldChangeCb()
        {
            return b => { text = b.ToString(); };
        }

        Action<float> IFieldChangeCb<float>.GetFieldChangeCb()
        {
            return b => { text = b.ToString(CultureInfo.InvariantCulture); };
        }

        Action<long> IFieldChangeCb<long>.GetFieldChangeCb()
        {
            return b => { text = b.ToString(); };
        }

        Action<double> IFieldChangeCb<double>.GetFieldChangeCb()
        {
            return b => { text = b.ToString(CultureInfo.InvariantCulture); };
        }
    }
}