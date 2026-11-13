#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Builds the Dark Elf village bodies that npcgrp names and the client did not have.
/// FBX on disk is Unreal units. <see cref="L2TownNpcFbxImport.ConfigureUuFbx"/> scales them.
/// Menu: Shnok/Characters/Build Dark Elf Village NPC Prefabs
/// </summary>
public static class L2DarkElfNpcPrefabBuilder
{
    const string AnimRoot = "Assets/Resources/Data/Animations";
    const string TexRoot = "Assets/Resources/Data/SysTextures/LineageNpcsTex";
    const string BasicController = "Assets/Resources/Data/Animations/Animator/_Template/Npc_Basic.controller";
    const string OverrideFolder = "Assets/Resources/Data/Animations/Animator/Npc";
    const string ActorShader = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";

    static readonly (string Package, string Stem)[] Npcs =
    {
        ("LineageNPCs", "a_patriarch_MDarkElf_m00"),
        ("LineageNPCs", "a_clergyman_MDarkElf_m00"),
        ("LineageNPCs", "a_clergyman_FDarkElf_m00"),
        ("LineageNPCs", "a_teleporter_FDarkElf_m00"),
        ("LineageNPCs", "a_trader_FDarkelf_m00"),
        ("LineageNPCs", "a_trader_MDarkElf_m00"),
        ("LineageNPCs4", "a_fighterguild_teacher_FDarkElf_m00"),
        ("LineageNPCs4", "a_mageguild_teacher_FDarkElf_m00"),
        ("LineageNPCs", "a_mageguild_teacher_MDarkElf_m00"),
        ("LineageNPCs", "a_guard_FDarkelf_m00"),
        ("LineageNPCs4", "a_common_people_MDarkelf_m00"),
        ("LineageNPCs", "a_common_people_FDarkElf_m00"),
        ("LineageNPCs", "a_fighterguild_teacher_MDarkElf_m00"),
        ("LineageNPCs", "a_guild_master_MDarkElf_m00"),
        ("LineageNPCs", "a_casino_FDarkElf_m00"),
    };

    static readonly string[] Suffixes = { "_t00_b00", "_t00_b01", "_t00_f", "_t00_h", "_t00_wand" };

    [MenuItem("Shnok/Characters/Build Dark Elf Village NPC Prefabs")]
    public static void Menu() => Debug.Log(Run());

    public static void RunCli()
    {
        string r;
        bool ok = true;
        try { r = Run(); }
        catch (System.Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[DarkElfNpc] " + r);
        EditorApplication.Exit(ok && !r.Contains("FAIL") && !r.Contains("MISSING") ? 0 : 1);
    }

    public static string Run()
    {
        var basic = AssetDatabase.LoadAssetAtPath<AnimatorController>(BasicController);
        if (basic == null)
            throw new System.Exception("missing " + BasicController);
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ActorShader);

        var lines = new List<string>();
        foreach (var npc in Npcs)
        {
            try
            {
                lines.Add(BuildOne(npc.Package, npc.Stem, basic, shader));
            }
            catch (System.Exception e)
            {
                lines.Add(npc.Stem + " FAIL " + e.Message);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return string.Join("\n", lines);
    }

    static string BuildOne(string package, string stem, AnimatorController basic, Shader shader)
    {
        string folder = AnimRoot + "/" + package + "/" + stem;
        string fbxPath = folder + "/Model/" + stem + ".fbx";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath) == null)
            return stem + " MISSING " + fbxPath;

        // bScaleDown puts the file in the same meter range as the elf teleporter FBX.
        L2TownNpcFbxImport.ConfigureUuFbx(fbxPath);
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
            return stem + " MISSING after import";

        Dictionary<string, Material> mats = BindMaterials(stem, shader);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        instance.name = stem;
        try
        {
            AssignMaterials(instance, mats);
            instance.transform.localPosition = Vector3.zero;
            float raw = L2TownNpcFbxImport.MeasureHumanoidHeight(instance);
            if (raw < L2TownNpcFbxImport.MinHeight || raw > L2TownNpcFbxImport.MaxHeight)
                instance.transform.localScale = Vector3.one * (1.6f / Mathf.Max(raw, 0.0001f));
            else
                instance.transform.localScale = Vector3.one;
            float height = L2TownNpcFbxImport.AssertHumanoidHeight(instance, stem);
            EnsureNpcComponents(instance);

            var animator = instance.GetComponent<Animator>();
            var clips = ExtractClips(stem, fbxPath, folder + "/Model");
            var ov = LoadOrCreateOverride(stem, basic);
            ApplyOverrides(ov, clips);
            animator.runtimeAnimatorController = ov;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            string prefabPath = folder + "/" + stem + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            return stem + " ok clips=" + clips.Count + " mats=" + mats.Count
                + " height=" + height.ToString("0.###");
        }
        finally
        {
            Object.DestroyImmediate(instance);
        }
    }

