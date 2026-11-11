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
}
