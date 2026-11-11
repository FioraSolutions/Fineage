using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public void ActivateGoSlot(
        int slot,
        float shaderStartTime,
        float seed,
        uint meshRandBase,
        uint spriteRandBase)
    {
        if (_particles == null || slot < 0 || slot >= _particles.Length)
        {
            return;
        }

        Renderer renderer = _particles[slot];
        if (renderer == null)
        {
            return;
        }

        // A group can fall back after having used GPU drawing on an earlier
        // playback/domain reload. DisableForGpuDraw leaves the component off.
        renderer.enabled = true;
        ApplySpawnTiming(renderer, shaderStartTime, seed);
        Material[] runtimeMaterials = renderer.materials;
        Material[] sharedMaterials = renderer.sharedMaterials;
        for (int materialIndex = 0; materialIndex < runtimeMaterials.Length; materialIndex++)
        {
            Material material = runtimeMaterials[materialIndex];
            if (material == null)
                continue;

            Material shared = sharedMaterials != null && materialIndex < sharedMaterials.Length
                ? sharedMaterials[materialIndex]
                : null;
            if (shared != null)
            {
                L2MaterialPropertyCopier.CopyLifetimeFadeAndFxFromShared(material, shared);
                L2MaterialPropertyCopier.CopyMeshAppRandStartSpinFromBaseState(
                    material,
                    shared,
                    meshRandBase,
                    slot);
            }

            if (material.HasProperty("_Alpha") && shared != null && shared.HasProperty("_Alpha"))
                material.SetFloat("_Alpha", shared.GetFloat("_Alpha"));

            if (material.HasProperty(DebugMeshPreviewShaderId))
                material.SetFloat(DebugMeshPreviewShaderId, 0f);

            material.SetFloat(StartTimeShaderId, shaderStartTime);
            material.SetFloat(SeedShaderId, seed);
            ApplySpawnSpin(renderer, seed);
            uint spriteSpawnState = L2MaterialPropertyCopier.AdvanceAppRandState(
                spriteRandBase,
                slot * L2AppRand.SpriteMotionSlotStride);
            L2MaterialPropertyCopier.SetSpriteMotionRandState(material, spriteSpawnState);
            L2MaterialPropertyCopier.SetSpriteSpinRandState(
                material,
                L2MaterialPropertyCopier.AdvanceAppRandState(spriteSpawnState, L2AppRand.SpriteSpinDraws));
            L2MaterialPropertyCopier.ApplyHealingPotionSe0MotionReplay(material, slot);
            SetWorldPositions(material);

            if (_host.SurfaceNormal != Vector3.zero)
                material.SetVector("_SurfaceNormals", _host.SurfaceNormal);
        }
    }

    public void ApplyCoordinateSystemToGoSlot(
        int slot,
        L2ParticleCoordinateSystem coordinateSystem)
    {
        if (_particles == null || slot < 0 || slot >= _particles.Length)
        {
            return;
        }

        Renderer renderer = _particles[slot];
        if (renderer == null)
        {
            return;
        }

        EnsureTransformOrigins();
        RestoreCoordinateSystemSlot(slot);

        Transform particleTransform = renderer.transform;
        if (coordinateSystem == L2ParticleCoordinateSystem.Spray)
        {
            // Native PTCS_Spray rotates position, velocity and acceleration by
            // the owner rotation once at SpawnParticle, then remains world-space.
            particleTransform.SetParent(null, true);
        }
        else if (coordinateSystem == L2ParticleCoordinateSystem.Independent)
        {
            // Native PTCS_Independent inherits spawn translation but not owner
            // rotation, and no longer follows later owner movement.
            Vector3 worldPosition = particleTransform.position;
            Vector3 worldScale = particleTransform.lossyScale;
            particleTransform.SetParent(null, false);
            particleTransform.SetPositionAndRotation(worldPosition, Quaternion.identity);
            particleTransform.localScale = worldScale;
        }
    }
}
