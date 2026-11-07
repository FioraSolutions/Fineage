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
    [MenuItem("L2/Characters/Migrate Race Materials (FDwarf)")] static void MenuFDwarf() => MigrateRace("FDwarf");
    [MenuItem("L2/Characters/Migrate Race Materials (MDwarf)")] static void MenuMDwarf() => MigrateRace("MDwarf");
    [MenuItem("L2/Characters/Migrate Race Materials (MOrc)")] static void MenuMOrc() => MigrateRace("MOrc");
    [MenuItem("L2/Characters/Migrate Race Materials (FOrc)")] static void MenuFOrc() => MigrateRace("FOrc");
    [MenuItem("L2/Characters/Fix Actor AlphaTest Body Mats (MOrc)")] static void MenuFixMOrcAlpha() => FixOpaqueBody("MOrc");
    [MenuItem("L2/Characters/Fix Actor AlphaTest Body Mats (FOrc)")] static void MenuFixFOrcAlpha() => FixOpaqueBody("FOrc");

    /// <summary>
    /// Body/armor mats already on L2ActorModulate2X but left with hair-style alpha test
    /// (queue 2450) — torso/legs vanish while gloves/boots (opaque) remain.
    /// </summary>
    public static int FixOpaqueBody(string race)
    {
        string folder = SysTex + race + "/Materials";
        if (!AssetDatabase.IsValidFolder(folder))
            return 0;
        int n = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            bool hair = name.Contains("_m00_bh") || name.Contains("_m00_ah")
                || name.EndsWith("_bh") || name.EndsWith("_ah");
            if (hair)
                continue;
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null || mat.shader.name.IndexOf("ActorModulate2X") < 0)
                continue;
            if (!mat.IsKeywordEnabled("_ALPHATEST_ON") && mat.renderQueue < 2400)
                continue;
            Apply(mat, mat.shader, hair: false);
            EditorUtility.SetDirty(mat);
            n++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[L2ActorRaceMaterials] {race}: opaque-fixed {n} body mats");
        return n;
    }

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

    public static void HideSkeletonPlaceholder(GameObject root)
    {
        if (root != null)
            L2ActorMeshPolicy.DisableSkeletonPlaceholders(root.transform);
    }
}
