using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Fixes white StaticMeshes after Interlude auto-bake.
/// Root cause: umodel exports textures/shaders to .work/umodel-export-XX_YY/&lt;pkg&gt;/{Texture,Shader},
/// but nothing copied them into Assets, so FBX import (materialSearch=Everywhere) found no matching
/// material and created empty URP Lit mats in StaticMeshes/&lt;pkg&gt;/Materials (_BaseMap = null => white).
/// This binder: 1) copies missing textures into Resources/Data/Textures/&lt;pkg&gt;,
/// 2) resolves each empty material via umodel *.props.txt (Shader/FinalBlend/... chain -> Diffuse texture)
/// or by name, 3) assigns _BaseMap/_MainTex (+ alpha clip / two-sided when the L2 shader says so).
/// Idempotent; only touches materials whose _BaseMap is empty.
/// </summary>
public static class L2InterludeMaterialBinder
{
    const string TexRoot = "Assets/Resources/Data/Textures";
    const string MeshRoot = "Assets/Resources/Data/StaticMeshes";
    static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

    [MenuItem("Shnok/0.1. [Interlude] Bind StaticMesh textures (fix white meshes)")]
    public static void BindAllMenu()
    {
        var log = new List<string>();
        Bind(null, log);
        foreach (var l in log) Debug.Log("[MatBinder] " + l);
    }

    /// <param name="exportRoots">umodel export dirs; null = all .work/umodel-export-*</param>
    public static void Bind(IEnumerable<string> exportRoots, List<string> log)
    {
        var roots = (exportRoots ?? Directory.GetDirectories(Path.Combine(RepoRoot, ".work"), "umodel-export-*"))
            .Where(Directory.Exists).ToList();

        int copied = CopyTextures(roots);
        log.Add("textures copied into Assets: " + copied);
        if (copied > 0) AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in roots)
            foreach (var f in Directory.GetFiles(r, "*.props.txt", SearchOption.AllDirectories))
                props[Path.GetFileName(f).Replace(".props.txt", "")] = f;

