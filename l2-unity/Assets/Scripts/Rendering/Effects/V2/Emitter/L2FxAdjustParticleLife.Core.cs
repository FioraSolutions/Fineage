using UnityEngine;

public static partial class L2FxAdjustParticleLife
{
    public static float ReadAuthoredMaxLife(Renderer renderer, float fallback)
    {
        if (renderer == null)
        {
            return Mathf.Max(0.01f, fallback);
        }

        Material[] shared = renderer.sharedMaterials;
        for (int i = 0; i < shared.Length; i++)
        {
            Material mat = shared[i];
            if (mat != null && mat.HasProperty(LifetimeRangeId))
            {
                return Mathf.Max(0.01f, mat.GetVector(LifetimeRangeId).y);
            }
        }

        return Mathf.Max(0.01f, fallback);
    }

    public static void ApplyToRenderer(Renderer renderer, float authoredMaxLife, float targetLife)
    {
        if (renderer == null)
        {
            return;
        }

        float authored = Mathf.Max(0.01f, authoredMaxLife);
        float target = Mathf.Max(0.01f, targetLife);
        float delta = target - authored;
        if (Mathf.Abs(delta) < 1e-4f)
        {
            return;
        }

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            ApplyToMaterial(materials[i], authored, target);
        }
    }
}
