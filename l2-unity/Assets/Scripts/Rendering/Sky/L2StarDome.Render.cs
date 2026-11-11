using System;
using UnityEngine;

public partial class L2StarDome : MonoBehaviour
{
    void LateUpdate()
    {
        if (Instance != this)
        {
            return;
        }

        EnsureDome();
        if (_tf == null || _smallMat == null || _largeMat == null)
        {
            return;
        }

        ApplySkyLayer(_tf.gameObject);

        float fade = NightFade();
        bool draw = fade > 0.01f;
        if (_mr != null)
        {
            _mr.enabled = draw;
        }

        _tf.gameObject.SetActive(draw);
        if (!draw)
        {
            return;
        }

        ApplyTint(_smallMat, SmallNightTint, fade);
        ApplyTint(_largeMat, LargeNightTint, fade);

        Camera cam = ResolveCamera();
        _tf.position = _followCamera && cam != null ? cam.transform.position : Vector3.zero;
        _tf.rotation = Quaternion.identity;
        _tf.localScale = Vector3.one;
        float skyZ = ResolveSkyDistance(cam);
        _smallMat.SetFloat(SkyDistanceId, skyZ);
        _largeMat.SetFloat(SkyDistanceId, skyZ);
    }

    static float NightFade()
    {
        L2CelestialLut.EnsureLoaded();
        if (!L2CelestialLut.IsReady)
        {
            WorldClock clock = WorldClockPacketServices.Clock;
            if (clock == null)
            {
                return 1f;
            }

            return clock.Clock.darkRatio > 0.01f ? 1f : 0f;
        }

        WorldClock hoursClock = WorldClockPacketServices.Clock;
        if (hoursClock == null)
        {
            hoursClock = WorldClock.EnsurePersistent();
        }

        float hours = hoursClock != null ? hoursClock.WorldHours : 0f;
        var sample = L2CelestialLut.SampleAt(hours);
        // Same night window as the moon disc. sunScale is 0 at night in the LUT.
        if (sample.sunScale > 0.02f)
        {
            return 0f;
        }

        return Mathf.Clamp01(sample.moonScale / 4.5f);
    }

    static void ApplyTint(Material mat, Color nightTint, float fade)
    {
        if (mat == null)
        {
            return;
        }

        mat.SetVector(StarColorId, new Vector4(
            nightTint.r * fade,
            nightTint.g * fade,
            nightTint.b * fade,
            nightTint.a));
    }

    void EnsureDome()
    {
        if (_smallStars == null)
        {
            _smallStars = Resources.Load<Texture2D>(SmallTexPath);
        }

        if (_largeStars == null)
        {
            _largeStars = Resources.Load<Texture2D>(LargeTexPath);
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _smallStars == null || _largeStars == null)
        {
            return;
        }

        ConfigureTex(_smallStars);
        ConfigureTex(_largeStars);

        if (_mesh == null || _mesh.name != L2StarDomeMesh.MeshName)
        {
            if (_mesh != null)
            {
                if (Application.isPlaying)
                    Destroy(_mesh);
                else
                    DestroyImmediate(_mesh);
            }

            _mesh = L2StarDomeMesh.Build();
            _mesh.hideFlags = HideFlags.DontSave;
            if (_mf != null)
            {
                _mf.sharedMesh = _mesh;
            }
        }

        if (_tf != null)
        {
            return;
        }

        var go = new GameObject(MeshObjectName);
        go.hideFlags = HideFlags.HideAndDontSave;
        _tf = go.transform;
        _tf.SetParent(transform, false);
        ApplySkyLayer(go);

        _mf = go.AddComponent<MeshFilter>();
        _mf.sharedMesh = _mesh;

        _mr = go.AddComponent<MeshRenderer>();
        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
        _mr.allowOcclusionWhenDynamic = false;
        _mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        _mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        _smallMat = MakeMat(shader, _smallStars, SmallNightTint, 2990);
        _largeMat = MakeMat(shader, _largeStars, LargeNightTint, 2991);
        _mr.sharedMaterials = new[] { _smallMat, _largeMat };
    }
}
