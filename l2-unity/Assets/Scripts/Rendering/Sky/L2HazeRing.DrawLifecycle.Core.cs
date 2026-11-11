using System;
using UnityEngine;

public partial class L2HazeRing
{
    public static L2HazeRing EnsureOn(GameObject host)
    {
        if (Instance != null)
        {
            return Instance;
        }

        L2HazeRing ring = host != null ? host.GetComponent<L2HazeRing>() : null;
        if (ring == null && host != null)
        {
            ring = host.AddComponent<L2HazeRing>();
        }

        return ring;
    }

    void ApplyHazeColor(Color tint)
    {
        CurrentTint = tint;
        if (_mat == null)
        {
            return;
        }

        // LUT is already display bytes / 255. SetColor would sRGB→linear and muddy the morning haze.
        _mat.SetVector(HazeColorId, new Vector4(tint.r, tint.g, tint.b, tint.a));
    }

    void ApplySessionOpacity()
    {
        if (s_SessionOpacitySet)
        {
            _opacity = Mathf.Clamp01(s_SessionOpacity);
            return;
        }

        s_SessionOpacity = Mathf.Clamp01(_opacity);
        s_SessionOpacitySet = true;
    }
}