        var tex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var g in AssetDatabase.FindAssets("t:Texture2D", new[] { TexRoot }))
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            string n = Path.GetFileNameWithoutExtension(p);
            if (!tex.TryGetValue(n, out var l)) tex[n] = l = new List<string>();
            l.Add(p);
        }

        int fixedN = 0, missing = 0;
        var missingNames = new List<string>();
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot, TexRoot }))
            {
                string mp = AssetDatabase.GUIDToAssetPath(g);
                var m = AssetDatabase.LoadAssetAtPath<Material>(mp);
                if (m == null) continue;
                bool hasBase = m.HasProperty("_BaseMap") && m.GetTexture("_BaseMap") != null;
                bool hasMain = m.HasProperty("_MainTex") && m.GetTexture("_MainTex") != null;
                if (hasBase || hasMain) continue;
                if (m.shader != null && m.shader.name != "Universal Render Pipeline/Lit" && !m.shader.name.StartsWith("Hidden/") && m.shader.name != "Standard") continue; // custom shaders (water etc.)

                var info = Resolve(m.name, props, tex, 0);
                string texPath = Pick(info.tex, tex, mp);
                if (texPath == null) { missing++; if (missingNames.Count < 40) missingNames.Add(m.name); continue; }
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                if (lit != null && m.shader != lit) m.shader = lit;
                m.SetTexture("_BaseMap", t);
                m.SetTexture("_MainTex", t);
                m.SetColor("_BaseColor", Color.white);
                m.SetColor("_Color", Color.white);
                m.SetFloat("_Smoothness", 0f);
                m.SetFloat("_SpecularHighlights", 0f); m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                m.SetFloat("_EnvironmentReflections", 0f); m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                if (info.cutout)
                {
                    m.SetFloat("_AlphaClip", 1f);
                    m.SetFloat("_Cutoff", 0.5f);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    m.SetOverrideTag("RenderType", "TransparentCutout");
                }
                if (info.twoSided) { m.SetFloat("_Cull", 0f); m.doubleSidedGI = true; }
                EditorUtility.SetDirty(m);
                fixedN++;
            }
        }
        finally { AssetDatabase.StopAssetEditing(); }
        AssetDatabase.SaveAssets();
        log.Add("materials bound: " + fixedN + ", still untextured: " + missing +
                (missingNames.Count > 0 ? " (e.g. " + string.Join(", ", missingNames) + ")" : ""));
        log.Add("translucent (water) materials fixed: " + FixTranslucent(props));
    }

    [MenuItem("Shnok/0.2. [Interlude] Fix water / translucent StaticMesh materials")]
    public static void FixWaterMenu()
    {
        string repo = RepoRoot;
        var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in Directory.GetDirectories(Path.Combine(repo, ".work"), "umodel-export-*"))
            foreach (var f in Directory.GetFiles(r, "*.props.txt", SearchOption.AllDirectories))
                props[Path.GetFileName(f).Replace(".props.txt", "")] = f;
        Debug.Log("[MatBinder] translucent (water) materials fixed: " + FixTranslucent(props));
    }

    /// <summary>
    /// L2 water (Oren_HEV_S WaterShader01 / WaterShaderPan01, FX_E_S elf_water_01sh...) is a Shader with
    /// Opacity texture + ZWrite=false + AlphaTest=false, i.e. alpha-blended. The generic bind treated
    /// "Opacity=" as cutout (clip 0.5) and the water diffuse alpha is low => whole surface clipped => no water.
    /// Here: URP Lit Transparent, alpha from _BaseColor (texture alpha ignored), two-sided, glossy.
    /// </summary>
    static int FixTranslucent(Dictionary<string, string> props)
    {
        int n = 0;
        foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot }))
        {
            string mp = AssetDatabase.GUIDToAssetPath(g);
            var m = AssetDatabase.LoadAssetAtPath<Material>(mp);
            if (m == null || m.shader == null || m.shader.name != "Universal Render Pipeline/Lit") continue;
            if (!props.TryGetValue(m.name, out var pf)) continue;
            string txt = File.ReadAllText(pf);
            bool noZ = Regex.IsMatch(txt, @"^\s*ZWrite\s*=\s*false", RegexOptions.Multiline);
            bool alphaTest = Regex.IsMatch(txt, @"^\s*AlphaTest\s*=\s*true", RegexOptions.Multiline);
            bool opacity = Regex.IsMatch(txt, @"^\s*Opacity\s*=\s*\w+'", RegexOptions.Multiline);
            bool water = Regex.IsMatch(m.name, "water", RegexOptions.IgnoreCase);
            if (!(noZ && opacity && !alphaTest) && !(water && !alphaTest && opacity)) continue;
            if (m.GetFloat("_Surface") == 1f && m.renderQueue == (int)UnityEngine.Rendering.RenderQueue.Transparent) continue;

            m.SetFloat("_Surface", 1f);
            m.SetFloat("_Blend", 0f);
            m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetFloat("_SrcBlendAlpha", (float)UnityEngine.Rendering.BlendMode.One);
            m.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetFloat("_ZWrite", 0f);
            m.SetFloat("_AlphaClip", 0f);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.SetOverrideTag("RenderType", "Transparent");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            m.SetFloat("_Cull", 0f);
            m.SetColor("_BaseColor", new Color(0.75f, 0.85f, 0.9f, 0.7f));
            m.SetColor("_Color", new Color(0.75f, 0.85f, 0.9f, 0.7f));
            m.SetFloat("_Smoothness", 0.85f);
            m.SetFloat("_SpecularHighlights", 1f); m.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
            m.SetFloat("_EnvironmentReflections", 1f); m.DisableKeyword("_ENVIRONMENTREFLECTIONS_OFF");

            // water diffuse alpha is ~0 in the UT2 dumps; drop it so opacity comes from _BaseColor.a
            var t = m.GetTexture("_BaseMap");
            if (t != null && UnityEditor.AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t)) is TextureImporter ti &&
                ti.alphaSource != TextureImporterAlphaSource.None)
            {
                ti.alphaSource = TextureImporterAlphaSource.None;
                ti.SaveAndReimport();
            }
            EditorUtility.SetDirty(m);
            n++;
        }
        AssetDatabase.SaveAssets();
        return n;
    }

    struct Info { public string tex; public bool cutout, twoSided; }

    static readonly Regex Ref = new Regex(@"^\s*(Diffuse|Material|Texture|Material1|Material2|SelfIllumination|FallbackMaterial)\s*=\s*\w+'(?:[\w]+\.)*([\w]+)'", RegexOptions.Multiline);

    static Info Resolve(string name, Dictionary<string, string> props, Dictionary<string, List<string>> tex, int depth)
    {
        var info = new Info();
        if (depth < 6 && props.TryGetValue(name, out var pf))
        {
            string txt = File.ReadAllText(pf);
            info.cutout = Regex.IsMatch(txt, @"^\s*AlphaTest\s*=\s*true", RegexOptions.Multiline)
                          || Regex.IsMatch(txt, @"^\s*Opacity\s*=\s*\w+'", RegexOptions.Multiline)
                          || Regex.IsMatch(txt, @"^\s*bAlphaTest\s*=\s*true", RegexOptions.Multiline);
            info.twoSided = Regex.IsMatch(txt, @"^\s*(TwoSided|TreatAsTwoSided)\s*=\s*true", RegexOptions.Multiline);
            foreach (Match mm in Ref.Matches(txt))
            {
                string r = mm.Groups[2].Value;
                if (tex.ContainsKey(r)) { info.tex = r; return info; }
                if (!string.Equals(r, name, StringComparison.OrdinalIgnoreCase))
                {
                    var sub = Resolve(r, props, tex, depth + 1);
                    if (sub.tex != null) { info.tex = sub.tex; info.cutout |= sub.cutout; info.twoSided |= sub.twoSided; return info; }
                }
            }
        }
        if (tex.ContainsKey(name)) { info.tex = name; return info; }
        string stripped = Regex.Replace(name, @"(_\d+|_shader|_sh)$", "", RegexOptions.IgnoreCase);
        if (tex.ContainsKey(stripped)) info.tex = stripped;
        return info;
    }

    static string Pick(string name, Dictionary<string, List<string>> tex, string matPath)
    {
        if (name == null || !tex.TryGetValue(name, out var l)) return null;
        // prefer texture from the "sister" package (oren_hev_s -> oren_hev_t)
        string pkg = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(matPath))) ?? "";
        string stem = Regex.Replace(pkg, @"_[sS][xX]?$", "", RegexOptions.IgnoreCase);
        return l.FirstOrDefault(p => p.IndexOf("/" + stem, StringComparison.OrdinalIgnoreCase) >= 0) ?? l[0];
    }

    static int CopyTextures(List<string> roots)
    {
        string texAbs = Path.Combine(RepoRoot, "l2-unity", TexRoot);
        var existingDirs = Directory.GetDirectories(texAbs).ToDictionary(d => Path.GetFileName(d), d => d, StringComparer.OrdinalIgnoreCase);
        int n = 0;
        foreach (var r in roots)
            foreach (var pkgDir in Directory.GetDirectories(r))
            {
                string src = Path.Combine(pkgDir, "Texture");
                if (!Directory.Exists(src)) continue;
                string pkg = Path.GetFileName(pkgDir);
                if (!existingDirs.TryGetValue(pkg, out var dst)) { dst = Path.Combine(texAbs, pkg); Directory.CreateDirectory(dst); existingDirs[pkg] = dst; }
                var have = new HashSet<string>(Directory.GetFiles(dst, "*", SearchOption.AllDirectories)
                    .Where(f => !f.EndsWith(".meta")).Select(Path.GetFileNameWithoutExtension), StringComparer.OrdinalIgnoreCase);
                foreach (var f in Directory.GetFiles(src))
                {
                    string ext = Path.GetExtension(f).ToLowerInvariant();
                    if (ext != ".tga" && ext != ".png" && ext != ".dds") continue;
                    if (have.Contains(Path.GetFileNameWithoutExtension(f))) continue;
                    File.Copy(f, Path.Combine(dst, Path.GetFileName(f)));
                    have.Add(Path.GetFileNameWithoutExtension(f));
                    n++;
                }
            }
        return n;
    }
}
