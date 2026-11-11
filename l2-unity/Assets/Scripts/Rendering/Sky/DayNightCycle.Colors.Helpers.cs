using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
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
}
