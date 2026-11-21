#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// After LineageWeapons FBX import, bake loadable prefabs like the handmade swords:
/// mesh + L2/World/ActorEnvSpecular on the root, Sword_Tip / Sword_Base children. Does not overwrite existing prefabs.
/// </summary>
public class L2WeaponPrefabBuilder : AssetPostprocessor
{
    const string ModelsFolder = "Assets/Resources/Data/Animations/LineageWeapons/Models";
    const string PrefabFolder = "Assets/Resources/Data/Animations/LineageWeapons";
    const string TexFolder = "Assets/Resources/Data/SysTextures/LineageWeaponsTex";
    const string MatFolder = TexFolder + "/Materials";
    static readonly string[] TexMapPaths =
    {
        "Assets/Scripts/Tools/Terrain/Blender/sword_tex_map.txt",
        "Assets/Scripts/Tools/Terrain/Blender/weapon_tex_map.txt",
    };

    static bool _queued;

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        for (int i = 0; i < importedAssets.Length; i++)
        {
            string path = importedAssets[i].Replace('\\', '/');
            if (!path.StartsWith(ModelsFolder) || !path.EndsWith(".fbx"))
                continue;
            QueueBuild();
            return;
        }
    }

    [MenuItem("L2/Weapons/Build Missing Weapon Prefabs")]
    public static void BuildAllMenu()
    {
        int n = BuildMissingPrefabs();
        Debug.Log("[L2WeaponPrefabBuilder] built " + n + " prefabs");
    }

    public static void BuildAll()
    {
        BuildMissingPrefabs();
    }

    static void QueueBuild()
    {
        if (_queued)
            return;
        _queued = true;
        EditorApplication.delayCall += () =>
        {
            _queued = false;
            BuildMissingPrefabs();
        };
    }

    /// <summary>Rebuild the loadable prefab for one LineageWeapons mesh after FBX repair.</summary>
    public static bool TryRebuildPrefabForName(string meshName)
    {
        if (string.IsNullOrEmpty(meshName))
            return false;
        string fbxPath = ModelsFolder + "/" + meshName + ".fbx";
        string prefabPath = PrefabFolder + "/" + meshName + ".prefab";
        Dictionary<string, string> texMap = LoadTexMap();
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            AssetDatabase.DeleteAsset(prefabPath);
        return TryBuildPrefab(fbxPath, prefabPath, meshName, texMap);
    }

    static Mesh EnsureVolumetricSourceMesh(Mesh source, string fbxPath)
    {
        WeaponMeshTopology.Report report = WeaponMeshTopology.Analyze(source, source.name);
        if (!report.IsDegenerate)
            return source;
        Mesh fixedMesh = WeaponMeshTopology.BuildSolidifiedMesh(source);
        if (fixedMesh == null)
            return source;
        Debug.LogWarning(
            "[L2WeaponPrefabBuilder] weapon FBX '"
            + fbxPath
            + "' is single-sided ("
            + report.FailureReason
            + "). Using in-editor solidify; rebake with tools/interlude/weapon_mesh_solidify.py --fix");
        return fixedMesh;
    }

    public static int BuildMissingPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(ModelsFolder) || !AssetDatabase.IsValidFolder(PrefabFolder))
            return 0;

        Dictionary<string, string> texMap = LoadTexMap();
        string[] fbxGuids = AssetDatabase.FindAssets("t:Model", new[] { ModelsFolder });
        int built = 0;
        for (int i = 0; i < fbxGuids.Length; i++)
        {
            string fbxPath = AssetDatabase.GUIDToAssetPath(fbxGuids[i]);
            if (!fbxPath.EndsWith(".fbx"))
                continue;
            string name = Path.GetFileNameWithoutExtension(fbxPath);
            string prefabPath = PrefabFolder + "/" + name + ".prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                continue;
            if (TryBuildPrefab(fbxPath, prefabPath, name, texMap))
                built++;
        }

        if (built > 0)
            AssetDatabase.SaveAssets();
        return built;
    }

    static bool TryBuildPrefab(
        string fbxPath,
        string prefabPath,
        string name,
        Dictionary<string, string> texMap)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (model == null)
        {
            Debug.LogWarning("[L2WeaponPrefabBuilder] no model at " + fbxPath);
            return false;
        }

        GameObject src = Object.Instantiate(model);
        src.name = name;
        MeshFilter srcFilter = src.GetComponentInChildren<MeshFilter>();
        if (srcFilter == null || srcFilter.sharedMesh == null)
        {
            Object.DestroyImmediate(src);
            Debug.LogError("[L2WeaponPrefabBuilder] no mesh in " + fbxPath);
            return false;
        }

        Mesh weaponMesh = EnsureVolumetricSourceMesh(srcFilter.sharedMesh, fbxPath);

        GameObject go = new GameObject(name);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mf.sharedMesh = weaponMesh;
        // Shields (and other multi-slot weapons) ship t00;t01 — assign every submesh slot.
        mr.sharedMaterials = GetOrCreateMaterials(name, srcFilter.sharedMesh, texMap);

        Transform srcMeshXf = srcFilter.transform;
        Transform tip = CopyOrCreateMarker(go.transform, src.transform, srcMeshXf, "Sword_Tip", true);
        Transform bladeBase = CopyOrCreateMarker(go.transform, src.transform, srcMeshXf, "Sword_Base", false);
        EnsureTipIsBladeEnd(tip, bladeBase);

        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(src);
        Debug.Log("[L2WeaponPrefabBuilder] " + prefabPath);
        return true;
    }

    static Transform CopyOrCreateMarker(
        Transform destRoot,
        Transform srcRoot,
        Transform srcMeshXf,
        string markerName,
        bool isTip)
    {
        Transform existing = FindNamed(srcRoot, markerName);
        GameObject marker = new GameObject(markerName);
        marker.transform.SetParent(destRoot, false);
        if (existing != null)
        {
            marker.transform.localPosition = srcMeshXf.InverseTransformPoint(existing.position);
            return marker.transform;
        }

        MeshFilter mf = destRoot.GetComponent<MeshFilter>();
        Mesh mesh = mf != null ? mf.sharedMesh : null;
        if (mesh == null)
            return marker.transform;

        Bounds b = mesh.bounds;
        Vector3 size = b.size;
        Vector3 min;
        Vector3 max;
        if (size.x >= size.y && size.x >= size.z)
        {
            min = new Vector3(b.min.x, b.center.y, b.center.z);
            max = new Vector3(b.max.x, b.center.y, b.center.z);
        }
        else if (size.z >= size.x && size.z >= size.y)
        {
            min = new Vector3(b.center.x, b.center.y, b.min.z);
            max = new Vector3(b.center.x, b.center.y, b.max.z);
        }
        else
        {
            min = new Vector3(b.center.x, b.min.y, b.center.z);
            max = new Vector3(b.center.x, b.max.y, b.center.z);
        }

        // Handmade swords: Sword_Tip = more negative X (blade) after FBX import.
        marker.transform.localPosition = isTip ? min : max;
        return marker.transform;
    }

    static void EnsureTipIsBladeEnd(Transform tip, Transform bladeBase)
    {
        if (tip == null || bladeBase == null)
            return;
        Vector3 t = tip.localPosition;
        Vector3 b = bladeBase.localPosition;
        Vector3 delta = t - b;
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y) && Mathf.Abs(delta.x) >= Mathf.Abs(delta.z))
        {
            if (t.x > b.x)
            {
                tip.localPosition = b;
                bladeBase.localPosition = t;
            }
            return;
        }

        if (t.sqrMagnitude < b.sqrMagnitude)
        {
            tip.localPosition = b;
            bladeBase.localPosition = t;
        }
    }

    static Transform FindNamed(Transform root, string name)
    {
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindNamed(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// One material per mesh submesh. Slot 0 → t00, slot 1 → t01, … (Weapongrp texture={t00;t01}).
    /// </summary>
    static Material[] GetOrCreateMaterials(string meshName, Mesh mesh, Dictionary<string, string> texMap)
    {
        int count = mesh != null ? Mathf.Max(1, mesh.subMeshCount) : 1;
        var mats = new Material[count];
        for (int i = 0; i < count; i++)
            mats[i] = GetOrCreateMaterial(meshName, texMap, slotIndex: i);
        return mats;
    }

    static Material GetOrCreateMaterial(string meshName, Dictionary<string, string> texMap, int slotIndex = 0)
    {
        string texStem = null;
        if (texMap != null)
            texMap.TryGetValue(meshName, out texStem);
        if (string.IsNullOrEmpty(texStem))
            texStem = meshName.Replace("_m00_wp", "_t00_wp").Replace("_m00_sh", "_t00_sh").Replace("_m00", "_t00");
        if (slotIndex > 0)
        {
            texStem = PromoteTextureSlot(texStem, slotIndex);
            // No t01 file: reuse t00. A blank second slot draws the shield face black.
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(TexFolder + "/" + texStem + ".png") == null)
                return GetOrCreateMaterial(meshName, texMap, slotIndex: 0);
        }

        string matPath = MatFolder + "/" + texStem + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (existing != null)
            return existing;

        if (!AssetDatabase.IsValidFolder(MatFolder))
            AssetDatabase.CreateFolder(TexFolder, "Materials");

        // Shield alpha is a hole. Weapon alpha is the specularity mask, added as Shine0.
        bool shield = texStem.EndsWith("_sh");
        Material mat = new Material(Shader.Find(shield
            ? "L2/World/ActorModulate2X"
            : "L2/World/ActorEnvSpecular"));
        mat.name = texStem;

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexFolder + "/" + texStem + ".png");
        if (tex != null)
        {
            mat.SetTexture("_BaseMap", tex);
            mat.SetTexture("_MainTex", tex);
            if (!shield)
                mat.SetTexture("_SpecMask", tex);
        }

        if (!shield)
        {
            Texture2D env = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Resources/Data/SysTextures/CubeMaps/Shine0.png");
            if (env != null)
                mat.SetTexture("_EnvMap", env);
            // Most weapon PNGs carry the coverage mask in alpha (255 across the whole UV
            // island, 0 on the padding) or no alpha at all. Feeding either to the
            // specularity mask makes the entire weapon add the full Shine0 reflection,
            // which reads as a mirror instead of the few polished edges Interlude
            // highlights. Only a texture with a real alpha ramp is a specularity mask;
            // everything else must use the luma fallback, which picks out bright metal.
            mat.SetFloat("_SpecFromAlpha", TextureHasSpecularRamp(tex) ? 1f : 0f);
            mat.SetFloat("_ActorAlpha", 1f);
        }

        mat.SetFloat("_Cull", 0f);

        // A weapon PNG stores the coverage mask in alpha: 255 on the island, 0 on the
        // black padding, and the mesh UVs of the padding triangles sit outside every
        // island. Without the clip the padding draws as flat black, which is the band
        // and the loose diamonds around a dagger. AlphaRef 128 matches the mask.
        if (!shield && TextureHasCoverageAlpha(tex))
        {
            mat.SetFloat("_AlphaClip", 1f);
            mat.SetFloat("_Cutoff", 0.498039216f);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.SetOverrideTag("RenderType", "TransparentCutout");
            mat.renderQueue = 2450;
        }

        // Shields commonly use FinalBlend AlphaTest + TwoSided (thin geometry needs Cull Off).
        if (shield)
            ApplyFinalBlendCutout(mat);

        AssetDatabase.CreateAsset(mat, matPath);
        return mat;
    }

    /// <summary>arcana_mace_t00_wp → arcana_mace_t01_wp; imperial_…_t00_sh → …_t01_sh.</summary>
    static string PromoteTextureSlot(string t00Stem, int slotIndex)
    {
        string marker = "_t00_";
        int at = t00Stem.IndexOf(marker, System.StringComparison.Ordinal);
        if (at < 0)
            return t00Stem + "_t" + slotIndex.ToString("00");
        return t00Stem.Substring(0, at) + "_t" + slotIndex.ToString("00") + "_" + t00Stem.Substring(at + marker.Length);
    }

    /// <summary>RGBA weapon PNG keeps the specularity mask in alpha. RGB export dropped it.</summary>
    /// <summary>True when the alpha channel is a real ramp, not coverage or a constant.</summary>
    static bool TextureHasSpecularRamp(Texture2D tex)
    {
        if (tex == null || !tex.isReadable)
            return false;
        Color32[] pixels = tex.GetPixels32();
        int partial = 0;
        int solid = 0;
        int clear = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            byte a = pixels[i].a;
            if (a == 0) clear++;
            else if (a == 255) solid++;
            else partial++;
        }
        // Nothing graded means no specularity mask: a missing alpha is 255 everywhere and
        // a coverage mask is 0/255 only. A real ramp always leaves partial values, with or
        // without fully opaque pixels.
        if (partial == 0)
            return false;
        return partial > (clear + solid) / 1000;
    }

    static bool PngHasAlpha(string assetPath)
    {
        string full = Path.GetFullPath(assetPath);
        if (!File.Exists(full))
            return false;
        byte[] head = new byte[32];
        using (FileStream stream = File.OpenRead(full))
        {
            if (stream.Read(head, 0, head.Length) < 26)
                return false;
        }
        return head[25] == 4 || head[25] == 6;
    }

    /// <summary>
    /// True when alpha is a coverage mask rather than a specularity ramp: the padding is
    /// fully transparent, nothing sits between the two values, and the islands are solid.
    /// Those textures must be alpha-clipped, the soft ones must not be.
    /// </summary>
    static bool TextureHasCoverageAlpha(Texture2D tex)
    {
        if (tex == null || !tex.isReadable)
            return false;
        Color32[] pixels = tex.GetPixels32();
        int transparent = 0;
        int partial = 0;
        int solid = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            byte a = pixels[i].a;
            if (a == 0)
                transparent++;
            else if (a == 255)
                solid++;
            else
                partial++;
        }
        if (transparent == 0 || solid == 0)
            return false;
        return partial <= (transparent + solid) / 50;
    }

    /// <summary>Mobius FinalBlend AlphaTest=true / AlphaRef=160 / TwoSided → URP cutout + Cull Off.</summary>
    static void ApplyFinalBlendCutout(Material mat)
    {
        mat.SetFloat("_AlphaClip", 1f);
        mat.SetFloat("_Cutoff", 160f / 255f);
        mat.SetFloat("_Cull", 0f);
        mat.SetFloat("_ZWrite", 1f);
        mat.EnableKeyword("_ALPHATEST_ON");
        mat.SetOverrideTag("RenderType", "TransparentCutout");
        mat.renderQueue = 2450;
        mat.doubleSidedGI = true;
    }

    static Dictionary<string, string> LoadTexMap()
    {
        var map = new Dictionary<string, string>();
        for (int i = 0; i < TexMapPaths.Length; i++)
        {
            string full = Path.GetFullPath(TexMapPaths[i]);
            if (!File.Exists(full))
                continue;
            foreach (string raw in File.ReadAllLines(full))
            {
                string line = raw.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;
                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                    continue;
                map[parts[0].Trim()] = parts[1].Trim();
            }
        }
        return map;
    }
}
#endif
