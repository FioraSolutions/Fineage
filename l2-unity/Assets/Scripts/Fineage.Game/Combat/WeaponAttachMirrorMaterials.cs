using UnityEngine;

/// <summary>
/// Runtime and validation policy for mirrored weapon mounts (negative X scale at socket).
/// Negative scale flips winding; thin volumetric sub-parts need double-sided materials, not cull flip.
/// </summary>
public static class WeaponAttachMirrorMaterials
{
    public const float CullOff = 0f;
    public const float CullFront = 1f;
    public const float CullBack = 2f;

    public static void ApplyForNegativeScaleX(GameObject weaponRoot)
    {
        if (weaponRoot == null)
            return;
        WeaponAttachApplicator.RepairSubmeshMaterialsPublic(weaponRoot);
        ForceDoubleSidedInstanceMaterials(weaponRoot);
    }

    public static bool WouldRenderFlatUnderMirror(Material mat)
    {
        if (mat == null)
            return false;
        if (!mat.HasProperty("_Cull"))
            return ShaderLooksSingleSided(mat.shader);
        float cull = mat.GetFloat("_Cull");
        return cull > 0.5f;
    }

    public static bool IsMirrorSafeAfterRuntimeFix(Material mat)
    {
        if (mat == null)
            return true;
        if (!mat.HasProperty("_Cull"))
            return !ShaderLooksSingleSided(mat.shader);
        return true;
    }

    static bool ShaderLooksSingleSided(Shader shader)
    {
        if (shader == null)
            return true;
        string n = shader.name;
        if (n.IndexOf("Universal Render Pipeline", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("URP", System.StringComparison.Ordinal) >= 0)
            return true;
        return false;
    }

    public static void ForceDoubleSidedInstanceMaterials(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            Material[] shared = renderer.sharedMaterials;
            bool needsCopy = false;
            for (int m = 0; m < shared.Length; m++)
            {
                Material mat = shared[m];
                if (mat != null && mat.HasProperty("_Cull") && mat.GetFloat("_Cull") > 0.5f)
                {
                    needsCopy = true;
                    break;
                }
            }
            if (!needsCopy)
                continue;

            var copy = new Material[shared.Length];
            for (int m = 0; m < shared.Length; m++)
            {
                if (shared[m] == null)
                    continue;
                copy[m] = new Material(shared[m]);
                if (copy[m].HasProperty("_Cull"))
                    copy[m].SetFloat("_Cull", CullOff);
            }
            renderer.sharedMaterials = copy;
        }
    }
}