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

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        ApplySessionOpacity();
        L2HazeLut.EnsureLoaded();
        EnsureRing();
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        DestroyRing();
    }

    void LateUpdate()
    {
        if (Instance != this)
        {
            return;
        }

        L2HazeLut.EnsureLoaded();
        EnsureRing();
        if (_tf == null || _mat == null)
        {
            return;
        }

        ApplyHazeLayer(_tf.gameObject);

        WorldClock clock = WorldClockPacketServices.Clock;
        if (clock == null)
        {
            clock = WorldClock.EnsurePersistent();
        }

        float hours = clock != null ? clock.WorldHours : 12f;
        Color tint = L2HazeLut.SampleAt(hours);
        float opacity = Mathf.Clamp01(_opacity);
        s_SessionOpacity = opacity;
        s_SessionOpacitySet = true;
        tint.a = opacity;
        ApplyHazeColor(tint);

        bool draw = opacity > 0.001f;
        if (_mr != null)
        {
            _mr.enabled = draw;
        }

        _tf.gameObject.SetActive(draw);

        Camera cam = ResolveCamera();
        _tf.position = _followCamera && cam != null ? cam.transform.position : Vector3.zero;
        _tf.rotation = Quaternion.identity;
        _tf.localScale = Vector3.one;
        _mat.SetFloat("_SkyDistance", ResolveSkyDistance(cam));
    }
}
