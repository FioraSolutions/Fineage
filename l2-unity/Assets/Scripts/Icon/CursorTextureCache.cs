using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity <see cref="Cursor.SetCursor"/> requires RGBA32, readable, no mip chain textures.
/// Cursor PNGs are imported as sprites; cache a single runtime copy per source asset.
/// </summary>
public static class CursorTextureCache
{
    static readonly Dictionary<int, Texture2D> Cache = new Dictionary<int, Texture2D>();

    public static Texture2D Prepare(Texture2D source)
    {
        if (source == null)
            return null;

        int key = source.GetInstanceID();
        if (Cache.TryGetValue(key, out Texture2D cached) && cached != null)
            return cached;

        if (source.isReadable && source.format == TextureFormat.RGBA32 && source.mipmapCount <= 1)
        {
            Cache[key] = source;
            return source;
        }

        int w = Mathf.Max(1, source.width);
        int h = Mathf.Max(1, source.height);

        try
        {
            Texture2D copy;
            if (source.isReadable)
            {
                Color32[] pixels = source.GetPixels32();
                copy = new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false, linear: false);
                copy.SetPixels32(pixels);
            }
            else
            {
                RenderTexture rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(source, rt);
                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = rt;
                copy = new Texture2D(w, h, TextureFormat.RGBA32, mipChain: false, linear: false);
                copy.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);
            }

            copy.filterMode = FilterMode.Point;
            copy.wrapMode = TextureWrapMode.Clamp;
            copy.Apply(false, makeNoLongerReadable: false);
            Cache[key] = copy;
            return copy;
        }
        catch (UnityException ex)
        {
            Debug.LogError("[Cursor] cannot prepare cursor texture " + source.name + ": " + ex.Message);
            return null;
        }
    }
}