using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static void CopyMeshSpawnAppRandFromBaseState(
        Material runtimeMat,
        Material sharedMat,
        uint baseState,
        int slotIndex)
    {
        if (!IsMeshSpawnParticleMaterial(sharedMat) || runtimeMat == null)
        {
            return;
        }

        CopyVectorIfPresent(runtimeMat, sharedMat, StartLocationZRangeUUId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityZRangeUUId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSizeZRangeId);
        CopyFloatIfPresent(runtimeMat, sharedMat, StartSizeXYId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartLocationOffsetUeId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartLocationRangeXUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartLocationRangeYUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartLocationRangeZUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeXUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeYUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeZUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSizeRangeXUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSizeRangeYUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSizeRangeZUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinYawRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinPitchRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinRollRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpsYawRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpsPitchRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpsRollRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpinCCWorCWId);
        CopyVectorIfPresent(runtimeMat, sharedMat, ColorMultiplierId);
        CopyVectorIfPresent(runtimeMat, sharedMat, LifetimeRangeId);
        CopyVectorIfPresent(runtimeMat, sharedMat, InitialDelayRangeId);

        uint velocityState = AdvanceAppRandState(baseState, slotIndex * MeshSpawnSlotToSlotDrawCount);
        SetMeshSpawnRandState(runtimeMat, velocityState);
        SetStartSpinRandState(
            runtimeMat,
            AdvanceAppRandState(velocityState, MeshSpawnDrawsBeforeStartSpin));
    }

    public static void SetMeshSpawnRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(MeshSpawnRandStateBitsId))
        {
            mat.SetFloat(
                MeshSpawnRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }

    public static uint ReadMeshSpawnRandState(Material mat)
    {
        if (mat != null && mat.HasProperty(MeshSpawnRandStateBitsId))
        {
            float bits = mat.GetFloat(MeshSpawnRandStateBitsId);
            if (bits != 0f)
            {
                return unchecked((uint)BitConverter.SingleToInt32Bits(bits));
            }
        }

        return 0u;
    }

    public static void CopyMeshAppRandStartSpinFromBaseState(
        Material runtimeMat,
        Material sharedMat,
        uint baseState,
        int slotIndex)
    {
        if (runtimeMat == null || sharedMat == null)
        {
            return;
        }

        bool hasMeshSpawn = IsMeshSpawnParticleMaterial(sharedMat);
        bool hasStartSpin = IsMeshStartSpinMaterial(sharedMat);
        if (!hasMeshSpawn && !hasStartSpin)
            return;

        if (hasMeshSpawn)
        {
            // Mesh spawn base is before StartVelocity; MeshEmitter3 base is before StartSpin.
            CopyMeshSpawnAppRandFromBaseState(runtimeMat, sharedMat, baseState, slotIndex);
            return;
        }

        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinYawRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinPitchRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartSpinRollRangeUcId);

        SetStartSpinRandState(
            runtimeMat,
            ComputeMeshEmitter3StartSpinState(baseState, slotIndex));
    }

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
