using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Shared L2 canvas glyph batch: screen pixels → StructuredBuffer → one RenderPrimitivesIndexed.
/// Used by <see cref="NameplatesManager"/> and <see cref="LobbyNameplatesManager"/>.
/// </summary>
public sealed partial class L2NameplateScreenBatch : IDisposable
{
    private const string ShaderResourcePath = "Data/Shaders/UI/L2BitmapFontScreenSpace";
    private const string ShaderName = "L2/UI/BitmapFontScreenSpace";
    private const int GlyphStride = 32;

    private readonly List<Vector3> _pixelVerts = new List<Vector3>(256);
    private readonly List<Vector2> _uvs = new List<Vector2>(256);
    private readonly List<Color32> _colors = new List<Color32>(256);
    private readonly List<int> _indices = new List<int>(384);
    private readonly List<GlyphVertex> _glyphs = new List<GlyphVertex>(256);
    private readonly Bounds _drawBounds = new Bounds(Vector3.zero, Vector3.one * 100000f);

    private Material _material;
    private GraphicsBuffer _glyphBuffer;
    private GraphicsBuffer _indexBuffer;
    private GlyphVertex[] _glyphScratch;
    private int[] _indexScratch;
    private int _glyphCapacity;
    private int _indexCapacity;
    private int _drawIndexCount;
    private bool _disposed;

    [StructLayout(LayoutKind.Sequential)]
    private struct GlyphVertex
    {
        public Vector2 ScreenPos;
        public float Depth;
        public float Pad0;
        public Vector2 UV;
        public uint Color;
        public uint Pad1;
    }

    public bool IsReady => _material != null;

    public void EnsureMaterial(Texture atlas)
    {
        if (_material != null)
        {
            if (atlas != null)
            {
                _material.mainTexture = atlas;
                _material.SetTexture("_MainTex", atlas);
            }

            return;
        }

        Shader shader = Resources.Load<Shader>(ShaderResourcePath);
        if (shader == null)
        {
            shader = Shader.Find(ShaderName);
        }

        if (shader == null)
        {
            // Debug.LogError($"[L2NameplateScreenBatch] Shader '{ShaderName}' not found.");
            return;
        }

        _material = new Material(shader)
        {
            name = "L2ScreenBitmapFont (Shared)",
            hideFlags = HideFlags.HideAndDontSave,
            renderQueue = 5000
        };
        if (atlas != null)
        {
            _material.mainTexture = atlas;
            _material.SetTexture("_MainTex", atlas);
        }
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

    /// <summary>
    /// L2 canvas line: Floor pixel origin + optional frac discard, AppendString → screen verts (Y up).
    /// </summary>

    /// <summary>
    /// L2 DrawTile / DrawTargetTex quad. <paramref name="x1"/>..<paramref name="y2"/> are
    /// canvas top-down pixels (same space as <see cref="AppendLine"/> yTop).
    /// UV defaults to full texture; pass atlas pixel rect for cropped HeadDisplay sprites.
    /// </summary>


    public bool HasGeometry => _glyphs.Count > 0 && _indices.Count > 0;

    /// <summary>Upload buffers and submit one indexed procedural draw for <paramref name="cam"/>.</summary>

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
