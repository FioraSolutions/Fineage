using System;
using UnityEngine;

public static partial class L2AppRand
{
    public static uint Advance(uint state, int drawCount)
    {
        if (drawCount <= 0)
            return state;

        uint mul = Multiplier;
        uint add = Increment;
        uint a = 1u;
        uint c = 0u;
        int n = drawCount;
        while (n > 0)
        {
            if ((n & 1) != 0)
            {
                c = unchecked(mul * c + add);
                a = unchecked(mul * a);
            }

            add = unchecked(mul * add + add);
            mul = unchecked(mul * mul);
            n >>= 1;
        }

        return unchecked(a * state + c);
    }

    public static float BitsToFloat(uint state)
    {
        // Bit-identical to Unity.Mathematics math.asfloat without asmdef package ref.
        return BitConverter.Int32BitsToSingle(unchecked((int)state));
    }
}
