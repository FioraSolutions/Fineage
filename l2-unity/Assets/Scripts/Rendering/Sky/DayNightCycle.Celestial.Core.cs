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
}
