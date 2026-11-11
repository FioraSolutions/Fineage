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
}
