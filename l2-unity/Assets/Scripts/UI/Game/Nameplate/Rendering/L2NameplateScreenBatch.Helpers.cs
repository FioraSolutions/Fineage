using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2NameplateScreenBatch : IDisposable
{
    /// <summary>
    /// Clip-space NDC z for a world point, pulled slightly toward the camera so
    /// the label does not z-fight the owner's mesh.
    /// </summary>
    public static float ClipNdcZ(Camera cam, Vector3 world)
    {
        if (cam == null)
            return 0.5f;

        Vector3 camPos = cam.transform.position;
        Vector3 toCam = camPos - world;
        float dist = toCam.magnitude;
        if (dist > 0.01f)
            world += toCam * (WorldGrounding.NameplateDepthBiasMeters / dist);

        Vector4 clip = cam.projectionMatrix * cam.worldToCameraMatrix * new Vector4(world.x, world.y, world.z, 1f);
        if (Mathf.Abs(clip.w) < 1e-5f)
            return 0.5f;
        return clip.z / clip.w;
    }

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
