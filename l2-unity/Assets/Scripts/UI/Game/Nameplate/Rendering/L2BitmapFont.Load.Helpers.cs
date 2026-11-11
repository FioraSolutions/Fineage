using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public sealed partial class L2BitmapFont
{
    public static L2BitmapFont LoadFromResources(
        string atlasResourcePath = "Data/UI/Font/L2Lobby/l2_lobby_font_atlas",
        string csvResourcePath = "Data/UI/Font/L2Lobby/ul2font_ascii")
    {
        Texture2D atlas = Resources.Load<Texture2D>(atlasResourcePath);
        TextAsset csv = Resources.Load<TextAsset>(csvResourcePath);
        if (atlas == null)
        {
            // Debug.LogError($"[L2BitmapFont] Atlas not found at Resources/{atlasResourcePath}");
            return null;
        }

        if (csv == null)
        {
            // Debug.LogError($"[L2BitmapFont] CSV not found at Resources/{csvResourcePath}");
            return null;
        }

        return new L2BitmapFont(atlas, csv);
    }

    public bool TryGetGlyph(char ch, out Glyph glyph)
    {
        int code = ch;
        if (code < 0 || code >= _byCode.Length || !_byCode[code].Valid)
        {
            glyph = default;
            return false;
        }

        glyph = _byCode[code];
        return true;
    }
}
