using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static float UIntBitsToFloat(uint state)
    {
        return BitConverter.Int32BitsToSingle(unchecked((int)state));
    }

    public static uint CreateFiniteAppRandState()
    {
        uint high = (uint)UnityEngine.Random.Range(0, 32768);
        uint low = (uint)UnityEngine.Random.Range(0, 65536);
        uint state = ((high << 16) | low) & 0x7F7FFFFFu;
        return state == 0u ? 1u : state;
    }

    public static uint AdvanceAppRandState(uint state, int drawCount)
    {
        return L2AppRand.Advance(state, drawCount);
    }
}
