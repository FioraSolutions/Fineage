using System;
using UnityEngine;

public partial class NumberUtils
{
    public static float ScaleToUnity(float value) {
        return (float) value * defaultMetrUnit;
    }

    public static float ScaleToUnityPlus(float value)
    {
        //back up 35
        return value * (1f / 52.5f) + 0.25f;
    }

    public static float ScaleAnimSpeedToUnity(float value)
    {
        return value / 68f - 0.25f;
    }

    public static float ScaleAnimSpeedFigtherToUnity(float value)
    {
        return value / 68f;
    }

    public static float ScaleAnimSpeedMagicToUnity(float serverValue)
    {
   
        float calculatedMultiplier = serverValue * 0.0033f;
        return Mathf.Clamp(calculatedMultiplier, 0.25f, 0.8f);
    }
}
