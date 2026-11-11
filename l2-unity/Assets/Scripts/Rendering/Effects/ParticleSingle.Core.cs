using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle : EffectPart
{
    void OnDestroy()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropAdenaDestroyLog.Event(
            // "PARTICLE_SINGLE_ON_DESTROY",
            // this,
            // $"duration={_duration:F3}s spawnedAt={_lifetime.SpawnedAt:F3}s " +
            // $"active={_lifetime.Active} stopped={_lifetime.Stopped} spawnedCount={_lifetime.SpawnedCount}",
            // includeStack: true);
#endif
    }

    private void CompleteImmediateStop(float now)
    {
        _holdController.CompleteImmediateStop();
        ApplyCompositeShaderHoldToAllRuntimeMaterials(now);
        _lifetime.Stopped = true;
    }

    private void SetOwnerWorldPos(Material material)
    {
        if (material == null || !material.HasProperty(L2MaterialPropertyCopier.OwnerWorldPosProperty))
        {
            return;
        }

        material.SetVector(L2MaterialPropertyCopier.OwnerWorldPosProperty, ResolveOwnerWorldPosForShader(material));
    }
}
