using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private string GetAttr(string key, string defaultValue)
    {
        return _attrs.TryGetValue(key.ToLowerInvariant(), out string value) ? value : defaultValue;
    }

    private bool TryFloat(string value, out float result)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    private bool TryInt(string value, out int result)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }

    private bool IsTruthy(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        value = value.Trim().ToLowerInvariant();

        return value == "true" ||
               value == "1" ||
               value == "yes" ||
               value == "checked" ||
               value == "selected" ||
               value == "disabled" ||
               value == "readonly" ||
               value == "nowrap";
    }

    private string Decode(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&");
    }

    private Texture2D LoadTexture(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        if (TextureCache.TryGetValue(path, out Texture2D cached))
            return cached;

        if (MissingTextureCache.Contains(path))
            return null;

        Texture2D texture = Resources.Load<Texture2D>(path);

        if (texture != null)
        {
            TextureCache[path] = texture;
            return texture;
        }

        MissingTextureCache.Add(path);
        // Debug.LogWarning("L2HtmlRenderer: texture not found in Resources: " + path);
        return null;
    }
}
