using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static uint ComputeMeshEmitter3StartSpinState(uint sharedBaseState, int slotIndex)
    {
        return AdvanceAppRandState(sharedBaseState, slotIndex * MeshEmitter3SlotToSlotDrawCount);
    }

    public static uint ComputeMeshSpawnStartSpinState(uint sharedBaseState, int slotIndex)
    {
        uint velocityState = AdvanceAppRandState(
            sharedBaseState,
            slotIndex * MeshSpawnSlotToSlotDrawCount);
        return AdvanceAppRandState(velocityState, MeshSpawnDrawsBeforeStartSpin);
    }

    public static uint ReadStartSpinRandState(Material mat)
    {
        if (mat == null)
        {
            return 0u;
        }

        if (mat.HasProperty(StartSpinRandStateBitsId))
        {
            float bits = mat.GetFloat(StartSpinRandStateBitsId);
            if (bits != 0f)
            {
                return unchecked((uint)BitConverter.SingleToInt32Bits(bits));
            }
        }

        return 0u;
    }

    public static void SetStartSpinRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(StartSpinRandStateBitsId))
        {
            mat.SetFloat(
                StartSpinRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }
}
