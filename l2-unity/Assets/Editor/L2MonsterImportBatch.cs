#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Data-driven import of official Interlude monster FBX + textures into LineageMonsters* prefabs.
/// Reads a JSON manifest (package, asset, textures[]) and uses the same wiring as keltir/pets.
/// No per-mob gameplay branches — one pipeline for every listed mesh.
/// </summary>
public static class L2MonsterImportBatch
{
    const string ShaderPath = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";
    const string TemplateTex = "Assets/Resources/Data/SysTextures/LineageMonstersTex/gremlin_t00.png";
    const string WolfFbx = "Assets/Resources/Data/Animations/LineageMonsters/wolf_m00/wolf.fbx";
    const string BlenderScript = "Assets/Editor/L2EffectGenerator/Blender/psk_psa_to_fbx.py";
    const string BlenderAddon = "Assets/Editor/L2EffectGenerator/Blender/io_import_scene_unreal_psa_psk_280.py";

    [Serializable]
    class Manifest
    {
        public Item[] items;
    }

    [Serializable]
    class Item
    {
        public string package;
        public string asset;
        public string[] textures;
    }

    [MenuItem("L2/Monsters/Import Batch Manifest")]
    public static void RunCli()
    {
        string manifestPath = Environment.GetEnvironmentVariable("L2_MONSTER_IMPORT_MANIFEST");
        if (string.IsNullOrEmpty(manifestPath))
            manifestPath = Path.Combine(Application.dataPath, "..", "..", ".work/npc-audit/batch1_monsters_unity.json");
        Run(manifestPath);
    }

    public static void Run(string manifestPath)
    {
        if (!File.Exists(manifestPath))
        {
            UnityEngine.Debug.LogError("[MonsterImport] missing manifest " + manifestPath);
            return;
        }

        Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(manifestPath));
        if (manifest == null || manifest.items == null || manifest.items.Length == 0)
        {
            UnityEngine.Debug.LogError("[MonsterImport] empty manifest " + manifestPath);
            return;
        }

        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        if (shader == null)
        {
            UnityEngine.Debug.LogError("[MonsterImport] missing shader " + ShaderPath);
            return;
        }

        int ok = 0;
        for (int i = 0; i < manifest.items.Length; i++)
        {
            Item item = manifest.items[i];
            try
            {
                if (BuildOne(item, shader))
                    ok++;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[MonsterImport] FAIL " + item.package + "." + item.asset + " " + e);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("[MonsterImport] built " + ok + "/" + manifest.items.Length);
    }

    static bool BuildOne(Item item, Shader shader)
    {
        if (string.IsNullOrEmpty(item.package) || string.IsNullOrEmpty(item.asset))
            return false;

        string folder = "Assets/Resources/Data/Animations/" + item.package + "/" + item.asset;
        string fbx = folder + "/" + item.asset + ".fbx";
        string prefab = folder + "/" + item.asset + ".prefab";
        if (!File.Exists(Abs(fbx)))
        {
            UnityEngine.Debug.LogError("[MonsterImport] FBX missing " + fbx);
            return false;
        }

        ApplyTextureImporters(item);
        string[] mats = EnsureMaterials(item, shader);
        ConfigureModelImporter(fbx);

        GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>(fbx);
        if (src == null)
        {
            UnityEngine.Debug.LogError("[MonsterImport] cannot load " + fbx);
            return false;
        }

        GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(src);
        inst.name = item.asset;
        int entityLayer = LayerMask.NameToLayer("Entity");
        if (entityLayer >= 0)
            SetLayer(inst, entityLayer);
        AssignMaterials(inst, mats);
        EnsureClickArea(inst);

        PrefabUtility.SaveAsPrefabAsset(inst, prefab);
        UnityEngine.Object.DestroyImmediate(inst);

        int written = WriteClipFiles(item.asset, fbx);
        StripAnimationsFromFbx(item, fbx);
        // Blender may rewrite the FBX after clips are extracted; re-assert the meter contract.
        L2TownNpcFbxImport.EnsureMeterContract(fbx);
        L2MonsterPrefabBuilder.WireSingle(prefab);
        FitCapsule(prefab);
        UnityEngine.Debug.Log("[MonsterImport] ready " + item.asset + " clips=" + written + " mats=" + mats.Length);
        return written > 0 || File.Exists(Abs(prefab));
    }

    static void ApplyTextureImporters(Item item)
    {
        TextureImporter template = AssetImporter.GetAtPath(TemplateTex) as TextureImporter;
        if (item.textures == null)
            return;
        for (int i = 0; i < item.textures.Length; i++)
        {
            string texPath = TextureAssetPath(item.textures[i]);
            if (texPath == null)
                continue;
            TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
                importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
            }

            if (importer == null || template == null)
                continue;

            importer.sRGBTexture = template.sRGBTexture;
            importer.isReadable = template.isReadable;
            importer.mipmapEnabled = template.mipmapEnabled;
            importer.alphaIsTransparency = template.alphaIsTransparency;
            importer.filterMode = template.filterMode;
            importer.wrapMode = template.wrapMode;
            importer.anisoLevel = template.anisoLevel;
            importer.SaveAndReimport();
        }
    }

