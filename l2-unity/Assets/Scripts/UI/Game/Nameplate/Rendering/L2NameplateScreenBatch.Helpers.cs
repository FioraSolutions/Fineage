using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2NameplateScreenBatch : IDisposable
{
    public void BeginFrame()
    {
        _pixelVerts.Clear();
        _uvs.Clear();
        _colors.Clear();
        _indices.Clear();
        _glyphs.Clear();
        _drawIndexCount = 0;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ReleaseBuffers();

        if (_material != null)
        {
            UnityEngine.Object.Destroy(_material);
            _material = null;
        }
    }
}
