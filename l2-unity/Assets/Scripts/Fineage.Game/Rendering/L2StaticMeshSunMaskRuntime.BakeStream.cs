using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2StaticMeshSunMaskRuntime
{
    void Initialize(MeshFilter filter, byte[] visibility)
    {
        var packed = new float[visibility.Length];
        for (int i = 0; i < visibility.Length; i++)
            packed[i] = visibility[i] / 255f;
        Mesh stream = CreateStream(filter.sharedMesh, packed);
        Attach(filter, stream, true);
    }

    static Mesh CreateStream(Mesh source, float[] packed)
    {
        var stream = new Mesh
        {
            name = source.name + "_L2SunMaskStream",
            bounds = source.bounds
        };
        stream.SetVertexBufferParams(
            packed.Length,
            new VertexAttributeDescriptor(
                VertexAttribute.TexCoord3,
                VertexAttributeFormat.Float32,
                1));
        stream.SetVertexBufferData(
            packed,
            0,
            0,
            packed.Length,
            0,
            MeshUpdateFlags.DontRecalculateBounds);
        return stream;
    }

    void Attach(MeshFilter filter, Mesh stream, bool ownsStream)
    {
        _streamMesh = stream;
        _ownsStream = ownsStream;
        _renderer = filter.GetComponent<MeshRenderer>();
        if (_renderer != null)
        {
            _renderer.additionalVertexStreams = _streamMesh;
            var properties = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(properties);
            properties.SetFloat(HasMaskId, 1f);
            _renderer.SetPropertyBlock(properties);
        }
    }

    void OnDestroy()
    {
        if (_renderer != null && _renderer.additionalVertexStreams == _streamMesh)
            _renderer.additionalVertexStreams = null;
        if (_streamMesh == null || !_ownsStream)
            return;
        if (Application.isPlaying)
            Destroy(_streamMesh);
        else
            DestroyImmediate(_streamMesh);
    }
}