    static string[] EnsureMaterials(Item item, Shader shader)
    {
        if (item.textures == null || item.textures.Length == 0)
            return Array.Empty<string>();

        var paths = new List<string>();
        for (int i = 0; i < item.textures.Length; i++)
        {
            string texKey = item.textures[i];
            string texPath = TextureAssetPath(texKey);
            string matPath = MaterialAssetPath(texKey);
            if (matPath == null)
                continue;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                string dir = Path.GetDirectoryName(matPath).Replace('\\', '/');
                Directory.CreateDirectory(Abs(dir));
                mat = new Material(shader);
                mat.name = Path.GetFileNameWithoutExtension(matPath);
                AssetDatabase.CreateAsset(mat, matPath);
            }

            Texture tex = texPath != null ? AssetDatabase.LoadAssetAtPath<Texture>(texPath) : null;
            if (tex != null && mat.HasProperty("_BaseMap"))
            {
                mat.SetTexture("_BaseMap", tex);
                EditorUtility.SetDirty(mat);
            }

            paths.Add(matPath);
        }

        return paths.ToArray();
    }

    static string TextureAssetPath(string texKey)
    {
        if (string.IsNullOrEmpty(texKey) || texKey.IndexOf('.') < 0)
            return null;
        string pkg = texKey.Substring(0, texKey.IndexOf('.'));
        string asset = texKey.Substring(texKey.IndexOf('.') + 1);
        string path = "Assets/Resources/Data/SysTextures/" + pkg + "/" + asset + ".png";
        return File.Exists(Abs(path)) ? path : null;
    }

    static string MaterialAssetPath(string texKey)
    {
        if (string.IsNullOrEmpty(texKey) || texKey.IndexOf('.') < 0)
            return null;
        string pkg = texKey.Substring(0, texKey.IndexOf('.'));
        string asset = texKey.Substring(texKey.IndexOf('.') + 1);
        return "Assets/Resources/Data/SysTextures/" + pkg + "/Materials/" + asset + ".mat";
    }

    static void ConfigureModelImporter(string fbxPath)
    {
        L2TownNpcFbxImport.EnsureMeterContract(fbxPath);
        if (AssetImporter.GetAtPath(fbxPath) is ModelImporter importer)
        {
            importer.useFileUnits = true;
            importer.importBlendShapes = true;
            importer.SaveAndReimport();
        }

        L2TownNpcFbxImport.AssertMeterImporter(fbxPath);
    }

    static void AssignMaterials(GameObject inst, string[] materialPaths)
    {
        if (materialPaths == null || materialPaths.Length == 0)
            return;
        var mats = new Material[materialPaths.Length];
        for (int i = 0; i < materialPaths.Length; i++)
            mats[i] = AssetDatabase.LoadAssetAtPath<Material>(materialPaths[i]);

        Renderer[] renderers = inst.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            Material[] slots = renderers[r].sharedMaterials;
            var next = new Material[slots.Length];
            for (int s = 0; s < slots.Length; s++)
                next[s] = mats[Math.Min(s, mats.Length - 1)];
            renderers[r].sharedMaterials = next;
        }
    }

    static void EnsureClickArea(GameObject inst)
    {
        if (inst.transform.Find("click_area") != null)
            return;
        GameObject click = GameObject.CreatePrimitive(PrimitiveType.Cube);
        click.name = "click_area";
        click.transform.SetParent(inst.transform, false);
        click.transform.localPosition = new Vector3(0f, 0.28f, 0f);
        click.transform.localScale = new Vector3(0.4f, 0.56f, 0.4f);
        int clickLayer = LayerMask.NameToLayer("EntityClick");
        if (clickLayer >= 0)
            click.layer = clickLayer;
        click.tag = "Npc";
        MeshRenderer clickRenderer = click.GetComponent<MeshRenderer>();
        if (clickRenderer != null)
            clickRenderer.enabled = false;
    }

    static int WriteClipFiles(string stem, string fbxPath)
    {
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer == null)
            return 0;

        ModelImporterClipAnimation[] takes = importer.defaultClipAnimations;
        if (takes == null || takes.Length == 0)
        {
            UnityEngine.Debug.LogError("[MonsterImport] no takes " + stem);
            return 0;
        }

        var defs = new ModelImporterClipAnimation[takes.Length];
        for (int i = 0; i < takes.Length; i++)
        {
            ModelImporterClipAnimation src = takes[i];
            string take = string.IsNullOrEmpty(src.takeName) ? src.name : src.takeName;
            var clip = new ModelImporterClipAnimation();
            clip.name = ShortName(take);
            clip.takeName = take;
            clip.firstFrame = src.firstFrame;
            clip.lastFrame = src.lastFrame;
            clip.loopTime = ShouldLoop(clip.name);
            defs[i] = clip;
        }

        importer.clipAnimations = defs;
        importer.importAnimation = true;
        importer.SaveAndReimport();

        string folder = Path.GetDirectoryName(fbxPath).Replace('\\', '/');
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        int written = 0;
        for (int i = 0; i < assets.Length; i++)
        {
            AnimationClip src = assets[i] as AnimationClip;
            if (src == null || src.name.StartsWith("__preview__"))
                continue;
            string safe = src.name.Replace("|", "_").Replace("/", "_").Replace("\\", "_");
            string path = folder + "/" + stem + "." + safe + ".anim";
            if (File.Exists(Abs(path)))
                AssetDatabase.DeleteAsset(path);
            AnimationClip copy = UnityEngine.Object.Instantiate(src);
            copy.name = stem + "." + safe;
            L2AnimClipPipeline.StripRedundantEulerCurves(copy);
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(copy);
            settings.loopTime = ShouldLoop(safe);
            AnimationUtility.SetAnimationClipSettings(copy, settings);
            AssetDatabase.CreateAsset(copy, path);
            written++;
        }

        AssetDatabase.SaveAssets();
        return written;
    }

    static void StripAnimationsFromFbx(Item item, string fbxPath)
    {
        string export = Environment.GetEnvironmentVariable("L2_MONSTER_EXPORT");
        if (string.IsNullOrEmpty(export))
            return;
        string psk = Path.Combine(export, item.package, "SkeletalMesh", item.asset + ".psk");
        string psa = Path.Combine(export, item.package, "MeshAnimation",
            item.asset.Replace("_m00", "") + "_anim.psa");
        if (!File.Exists(psk))
            return;
        if (!File.Exists(psa))
        {
            // shared clips (e.g. ti_mi_tran uses ti_mi_riran_anim) — leave animated FBX
            return;
        }

        string project = Directory.GetCurrentDirectory();
        string blender = L2DiagPaths.BlenderExe;
        string script = Path.Combine(project, BlenderScript);
        string addon = Path.Combine(project, BlenderAddon);
        string fbx = Path.Combine(project, fbxPath);
        string reference = Path.Combine(project, WolfFbx);
        var start = new ProcessStartInfo();
        start.FileName = blender;
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.Arguments =
            "-b -P \"" + script + "\" -- \"" + psk + "\" \"" + psa + "\" \"" + fbx + "\" \"" + addon + "\" \"" + reference + "\" mesh";
        using (Process process = Process.Start(start))
        {
            if (process != null)
                process.WaitForExit();
        }

        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer == null)
            return;
        importer.importAnimation = false;
        importer.clipAnimations = new ModelImporterClipAnimation[0];
        importer.SaveAndReimport();
    }

    static string ShortName(string take)
    {
        int bar = take.LastIndexOf('|');
        return bar >= 0 && bar + 1 < take.Length ? take.Substring(bar + 1) : take;
    }

    static bool ShouldLoop(string name)
    {
        string n = name.ToLowerInvariant();
        if (n.Contains("death") && !n.Contains("deathwait"))
            return false;
        return n.Contains("wait") || n.Contains("walk") || n.Contains("run") || n.Contains("swim");
    }

    static string Abs(string assetPath)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), assetPath);
    }

    static void FitCapsule(string prefabPath)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            SkinnedMeshRenderer[] skins = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            bool has = false;
            Bounds bounds = new Bounds();
            for (int i = 0; i < skins.Length; i++)
            {
                if (skins[i] == null)
                    continue;
                if (!has)
                {
                    bounds = skins[i].bounds;
                    has = true;
                }
                else
                {
                    bounds.Encapsulate(skins[i].bounds);
                }
            }

            if (!has)
                return;

            Vector3 center = root.transform.InverseTransformPoint(bounds.center);
            float height = Mathf.Max(0.25f, bounds.size.y);
            float radius = Mathf.Max(0.08f, Mathf.Max(bounds.extents.x, bounds.extents.z) * 0.55f);
            if (radius > height * 0.5f)
                radius = height * 0.5f;

            CharacterController body = root.GetComponent<CharacterController>();
            if (body != null)
            {
                body.height = height;
                body.radius = radius;
                body.center = new Vector3(0f, center.y, 0f);
            }

            Transform click = root.transform.Find("click_area");
            if (click != null)
            {
                click.localPosition = new Vector3(0f, center.y, 0f);
                click.localScale = new Vector3(
                    Mathf.Max(0.3f, bounds.size.x),
                    Mathf.Max(0.3f, bounds.size.y),
                    Mathf.Max(0.3f, bounds.size.z));
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            if (child.name == "click_area")
                continue;
            SetLayer(child.gameObject, layer);
        }
    }
}
#endif
