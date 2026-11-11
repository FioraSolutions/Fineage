using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle : EffectPart
{
    static bool RequiresExpandedBounds(Material material)
    {
        const string expandBoundsProperty = "_ExpandShaderBounds";
        return material != null &&
               material.HasProperty(expandBoundsProperty) &&
               material.GetFloat(expandBoundsProperty) > 0.5f;
    }

    bool RequiresExpandedBounds(Renderer renderer)
    {
        if (renderer == null)
            return false;

        Material[] materials = renderer.sharedMaterials;
        if (materials == null)
            return false;

        for (int i = 0; i < materials.Length; i++)
        {
            if (RequiresExpandedBounds(materials[i]))
                return true;
        }

        return false;
    }

    void ExpandVertexMotionBounds()
    {
        Renderer renderer = ResolveRenderer();
        if (!RequiresExpandedBounds(renderer))
            return;

        // Culling sees imported CPU bounds before the vertex shader applies
        // StartSize, UC offset, velocity and acceleration. coin00 is only
        // ~6 mm wide in the FBX but its shader path moves it about one metre.
        renderer.allowOcclusionWhenDynamic = false;
        renderer.localBounds = new Bounds(
            new Vector3(0f, 0.5f, 0f),
            new Vector3(8f, 8f, 8f));
    }

    static float ReadMaxInitialDelay(Renderer renderer)
    {
        float maxDelay = 0f;
        if (renderer == null || renderer.sharedMaterials == null)
            return maxDelay;

        Material[] materials = renderer.sharedMaterials;
        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            if (material == null || !material.HasProperty("_InitialDelayRange"))
                continue;
            maxDelay = Mathf.Max(maxDelay, material.GetVector("_InitialDelayRange").y);
        }

        return maxDelay;
    }
}
