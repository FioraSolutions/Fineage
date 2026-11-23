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
        BuildChainIndex(tex);

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

                var ue2 = L2Ue2ShaderProps.Analyze(m.name, props);
                if (ue2.mode == L2Ue2ShaderProps.RenderMode.HiddenCollisionOnly)
                {
                    ApplyCollisionOnly(m);
                    fixedN++;
                    continue;
                }
                if (ue2.mode == L2Ue2ShaderProps.RenderMode.Unknown && !string.IsNullOrEmpty(ue2.detail))
                    Debug.LogError("[MatBinder] " + m.name + ": " + ue2.detail);

                var info = Resolve(m.name, props, tex, 0);
                info.cutout |= ue2.mode == L2Ue2ShaderProps.RenderMode.MaskedCutout && ue2.alphaTest;
                info.twoSided |= ue2.twoSided;
                string texPath = Pick(info.tex ?? ue2.resolvedDiffuseTexture, tex, mp);
                if (texPath == null) {
                    if (ue2.mode != L2Ue2ShaderProps.RenderMode.HiddenCollisionOnly) {
                        missing++;
                        if (missingNames.Count < 40) missingNames.Add(m.name);
                        Debug.LogError("[MatBinder] missing diffuse for material " + m.name + " (mesh " + mp + ")");
                    }
                    continue;
                }
                var t = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                if (lit != null && m.shader != lit) m.shader = lit;
                m.SetTexture("_BaseMap", t);
                m.SetTexture("_MainTex", t);
                m.SetColor("_BaseColor", Color.white);
                m.SetColor("_Color", Color.white);
                m.SetFloat("_Smoothness", 0f);
                m.SetFloat("_SpecularHighlights", 0f); m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                m.SetFloat("_EnvironmentReflections", 0f); m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
                if (info.cutout || ue2.alphaTest)
                {
                    m.SetFloat("_AlphaClip", 1f);
                    m.SetFloat("_Cutoff", ue2.alphaRef > 0 ? ue2.alphaRef / 255f : 0.5f);
                    m.EnableKeyword("_ALPHATEST_ON");
                    m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    m.SetOverrideTag("RenderType", "TransparentCutout");
                }
                if (info.twoSided) { m.SetFloat("_Cull", 0f); m.doubleSidedGI = true; }
                if (props.ContainsKey(m.name))
                {
                    L2Ue2MaterialChain.Eval chain = L2Ue2MaterialChain.Evaluate(m.name, props);
                    L2Ue2MaterialApply.Apply(m, chain, 0f, reference => LoadChainTexture(reference, tex, m.name));
                }
                EditorUtility.SetDirty(m);
                fixedN++;
            }
        }
        finally { AssetDatabase.StopAssetEditing(); }
        AssetDatabase.SaveAssets();
        log.Add("materials bound: " + fixedN + ", still untextured: " + missing +
                (missingNames.Count > 0 ? " (e.g. " + string.Join(", ", missingNames) + ")" : ""));
        log.Add("translucent (water) materials fixed: " + FixTranslucent(props));
        log.Add("SpeedTree->Lit opaque shader mats fixed: " + FixSpeedTreeOpaque(props));
        log.Add("collision-only shader mats: " + FixCollisionOnlyFromProps(props));
        log.Add(L2InterludeStaticMeshColliders.Run(null));
        log.Add(L2InterludeWaterVolumes.Run(null));
        log.Add("two-sided cull from original props: " + ApplySharedTwoSided(props));
    }

    /// <summary>
    /// Textured materials used to skip the binder, so TwoSided stayed at the importer's Cull Back.
    /// Every material with props takes the shared cull rule, on every map.
    /// </summary>
    static int ApplySharedTwoSided(Dictionary<string, string> props)
    {
        int n = 0;
        foreach (string g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot, TexRoot }))
        {
            string mp = AssetDatabase.GUIDToAssetPath(g);
            Material m = AssetDatabase.LoadAssetAtPath<Material>(mp);
            if (m == null || !props.ContainsKey(m.name))
                continue;
            L2Ue2ShaderProps.PropsInfo info = L2Ue2ShaderProps.Analyze(m.name, props);
            if (!string.IsNullOrEmpty(info.detail) && info.detail.StartsWith("missing props", StringComparison.Ordinal))
                continue;
            float have = m.HasProperty("_Cull") ? m.GetFloat("_Cull") : -1f;
            float want = L2TwoSidedMaterials.CullFor(info.twoSided);
            if (Mathf.Abs(have - want) < 0.01f && m.doubleSidedGI == info.twoSided)
                continue;
            if (!L2TwoSidedMaterials.Apply(m, info.twoSided, m.name))
                continue;
            EditorUtility.SetDirty(m);
            n++;
        }
        if (n > 0)
            AssetDatabase.SaveAssets();
        return n;
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
        Debug.Log("[MatBinder] SpeedTree->Lit opaque shader mats fixed: " + FixSpeedTreeOpaque(props));
        Debug.Log("[MatBinder] collision-only shader mats: " + FixCollisionOnlyFromProps(props));
        Debug.Log("[Water] " + L2InterludeWaterVolumes.Run(null));
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
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        Dictionary<string, List<string>> tex = null;
        foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot }))
        {
            string mp = AssetDatabase.GUIDToAssetPath(g);
            var m = AssetDatabase.LoadAssetAtPath<Material>(mp);
            if (m == null || m.shader == null) continue;
            if (!props.TryGetValue(m.name, out var pf)) continue;
            bool isLit = m.shader.name == "Universal Render Pipeline/Lit";
            // O_E_Water's FBX slot is "waterfall001_shader" -> pre-existing field_deco_S mat on custom
            // TextureAnimAlpha shadergraph with empty _FramesArray => invisible. Convert such water mats to Lit.
            bool emptyCustom = !isLit && Regex.IsMatch(m.name, "water", RegexOptions.IgnoreCase)
                && (!m.HasProperty("_FramesArray") || m.GetTexture("_FramesArray") == null)
                && (!m.HasProperty("_BaseMap") || m.GetTexture("_BaseMap") == null);
            if (!isLit && !emptyCustom) continue;
            string txt = File.ReadAllText(pf);
            bool noZ = Regex.IsMatch(txt, @"^\s*ZWrite\s*=\s*false", RegexOptions.Multiline);
            bool alphaTest = Regex.IsMatch(txt, @"^\s*AlphaTest\s*=\s*true", RegexOptions.Multiline);
            bool opacity = Regex.IsMatch(txt, @"^\s*Opacity\s*=\s*\w+'", RegexOptions.Multiline);
            bool water = Regex.IsMatch(m.name, "water", RegexOptions.IgnoreCase);
            if (!(noZ && opacity && !alphaTest) && !(water && !alphaTest && (opacity || noZ))) continue;
            if (isLit && m.GetFloat("_Surface") == 1f && m.renderQueue == (int)UnityEngine.Rendering.RenderQueue.Transparent) continue;
            if (!isLit)
            {
                if (lit == null) continue;
                if (tex == null)
                {
                    tex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    foreach (var tg in AssetDatabase.FindAssets("t:Texture2D", new[] { TexRoot }))
                    {
                        string tp = AssetDatabase.GUIDToAssetPath(tg);
                        string tn = Path.GetFileNameWithoutExtension(tp);
                        if (!tex.TryGetValue(tn, out var tl)) tex[tn] = tl = new List<string>();
                        tl.Add(tp);
                    }
                }
                string texPath = Pick(Resolve(m.name, props, tex, 0).tex, tex, mp);
                m.shader = lit;
                if (texPath != null)
                {
                    var wt = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                    m.SetTexture("_BaseMap", wt);
                    m.SetTexture("_MainTex", wt);
                }
            }

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
            m.SetColor("_BaseColor", new Color(0.85f, 0.95f, 1f, 0.55f));
            m.SetColor("_Color", new Color(0.85f, 0.95f, 1f, 0.55f));
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

    /// <summary>
    /// UE Shader mats (e.g. oren_hev_t elf_vi_0002 = Elven Village stair/bridge deck) were imported onto
    /// URP SpeedTree8 shadergraph (meant for leaves). On a plain StaticMesh its wind/billboard vertex path
    /// collapses the geometry => deck invisible while railings (Lit) render. Here: any SpeedTree mat whose
    /// L2 Shader props say AlphaTest=false + ZWrite=true (solid surface, not foliage/water) -> URP Lit opaque
    /// (alpha clip only if the diffuse texture actually has alpha), two-sided per TwoSided.
    /// </summary>
    static int FixSpeedTreeOpaque(Dictionary<string, string> props)
    {
        int n = 0;
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) return 0;
        Dictionary<string, List<string>> tex = null;
        foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot, TexRoot }))
        {
            string mp = AssetDatabase.GUIDToAssetPath(g);
            var m = AssetDatabase.LoadAssetAtPath<Material>(mp);
            if (m == null || m.shader == null || m.shader.name.IndexOf("SpeedTree", StringComparison.OrdinalIgnoreCase) < 0) continue;
            if (Regex.IsMatch(m.name, "water|leaf|leaves|grass|bush", RegexOptions.IgnoreCase)) continue;
            if (!props.TryGetValue(m.name, out var pf)) continue;
            string txt = File.ReadAllText(pf);
            var ue2 = L2Ue2ShaderProps.Analyze(m.name, props);
            if (ue2.mode == L2Ue2ShaderProps.RenderMode.MaskedCutout)
            {
                if (tex == null)
                {
                    tex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    foreach (var tg in AssetDatabase.FindAssets("t:Texture2D", new[] { TexRoot }))
                    {
                        string tp = AssetDatabase.GUIDToAssetPath(tg);
                        string tn = Path.GetFileNameWithoutExtension(tp);
                        if (!tex.TryGetValue(tn, out var tl)) tex[tn] = tl = new List<string>();
                        tl.Add(tp);
                    }
                }
                // Bridge railings / grates: AlphaTest in L2 → URP cutout (do not force opaque).
                string cutoutTexPath = Pick(Resolve(m.name, props, tex, 0).tex, tex, mp);
                if (lit != null) m.shader = lit;
                if (cutoutTexPath != null)
                {
                    var cutTex = AssetDatabase.LoadAssetAtPath<Texture2D>(cutoutTexPath);
                    m.SetTexture("_BaseMap", cutTex);
                    m.SetTexture("_MainTex", cutTex);
                }
                m.SetFloat("_AlphaClip", 1f);
                m.SetFloat("_Cutoff", ue2.alphaRef > 0 ? ue2.alphaRef / 255f : 0.5f);
                m.EnableKeyword("_ALPHATEST_ON");
                m.SetFloat("_Cull", ue2.twoSided ? 0f : 2f);
                m.doubleSidedGI = ue2.twoSided;
                EditorUtility.SetDirty(m);
                n++;
                continue;
            }
            if (!Regex.IsMatch(txt, @"^\s*AlphaTest\s*=\s*false", RegexOptions.Multiline)) continue;
            if (!Regex.IsMatch(txt, @"^\s*ZWrite\s*=\s*true", RegexOptions.Multiline)) continue;
            if (tex == null)
            {
                tex = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (var tg in AssetDatabase.FindAssets("t:Texture2D", new[] { TexRoot }))
                {
                    string tp = AssetDatabase.GUIDToAssetPath(tg);
                    string tn = Path.GetFileNameWithoutExtension(tp);
                    if (!tex.TryGetValue(tn, out var tl)) tex[tn] = tl = new List<string>();
                    tl.Add(tp);
                }
            }
            var info = Resolve(m.name, props, tex, 0);
            Texture t = null;
            string texPath = Pick(info.tex, tex, mp);
            if (texPath != null) t = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (t == null && m.HasProperty("_MainTex")) t = m.GetTexture("_MainTex");
            if (t == null) continue;

            m.shader = lit;
            m.shaderKeywords = new string[0];
            m.SetTexture("_BaseMap", t);
            m.SetTexture("_MainTex", t);
            m.SetColor("_BaseColor", Color.white);
            m.SetColor("_Color", Color.white);
            m.SetFloat("_Surface", 0f);
            m.SetFloat("_ZWrite", 1f);
            m.SetFloat("_SrcBlend", 1f); m.SetFloat("_DstBlend", 0f);
            m.SetFloat("_Smoothness", 0f);
            m.SetFloat("_SpecularHighlights", 0f); m.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
            m.SetFloat("_EnvironmentReflections", 0f); m.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
            // AlphaTest=false in L2 => solid: never alpha-clip (diffuse alpha is garbage on these, clipped the whole surface)
            m.SetFloat("_AlphaClip", 0f); m.DisableKeyword("_ALPHATEST_ON");
            m.SetOverrideTag("RenderType", "Opaque");
            m.renderQueue = -1;
            bool two = Regex.IsMatch(txt, @"^\s*(TwoSided|TreatAsTwoSided)\s*=\s*true", RegexOptions.Multiline);
            m.SetFloat("_Cull", two ? 0f : 2f);
            m.doubleSidedGI = two;
            EditorUtility.SetDirty(m);
            n++;
        }
        AssetDatabase.SaveAssets();
        return n;
    }

    static Shader _collisionOnly;

    static void ApplyCollisionOnly(Material m)
    {
        if (_collisionOnly == null) _collisionOnly = Shader.Find("L2/World/CollisionOnly");
        if (_collisionOnly == null)
        {
            Debug.LogError("[MatBinder] L2/World/CollisionOnly shader missing; cannot hide collision-only material " + m.name);
            return;
        }
        m.shader = _collisionOnly;
        m.SetOverrideTag("RenderType", "Opaque");
        EditorUtility.SetDirty(m);
    }

    static int FixCollisionOnlyFromProps(Dictionary<string, string> props)
    {
        int n = 0;
        foreach (var g in AssetDatabase.FindAssets("t:Material", new[] { MeshRoot }))
        {
            string mp = AssetDatabase.GUIDToAssetPath(g);
            var m = AssetDatabase.LoadAssetAtPath<Material>(mp);
            if (m == null) continue;
            var ue2 = L2Ue2ShaderProps.Analyze(m.name, props);
            if (ue2.mode != L2Ue2ShaderProps.RenderMode.HiddenCollisionOnly) continue;
            if (m.shader != null && m.shader.name == "L2/World/CollisionOnly") continue;
            ApplyCollisionOnly(m);
            n++;
        }
        AssetDatabase.SaveAssets();
        return n;
    }

    static readonly Dictionary<string, string> ChainAssets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    static readonly List<string> ChainNames = new List<string>();
    static readonly List<string> ChainPackages = new List<string>();
    static readonly List<string> ChainResources = new List<string>();

    static void BuildChainIndex(Dictionary<string, List<string>> tex)
    {
        ChainAssets.Clear();
        ChainNames.Clear();
        ChainPackages.Clear();
        ChainResources.Clear();
        foreach (var kv in tex)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                string assetPath = kv.Value[i].Replace('\\', '/');
                const string marker = "/Textures/";
                int at = assetPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (at < 0)
                    continue;
                string rel = assetPath.Substring(at + marker.Length);
                int slash = rel.IndexOf('/');
                string package = slash > 0 ? rel.Substring(0, slash) : "";
                string noExt = Path.ChangeExtension(rel, null).Replace('\\', '/');
                string resource = "Data/Textures/" + noExt;
                ChainNames.Add(kv.Key);
                ChainPackages.Add(package);
                ChainResources.Add(resource);
                if (!ChainAssets.ContainsKey(resource))
                    ChainAssets.Add(resource, assetPath);
            }
        }
    }

    static Texture LoadChainTexture(string reference, Dictionary<string, List<string>> tex, string material)
    {
        if (ChainNames.Count == 0)
            BuildChainIndex(tex);
        var errors = new List<string>();
        string resource = L2Ue2MaterialApply.PickTexturePath(ChainNames, ChainPackages, ChainResources, reference, errors, material);
        for (int i = 0; i < errors.Count; i++)
            Debug.LogError(errors[i]);
        if (resource == null)
            return null;
        if (!ChainAssets.TryGetValue(resource, out string assetPath))
        {
            Debug.LogError("[Ue2Material] material " + material + " texture " + reference + " missing");
            return null;
        }
        return AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
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
