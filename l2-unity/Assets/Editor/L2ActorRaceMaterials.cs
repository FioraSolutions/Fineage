using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Character (race) material migration: body, armor, face and hair go on the client actor shader
// L2/World/ActorModulate2X, same settings as the working MMagic materials:
//   body/armor/face: Cull None, opaque, queue 2000, no alpha test
//   hair (*_m00_ah / *_m00_bh): Cull None, alpha test GREATER 127 (cutoff 0.498039216), queue 2450
// URP Lit (PBR + self shadows) and the hair shader graph caused the dark neck / dark under-layer / dark hair double.
// Only URP Lit and Shader Graph materials are converted; client env/specular/add shaders are left alone.
// Usage: menu L2/Characters/Migrate Race Materials (selected folder), or
//   -executeMethod L2ActorRaceMaterials.RunCli -race MElf [-race FElf ...]
public static class L2ActorRaceMaterials
{
    public const string ShaderPath = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";
    const string SysTex = "Assets/Resources/Data/SysTextures/";

    public static void RunCli()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == "-race") MigrateRace(args[i + 1]);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("L2/Characters/Migrate Race Materials (MElf)")] static void MenuMElf() => MigrateRace("MElf");
    [MenuItem("L2/Characters/Migrate Race Materials (FElf)")] static void MenuFElf() => MigrateRace("FElf");

    public static int MigrateRace(string race)
    {
        int n = MigrateFolder(SysTex + race + "/Materials", null);
        n += MigrateFolder(SysTex + "LineageFaceTex/Materials", race + "_");
        AssetDatabase.SaveAssets();
        Debug.Log($"[L2ActorRaceMaterials] {race}: converted {n} materials");
        return n;
    }

    public static int MigrateFolder(string folder, string namePrefix)
    {
        if (!AssetDatabase.IsValidFolder(folder)) return 0;
        Shader actor = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        if (actor == null) { Debug.LogError("[L2ActorRaceMaterials] missing " + ShaderPath); return 0; }
        int n = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            if (namePrefix != null && !name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase)) continue;
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || !NeedsMigration(mat)) continue;
            Apply(mat, actor, name.Contains("_m00_"));
            EditorUtility.SetDirty(mat);
            n++;
        }
        return n;
    }

    static bool NeedsMigration(Material m)
    {
        if (m.shader == null) return true;
        string s = m.shader.name;
        if (s == "Universal Render Pipeline/Lit" || s == "Standard" || s.StartsWith("Shader Graphs/")) return true;
        string p = AssetDatabase.GetAssetPath(m.shader);
        return p != null && p.EndsWith(".shadergraph");
    }

    public static void Apply(Material mat, Shader actor, bool hair)
    {
        Texture tex = mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : null;
        if (tex == null && mat.HasProperty("_MainTex")) tex = mat.GetTexture("_MainTex");
        if (tex == null) foreach (string p in mat.GetTexturePropertyNames()) { tex = mat.GetTexture(p); if (tex != null) break; }

        mat.shader = actor;
        mat.shaderKeywords = new string[0];
        mat.SetTexture("_BaseMap", tex);
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_BaseColor", Color.white);
        mat.SetFloat("_Cull", 0f);
        mat.SetFloat("_ActorAlpha", 1f);
        mat.SetFloat("_Cutoff", 0.498039216f);
        mat.doubleSidedGI = true;
        if (hair)
        {
            mat.SetFloat("_AlphaClip", 1f);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = 2450;
        }
        else
        {
            mat.SetFloat("_AlphaClip", 0f);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = 2000;
        }
    }

    // *_m000_b.mo in the anim skeleton is only a bone holder (bare feet, untextured material_0).
    // The client draws the equipped boots instead; leaving it on shows white feet through sandals/soles.
    public static void HideSkeletonPlaceholder(GameObject root)
    {
        foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            if (smr.gameObject.name.EndsWith("_m000_b.mo", StringComparison.OrdinalIgnoreCase)) smr.enabled = false;
    }
}
