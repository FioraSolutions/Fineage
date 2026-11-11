using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
    void Update()
    {
        try
        {
            if (_clock == null)
            {
                _clock = WorldClock.EnsurePersistent();
            }
            if (_clock == null)
            {
                _clock = GetComponent<WorldClock>();
            }
            if (_clock == null)
            {
                _clock = WorldClockPacketServices.Clock;
            }
            if (_mainLight == null)
            {
                _mainLight = GetComponent<Light>();
            }
            if (_clock == null || _mainLight == null)
            {
                return;
            }

            float hours = _clock.WorldHours;
            if (_skyReady && Mathf.Abs(hours - _appliedHours) < 0.0001f)
            {
                L2ActorPointLights.Push();
                return;
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

            bool usedL2LightDir = false;
            if (_useL2Celestial && L2CelestialLut.IsReady)
            {
                usedL2LightDir = ApplyL2CelestialLight();
            }

            if (!usedL2LightDir)
            {
                float mainLightLerpValue = _clock.Clock.dayRatio > 0 ? _clock.Clock.dayRatio : _clock.Clock.nightRatio;
                float sunRotation = Mathf.Lerp(0 - _horizonOffsetDegree, 180 + _horizonOffsetDegree, mainLightLerpValue);
                transform.eulerAngles = new Vector3(sunRotation, _mainLightRotY, 0);

                if (transform.eulerAngles.x < -1 || transform.eulerAngles.x > 181)
                {
                    _mainLight.intensity = 0;
                }
                else
                {
                    UpdateMainLightIntensity();
                }
            }

            ShareMainLightRotation();

            UpdateAmbientLightIntensity();

            if (_useL2ColorLut)
            {
                L2DayNightLut.EnsureLoaded();
                L2EnvWorldLut.EnsureLoaded();
                L2ActorLightLut.EnsureLoaded();
            }

            if (_useL2ColorLut && L2DayNightLut.IsReady)
            {
                ApplyL2ColorLut();
            }
            else
            {
                UpdateSkyColor();
                UpdateFogColor();
                UpdateAmbientLightColor();
                UpdateLightColor();
            }

            UpdateCloudsOpacity();
            if (_useL2Celestial)
            {
                HideSkyboxSunMoon();
            }
            else if (_skyboxMaterial != null)
            {
                UpdateMainLightTexture();
            }

            _appliedHours = _clock.WorldHours;
            _skyReady = true;
        }
        catch (Exception)
        {
            // Debug.LogException(e);
        }

    }
}