    static Dictionary<string, Material> BindMaterials(string stem, Shader shader)
    {
        var map = new Dictionary<string, Material>();
        Directory.CreateDirectory(TexRoot + "/Materials");
        foreach (string suf in Suffixes)
        {
            string matPath = TexRoot + "/Materials/" + stem + suf + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                Texture2D tex = LoadTex(stem, suf);
                if (tex == null || shader == null)
                    continue;
                mat = new Material(shader) { name = stem + suf };
                mat.SetTexture("_BaseMap", tex);
                mat.SetTexture("_MainTex", tex);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            else if (shader != null)
            {
                mat.shader = shader;
                Texture2D tex = LoadTex(stem, suf);
                if (tex != null)
                {
                    mat.SetTexture("_BaseMap", tex);
                    mat.SetTexture("_MainTex", tex);
                }
            }

            EditorUtility.SetDirty(mat);
            map[suf] = mat;
        }

        return map;
    }

    static Texture2D LoadTex(string stem, string suf)
    {
        string png = TexRoot + "/" + stem + suf + ".png";
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(png);
        if (tex != null)
            return tex;
        return AssetDatabase.LoadAssetAtPath<Texture2D>(TexRoot + "/" + stem + suf + ".tga");
    }

    static void AssignMaterials(GameObject root, Dictionary<string, Material> mats)
    {
        if (root == null || mats.Count == 0)
            return;

        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
                continue;
            string n = renderer.name.ToLowerInvariant();
            Material pick = null;
            if (n.Contains("wand"))
                mats.TryGetValue("_t00_wand", out pick);
            else if (n.Contains("_b00") || n.EndsWith("b00"))
                mats.TryGetValue("_t00_b00", out pick);
            else if (n.Contains("_b01") || n.EndsWith("b01"))
                mats.TryGetValue("_t00_b01", out pick);
            else if (n.Contains("_f") && !n.Contains("_b0"))
                mats.TryGetValue("_t00_f", out pick);
            else if (n.Contains("_h"))
                mats.TryGetValue("_t00_h", out pick);

            if (pick == null && mats.TryGetValue("_t00_b00", out var body))
                pick = body;
            if (pick == null)
                continue;

            var slots = renderer.sharedMaterials;
            if (slots == null || slots.Length == 0)
                renderer.sharedMaterial = pick;
            else
            {
                for (int i = 0; i < slots.Length; i++)
                    slots[i] = pick;
                renderer.sharedMaterials = slots;
            }
            renderer.enabled = true;
        }
    }

    static Dictionary<string, AnimationClip> ExtractClips(string stem, string fbxPath, string outDir)
    {
        Directory.CreateDirectory(outDir);
        var map = new Dictionary<string, AnimationClip>();
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(fbxPath).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__"))
                continue;
            string safe = stem + ".ao_" + src.name.Replace('|', '_').Replace(' ', '_');
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
                var copy = Object.Instantiate(src);
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
        if (i >= 0)
            n = n.Substring(i + 1);
        if (n.StartsWith("ao_"))
            n = n.Substring(3);
        return n;
    }

    static AnimatorOverrideController LoadOrCreateOverride(string stem, AnimatorController basic)
    {
        Directory.CreateDirectory(OverrideFolder);
        string path = OverrideFolder + "/" + stem + ".overrideController";
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

    static void ApplyOverrides(AnimatorOverrideController ov, Dictionary<string, AnimationClip> clips)
    {
        AnimationClip Find(params string[] keys)
        {
            foreach (string key in keys)
            {
                foreach (var kv in clips)
                {
                    if (kv.Key.Contains(key))
                        return kv.Value;
                }
            }

            return clips.Count > 0 ? clips.Values.First() : null;
        }

        AnimationClip wait = Find("wait_1hs", "wait");
        AnimationClip death = Find("death_hand", "death");
        AnimationClip deathWait = Find("deathwait", "death_wait");
        AnimationClip social = Find("social01", "social");
        AnimationClip spwait = Find("spwait01", "spwait");

        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        ov.GetOverrides(pairs);
        var next = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var kv in pairs)
        {
            string slot = kv.Key != null ? kv.Key.name.ToLowerInvariant() : "";
            AnimationClip o = null;
            if (slot.Contains("deathwait") || slot.Contains("death_wait"))
                o = deathWait ?? death ?? wait;
            else if (slot.Contains("death"))
                o = death ?? wait;
            else if (slot.Contains("social"))
                o = social ?? wait;
            else if (slot.Contains("spwait") || slot.Contains("atkwait") || slot.Contains("wait")
                     || slot.Contains("walk") || slot.Contains("run") || slot.Contains("atk"))
                o = (slot.Contains("spwait") ? spwait : null) ?? wait;
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
            int layer = LayerMask.NameToLayer("EntityClick");
            go.layer = layer >= 0 ? layer : 0;
        }
        else
            click.gameObject.tag = "Npc";
    }
}
#endif
