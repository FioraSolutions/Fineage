using System.Collections.Generic;
using UnityEngine;

public partial class L2Particle
{
    private bool ShouldTraceLifetime()
    {
        return !string.IsNullOrEmpty(name) && name.IndexOf(LifetimeTraceEffectName, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private string BuildShaderLifetimeRangesSnapshot()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            return "no_renderers";
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int logged = 0;
        for (int i = 0; i < renderers.Length && logged < 6; i++)
        {
            Renderer r = renderers[i];
            if (r == null)
            {
                continue;
            }

            Material[] mats = r.sharedMaterials;
            if (mats == null)
            {
                continue;
            }

            for (int m = 0; m < mats.Length && logged < 6; m++)
            {
                Material mat = mats[m];
                if (mat == null || !mat.HasProperty("_LifetimeRange"))
                {
                    continue;
                }

                Vector4 life = mat.GetVector("_LifetimeRange");
                if (sb.Length > 0)
                {
                    sb.Append(" | ");
                }

                sb.Append(mat.name).Append(" life=(").Append(life.x.ToString("F3")).Append(',').Append(life.y.ToString("F3")).Append(')');
                logged++;
            }
        }

        return sb.Length > 0 ? sb.ToString() : "no_LifetimeRange";
    }
}
