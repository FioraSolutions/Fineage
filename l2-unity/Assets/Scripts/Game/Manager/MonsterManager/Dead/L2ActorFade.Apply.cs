using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class L2ActorFade
{
    public void Restore(Renderer[] renderers, Material[][] sharedBackup, Material[][] instances)
    {
        if (renderers == null || sharedBackup == null)
        {
            return;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            Material[] shared = i < sharedBackup.Length ? sharedBackup[i] : null;
            if (renderer == null || shared == null)
            {
                continue;
            }

            renderer.sharedMaterials = shared;

            Material[] fadeMats = instances != null && i < instances.Length ? instances[i] : null;
            if (fadeMats == null)
            {
                continue;
            }

            for (int m = 0; m < fadeMats.Length; m++)
            {
                DestroyIfNotShared(fadeMats[m], shared);
            }
        }
    }

    bool TryBegin(
        Entity entity,
        byte startAlphaByte,
        bool captureShared,
        out Renderer[] renderers,
        out Material[][] instances,
        out Material[][] sharedBackup)
    {
        renderers = null;
        instances = null;
        sharedBackup = null;
        if (entity == null || entity.gameObject == null)
        {
            return false;
        }

        Renderer[] found = entity.gameObject.GetComponentsInChildren<Renderer>(true);
        int count = CountFadeRenderers(found);
        if (count == 0)
        {
            return false;
        }

        renderers = new Renderer[count];
        instances = new Material[count][];
        if (captureShared)
        {
            sharedBackup = new Material[count][];
        }

        int write = 0;
        for (int i = 0; i < found.Length; i++)
        {
            Renderer renderer = found[i];
            if (!IsFadeRenderer(renderer))
            {
                continue;
            }

            if (captureShared)
            {
                sharedBackup[write] = renderer.sharedMaterials;
            }

            Material[] mats = renderer.materials;
            for (int m = 0; m < mats.Length; m++)
            {
                PrepareCurrentShader(mats[m]);
            }

            renderers[write] = renderer;
            instances[write] = mats;
            write++;
        }

        SetAlphaByte(instances, startAlphaByte);
        return true;
    }

    static int CountFadeRenderers(Renderer[] found)
    {
        int count = 0;
        for (int i = 0; i < found.Length; i++)
        {
            if (IsFadeRenderer(found[i]))
            {
                count++;
            }
        }

        return count;
    }

    static byte ClampAlphaByte(float value)
    {
        if (value <= 0f)
        {
            return 0;
        }

        if (value >= 255f)
        {
            return 255;
        }

        return (byte)value;
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

    static void PrepareCurrentShader(Material mat)
    {
        if (mat == null)
        {
            return;
        }

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
