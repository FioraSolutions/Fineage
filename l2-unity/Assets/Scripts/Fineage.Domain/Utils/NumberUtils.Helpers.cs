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

    public static float NpcLocomotionAnimRate()
    {
        return L2NpcMovePlayRate * L2SequenceFps / UnityClipFps;
    }

    public static float NpcLocomotionAnimRate(float sourceRate)
    {
        if (sourceRate <= 0f)
            sourceRate = L2SequenceFps;
        return L2NpcMovePlayRate * sourceRate / UnityClipFps;
    }

    public static float FloorToNearest(float value, float step) {
        return step * Mathf.Floor(value / step);
    }

    public static float FromIntToFLoat(int value) {
        // Convert integer to byte array
        byte[] byteArray = BitConverter.GetBytes(value);

        // Convert byte array to float
        float floatValue = BitConverter.ToSingle(byteArray, 0);

        return floatValue;
    }
}
