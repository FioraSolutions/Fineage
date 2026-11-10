using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
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
