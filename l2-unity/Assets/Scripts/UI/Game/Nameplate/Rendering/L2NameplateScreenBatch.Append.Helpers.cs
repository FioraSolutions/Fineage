public sealed partial class L2NameplateScreenBatch
{
    public void AppendQuad(
        float x1,
        float y1,
        float x2,
        float y2,
        float depth,
        float screenH,
        Color color,
        bool snapPixels = true,
        float u0 = 0f,
        float v0 = 0f,
        float u1 = 1f,
        float v1 = 1f)
    {
        if (snapPixels)
        {
            x1 = Mathf.Floor(x1);
            y1 = Mathf.Floor(y1);
            x2 = Mathf.Floor(x2);
            y2 = Mathf.Floor(y2);
        }

        if (x2 <= x1 || y2 <= y1)
        {
            return;
        }

        uint packed = PackColor(color);
        int baseIndex = _glyphs.Count;

        // TL, TR, BR, BL in canvas top-down. Unity UV V=0 at texture bottom.
        // v1 = top of source rect, v0 = bottom (Unity space).
        AddQuadVert(x1, y1, u0, v1, depth, screenH, packed);
        AddQuadVert(x2, y1, u1, v1, depth, screenH, packed);
        AddQuadVert(x2, y2, u1, v0, depth, screenH, packed);
        AddQuadVert(x1, y2, u0, v0, depth, screenH, packed);

        _indices.Add(baseIndex);
        _indices.Add(baseIndex + 1);
        _indices.Add(baseIndex + 2);
        _indices.Add(baseIndex);
        _indices.Add(baseIndex + 2);
        _indices.Add(baseIndex + 3);
    }

    private void AddQuadVert(
        float canvasX,
        float canvasY,
        float u,
        float v,
        float depth,
        float screenH,
        uint packed)
    {
        _glyphs.Add(new GlyphVertex
        {
            ScreenPos = new Vector2(canvasX, screenH - canvasY),
            Depth = depth,
            Pad0 = 0f,
            UV = new Vector2(u, v),
            Color = packed,
            Pad1 = 0u
        });
    }
}
