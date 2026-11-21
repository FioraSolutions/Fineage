using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2ActorFade
{
    static readonly int AlphaClipId = Shader.PropertyToID("_AlphaClip");

    static void LogFadeBeginFailure(Entity entity, Renderer[] found, string reason)
    {
        string label = entity != null ? entity.name : "null-entity";
        if (entity?.Identity != null && entity.Identity.Id > 0)
            label = $"{entity.Identity.Name} (objectId={entity.Identity.Id})";
        Debug.LogError($"[L2ActorFade] TryBegin failed for {label}: {reason}.");
        if (found == null)
            return;
        for (int i = 0; i < found.Length; i++)
        {
            Renderer r = found[i];
            if (r == null)
                continue;
            Debug.LogError(
                $"[L2ActorFade]   renderer '{r.name}' type={r.GetType().Name} enabled={r.enabled} layer={r.gameObject.layer}");
        }
    }

    static void SetAlpha(Material[][] instances, float alpha)
    {
        if (instances == null)
        {
            return;
        }

        for (int i = 0; i < instances.Length; i++)
        {
            Material[] mats = instances[i];
            if (mats == null)
            {
                continue;
            }

            for (int m = 0; m < mats.Length; m++)
            {
                ApplyAlpha(mats[m], alpha);
            }
        }
    }

    static void ApplyAlpha(Material mat, float alpha)
    {
        if (mat == null)
        {
            return;
        }

        if (mat.HasProperty(ActorAlphaId))
        {
            mat.SetFloat(ActorAlphaId, alpha);
            return;
        }

        if (mat.HasProperty(BaseColorId))
        {
            Color color = mat.GetColor(BaseColorId);
            color.a = alpha;
            mat.SetColor(BaseColorId, color);
        }
    }

    static bool IsFadeRenderer(Renderer renderer)
    {
        if (renderer == null || !renderer.enabled)
        {
            return false;
        }

        if (renderer is ParticleSystemRenderer)
        {
            return false;
        }

        if (renderer.gameObject.name == "click_area")
        {
            return false;
        }

        return renderer is SkinnedMeshRenderer || renderer is MeshRenderer;
    }

    static void DestroyIfNotShared(Material instance, Material[] shared)
    {
        if (instance == null)
        {
            return;
        }

        for (int s = 0; s < shared.Length; s++)
        {
            if (instance == shared[s])
            {
                return;
            }
        }

        Object.Destroy(instance);
    }

    static bool MaterialSupportsFade(Material mat)
    {
        return mat != null && (mat.HasProperty(ActorAlphaId) || mat.HasProperty(BaseColorId));
    }

    static void PrepareCurrentShader(Material mat)
    {
        if (mat == null)
        {
            return;
        }

        // Semi-transparent STEALTH must not alpha-test at full texture alpha (L2ActorEnvAdd default).
        mat.DisableKeyword("_ALPHATEST_ON");
        if (mat.HasProperty(AlphaClipId))
            mat.SetFloat(AlphaClipId, 0f);

        if (mat.HasProperty(SrcBlendId))
        {
            mat.SetFloat(SrcBlendId, (float)BlendMode.SrcAlpha);
        }

        if (mat.HasProperty(DstBlendId))
        {
            mat.SetFloat(DstBlendId, (float)BlendMode.OneMinusSrcAlpha);
        }

        if (mat.HasProperty(ZWriteId))
        {
            mat.SetFloat(ZWriteId, 0f);
        }

        mat.EnableKeyword(FadeKeyword);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.renderQueue = (int)RenderQueue.Transparent;
        if (mat.HasProperty(ActorAlphaId))
        {
            mat.SetFloat(ActorAlphaId, 1f);
        }
    }
}
