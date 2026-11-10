using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    /// </summary>
    /// LIVE: Wave + Spirit (same 31-draw slot stride).
    /// immediately before slot 0 StartVelocityRange GetRand.
    /// MeshEmitter SpawnParticle TLS stream. <paramref name="baseState"/> is
    /// <summary>
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

    /// </summary>
    /// CPU mirror of L2Fx_MeshSpawnParticle_SampleLocVelSizeZ for snapshot logs.
    /// <summary>
    public static void SampleMeshSpawnLocVelSizeZ(
        Material mat,
        uint stateBeforeVelocity,
        out float locationZ,
        out float velocityZ,
        out float sizeZ)
    {
        uint state = stateBeforeVelocity;
        Vector4 velRange = mat != null && mat.HasProperty(StartVelocityZRangeUUId)
            ? mat.GetVector(StartVelocityZRangeUUId)
            : new Vector4(-3.6f, 3.6f, 0f, 0f);
        Vector4 locRange = mat != null && mat.HasProperty(StartLocationZRangeUUId)
            ? mat.GetVector(StartLocationZRangeUUId)
            : new Vector4(-3.6f, 3.6f, 0f, 0f);
        Vector4 sizeZRange = mat != null && mat.HasProperty(StartSizeZRangeId)
            ? mat.GetVector(StartSizeZRangeId)
            : new Vector4(-0.0408f, 0.0408f, 0f, 0f);
        float sizeXY = mat != null && mat.HasProperty(StartSizeXYId)
            ? mat.GetFloat(StartSizeXYId)
            : 0.132f;
        Vector4 colorMul = mat != null && mat.HasProperty(ColorMultiplierId)
            ? mat.GetVector(ColorMultiplierId)
            : new Vector4(1f, 0.774f, 0.6f, 0f);
        Vector4 lifetimeRange = mat != null && mat.HasProperty(LifetimeRangeId)
            ? mat.GetVector(LifetimeRangeId)
            : new Vector4(1f, 1f, 0f, 0f);
        Vector4 delayRange = mat != null && mat.HasProperty(InitialDelayRangeId)
            ? mat.GetVector(InitialDelayRangeId)
            : Vector4.zero;

        velocityZ = AppRandFRangeVectorZ(0f, 0f, 0f, 0f, velRange.x, velRange.y, ref state);
        locationZ = AppRandFRangeVectorZ(0f, 0f, 0f, 0f, locRange.x, locRange.y, ref state);

        AppRandFRange(0f, 1f, ref state);
        for (int i = 0; i < 6; i++)
        {
            AppRandFrand(ref state);
        }

        AppRandFRangeVectorZ(
            colorMul.x, colorMul.x,
            colorMul.y, colorMul.y,
            colorMul.z, colorMul.z,
            ref state);
        AppRandFRange(lifetimeRange.x, lifetimeRange.y, ref state);
        AppRandFRange(delayRange.x, delayRange.y, ref state);
        AppRandFRange(1f, 1f, ref state);

        sizeZ = AppRandFRangeVectorZ(
            sizeXY, sizeXY,
            sizeXY, sizeXY,
            sizeZRange.x, sizeZRange.y,
            ref state);
    }

    /// </summary>
    /// Leaves state at StartSpin entry (draw 22).
    /// CPU mirror of L2Fx_MeshSpawnParticle_SampleLocVelSize (Spirit / anisotropic mesh).
    /// <summary>
    public static void SampleMeshSpawnLocVelSize(
        Material mat,
        uint stateBeforeVelocity,
        out Vector3 velocityUe,
        out Vector3 locationUe,
        out Vector3 colorMulRgb,
        out float lifetimeSeconds,
        out float initialDelaySeconds,
        out Vector3 sizeUe)
    {
        uint state = stateBeforeVelocity;
        Vector2 velX = ReadMinMax(mat, StartVelocityRangeXUcId, 10f, 10f);
        Vector2 velY = ReadMinMax(mat, StartVelocityRangeYUcId, -20f, 20f);
        Vector2 velZ = ReadMinMax(mat, StartVelocityRangeZUcId, -20f, 20f);
        Vector2 locX = ReadMinMax(mat, StartLocationRangeXUcId, 0f, 0f);
        Vector2 locY = ReadMinMax(mat, StartLocationRangeYUcId, 0f, 0f);
        Vector2 locZ = ReadMinMax(mat, StartLocationRangeZUcId, 0f, 0f);
        Vector4 colorMul = mat != null && mat.HasProperty(ColorMultiplierId)
            ? mat.GetVector(ColorMultiplierId)
            : new Vector4(0.6f, 0.6f, 0.6f, 0f);
        Vector2 life = ReadMinMax(mat, LifetimeRangeId, 1f, 1.5f);
        Vector2 delay = ReadMinMax(mat, InitialDelayRangeId, 0f, 0f);
        Vector2 sizeX = ReadMinMax(mat, StartSizeRangeXUcId, 0.015f, 0.015f);
        Vector2 sizeY = ReadMinMax(mat, StartSizeRangeYUcId, 0.1f, 0.1f);
        Vector2 sizeZ = ReadMinMax(mat, StartSizeRangeZUcId, 0.1f, 0.1f);

        velocityUe = AppRandFRangeVector(velX, velY, velZ, ref state);
        locationUe = AppRandFRangeVector(locX, locY, locZ, ref state);

        AppRandFRange(0f, 1f, ref state);
        for (int i = 0; i < 6; i++)
        {
            AppRandFrand(ref state);
        }

        colorMulRgb = AppRandFRangeVector(
            new Vector2(colorMul.x, colorMul.x),
            new Vector2(colorMul.y, colorMul.y),
            new Vector2(colorMul.z, colorMul.z),
            ref state);
        lifetimeSeconds = AppRandFRange(life.x, life.y, ref state);
        initialDelaySeconds = AppRandFRange(delay.x, delay.y, ref state);
        AppRandFRange(1f, 1f, ref state);
        sizeUe = AppRandFRangeVector(sizeX, sizeY, sizeZ, ref state);
    }

    private static Vector2 ReadMinMax(Material mat, int propertyId, float defaultMin, float defaultMax)
    {
        if (mat != null && mat.HasProperty(propertyId))
        {
            Vector4 v = mat.GetVector(propertyId);
            return new Vector2(v.x, v.y);
        }

        return new Vector2(defaultMin, defaultMax);
    }

    private static uint AppRandStep(ref uint state)
    {
        state = unchecked(state * AppRandMultiplier + AppRandIncrement);
        return (state >> 16) & 0x7fffu;
    }

    private static float AppRandFrand(ref uint state)
    {
        return AppRandStep(ref state) / 32767f;
    }

    private static float AppRandFRange(float min, float max, ref uint state)
    {
        return AppRandFrand(ref state) * (min - max) + max;
    }

    private static float AppRandFRangeVectorZ(
        float xMin, float xMax,
        float yMin, float yMax,
        float zMin, float zMax,
        ref uint state)
    {
        float z = AppRandFRange(zMin, zMax, ref state);
        AppRandFRange(yMin, yMax, ref state);
        AppRandFRange(xMin, xMax, ref state);
        return z;
    }

    private static Vector3 AppRandFRangeVector(
        Vector2 xRange,
        Vector2 yRange,
        Vector2 zRange,
        ref uint state)
    {
        float z = AppRandFRange(zRange.x, zRange.y, ref state);
        float y = AppRandFRange(yRange.x, yRange.y, ref state);
        float x = AppRandFRange(xRange.x, xRange.y, ref state);
        return new Vector3(x, y, z);
    }

    /// </summary>
    /// is immediately before that slot's Roll/Z StartSpin draw.
    /// The supplied base state is generated once per effect play; the stored state
    /// Copies one live appRand sequence into a m_u004_b / MeshEmitter3 slot.
    /// <summary>
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

    /// </summary>
    /// within the slot (slot*31 + 22 from sharedBase). LIVE: Wave + Spirit.
    /// Mesh SpawnParticle: sharedBase is before StartVelocity. StartSpin is +22 draws
    /// <summary>
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

    /// </summary>
    /// appFrand draws from this state.
    /// FRangeVector::GetRand call. The shader consumes the verified nine
    /// Writes the state immediately before SpriteEmitter StartSpin's
    /// <summary>
    public static void SetSpriteSpinRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(SpriteSpinRandStateBitsId))
        {
            mat.SetFloat(
                SpriteSpinRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }

    /// </summary>
    /// FRangeVector::GetRand call. The shader consumes its Z/Y/X draws.
    /// Writes the state immediately before SpriteEmitter StartVelocityRange's
    /// <summary>
    public static void SetSpriteMotionRandState(Material mat, uint state)
    {
        if (mat != null && mat.HasProperty(SpriteMotionRandStateBitsId))
        {
            mat.SetFloat(
                SpriteMotionRandStateBitsId,
                BitConverter.Int32BitsToSingle(unchecked((int)state)));
        }
    }

    /// </summary>
    /// while _L2MotionReplayEnabled remains zero on the shared material.
    /// It is intentionally limited to slot 0 and does not alter normal gameplay
    /// Opt-in diagnostic replay for the captured m_u004_b SpriteEmitter0 slot 0.
    /// <summary>
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

    public static float UIntBitsToFloat(uint state)
    {
        return BitConverter.Int32BitsToSingle(unchecked((int)state));
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

    /// </summary>
    /// It is a state seed, not a float numeric value.
    /// Produces a finite bit-pattern that survives Material.SetFloat/asuint.
    /// <summary>
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
