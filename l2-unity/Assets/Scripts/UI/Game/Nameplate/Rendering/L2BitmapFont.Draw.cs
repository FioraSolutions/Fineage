using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed partial class L2BitmapFont
{
    public float MeasureWidth(string text, float pixelScale)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0f;
        }

        float w = 0f;
        for (int i = 0; i < text.Length; i++)
        {
            if (TryGetGlyph(text[i], out Glyph g))
            {
                w += g.W * pixelScale;
            }
        }

        return w;
    }

    public float MeasureHeight(float pixelScale)
    {
        return _lineHeight * pixelScale;
    }

    public void AppendString(
        string text,
        float x,
        float y,
        float pixelScale,
        Color32 color,
        List<Vector3> verts,
        List<Vector2> uvs,
        List<Color32> colors,
        List<int> indices)
    {
        if (string.IsNullOrEmpty(text) || verts == null)
        {
            return;
        }

        float penX = x;
        float invW = _invW;
        float invH = _invH;

        for (int i = 0; i < text.Length; i++)
        {
            if (!TryGetGlyph(text[i], out Glyph g))
            {
                continue;
            }

            float gw = g.W * pixelScale;
            float gh = g.H * pixelScale;
            int vi = verts.Count;

            // Quad: TL, TR, BR, BL (Y down)
            verts.Add(new Vector3(penX, y, 0f));
            verts.Add(new Vector3(penX + gw, y, 0f));
            verts.Add(new Vector3(penX + gw, y + gh, 0f));
            verts.Add(new Vector3(penX, y + gh, 0f));

            float u0 = g.U * invW;
            float u1 = (g.U + g.W) * invW;
            // Atlas V=0 is top strip; Unity UV V=0 is bottom — flip.
            float vTop = 1f - (g.V * invH);
            float vBot = 1f - ((g.V + g.H) * invH);
            uvs.Add(new Vector2(u0, vTop));
            uvs.Add(new Vector2(u1, vTop));
            uvs.Add(new Vector2(u1, vBot));
            uvs.Add(new Vector2(u0, vBot));

            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);

            indices.Add(vi);
            indices.Add(vi + 1);
            indices.Add(vi + 2);
            indices.Add(vi);
            indices.Add(vi + 2);
            indices.Add(vi + 3);

            penX += gw;
        }
    }

    public void DrawStringGUI(string text, float x, float y, float pixelScale)
    {
        if (string.IsNullOrEmpty(text) || _atlas == null)
        {
            return;
        }

        float penX = x;
        float aw = AtlasWidth;
        float ah = AtlasHeight;

        for (int i = 0; i < text.Length; i++)
        {
            if (!TryGetGlyph(text[i], out Glyph g))
            {
                continue;
            }

            float gw = g.W * pixelScale;
            float gh = g.H * pixelScale;
            // Unity texcoords: V=0 at bottom; L2 atlas V=0 at top.
            Rect uv = new Rect(g.U / aw, 1f - (g.V + g.H) / ah, g.W / aw, g.H / ah);
            GUI.DrawTextureWithTexCoords(new Rect(penX, y, gw, gh), _atlas, uv);
            penX += gw;
        }
    }
}
