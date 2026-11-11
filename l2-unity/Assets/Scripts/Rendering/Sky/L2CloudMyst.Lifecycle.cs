using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudMyst
{
    public static L2CloudMyst EnsureOn(GameObject host)
    {
        if (!PortEnabled)
            return null;
        if (Instance != null)
            return Instance;
        if (host == null)
            return null;
        L2CloudMyst c = host.GetComponent<L2CloudMyst>();
        return c != null ? c : host.AddComponent<L2CloudMyst>();
    }

    void OnEnable()
    {
        if (!PortEnabled)
        {
            enabled = false;
            return;
        }

        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        EnsureMesh();
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
        DestroyMesh();
    }

    void LateUpdate()
    {
        if (Instance != this)
            return;

        EnsureMesh();
        if (_tf == null || _mat == null)
            return;

        _tf.gameObject.layer = 0;
        float fade = CloudFade() * Mathf.Clamp01(_opacity);
        bool draw = fade > 0.01f;
        if (_mr != null)
            _mr.enabled = draw;
        _tf.gameObject.SetActive(draw);
        if (!draw)
            return;

        _mat.SetVector(CloudColorId, new Vector4(fade, fade, fade, 1f));

        Camera cam = ResolveCamera();
        if (_followCamera && cam != null)
            PlaceInFrontOfCamera(cam);
        else
        {
            _tf.position = Vector3.zero;
            _tf.rotation = Quaternion.identity;
        }

        _tf.localScale = Vector3.one;
        _mat.SetVector(SkyParamsId, new Vector4(ResolveSkyDistance(cam), 0f, 0f, 0f));
    }

    static float CloudFade()
    {
        L2CelestialLut.EnsureLoaded();
        if (!L2CelestialLut.IsReady)
            return 1f;
        WorldClock clock = WorldClockPacketServices.Clock ?? WorldClock.EnsurePersistent();
        float hours = clock != null ? clock.WorldHours : 12f;
        return Mathf.Lerp(0.55f, 1f, Mathf.Clamp01(L2CelestialLut.SampleAt(hours).sunScale / 4.5f));
    }
}
