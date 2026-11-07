// fineage-touch 1794037730.455447
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Wires Immortal Plateau Orc clergy NPCs from exported FBX + textures:
/// e_clergyman_MOrc_m00 / e_clergyman_FOrc_m00 under LineageNPCs/.
/// Menu: Shnok/Characters/Build Orc Clergy NPC Prefabs
/// </summary>
public static class L2OrcNpcPrefabBuilder
{
    const string NpcRoot = "Assets/Resources/Data/Animations/LineageNPCs";
    const string TexRoot = "Assets/Resources/Data/SysTextures/LineageNPCsTex";
    const string BasicController = "Assets/Resources/Data/Animations/Animator/_Template/Npc_Basic.controller";
    const string OverrideFolder = "Assets/Resources/Data/Animations/Animator/Npc";
    const string ActorShader = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";
    static readonly string[] Stems = { "e_clergyman_MOrc_m00", "e_clergyman_FOrc_m00" };

    [MenuItem("Shnok/Characters/Build Orc Clergy NPC Prefabs")]
    public static void Menu() => Debug.Log(Run());

    public static void RunCli()
    {
        string r;
        bool ok = true;
        try { r = Run(); }
        catch (System.Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[OrcNpc] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static string Run()
    {
        var basic = AssetDatabase.LoadAssetAtPath<AnimatorController>(BasicController);
        if (basic == null)
            throw new System.Exception("missing " + BasicController);
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ActorShader);
        var log = new List<string>();
        foreach (string stem in Stems)
            log.Add(BuildOne(stem, basic, shader));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return string.Join(" | ", log);
    }

    static string BuildOne(string stem, AnimatorController basic, Shader shader)
    {
        string folder = NpcRoot + "/" + stem;
        string fbxPath = folder + "/Model/" + stem + ".fbx";
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
            return stem + " MISSING " + fbxPath;

        ConfigureFbxImporter(fbxPath);
        fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);

        Dictionary<string, Material> mats = BindTextures(stem, shader);

        string prefabPath = folder + "/" + stem + ".prefab";
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        instance.name = stem;
        try
        {
            AssignMaterials(instance, mats);
            instance.transform.localScale = Vector3.one;
            EnsureNpcComponents(instance);
            var animator = instance.GetComponent<Animator>();
            if (animator == null)
                animator = instance.AddComponent<Animator>();

            // Pull clips embedded in the FBX into standalone assets next to the prefab.
            var clips = ExtractClips(fbxPath, folder + "/Model");
            var ov = LoadOrCreateOverride(stem, basic);
            ApplyWaitOverrides(ov, basic, clips);
            animator.runtimeAnimatorController = ov;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            return stem + " prefab clips=" + clips.Count + " mats=" + mats.Count;
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(instance);
        }
    }

    static void ConfigureFbxImporter(string fbxPath)
    {
        var imp = (ModelImporter)global::UnityEditor.AssetImporter.GetAtPath(fbxPath);
        if (imp == null) return;
        imp.animationType = ModelImporterAnimationType.Generic;
        // NoAvatar — same as race anim FBXs. CreateFromThisModel + scale was a factor
        // in exploded limbs after the failed Blender bake reimport.
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.materialImportMode = ModelImporterMaterialImportMode.None;
        // Source FBX from .work/npc-export is raw L2 UU (pelvis Z ≈ 30). Match the
        // working elf NPC importer: Unity globalScale 0.019 — do NOT Blender-bake scale
        // into the FBX while animations are embedded (that exploded limbs on re-extract).
        imp.globalScale = 0.019f;
        imp.useFileScale = false;
        imp.animationCompression = ModelImporterAnimationCompression.Off;
        imp.SaveAndReimport();
    }

