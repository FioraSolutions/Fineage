using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    private string BuildLifetimeShaderSnapshot()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            return "no_renderers";
        }

        StringBuilder sb = new StringBuilder();
        int maxRenderersToLog = 12;
        for (int i = 0; i < renderers.Length && i < maxRenderersToLog; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || renderer.sharedMaterial == null)
            {
                continue;
            }

            Material mat = renderer.sharedMaterial;
            string hasLifetime = TryReadFloat(mat, "_HasLifeTime", "hasLifeTime");
            if (hasLifetime == "n/a")
            {
                hasLifetime = TryReadFloat(mat, "_HasLifetime", "hasLifetime");
            }

            string lifeTimeRange = TryReadVector(mat, "_LifeTimeRange", "lifeTimeRange");
            if (lifeTimeRange == "n/a")
            {
                lifeTimeRange = TryReadFloat(mat, "_LifeTimeRange", "lifeTimeRangeScalar");
            }
            if (lifeTimeRange == "n/a")
            {
                lifeTimeRange = TryReadVector(mat, "_LifetimeRange", "lifetimeRange");
            }
            if (lifeTimeRange == "n/a")
            {
                lifeTimeRange = TryReadFloat(mat, "_LifetimeRange", "lifetimeRangeScalar");
            }

            if (sb.Length > 0)
            {
                sb.Append(" | ");
            }

            sb.Append($"{renderer.name}:{mat.name}({hasLifetime},{lifeTimeRange})");
        }

        if (renderers.Length > maxRenderersToLog)
        {
            sb.Append($" | ... +{renderers.Length - maxRenderersToLog} renderers");
        }

        return sb.Length > 0 ? sb.ToString() : "no_material_data";
    }

    private string TryReadFloat(Material mat, string property, string label)
    {
        if (mat.HasProperty(property))
        {
            return $"{label}={mat.GetFloat(property):F3}";
        }

        return "n/a";
    }

    private string TryReadVector(Material mat, string property, string label)
    {
        if (mat.HasProperty(property))
        {
            Vector4 v = mat.GetVector(property);
            return $"{label}=({v.x:F3},{v.y:F3},{v.z:F3},{v.w:F3})";
        }

        return "n/a";
    }
}
