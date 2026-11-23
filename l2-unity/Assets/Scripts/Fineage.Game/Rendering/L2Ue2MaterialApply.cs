using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes one <see cref="L2Ue2MaterialChain.Eval"/> onto a material.
/// Time-varying TexPanner / TexOscillator offsets go through <see cref="ApplyUv"/>.
/// </summary>
public static class L2Ue2MaterialApply
{
    public static void Apply(Material material, L2Ue2MaterialChain.Eval eval, float time, Func<string, Texture> textures)
    {
        if (material == null || eval == null)
            return;
        bool chain = L2Ue2MaterialChain.NeedsChainShader(eval);
        if (chain && (material.shader == null || material.shader.name != L2Ue2MaterialChain.ChainShaderName))
        {
            Shader shader = Shader.Find(L2Ue2MaterialChain.ChainShaderName);
            if (shader == null)
            {
                Debug.LogError("[Ue2Material] material " + eval.MaterialName + " op Shader shader " +
                               L2Ue2MaterialChain.ChainShaderName + " missing");
            }
            else
            {
                material.shader = shader;
            }
        }

        SetTex(material, eval, "_BaseMap", eval.DiffuseTexture, textures);
        SetTex(material, eval, "_MainTex", eval.DiffuseTexture, textures);
        if (chain || material.HasProperty("_OpacityMap"))
            SetTex(material, eval, "_OpacityMap", eval.OpacityTexture, textures);
        SetTex(material, eval, "_SpecularMap", eval.SpecularTexture, textures);
        SetTex(material, eval, "_SpecMask", eval.SpecularityMaskTexture, textures);
        SetTex(material, eval, "_EmissionMap", eval.SelfIlluminationTexture, textures);
        SetTex(material, eval, "_EmissionMask", eval.SelfIlluminationMaskTexture, textures);
        SetTex(material, eval, "_DetailMap", eval.DetailTexture, textures);
        SetTex(material, eval, "_EnvMap", eval.EnvTexture, textures);
        SetTex(material, eval, "_CombineMap", eval.CombineTexture, textures);
        SetTex(material, eval, "_CombineMask", eval.CombineMaskTexture, textures);

        SetFloat(material, "_CombineOp", eval.CombineOp);
        SetFloat(material, "_AlphaOp", eval.AlphaOp);
        SetFloat(material, "_EnvMapType", eval.EnvMapType);
        bool separateOpacity = !string.IsNullOrEmpty(eval.OpacityTexture)
            && !string.Equals(eval.OpacityTexture, eval.DiffuseTexture, StringComparison.OrdinalIgnoreCase);
        SetFloat(material, "_HasOpacity", separateOpacity ? 1f : 0f);
        SetFloat(material, "_HasSpec", string.IsNullOrEmpty(eval.SpecularTexture) ? 0f : 1f);
        SetFloat(material, "_HasSpecMask", string.IsNullOrEmpty(eval.SpecularityMaskTexture) ? 0f : 1f);
        SetFloat(material, "_HasEmit", string.IsNullOrEmpty(eval.SelfIlluminationTexture) ? 0f : 1f);
        SetFloat(material, "_HasEmitMask", string.IsNullOrEmpty(eval.SelfIlluminationMaskTexture) ? 0f : 1f);
        SetFloat(material, "_HasDetail", string.IsNullOrEmpty(eval.DetailTexture) ? 0f : 1f);
        SetFloat(material, "_HasEnv", string.IsNullOrEmpty(eval.EnvTexture) ? 0f : 1f);
        SetFloat(material, "_HasCombine", eval.CombineOp >= 0 ? 1f : 0f);
        SetFloat(material, "_HasCombineMask", string.IsNullOrEmpty(eval.CombineMaskTexture) ? 0f : 1f);
        SetFloat(material, "_DetailScale", eval.DetailScale <= 0f ? 1f : eval.DetailScale);
        SetFloat(material, "_Modulate2X", eval.ModulateStaticLighting2X ? 1f : 0f);
        SetFloat(material, "_ModulateSpecular2X", eval.ModulateSpecular2X ? 1f : 0f);
        SetFloat(material, "_LightSpecular", eval.PerformLightingOnSpecularPass ? 1f : 0f);
        SetFloat(material, "_AlphaClip", eval.AlphaTest ? 1f : 0f);
        if (eval.AlphaTest)
        {
            float cutoff = eval.HasAlphaRef ? eval.AlphaRef / 255f : 0f;
            SetFloat(material, "_Cutoff", cutoff);
            if (!eval.HasAlphaRef)
                Debug.LogError("[Ue2Material] material " + eval.MaterialName + " op Shader property AlphaRef missing");
        }
        SetFloat(material, "_Cull", eval.TwoSided ? 0f : 2f);
        if (eval.HasZWrite)
            SetFloat(material, "_ZWrite", eval.ZWrite ? 1f : 0f);
        SetFloat(material, "_SrcBlend", eval.SrcBlend);
        SetFloat(material, "_DstBlend", eval.DstBlend);

        bool transparent = eval.SrcBlend != 1 || eval.DstBlend != 0;
        if (transparent)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else if (eval.AlphaTest)
        {
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        }

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", Color.white);
        ApplyUv(material, eval, time);
    }

