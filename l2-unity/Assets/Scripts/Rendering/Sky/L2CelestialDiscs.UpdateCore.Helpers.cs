using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Globalization;
using UnityEngine.Rendering;

public partial class L2CelestialDiscs
{
    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
        DestroyDisc(ref _sunTf, ref _sunMat);
        DestroyDisc(ref _moonTf, ref _moonMat);
        _sunMr = null;
        _moonMr = null;
        _sunTrack = default;
        _moonTrack = default;
    }

    void LateUpdate()
    {
        L2CelestialLut.EnsureLoaded();
        L2DayNightLut.EnsureLoaded();
        if (!L2CelestialLut.IsReady)
        {
            return;
        }

        WorldClock clock = WorldClockPacketServices.Clock;
        if (clock == null)
        {
            clock = WorldClock.EnsurePersistent();
        }
        if (clock == null)
        {
            return;
        }

        EnsureDiscs();
        if (_sunTf == null || _moonTf == null)
        {
            return;
        }

        LateUpdateApplyLayer();

        var sample = L2CelestialLut.SampleAt(clock.WorldHours);
        Color sunTint = Color.white;
        if (L2DayNightLut.IsReady)
        {
            sunTint = L2DayNightLut.SampleAt(clock.WorldHours).sun;
        }

        Camera cam = ResolveCamera();
        Vector3 camPos = cam != null ? cam.transform.position : Vector3.zero;

        bool periodic = Application.isPlaying && _logTrajectory && Time.unscaledTime >= _nextTrajLog;
        ApplyBody("SUN", ref _sunTrack, _sunTf, _sunMr, _sunMat, cam, camPos, clock.WorldHours, sample,
            sample.sunOffsetUnity, sample.sunDirUnity, sample.sunElevDeg,
            sample.sunDrawScale, sample.sunScale, SunRefSideUu, SunRefDrawScale, SunRefDistanceUu, sunTint, false, periodic);
        ApplyBody("MOON", ref _moonTrack, _moonTf, _moonMr, _moonMat, cam, camPos, clock.WorldHours, sample,
            sample.moonOffsetUnity, sample.moonDirUnity, sample.moonElevDeg,
            sample.moonDrawScale, sample.moonScale, MoonRefSideUu, MoonRefDrawScale, MoonRefDistanceUu,
            Color.white, true, periodic);
        if (periodic)
        {
            _nextTrajLog = Time.unscaledTime + Mathf.Max(0.05f, _logInterval);
        }
    }
}
