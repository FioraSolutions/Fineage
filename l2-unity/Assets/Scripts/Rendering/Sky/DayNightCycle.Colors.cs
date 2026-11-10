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

    private void UpdateSkyColor()
    {
        Color skyColor = _skyboxMaterial.GetColor("_GradientColor1");
        if (_clock.Clock.dawnRatio > 0 && _clock.Clock.dawnRatio < 1)
        {
            skyColor = Color.Lerp(_nightColor, _dawnColor, _clock.Clock.dawnRatio);
        }
        if (_clock.Clock.brightRatio > 0)
        {
            if (_clock.Clock.brightRatio < 0.2f)
            {
                skyColor = Color.Lerp(_dawnColor, _dayColor, _clock.Clock.brightRatio / 0.2f);
            }
            else if (_clock.Clock.brightRatio < 1f)
            {
                skyColor = _dayColor;
            }
        }
        if (_clock.Clock.darkRatio > 0)
        {
            if (_clock.Clock.darkRatio < 0.05f)
            {
                skyColor = Color.Lerp(_duskColor, _nightColor, _clock.Clock.darkRatio / 0.05f);
            }
            else if (_clock.Clock.darkRatio < 1f)
            {
                skyColor = _nightColor;
            }
        }
        if (_clock.Clock.duskRatio > 0 && _clock.Clock.duskRatio < 1)
        {
            skyColor = Color.Lerp(_dayColor, _duskColor, _clock.Clock.duskRatio);
        }

        ConfigureFlatSkyDome(skyColor);
    }

    private void UpdateLightColor()
    {
        Color lightColor = _mainLight.color;
        if (_clock.Clock.dawnRatio > 0 && _clock.Clock.dawnRatio < 1)
        {
            lightColor = Color.Lerp(_mainLightNightColor, _mainLightDawnColor, _clock.Clock.dawnRatio);
        }
        if (_clock.Clock.brightRatio > 0)
        {
            if (_clock.Clock.brightRatio < 0.2f)
            {
                lightColor = Color.Lerp(_mainLightDawnColor, _mainLightDayColor, _clock.Clock.brightRatio / 0.2f);
            }
            else if (_clock.Clock.brightRatio < 1f)
            {
                lightColor = _mainLightDayColor;
            }
        }
        if (_clock.Clock.darkRatio > 0)
        {
            if (_clock.Clock.darkRatio < 0.05f)
            {
                lightColor = Color.Lerp(_mainLightduskColor, _mainLightNightColor, _clock.Clock.darkRatio / 0.05f);
            }
            else if (_clock.Clock.darkRatio < 1f)
            {
                lightColor = _mainLightNightColor;
            }
        }
        if (_clock.Clock.duskRatio > 0 && _clock.Clock.duskRatio < 1)
        {
            lightColor = Color.Lerp(_mainLightDayColor, _mainLightduskColor, _clock.Clock.duskRatio);
        }
        _mainLight.color = lightColor;
    }

    private void UpdateFogColor()
    {
        HeightFogGlobal heightFog = RenderingPacketServices.HeightFog;
        if (heightFog == null)
        {
            return;
        }

        Color fogColorStart = heightFog.fogColorStart;
        Color fogColorEnd = heightFog.fogColorEnd;
        float directionalIntensity = heightFog.directionalIntensity;
        if (_clock.Clock.dawnRatio > 0 && _clock.Clock.dawnRatio < 1)
        {
            fogColorStart = Color.Lerp(fogColorStart, _dayFogColorStart, _clock.Clock.dawnRatio);
            fogColorEnd = Color.Lerp(fogColorEnd, _dayFogColorEnd, _clock.Clock.dawnRatio);
            directionalIntensity = Mathf.Lerp(directionalIntensity, _dayDirectionalIntensity, _clock.Clock.dawnRatio);
        }
        if (_clock.Clock.brightRatio > 0 && _clock.Clock.brightRatio < 1)
        {
            fogColorStart = _dayFogColorStart;
            fogColorEnd = _dayFogColorEnd;
            directionalIntensity = _dayDirectionalIntensity;
        }
        if (_clock.Clock.duskRatio > 0 && _clock.Clock.duskRatio < 1)
        {
            fogColorStart = Color.Lerp(_dayFogColorStart, _nightFogColorStart, _clock.Clock.duskRatio);
            fogColorEnd = Color.Lerp(_dayFogColorEnd, _nightFogColorEnd, _clock.Clock.duskRatio);
            directionalIntensity = Mathf.Lerp(directionalIntensity, _nightDirectionalIntensity, _clock.Clock.duskRatio);
        }
        if (_clock.Clock.darkRatio > 0 && _clock.Clock.darkRatio < 1)
        {
            fogColorStart = _nightFogColorStart;
            fogColorEnd = _nightFogColorEnd;
            directionalIntensity = _nightDirectionalIntensity;
        }

        heightFog.fogColorStart = fogColorStart;
        heightFog.fogColorEnd = fogColorEnd;
        heightFog.directionalIntensity = directionalIntensity;
    }

    private void UpdateAmbientLightIntensity()
    {
        // Ambient light intensity
        RenderSettings.ambientIntensity = AdjustIntensity(_ambientMinIntensity, _ambientMaxIntensity, _clock.Clock.dawnRatio, _clock.Clock.duskRatio); ;
    }

    private void UpdateAmbientLightColor()
    {
        // Ambient light intensity
        Color lightColor = RenderSettings.ambientSkyColor;

        if (_clock.Clock.dawnRatio > 0 && _clock.Clock.dawnRatio < 1)
        {
            lightColor = Color.Lerp(_ambientLightNightColor, _ambientLightDawnColor, _clock.Clock.dawnRatio);
        }
        if (_clock.Clock.brightRatio > 0)
        {
            if (_clock.Clock.brightRatio < 0.2f)
            {
                lightColor = Color.Lerp(_ambientLightDawnColor, _ambientLightDayColor, _clock.Clock.brightRatio / 0.2f);
            }
            else if (_clock.Clock.brightRatio < 1f)
            {
                lightColor = _ambientLightDayColor;
            }
        }
        if (_clock.Clock.darkRatio > 0)
        {
            if (_clock.Clock.darkRatio < 0.05f)
            {
                lightColor = Color.Lerp(_ambientLightduskColor, _ambientLightNightColor, _clock.Clock.darkRatio / 0.05f);
            }
            else if (_clock.Clock.darkRatio < 1f)
            {
                lightColor = _ambientLightNightColor;
            }
        }
        if (_clock.Clock.duskRatio > 0 && _clock.Clock.duskRatio < 1)
        {
            lightColor = Color.Lerp(_ambientLightDayColor, _ambientLightduskColor, _clock.Clock.duskRatio);
        }

        RenderSettings.ambientSkyColor = lightColor;
    }

    private void UpdateMainLightIntensity()
    {
        // Main light intensity
        _mainLight.intensity = AdjustIntensity(_mainLightMinIntensity, _mainLightMaxIntensity, _clock.Clock.dawnRatio, _clock.Clock.duskRatio);
    }

    private float AdjustIntensity(float minIntensity, float fullIntensity, float dawnRatio, float duskRatio)
    {
        if (duskRatio > 0)
        {
            return Mathf.Lerp(fullIntensity, minIntensity, duskRatio);
        }
        else
        {
            return Mathf.Lerp(minIntensity, fullIntensity, dawnRatio);
        }
    }

    private void UpdateCloudsOpacity()
    {
        if (_skyboxMaterial == null)
        {
            return;
        }

        // MIT skybox cloud planes stay off. L2 sheets are L2CloudSimple / L2CloudMyst.
        _skyboxMaterial.SetFloat("_Clouds_Opacity", 0f);
        _skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", 0f);
    }

    private void UpdateMainLightTexture()
    {
        // Update texture 
        if (_clock.Clock.dayRatio > 0)
        {
            _skyboxMaterial.SetTexture("_SunMoon", _sunTexture);
            _skyboxMaterial.SetTextureScale("_SunMoon", _sunTiling);
        }
        else
        {
            _skyboxMaterial.SetTexture("_SunMoon", _moonTexture);
            _skyboxMaterial.SetTextureScale("_SunMoon", _moonTiling);
        }
    }

    private void ShareMainLightRotation()
    {
        if (_skyboxMaterial == null)
        {
            return;
        }

        if (_skyboxMaterial.HasProperty("_MainLightForward"))
            _skyboxMaterial.SetVector("_MainLightForward", transform.forward);
        if (_skyboxMaterial.HasProperty("_MainLightUp"))
            _skyboxMaterial.SetVector("_MainLightUp", transform.up);
        if (_skyboxMaterial.HasProperty("_MainLightRight"))
            _skyboxMaterial.SetVector("_MainLightRight", transform.right);
    }

}
