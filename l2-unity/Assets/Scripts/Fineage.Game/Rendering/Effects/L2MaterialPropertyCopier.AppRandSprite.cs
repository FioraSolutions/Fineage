using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static void SetSpriteSpinRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(SpriteSpinRandStateBitsId))
        {
            mat.SetFloat(
                SpriteSpinRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }

    public static void SetSpriteMotionRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(SpriteMotionRandStateBitsId))
        {
            mat.SetFloat(
                SpriteMotionRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }

    public static bool ApplyHealingPotionSe0MotionReplay(Material mat, int slotIndex)
    {
        if (mat == null || slotIndex != 0 ||
            !mat.HasProperty(L2MotionReplayEnabledId) ||
            mat.GetFloat(L2MotionReplayEnabledId) <= 0.5f)
        {
            return false;
        }

        SetSpriteMotionRandState(mat, HealingPotionSe0ReplaySpawnState);
        SetSpriteSpinRandState(mat, AdvanceAppRandState(HealingPotionSe0ReplaySpawnState, 22));
        if (mat.HasProperty(SpawnDeltaTimeId))
        {
            mat.SetFloat(SpawnDeltaTimeId, HealingPotionSe0ReplaySpawnDeltaTime);
        }

        return true;
    }

    public static uint ReadSpriteSpinRandState(Material mat)
    {
        if (mat != null && mat.HasProperty(SpriteSpinRandStateBitsId))
        {
            float bits = mat.GetFloat(SpriteSpinRandStateBitsId);
            if (bits != 0f)
            {
                return unchecked((uint)BitConverter.SingleToInt32Bits(bits));
            }
        }

        return 0u;
    }

    public static void ResolveGpuInstanceRandBits(
        Material sharedMat,
        uint meshEmitter3AppRandBaseState,
        uint spriteEmitterAppRandBaseState,
        int slotIndex,
        out float meshSpawnRandBits,
        out float startSpinRandBits,
        out float spriteMotionRandBits,
        out float spriteSpinRandBits)
    {
        L2AppRand.ResolveGpuInstanceRandBits(
            IsMeshSpawnParticleMaterial(sharedMat),
            IsMeshStartSpinMaterial(sharedMat),
            meshEmitter3AppRandBaseState,
            spriteEmitterAppRandBaseState,
            slotIndex,
            out meshSpawnRandBits,
            out startSpinRandBits,
            out spriteMotionRandBits,
            out spriteSpinRandBits);
    }
}
