using UnityEngine;
using System.Collections.Generic;

public static partial class CompositeProjectileLaunchHelper
{
    private static void RestartHiddenVisuals(BaseEffect spawned)
    {
        L2Particle particle = spawned as L2Particle;
        if (particle != null)
        {
            particle.ResetTimer();
        }
    }

    private static CompositeProjectileConfig GetProjectileConfig(CompositePrefabPart part)
    {
        return part != null && part.projectile != null ? part.projectile : new CompositeProjectileConfig();
    }

    public static void SetPartVisualsVisible(Transform root, bool visible)
    {
        SetVisualsVisible(root, visible);
    }

    private static void SetVisualsVisible(Transform root, bool visible)
    {
        if (root == null)
        {
            return;
        }

        ParticleEmitterV2.SetVisible(root, visible);

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || IsOwnedByGpuStream(renderer))
            {
                continue;
            }

            renderer.enabled = visible;
        }

        ParticleSystem[] particleSystems = root.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem system = particleSystems[i];
            if (visible)
            {
                system.Play(true);
            }
            else
            {
                system.Pause(true);
                system.Clear(true);
            }
        }
    }

    private static bool IsOwnedByGpuStream(Renderer renderer)
    {
        ParticleStreamDriver driver = renderer.GetComponentInParent<ParticleStreamDriver>();
        if (driver != null && driver.IsGpuDraw)
        {
            return true;
        }

        ParticleGroupV2 groupV2 = renderer.GetComponentInParent<ParticleGroupV2>();
        return groupV2 != null && groupV2.IsGpuDraw;
    }
}
