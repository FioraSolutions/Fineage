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
}
