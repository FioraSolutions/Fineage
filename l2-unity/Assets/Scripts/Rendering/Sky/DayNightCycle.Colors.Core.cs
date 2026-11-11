using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
    private void ApplyL2ColorLut()
    {
        var sample = L2DayNightLut.SampleAt(_clock.WorldHours);

        if (_skyboxMaterial != null)
        {
            // Flat L2 display sky: G1 fills the dome. Clouds / HDR G3 diluted the
            // LUT into dirty turquoise in unity_soon.rdc (#83A4BE vs #62ADEA).
            Color displaySky = L2DayNightLut.ApplyPostSky(sample.sky, _clock.WorldHours);
            ConfigureFlatSkyDome(displaySky);
        }

        // L2FxSkyDebug.LogLut(_clock != null ? _clock.WorldHours : -1f, sample, _skyboxMaterial);

        _mainLight.color = sample.sun;

        // Actor LUT is for pawns. Floor/world use TimeEnv0 TerrainAmbient
        // (UL2NEnvManager::GetTerrainAmbientColor) as GI — before Yebis.
        L2EnvWorldLut.PushGlobals(_clock.WorldHours, sample.actorAmbient, sample.sun);
        L2ActorLightLut.PushGlobals(_clock.WorldHours);
        L2ActorPointLights.Push();

        HeightFogGlobal heightFog = RenderingPacketServices.HeightFog;
        if (heightFog != null)
        {
            heightFog.fogColorStart = sample.sky;
            heightFog.fogColorEnd = sample.sky * 0.74f;
            float fogDir = sample.sun.r + sample.sun.g + sample.sun.b > 0.05f
                ? _dayDirectionalIntensity
                : _nightDirectionalIntensity;
            heightFog.directionalIntensity = fogDir;
        }
    }

    private void ConfigureFlatSkyDome(Color skyColor)
    {
        if (_skyboxMaterial != null && _skyboxMaterial.HasProperty("_GradientColor1"))
            _skyboxMaterial.SetColor("_GradientColor1", skyColor);

        if (_skyboxMaterial != null && RenderSettings.skybox != _skyboxMaterial)
            RenderSettings.skybox = _skyboxMaterial;

        ApplyCameraSkyClear(skyColor);
    }

    private static void ApplyCameraSkyClear(Color skyColor)
    {
        // Edit-mode DayNightCycle must not rewrite login/lobby camera clears.
        if (!Application.isPlaying)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        // Original colour pass clears black. The sky pass then paints the upper
        // blue vault; the lower black is where haze/ground live.
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = L2FxCompositorRuntime.PreferGpuQueue ? Color.black : skyColor;
    }
}