    public static void ApplyUv(Material material, L2Ue2MaterialChain.Eval eval, float time)
    {
        if (material == null || eval == null)
            return;
        SetSt(material, "_BaseMap", eval.DiffuseUv, time);
        SetSt(material, "_MainTex", eval.DiffuseUv, time);
        SetSt(material, "_OpacityMap", eval.OpacityUv, time);
        SetSt(material, "_SpecularMap", eval.SpecularUv, time);
        SetSt(material, "_EmissionMap", eval.SelfIllumUv, time);
        SetSt(material, "_EnvMap", eval.SpecularUv, time);
        SetSt(material, "_CombineMap", eval.CombineUv, time);
    }

    public static string PickTexturePath(IReadOnlyList<string> names, IReadOnlyList<string> packages, IReadOnlyList<string> paths, string reference, List<string> errors, string material)
    {
        if (string.IsNullOrEmpty(reference) || names == null)
            return null;
        string package = L2Ue2MaterialChain.PackageName(reference);
        string shortName = L2Ue2MaterialChain.ShortName(reference);
        string fallback = null;
        int fallbackCount = 0;
        string packaged = null;
        int packagedCount = 0;
        int count = Math.Min(names.Count, Math.Min(packages.Count, paths.Count));
        for (int i = 0; i < count; i++)
        {
            if (!string.Equals(names[i], shortName, StringComparison.OrdinalIgnoreCase))
                continue;
            fallbackCount++;
            if (fallback == null)
                fallback = paths[i];
            if (package != null && string.Equals(packages[i], package, StringComparison.OrdinalIgnoreCase))
            {
                packagedCount++;
                if (packaged == null)
                    packaged = paths[i];
            }
        }
        if (fallbackCount == 0)
        {
            errors?.Add("[Ue2Material] material " + material + " texture " + reference + " missing");
            return null;
        }
        if (fallbackCount == 1)
            return fallback;
        if (package != null && packagedCount == 1)
            return packaged;
        errors?.Add("[Ue2Material] material " + material + " texture " + reference + " ambiguous");
        return null;
    }

    /// <summary>
    /// A material that already shows a texture from one package must not take a diffuse
    /// from another package. An empty current texture, or a texture the index does not
    /// know, does not contradict the props.
    /// </summary>
    public static bool TexturePackageAgrees(
        string currentTexture,
        string diffuseReference,
        IReadOnlyList<string> names,
        IReadOnlyList<string> packages,
        out string detail)
    {
        detail = null;
        if (string.IsNullOrEmpty(currentTexture) || string.IsNullOrEmpty(diffuseReference))
            return true;

        string currentShort = L2Ue2MaterialChain.ShortName(currentTexture);
        string diffuseShort = L2Ue2MaterialChain.ShortName(diffuseReference);
        if (string.Equals(currentShort, diffuseShort, StringComparison.OrdinalIgnoreCase))
            return true;

        string currentPackage = UniquePackage(names, packages, currentShort);
        string diffusePackage = L2Ue2MaterialChain.PackageName(diffuseReference);
        if (string.IsNullOrEmpty(diffusePackage))
            diffusePackage = UniquePackage(names, packages, diffuseShort);
        if (string.IsNullOrEmpty(currentPackage) || string.IsNullOrEmpty(diffusePackage))
            return true;
        if (string.Equals(currentPackage, diffusePackage, StringComparison.OrdinalIgnoreCase))
            return true;

        detail = "texture " + currentTexture + " package " + currentPackage
            + " vs props diffuse " + diffuseReference + " package " + diffusePackage;
        return false;
    }

    static string UniquePackage(IReadOnlyList<string> names, IReadOnlyList<string> packages, string shortName)
    {
        if (names == null || packages == null || string.IsNullOrEmpty(shortName))
            return null;
        string found = null;
        int count = Math.Min(names.Count, packages.Count);
        for (int i = 0; i < count; i++)
        {
            if (!string.Equals(names[i], shortName, StringComparison.OrdinalIgnoreCase))
                continue;
            if (found == null)
                found = packages[i];
            else if (!string.Equals(found, packages[i], StringComparison.OrdinalIgnoreCase))
                return null;
        }
        return found;
    }

    public static void LoadIndex(string tsv, List<string> names, List<string> packages, List<string> paths)
    {
        if (!File.Exists(tsv))
        {
            Debug.LogError("[Ue2Material] texture index missing " + tsv);
            return;
        }
        string[] lines = File.ReadAllLines(tsv);
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0 && lines[i].StartsWith("name\t", StringComparison.Ordinal))
                continue;
            string[] cells = lines[i].Split('\t');
            if (cells.Length < 3 || cells[0].Length == 0)
                continue;
            names.Add(cells[0]);
            packages.Add(cells[1]);
            paths.Add(cells[2]);
        }
    }

    static void SetTex(Material material, L2Ue2MaterialChain.Eval eval, string prop, string reference, Func<string, Texture> textures)
    {
        if (string.IsNullOrEmpty(reference) || textures == null || !material.HasProperty(prop))
            return;
        Texture tex = textures(reference);
        if (tex == null)
            return;
        material.SetTexture(prop, tex);
    }

    static void SetFloat(Material material, string prop, float value)
    {
        if (material.HasProperty(prop))
            material.SetFloat(prop, value);
    }

    static void SetSt(Material material, string textureProp, L2Ue2MaterialChain.UvMod uv, float time)
    {
        if (!material.HasProperty(textureProp))
            return;
        L2Ue2MaterialChain.UvSample sample = L2Ue2MaterialChain.SampleUv(uv, time);
        material.SetTextureScale(textureProp, new Vector2(sample.ScaleU, sample.ScaleV));
        material.SetTextureOffset(textureProp, new Vector2(sample.OffsetU, sample.OffsetV));
    }
}
