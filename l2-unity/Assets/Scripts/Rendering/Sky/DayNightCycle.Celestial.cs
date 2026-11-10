using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
    void EnsureCelestialDiscs()
    {
        if (_celestialDiscs == null)
        {
            _celestialDiscs = GetComponent<L2CelestialDiscs>();
        }

        if (_celestialDiscs == null)
        {
            _celestialDiscs = gameObject.AddComponent<L2CelestialDiscs>();
        }

        if (!_celestialDiscs.enabled)
        {
            _celestialDiscs.enabled = true;
        }
    }

    void EnsureHazeRing()
    {
        _hazeRing = L2HazeRing.EnsureOn(gameObject);
    }

    void EnsureStarDome()
    {
        _starDome = L2StarDome.EnsureOn(gameObject);
    }

    void StripLegacyCombinedClouds()
    {
        L2CloudLayers.StripLegacyCloudChildren(transform);
        L2CloudLayers leftover = GetComponent<L2CloudLayers>();
        if (leftover != null)
        {
            if (Application.isPlaying)
                Destroy(leftover);
            else
                DestroyImmediate(leftover);
        }
    }

    void EnsureCloudSimple()
    {
        _cloudSimple = L2CloudSimple.EnsureOn(gameObject);
    }

    void EnsureCloudMyst()
    {
        _cloudMyst = L2CloudMyst.EnsureOn(gameObject);
    }

    void DisableCloudMyst()
    {
        L2CloudMyst myst = _cloudMyst != null ? _cloudMyst : GetComponent<L2CloudMyst>();
        if (myst == null)
            return;
        if (myst.enabled)
            myst.enabled = false;
        _cloudMyst = null;
    }

    bool ApplyL2CelestialLight()
    {
        if (_clock == null || _mainLight == null)
        {
            return false;
        }

        var celestial = L2CelestialLut.SampleAt(_clock.WorldHours);
        Vector3 dir = celestial.sunDirUnity;
        if (dir.sqrMagnitude < 1e-6f)
        {
            return false;
        }

        Vector3 forward = -dir.normalized;
        Vector3 up = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(forward, up)) > 0.99f)
        {
            up = Vector3.forward;
        }

        transform.rotation = Quaternion.LookRotation(forward, up);

        if (celestial.sunScale > 0.02f && celestial.sunElevDeg > 1f)
        {
            UpdateMainLightIntensity();
        }
        else
        {
            _mainLight.intensity = 0f;
            _mainLight.color = Color.black;
        }

        return true;
    }

    void HideSkyboxSunMoon()
    {
        if (_skyboxMaterial == null)
        {
            return;
        }

        if (_skyboxMaterial.HasProperty("_SunMoonSize"))
        {
            _skyboxMaterial.SetFloat("_SunMoonSize", 0f);
        }
    }
}
