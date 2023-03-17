using System;
using TMPro;
using UnityEngine;

namespace Framework
{
    public class CustomTextMesh3D : TextMeshPro
    {
        [SerializeField] private Color32 customOutLineColor;
        [SerializeField, Range(0f, 1f)] private float customOutLineWidth;
        [SerializeField] private bool shadow;
        [SerializeField] private Color shadowColor = new Color(0, 0, 0, 1f);
        [SerializeField, Range(-1f, 1f)] private float offsetX = 0;
        [SerializeField, Range(-1f, 1f)] private float offsetY = 0.5f;
        [SerializeField, Range(-1f, 1f)] private float dilate = 0.1f;
        [SerializeField, Range(0f, 1f)] private float softness = 0;
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");
        private static readonly int underlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
        private static readonly int underlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
        private static readonly int underlayDilate = Shader.PropertyToID("_UnderlayDilate");
        private static readonly int underlaySoftness = Shader.PropertyToID("_UnderlaySoftness");

        protected override void Awake()
        {
            base.Awake();
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
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
            }

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
    }
}