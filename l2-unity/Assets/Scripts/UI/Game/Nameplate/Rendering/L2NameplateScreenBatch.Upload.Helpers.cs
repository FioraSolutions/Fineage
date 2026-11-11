using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public sealed partial class L2NameplateScreenBatch
{
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
