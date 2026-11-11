using UnityEngine;

public static partial class L2FxAlphaBlendDrawOrder
{
    public static void SetGroupSorting(ParticleGroupV2 group, int sortingOrder)
    {
        if (group == null)
        {
            return;
        }

        Renderer[] particles = group.CaptureAuthoring().particles;
        if (particles == null || particles.Length == 0)
        {
            particles = group.GetComponentsInChildren<Renderer>(true);
        }

        if (particles == null)
        {
            return;
        }

        for (int i = 0; i < particles.Length; i++)
        {
            Renderer renderer = particles[i];
            if (renderer != null)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }
    }

    static bool HasBlendBetweenFlipbook(ParticleGroupV2 group)
    {
        Renderer[] particles = group.CaptureAuthoring().particles;
        Renderer first = null;
        if (particles != null)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    first = particles[i];
                    break;
                }
            }
        }

        if (first == null)
        {
            first = group.GetComponentInChildren<Renderer>(true);
        }

        if (first == null)
        {
            return false;
        }

        Material[] materials = first.sharedMaterials;
        if (materials == null)
        {
            return false;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            if (material != null &&
                material.HasProperty(FlipbookModeId) &&
                material.GetFloat(FlipbookModeId) > 2.5f)
            {
                return true;
            }
        }

        return false;
    }
}
