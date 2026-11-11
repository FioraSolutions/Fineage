public sealed partial class L2NameplateScreenBatch
{
    public void AppendLine(
        L2BitmapFont font,
        string text,
        float x,
        float yTop,
        float scale,
        Color color,
        float depth,
        float screenH,
        bool discardSubpixelFrac)
    {
        if (font == null || string.IsNullOrEmpty(text))
        {
            return;
        }

        float ox = Mathf.Floor(x);
        float oy = Mathf.Floor(yTop);
        float fracX = x - ox;
        float fracY = yTop - oy;
        if (discardSubpixelFrac)
        {
            fracX = 0f;
            fracY = 0f;
        }

        int vertStart = _pixelVerts.Count;
        font.AppendString(text, ox, oy, scale, color, _pixelVerts, _uvs, _colors, _indices);

        uint packed = PackColor(color);
        for (int v = vertStart; v < _pixelVerts.Count; v++)
        {
            Vector3 p = _pixelVerts[v];
            float sx = p.x + fracX;
            float sy = (screenH - p.y) - fracY;
            _glyphs.Add(new GlyphVertex
            {
                ScreenPos = new Vector2(sx, sy),
                Depth = depth,
                Pad0 = 0f,
                UV = _uvs[v],
                Color = packed,
                Pad1 = 0u
            });
        }
    }
}
