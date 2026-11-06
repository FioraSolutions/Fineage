#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// EV guild/shop doorways: decorative Opacity *_shader mats + GDS_Ent MeshCollider on the punched arch.
/// Never strip GDS_Main — its MeshCollider is the terrace walk deck (strip → fall to Y~-65).
/// Never touch elf_vi_0002 / elf_vi_117 / elf_vi_111 — walk decks (Transparent/delete = floor holes).
/// Never delete upward faces from O_E_GDS_Ent — use tools/interlude/restore_gds_guild_floors.py if punched.
/// Menu: Shnok/[Debug] Unblock Elven doorways (mats + Ent colliders)
/// </summary>
public static class L2DoorwayUnblockCli
{
    static readonly string[] OpacityMatNames =
    {
        "elf_vi_034_shader",
        "elf_vi_025_shader",
        "elf_vi_026_shader",
        "efl_vi_022_shader",
    };

    static readonly string[] OpacityTexMetaGlobs =
    {
        "Assets/Resources/Data/Textures/oren_hev_t/elf_vi_002.png.meta",
        "Assets/Resources/Data/Textures/oren_hev_t/elf_vi_034.png.meta",
        "Assets/Resources/Data/Textures/oren_hev_t/elf_vi_025.png.meta",
        "Assets/Resources/Data/Textures/oren_hev_t/elf_vi_026.png.meta",
    };

    [MenuItem("Shnok/[Debug] Unblock Elven doorways (mats + Ent colliders)")]
    public static void Menu() => Run();

    public static void RunCli()
    {
        bool ok = Run();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static bool Run()
    {
        int texN = FixTextureAlphaMeta();
        int matN = FixOpacityMaterials();
        int stripped = StripGdsEntCollidersOnly();
        int restored = EnsureGdsMainColliders();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[DoorwayUnblock] texMeta=" + texN + " materials=" + matN
            + " GDS_Ent stripped prefabs=" + stripped
            + " GDS_Main colliders restored=" + restored);
        return true;
    }

    static int FixTextureAlphaMeta()
    {
        int n = 0;
        foreach (var metaPath in OpacityTexMetaGlobs)
        {
            if (!File.Exists(metaPath)) continue;
            string text = File.ReadAllText(metaPath);
            string next = text.Replace("alphaIsTransparency: 0", "alphaIsTransparency: 1");
            if (next == text) continue;
            File.WriteAllText(metaPath, next);
            n++;
        }
        if (n > 0) AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        return n;
    }

    static int FixOpacityMaterials()
    {
        int n = 0;
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        foreach (var name in OpacityMatNames)
        {
            foreach (var g in AssetDatabase.FindAssets(name + " t:Material"))
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                if (!string.Equals(Path.GetFileNameWithoutExtension(path), name, System.StringComparison.OrdinalIgnoreCase))
                    continue;
                var m = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (m == null) continue;
                if (lit != null && m.shader != lit) m.shader = lit;

                // Filigree alpha is binary; AlphaClip avoids URP Premultiply black curtains.
                m.SetFloat("_Surface", 0f);
                m.SetFloat("_Blend", 0f);
                if (m.HasProperty("_BlendModePreserveSpecular")) m.SetFloat("_BlendModePreserveSpecular", 0f);
                m.SetFloat("_AlphaClip", 1f);
                m.SetFloat("_Cutoff", 0.5f);
                m.EnableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.DisableKeyword("_ALPHAMODULATE_ON");
                m.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.EnableKeyword("_SURFACE_TYPE_OPAQUE");
                m.SetOverrideTag("RenderType", "TransparentCutout");
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.SetInt("_ZWrite", 1);
                m.SetFloat("_Cull", 0f);
                m.doubleSidedGI = true;
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                if (m.HasProperty("_BaseColor"))
                {
                    Color c = m.GetColor("_BaseColor");
                    c.a = 1f;
                    m.SetColor("_BaseColor", c);
                }
                EditorUtility.SetDirty(m);
                n++;
            }
        }
        return n;
    }

    /// <summary>
    /// Only GDS_Ent: punched arch MeshCollider hitch-blocks the capsule. GDS_Main must keep its collider
    /// (terrace walk deck).
    /// </summary>
    static int StripGdsEntCollidersOnly()
    {
        int prefabs = 0;
        foreach (var d in Directory.GetDirectories("Assets/Resources/Data/Maps"))
        {
            string p = Path.Combine(d, "StaticMeshes.prefab").Replace('\\', '/');
            if (!File.Exists(p)) continue;
            var root = PrefabUtility.LoadPrefabContents(p);
            bool dirty = false;
            try
            {
                foreach (var mc in root.GetComponentsInChildren<MeshCollider>(true))
                {
                    if (mc == null) continue;
                    string path = PathOf(mc.transform);
                    string n = mc.gameObject.name;
                    bool ent = path.IndexOf("GDS_Ent", System.StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("GDS_Ent", System.StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!ent) continue;
                    Object.DestroyImmediate(mc);
                    dirty = true;
                }
                if (dirty)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, p);
                    prefabs++;
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        return prefabs;
    }

    /// <summary>
    /// Re-add MeshCollider on GDS_Main after earlier mistaken strip (player fell to Y~-65).
    /// </summary>
    static int EnsureGdsMainColliders()
    {
        int layer = LayerMask.NameToLayer("StaticMesh");
        int added = 0;
        foreach (var d in Directory.GetDirectories("Assets/Resources/Data/Maps"))
        {
            string p = Path.Combine(d, "StaticMeshes.prefab").Replace('\\', '/');
            if (!File.Exists(p)) continue;
            var root = PrefabUtility.LoadPrefabContents(p);
            bool dirty = false;
            try
            {
                foreach (var mf in root.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (mf == null || mf.sharedMesh == null) continue;
                    string path = PathOf(mf.transform);
                    string n = mf.gameObject.name;
                    bool main = path.IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!main) continue;
                    if (layer >= 0) mf.gameObject.layer = layer;
                    var mc = mf.GetComponent<MeshCollider>();
                    if (mc == null)
                    {
                        mc = mf.gameObject.AddComponent<MeshCollider>();
                        added++;
                        dirty = true;
                    }
                    if (mc.sharedMesh != mf.sharedMesh)
                    {
                        mc.sharedMesh = mf.sharedMesh;
                        dirty = true;
                    }
                }
                if (dirty) PrefabUtility.SaveAsPrefabAsset(root, p);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        return added;
    }

    static string PathOf(Transform t)
    {
        string s = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            s = t.name + "/" + s;
        }
        return s;
    }
}
#endif
