

public sealed partial class L2NameplateScreenBatch
{
    public bool UploadAndDraw(Camera cam)
    {
        if (_disposed || _material == null || cam == null || !HasGeometry)
        {
            return false;
        }

        EnsureBuffers(_glyphs.Count, _indices.Count);

        for (int i = 0; i < _glyphs.Count; i++)
        {
            _glyphScratch[i] = _glyphs[i];
        }

        for (int i = 0; i < _indices.Count; i++)
        {
            _indexScratch[i] = _indices[i];
        }

        _glyphBuffer.SetData(_glyphScratch, 0, 0, _glyphs.Count);
        _indexBuffer.SetData(_indexScratch, 0, 0, _indices.Count);
        _drawIndexCount = _indices.Count;

        _material.SetBuffer("_GlyphBuffer", _glyphBuffer);
        if (L2FxCompositorRuntime.PreferGpuQueue)
        {
            L2NameplateOverlayQueue.Enqueue(
                _material,
                _glyphBuffer,
                _indexBuffer,
                _drawIndexCount,
                cam);
            return true;
        }

        var rp = new RenderParams(_material)
        {
            worldBounds = _drawBounds,
            camera = cam,
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = false,
            layer = 0
        };
        Graphics.RenderPrimitivesIndexed(
            rp,
            MeshTopology.Triangles,
            _indexBuffer,
            _drawIndexCount,
            0,
            1);
        return true;
    }

    private void EnsureBuffers(int glyphCount, int indexCount)
    {
        if (glyphCount <= 0 || indexCount <= 0)
        {
            return;
        }

        if (_glyphBuffer == null || _glyphCapacity < glyphCount)
        {
            _glyphBuffer?.Release();
            _glyphCapacity = Mathf.NextPowerOfTwo(Mathf.Max(256, glyphCount));
            _glyphBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                _glyphCapacity,
                GlyphStride);
            _glyphScratch = new GlyphVertex[_glyphCapacity];
        }

        if (_indexBuffer == null || _indexCapacity < indexCount)
        {
            _indexBuffer?.Release();
            _indexCapacity = Mathf.NextPowerOfTwo(Mathf.Max(384, indexCount));
            _indexBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Index,
                _indexCapacity,
                sizeof(int));
            _indexScratch = new int[_indexCapacity];
        }
    }

    private void ReleaseBuffers()
    {
        if (_glyphBuffer != null)
        {
            _glyphBuffer.Release();
            _glyphBuffer = null;
        }

        if (_indexBuffer != null)
        {
            _indexBuffer.Release();
            _indexBuffer = null;
        }

        _glyphCapacity = 0;
        _indexCapacity = 0;
        _glyphScratch = null;
        _indexScratch = null;
    }

    private static uint PackColor(Color32 c)
    {
        return (uint)(c.r | (c.g << 8) | (c.b << 16) | (c.a << 24));
    }

}