    static Dictionary<string, Material> BindTextures(string stem, Shader shader)
    {
        var map = new Dictionary<string, Material>();
        if (shader == null) return map;
        string[] suffixes = { "_t00_b00", "_t00_b01", "_t00_f", "_t00_h" };
        foreach (string suf in suffixes)
        {
            string texPath = TexRoot + "/" + stem + suf + ".tga";
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (tex == null) continue;
            string matPath = TexRoot + "/" + stem + suf + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(shader) { name = stem + suf };
                AssetDatabase.CreateAsset(mat, matPath);
            }
            mat.shader = shader;
            mat.SetTexture("_BaseMap", tex);
            mat.SetTexture("_MainTex", tex);
            mat.SetColor("_BaseColor", Color.white);
            mat.SetFloat("_Cull", 0f);
            mat.SetFloat("_AlphaClip", 0f);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.renderQueue = 2000;
            EditorUtility.SetDirty(mat);
            map[suf] = mat;
            // ModelTable.LoadArmorMaterial looks under SysTextures/.../Materials/.
            string materialsDirMat = TexRoot + "/Materials/" + stem + suf + ".mat";
            Directory.CreateDirectory(Path.GetDirectoryName(materialsDirMat) ?? ".");
            var materialsCopy = AssetDatabase.LoadAssetAtPath<Material>(materialsDirMat);
            if (materialsCopy == null)
            {
                materialsCopy = new Material(mat) { name = stem + suf };
                AssetDatabase.CreateAsset(materialsCopy, materialsDirMat);
            }
            else
                EditorUtility.CopySerialized(mat, materialsCopy);
            EditorUtility.SetDirty(materialsCopy);

            // Also keep a Model/Materials copy so FBX remaps / default slots can find them.
            string modelMat = NpcRoot + "/" + stem + "/Model/Materials/" + stem + suf + ".mat";
            Directory.CreateDirectory(Path.GetDirectoryName(modelMat) ?? ".");
            var modelCopy = AssetDatabase.LoadAssetAtPath<Material>(modelMat);
            if (modelCopy == null)
            {
                modelCopy = new Material(mat) { name = stem + suf };
                AssetDatabase.CreateAsset(modelCopy, modelMat);
            }
            else
                EditorUtility.CopySerialized(mat, modelCopy);
            EditorUtility.SetDirty(modelCopy);
        }
        return map;
    }

    static void AssignMaterials(GameObject root, Dictionary<string, Material> mats)
    {
        if (root == null || mats == null || mats.Count == 0)
            return;

        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
                continue;
            string n = renderer.name.ToLowerInvariant();
            Material pick = null;
            if (n.Contains("_b00") || n.EndsWith("b00"))
                mats.TryGetValue("_t00_b00", out pick);
            else if (n.Contains("_b01") || n.EndsWith("b01"))
                mats.TryGetValue("_t00_b01", out pick);
            else if (n.Contains("_f") && !n.Contains("_b0"))
                mats.TryGetValue("_t00_f", out pick);
            else if (n.Contains("_h"))
                mats.TryGetValue("_t00_h", out pick);

            if (pick == null)
            {
                // Fallback: match by material slot name suffix.
                var slots = renderer.sharedMaterials;
                for (int i = 0; i < slots.Length; i++)
                {
                    string slot = slots[i] != null ? slots[i].name.ToLowerInvariant() : "";
                    foreach (var kv in mats)
                    {
                        if (slot.Contains(kv.Key.TrimStart('_')) || n.Contains(kv.Key.TrimStart('_')))
                        {
                            slots[i] = kv.Value;
                            pick = kv.Value;
                            break;
                        }
                    }
                }
                if (pick != null)
                    renderer.sharedMaterials = slots;
                continue;
            }

            var one = renderer.sharedMaterials;
            if (one == null || one.Length == 0)
                renderer.sharedMaterial = pick;
            else
            {
                for (int i = 0; i < one.Length; i++)
                    one[i] = pick;
                renderer.sharedMaterials = one;
            }
            renderer.enabled = true;
        }
    }

    static Dictionary<string, AnimationClip> ExtractClips(string fbxPath, string outDir)
    {
        var map = new Dictionary<string, AnimationClip>();
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(fbxPath).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string safe = src.name.Replace('|', '_').Replace(' ', '_');
            string path = outDir + "/" + safe + ".anim";
            var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(src, existing);
                EditorUtility.SetDirty(existing);
                map[Normalize(src.name)] = existing;
            }
            else
            {
                var copy = UnityEngine.Object.Instantiate(src);
                copy.name = safe;
                AssetDatabase.CreateAsset(copy, path);
                map[Normalize(src.name)] = copy;
            }
        }
        return map;
    }

    static string Normalize(string n)
    {
        n = n.ToLowerInvariant();
        int i = n.LastIndexOf('|');
        if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith("ao_")) n = n.Substring(3);
        return n;
    }

    static AnimatorOverrideController LoadOrCreateOverride(string stem, AnimatorController basic)
    {
        string name = stem.EndsWith("_m00") ? stem.Substring(0, stem.Length - 4) : stem;
        string path = OverrideFolder + "/" + name + ".overrideController";
        Directory.CreateDirectory(OverrideFolder);
        var ov = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
        if (ov == null)
        {
            ov = new AnimatorOverrideController { runtimeAnimatorController = basic };
            AssetDatabase.CreateAsset(ov, path);
        }
        else
            ov.runtimeAnimatorController = basic;
        return ov;
    }

    static void ApplyWaitOverrides(
        AnimatorOverrideController ov,
        AnimatorController basic,
        Dictionary<string, AnimationClip> clips)
    {
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ov.GetOverrides(pairs);
        // Prefer wait / spwait / wait_hand style clips for idle slots.
        AnimationClip idle = null;
        foreach (string key in new[] { "wait_hand", "wait", "spwait02", "spwait01", "stand" })
        {
            foreach (var kv in clips)
            {
                if (kv.Key.Contains(key))
                {
                    idle = kv.Value;
                    break;
                }
            }
            if (idle != null) break;
        }
        if (idle == null && clips.Count > 0)
            idle = clips.Values.First();

        var next = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var kv in pairs)
        {
            AnimationClip o = null;
            string slot = kv.Key != null ? kv.Key.name.ToLowerInvariant() : "";
            if (idle != null && (slot.Contains("wait") || slot.Contains("spwait") || slot.Contains("idle")))
                o = idle;
            else
            {
                foreach (var c in clips)
                {
                    if (slot.Length > 0 && c.Key.Contains(slot.Replace(" ", "")))
                    {
                        o = c.Value;
                        break;
                    }
                }
            }
            next.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o));
        }
        ov.ApplyOverrides(next);
        EditorUtility.SetDirty(ov);
    }

    static void EnsureNpcComponents(GameObject root)
    {
        if (root.GetComponent<Animator>() == null)
            root.AddComponent<Animator>();
        if (root.GetComponent<NetworkAnimationController>() == null)
            root.AddComponent<NetworkAnimationController>();
        if (root.GetComponent<CharacterController>() == null)
        {
            var cc = root.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);
        }
        if (root.GetComponent<CharacterAnimationAudioHandler>() == null)
            root.AddComponent<CharacterAnimationAudioHandler>();
        if (root.GetComponent<NpcEntity>() == null)
            root.AddComponent<NpcEntity>();
        if (root.GetComponent<NpcStateMachine>() == null)
            root.AddComponent<NpcStateMachine>();

        Transform click = root.transform.Find("click_area");
        if (click == null)
        {
            var go = new GameObject("click_area");
            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            var col = go.AddComponent<CapsuleCollider>();
            col.height = 1.8f;
            col.radius = 0.4f;
            col.isTrigger = true;
            go.tag = "Npc";
            go.layer = LayerMask.NameToLayer("EntityClick") >= 0
                ? LayerMask.NameToLayer("EntityClick")
                : 0;
        }
        else
            click.gameObject.tag = "Npc";
    }
}
#endif
