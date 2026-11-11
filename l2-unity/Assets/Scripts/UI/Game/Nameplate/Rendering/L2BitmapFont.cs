using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// UL2Font-style ASCII atlas: glyph table from CSV + one page texture.
/// Glyph layout matches L2 dump: StartU, USize, StartV, VSize (pixels).
/// </summary>
public sealed partial class L2BitmapFont
{
    public struct Glyph
    {
        public int U;
        public int V;
        public int W;
        public int H;
        public bool Valid;
    }

    private readonly Texture2D _atlas;
    private readonly Glyph[] _byCode = new Glyph[128];
    private readonly float _invW;
    private readonly float _invH;
    private int _lineHeight = 14;

    public Texture2D Atlas => _atlas;
    public int LineHeight => _lineHeight;
    public float AtlasWidth => _atlas != null ? _atlas.width : 1f;
    public float AtlasHeight => _atlas != null ? _atlas.height : 1f;

}
