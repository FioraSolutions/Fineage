using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static bool IsMeshSpawnParticleMaterial(Material mat)
    {
        if (mat == null || !mat.HasProperty(MeshSpawnRandStateBitsId))
            return false;

        // Unified MeshEmitter exposes the full property superset. The mode,
        // rather than property existence, opts a material into SpawnParticle.
        return !mat.HasProperty(SpawnModeId) || mat.GetFloat(SpawnModeId) > 0.5f;
    }

    public static bool IsMeshStartSpinMaterial(Material mat)
    {
        if (mat == null || !mat.HasProperty(StartSpinRandStateBitsId))
            return false;

        return !mat.HasProperty(SpinParticlesId) || mat.GetFloat(SpinParticlesId) > 0.5f;
    }

    public static bool IsSpriteSpawnMaterial(Material mat)
    {
        if (mat == null || !mat.HasProperty(SpriteMotionRandStateBitsId))
            return false;

        return !mat.HasProperty(SpawnModeId) || mat.GetFloat(SpawnModeId) > 0.5f;
    }

    public static bool IsSpriteSpinMaterial(Material mat)
    {
        if (mat == null || !mat.HasProperty(SpriteSpinRandStateBitsId))
            return false;

        return !mat.HasProperty(SpriteSpinModeId) || mat.GetFloat(SpriteSpinModeId) > 0.5f;
    }

    public static bool IsShotNAtkMeshEmitter225SpiritMaterial(Material mat)
    {
        if (mat == null ||
            !IsMeshSpawnParticleMaterial(mat) ||
            !mat.HasProperty(StartLocationOffsetUeId))
        {
            return false;
        }

        // Distinguish from ShockWave (Offset=0, short FadeOut).
        Vector4 offset = mat.GetVector(StartLocationOffsetUeId);
        return offset.x < -1f;
    }

    public static bool IsShotNAtkMeshEmitter226ShockWaveMaterial(Material mat)
    {
        if (mat == null ||
            !IsMeshSpawnParticleMaterial(mat) ||
            !mat.HasProperty(StartLocationOffsetUeId))
        {
            return false;
        }

        if (IsShotNAtkMeshEmitter225SpiritMaterial(mat))
        {
            return false;
        }

        // UC FadeOutStartTime=0.0375; Spirit uses ~0.41.
        int fadeOutId = Shader.PropertyToID("_FadeOutStartTime");
        if (!mat.HasProperty(fadeOutId))
        {
            return false;
        }

        return mat.GetFloat(fadeOutId) < 0.1f;
    }
}
