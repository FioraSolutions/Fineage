using System;
using UnityEngine;

public partial class NumberUtils
{
    public static float ScaleAnimSpeedL2jToUnity(float value)
    {
        return value / 278f;
    }

    public static float ScaleAnimWalkSpeedL2jToUnity(float value)
    {
        return value / 20f;
    }

    public static float FloorToNearest(float value, float step) {
        return step * Mathf.Floor(value / step);
    }

    public static float FromIntToFLoat(int value) {
        byte[] byteArray = BitConverter.GetBytes(value);
        float floatValue = BitConverter.ToSingle(byteArray, 0);
        return floatValue;
    }
}