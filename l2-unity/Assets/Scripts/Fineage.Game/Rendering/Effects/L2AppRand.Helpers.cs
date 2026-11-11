using System;
using UnityEngine;

public static partial class L2AppRand
{
    public static void ResolveGpuInstanceRandBits(
        bool hasMeshSpawn,
        bool hasStartSpin,
        uint meshEmitter3AppRandBaseState,
        uint spriteEmitterAppRandBaseState,
        int slotIndex,
        out float meshSpawnRandBits,
        out float startSpinRandBits,
        out float spriteMotionRandBits,
        out float spriteSpinRandBits)
    {
        meshSpawnRandBits = 0f;
        startSpinRandBits = 0f;
        uint spriteMotion = Advance(spriteEmitterAppRandBaseState, slotIndex * SpriteMotionSlotStride);
        spriteMotionRandBits = BitsToFloat(spriteMotion);
        spriteSpinRandBits = BitsToFloat(Advance(spriteMotion, SpriteSpinDraws));

        if (hasMeshSpawn)
        {
            uint velocityState = Advance(
                meshEmitter3AppRandBaseState,
                slotIndex * MeshSpawnSlotToSlotDrawCount);
            meshSpawnRandBits = BitsToFloat(velocityState);
            startSpinRandBits = BitsToFloat(
                Advance(velocityState, MeshSpawnDrawsBeforeStartSpin));
            return;
        }

        if (hasStartSpin)
        {
            startSpinRandBits = BitsToFloat(
                Advance(
                    meshEmitter3AppRandBaseState,
                    slotIndex * MeshSpawnSlotToSlotDrawCount));
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void VerifyAdvanceMatchesLoop()
    {
        uint[] starts = { 1u, 12345u, 0x6FEC3FC2u };
        int[] counts = { 0, 1, 7, 22, 28, 31, 31 * 249, 250 * 31 + 22 };
        for (int s = 0; s < starts.Length; s++)
        {
            for (int c = 0; c < counts.Length; c++)
            {
                uint loop = starts[s];
                int n = counts[c];
                for (int i = 0; i < n; i++)
                    loop = unchecked(loop * Multiplier + Increment);
                uint fast = Advance(starts[s], n);
                if (loop != fast)
                {
                    // Debug.LogError(
                        // $"[L2AppRand] skip mismatch start={starts[s]} n={n} loop={loop} fast={fast}");
                    return;
                }
            }
        }
    }
}
