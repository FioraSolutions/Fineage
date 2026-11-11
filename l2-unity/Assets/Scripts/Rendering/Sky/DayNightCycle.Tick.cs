using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
    void OnEnable()
    {
        if (_useL2ColorLut)
        {
            L2DayNightLut.EnsureLoaded();
            L2EnvWorldLut.EnsureLoaded();
            L2ActorLightLut.EnsureLoaded();
        }

        if (_useL2Celestial)
        {
            L2CelestialLut.EnsureLoaded();
            EnsureCelestialDiscs();
        }

        if (_useL2HazeRing)
        {
            L2HazeLut.EnsureLoaded();
            EnsureHazeRing();
        }

        if (_useL2StarDome)
        {
            EnsureStarDome();
        }

        StripLegacyCombinedClouds();

        if (_useL2CloudSimple)
        {
            EnsureCloudSimple();
        }

        if (_useL2CloudMyst && L2CloudMyst.PortEnabled)
        {
            EnsureCloudMyst();
        }
        else
        {
            DisableCloudMyst();
        }
    }
}
