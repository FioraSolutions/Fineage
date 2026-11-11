using UnityEngine;

public static partial class EffectShaderLifetimeHelper
{
    public static void BeginCastEndShaderFade(Transform instanceTransform, float fadeSeconds)
    {
        if (instanceTransform == null)
        {
            return;
        }

        float fadeWindow = Mathf.Max(0.15f, fadeSeconds);
        float now = Time.time;
        float fadeOutStart = fadeWindow * 0.05f;

        Renderer[] renderers = instanceTransform.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material[] mats = renderer.materials;
            for (int j = 0; j < mats.Length; j++)
            {
                Material mat = mats[j];
                if (mat == null)
                {
                    continue;
                }

                if (mat.HasProperty(L2MaterialPropertyCopier.HasLifetimeId))
                {
                    mat.SetFloat(L2MaterialPropertyCopier.HasLifetimeId, 1f);
                }

                if (mat.HasProperty("_HasLifeTime"))
                {
                    mat.SetFloat("_HasLifeTime", 1f);
                }

                Vector4 lifetimeRange = new Vector4(fadeWindow, fadeWindow, 0f, 0f);
                if (mat.HasProperty(L2MaterialPropertyCopier.LifetimeRangeId))
                {
                    mat.SetVector(L2MaterialPropertyCopier.LifetimeRangeId, lifetimeRange);
                }

                if (mat.HasProperty("_LifeTimeRange"))
                {
                    mat.SetVector("_LifeTimeRange", lifetimeRange);
                }

                if (mat.HasProperty(L2MaterialPropertyCopier.StartTimeId))
                {
                    mat.SetFloat(L2MaterialPropertyCopier.StartTimeId, now);
                }

                if (mat.HasProperty(L2MaterialPropertyCopier.FadeInId))
                {
                    mat.SetFloat(L2MaterialPropertyCopier.FadeInId, 0f);
                }

                if (mat.HasProperty(L2MaterialPropertyCopier.FadeoutStartTimeId))
                {
                    mat.SetFloat(L2MaterialPropertyCopier.FadeoutStartTimeId, fadeOutStart);
                }
            }
        }
    }
}
