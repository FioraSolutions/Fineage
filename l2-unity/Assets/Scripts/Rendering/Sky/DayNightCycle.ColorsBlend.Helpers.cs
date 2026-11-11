using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle
{
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
