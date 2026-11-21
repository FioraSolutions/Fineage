using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Runtime paperdoll material normalization (armor matches body/face ActorModulate2X).</summary>
public static partial class L2ActorMaterialSetup
{
    static readonly Dictionary<int, Material> PaperdollMaterialCache = new Dictionary<int, Material>();

    static Shader _actorShader;

    static Shader ActorShader
    {
        get
        {
            if (_actorShader == null)
            {
                _actorShader = Shader.Find(ActorShaderName);
                if (_actorShader == null)
                {
                    Debug.LogError(
                        "[L2ActorMaterial] missing shader path="
                        + ActorShaderName
                        + " (paperdoll normalization)");
                }
            }

            return _actorShader;
        }
    }

    /// <summary>
    /// Returns a runtime instance using <see cref="ActorShaderName"/> for opaque armor/body slots.
    /// Shipped armor .mat files are sometimes still on L2/World/ActorEnvAdd; face/body use ActorModulate2X.
    /// </summary>
    public static Material EnsurePaperdollActorMaterial(Material source, string meshContext)
    {
        if (source == null)
            return null;

        if (IsPaperdollActorModulateOpaque(source) || IsPaperdollActorModulateCutout(source))
            return source;

        int key = source.GetInstanceID();
        if (PaperdollMaterialCache.TryGetValue(key, out Material cached) && cached != null)
            return cached;

        Shader actor = ActorShader;
        if (actor == null)
            return source;

        Texture albedo = PickAlbedo(source);
        if (albedo == null)
        {
            Debug.LogError(
                "[L2ActorMaterial] missing albedo texture material="
                + source.name
                + " mesh="
                + (meshContext ?? "?"));
            return source;
        }

        SetupResult resolve = Resolve(source.name, null);
        var mat = new Material(actor)
        {
            name = source.name + "_paperdoll",
            doubleSidedGI = true
        };
        mat.SetTexture("_BaseMap", albedo);
        if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", albedo);
        mat.SetColor("_BaseColor", Color.white);
        mat.SetFloat("_ActorAlpha", 1f);
        mat.SetFloat("_Cull", 0f);

        if (resolve.mode == SurfaceMode.AlphaCutout)
        {
            float cutoff = DefaultCutoff;
            mat.SetFloat("_AlphaClip", 1f);
            mat.SetFloat("_Cutoff", cutoff);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = (int)RenderQueue.AlphaTest;
        }
        else
        {
            mat.SetFloat("_AlphaClip", 0f);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = (int)RenderQueue.Geometry;
        }

        mat.SetFloat("_SrcBlend", (float)BlendMode.One);
        mat.SetFloat("_DstBlend", (float)BlendMode.Zero);
        mat.SetFloat("_ZWrite", 1f);

        string fromShader = source.shader != null ? source.shader.name : "(null)";
        if (!string.Equals(fromShader, ActorShaderName, StringComparison.Ordinal))
        {
            Debug.LogWarning(
                "[L2ActorMaterial] paperdoll shader normalized material="
                + source.name
                + " from="
                + fromShader
                + " mesh="
                + (meshContext ?? "?"));
        }

        PaperdollMaterialCache[key] = mat;
        return mat;
    }

    static Texture PickAlbedo(Material source)
    {
        if (source.HasProperty("_BaseMap"))
        {
            Texture t = source.GetTexture("_BaseMap");
            if (t != null)
                return t;
        }

        if (source.HasProperty("_MainTex"))
        {
            Texture t = source.GetTexture("_MainTex");
            if (t != null)
                return t;
        }

        return null;
    }

    static bool IsPaperdollActorModulateOpaque(Material mat)
    {
        if (mat.shader == null || mat.shader.name != ActorShaderName)
            return false;
        return !mat.IsKeywordEnabled("_ALPHATEST_ON") && mat.renderQueue < (int)RenderQueue.AlphaTest;
    }

    static bool IsPaperdollActorModulateCutout(Material mat)
    {
        if (mat.shader == null || mat.shader.name != ActorShaderName)
            return false;
        return mat.IsKeywordEnabled("_ALPHATEST_ON");
    }
}