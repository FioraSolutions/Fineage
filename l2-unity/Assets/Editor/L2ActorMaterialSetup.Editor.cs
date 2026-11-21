#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>Editor-side apply + audit for <see cref="L2ActorMaterialSetup"/>.</summary>
public static class L2ActorMaterialSetupEditor
{
    public static void ApplyToMaterial(
        Material mat,
        Shader actorShader,
        Texture albedo,
        string materialNameForResolve,
        IReadOnlyDictionary<string, string> propsByName,
        bool logMissingTexture)
    {
        if (mat == null)
            return;
        if (actorShader != null)
            mat.shader = actorShader;

        if (albedo != null)
        {
            if (mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", albedo);
            if (mat.HasProperty("_MainTex"))
                mat.SetTexture("_MainTex", albedo);
        }
        else if (logMissingTexture)
        {
            string ctx = materialNameForResolve ?? mat.name;
            Debug.LogError(
                "[L2ActorMaterial] missing alpha texture mesh/material="
                + ctx
                + " props="
                + (propsByName != null && propsByName.ContainsKey(L2ActorMaterialSetup.NormalizePropsKey(ctx))
                    ? "found"
                    : "missing"));
        }

        L2ActorMaterialSetup.SetupResult resolve =
            L2ActorMaterialSetup.Resolve(materialNameForResolve ?? mat.name, propsByName);

        mat.shaderKeywords = new string[0];
        mat.SetColor("_BaseColor", Color.white);
        mat.SetFloat("_ActorAlpha", 1f);
        mat.doubleSidedGI = true;
        mat.SetFloat("_Cull", 0f);

        if (resolve.mode == L2ActorMaterialSetup.SurfaceMode.HiddenCollisionOnly)
        {
            Debug.LogError(
                "[L2ActorMaterial] collision-only UE2 slot on actor mesh material="
                + mat.name
                + " detail="
                + resolve.detail);
        }

        if (resolve.mode == L2ActorMaterialSetup.SurfaceMode.AlphaCutout)
        {
            float cutoff = L2ActorMaterialSetup.CutoffFromProps(mat.name, propsByName);
            mat.SetFloat("_AlphaClip", 1f);
            mat.SetFloat("_Cutoff", cutoff);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetFloat("_ZWrite", 1f);
        }
        else
        {
            mat.SetFloat("_AlphaClip", 0f);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetFloat("_ZWrite", 1f);
        }

        resolve.applied = true;
        EditorUtility.SetDirty(mat);
    }

    [MenuItem("L2/Characters/Audit Actor Alpha Materials")]
    public static void AuditMenu()
    {
        var props = L2ActorMaterialSetup.CollectPropsIndex(null);
        var offenders = new List<string>();
        string[] folders =
        {
            "Assets/Resources/Data/SysTextures",
        };
        foreach (string guid in AssetDatabase.FindAssets("t:Material", folders))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
                continue;
            if (!L2ActorMaterialSetup.MaterialLooksLikeActorOpaqueMismatch(mat)
                && !L2ActorMaterialSetup.NamingImpliesCutout(mat.name))
                continue;
            if (L2ActorMaterialSetup.MaterialLooksLikeActorOpaqueMismatch(mat))
                offenders.Add(path);
        }

        if (offenders.Count == 0)
            Debug.Log("[L2ActorMaterial] audit: no opaque/cutout mismatches under SysTextures.");
        else
        {
            Debug.LogError(
                "[L2ActorMaterial] audit: "
                + offenders.Count
                + " materials need alpha cutout. First: "
                + string.Join(", ", offenders.GetRange(0, System.Math.Min(12, offenders.Count))));
        }
    }

    [MenuItem("L2/Characters/Apply Actor Material Flags (SysTextures)")]
    public static void ApplyAllSysTexturesMenu()
    {
        Shader actor = AssetDatabase.LoadAssetAtPath<Shader>(L2ActorRaceMaterials.ShaderPath);
        if (actor == null)
        {
            Debug.LogError("[L2ActorMaterial] missing actor shader");
            return;
        }

        var props = L2ActorMaterialSetup.CollectPropsIndex(null);
        int n = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Resources/Data/SysTextures" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
                continue;
            Texture tex = mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : null;
            if (tex == null && mat.HasProperty("_MainTex"))
                tex = mat.GetTexture("_MainTex");
            ApplyToMaterial(mat, actor, tex, mat.name, props, logMissingTexture: false);
            n++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[L2ActorMaterial] refreshed " + n + " SysTexture materials");
    }
}
#endif