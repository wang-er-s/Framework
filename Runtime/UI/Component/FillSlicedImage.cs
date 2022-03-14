using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class FillSlicedImage : Image
{
    static readonly Vector2[] s_VertScratch = new Vector2[4];
    static readonly Vector2[] s_UVScratch = new Vector2[4];

    private void GenerateSlicedSprite(VertexHelper toFill)
    {
        toFill.Clear();
        Vector4 outer, inner, padding, border;
        if (sprite != null)
        {
            outer = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            inner = UnityEngine.Sprites.DataUtility.GetInnerUV(sprite);
            padding = UnityEngine.Sprites.DataUtility.GetPadding(sprite);
            border = sprite.border;
        }
        else
        {
            outer = Vector4.zero;
            inner = Vector4.zero;
            padding = Vector4.zero;
            border = Vector4.zero;
        }
        Rect rect = GetPixelAdjustedRect();
        Rect fillRect = rect;
        fillRect.width = fillRect.width * fillAmount;
        Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, rect);
        padding = padding / pixelsPerUnit;
        
        s_VertScratch[0] = new Vector2(padding.x, padding.y);
        s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

        s_VertScratch[1].x = adjustedBorders.x;
        s_VertScratch[1].y = adjustedBorders.y;

        s_VertScratch[2].x = rect.width - adjustedBorders.z;
        s_VertScratch[2].y = rect.height - adjustedBorders.w;

        for (int i = 0; i < 4; ++i)
        {
            s_VertScratch[i].x += rect.x;
            s_VertScratch[i].y += rect.y;
        }

        s_UVScratch[0] = new Vector2(outer.x, outer.y);
        s_UVScratch[1] = new Vector2(inner.x, inner.y);
        s_UVScratch[2] = new Vector2(inner.z, inner.w);
        s_UVScratch[3] = new Vector2(outer.z, outer.w);
        for (int x = 0; x < 3; ++x)
        {
            int x2 = x + 1;

            for (int y = 0; y < 3; ++y)
            {
                if (!fillCenter && x == 1 && y == 1)
                    continue;

                int y2 = y + 1;

                if (fillOrigin == 0)
                {
                    float w = s_VertScratch[x2].x - s_VertScratch[x].x;
                    float uvw = s_UVScratch[x2].x - s_UVScratch[x].x;
                    float realAmount = (fillRect.x + fillRect.width - s_VertScratch[x].x) / w;
                    if (realAmount < 0) {
                        realAmount = 0;
                    }
                    if (realAmount > 1) {
                        realAmount = 1;
                    }
                    s_VertScratch[x2].x = s_VertScratch[x].x + w * realAmount;
                    s_UVScratch[x2].x = s_UVScratch[x].x + uvw * realAmount;
                }
                AddQuad(toFill,
                    new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                    new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                    color,
                    new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                    new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
            }
        }
    }

    private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
    {
        for (int axis = 0; axis <= 1; axis++)
        {
            // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
            // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
            float combinedBorders = border[axis] + border[axis + 2];
            if (rect.size[axis] < combinedBorders && combinedBorders != 0)
            {
                float borderScaleRatio = rect.size[axis] / combinedBorders;
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }
        }
        return border;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        GenerateSlicedSprite(toFill);
        if (overrideSprite == null)
        {
            base.OnPopulateMesh(toFill);
            return;
        }
    }

    static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
    {
        int startIndex = vertexHelper.currentVertCount;

        vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

        vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }

}