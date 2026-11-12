#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Rebuilds Lottery Ticket Seller mesh <c>a_casino_MHuman_m00</c> from UU FBX + PNG mats.
/// PSK has 4 sections (b00/b01/f/h); a bad prior FBX collapsed face/hair into body.
/// Menu: Shnok/Characters/Build Casino NPC Prefab
/// </summary>
public static class L2CasinoNpcPrefabBuilder
{
    const string Stem = "a_casino_MHuman_m00";
    const string NpcRoot = "Assets/Resources/Data/Animations/LineageNPCs";
    const string TexRoot = "Assets/Resources/Data/SysTextures/LineageNpcsTex";
    const string BasicController = "Assets/Resources/Data/Animations/Animator/_Template/Npc_Basic.controller";
    const string OverrideFolder = "Assets/Resources/Data/Animations/Animator/Npc";
    const string ActorShader = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";

    static readonly string[] MatSuffixes = { "_t00_b00", "_t00_b01", "_t00_f", "_t00_h" };

    [MenuItem("Shnok/Characters/Build Casino NPC Prefab")]
    public static void Menu() => Debug.Log(Run());

    public static void RunCli()
    {
        string r;
        bool ok = true;
        try { r = Run(); }
        catch (System.Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[CasinoNpc] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static string Run()
    {
        var basic = AssetDatabase.LoadAssetAtPath<AnimatorController>(BasicController);
        if (basic == null)
            throw new System.Exception("missing " + BasicController);
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ActorShader);

        string folder = NpcRoot + "/" + Stem;
        string fbxPath = folder + "/Model/" + Stem + ".fbx";
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
            throw new System.Exception("MISSING " + fbxPath);

        // On-disk FBX is still UU (pelvis ~30). Meter Configure + globalScale 1 = giants.
        L2TownNpcFbxImport.ConfigureUuFbx(fbxPath);
        fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);

        Dictionary<string, Material> mats = BindExistingMaterials(shader);
        string prefabPath = folder + "/" + Stem + ".prefab";
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        instance.name = Stem;
        try
        {
            AssignMaterials(instance, mats);
            instance.transform.localScale = Vector3.one;
            float height = L2TownNpcFbxImport.AssertHumanoidHeight(instance, Stem);
            EnsureNpcComponents(instance);
            var animator = instance.GetComponent<Animator>();
            if (animator == null)
                animator = instance.AddComponent<Animator>();

            var clips = ExtractClips(fbxPath, folder + "/Model");
            var ov = LoadOrCreateOverride(basic);
            ApplyOverrides(ov, clips);
            animator.runtimeAnimatorController = ov;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            int slots = 0;
            foreach (var r in instance.GetComponentsInChildren<Renderer>(true))
                if (r != null && r.sharedMaterials != null)
                    slots += r.sharedMaterials.Length;
            return Stem + " prefab clips=" + clips.Count + " mats=" + mats.Count
                + " slots=" + slots + " height=" + height.ToString("0.###");
        }
        finally
        {
            Object.DestroyImmediate(instance);
        }
    }

    static Dictionary<string, Material> BindExistingMaterials(Shader shader)
    {
        var map = new Dictionary<string, Material>();
        foreach (string suf in MatSuffixes)
        {
            string matPath = TexRoot + "/Materials/" + Stem + suf + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                string png = TexRoot + "/" + Stem + suf + ".png";
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(png);
                if (tex == null || shader == null)
                    continue;
                mat = new Material(shader) { name = Stem + suf };
                mat.SetTexture("_BaseMap", tex);
                mat.SetTexture("_MainTex", tex);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            if (shader != null)
                mat.shader = shader;
            EditorUtility.SetDirty(mat);
            map[suf] = mat;
        }
        return map;
    }

    /// <summary>
    /// Single skinned mesh with 4 submeshes (PSK MATT order b00/b01/f/h).
    /// Prefer slot-name suffix match; fall back to npcgrp order.
    /// </summary>
    static void AssignMaterials(GameObject root, Dictionary<string, Material> mats)
    {
        if (root == null || mats == null || mats.Count == 0)
            return;

        Material[] ordered = MatSuffixes
            .Select(s => mats.TryGetValue(s, out var m) ? m : null)
            .Where(m => m != null)
            .ToArray();

        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
                continue;

            var slots = renderer.sharedMaterials;
            if (slots == null || slots.Length == 0)
            {
                renderer.sharedMaterials = ordered;
                renderer.enabled = true;
                continue;
            }

            var next = new Material[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                string slot = slots[i] != null ? slots[i].name.ToLowerInvariant() : "";
                Material pick = null;
                foreach (var kv in mats)
                {
                    string key = kv.Key.TrimStart('_').ToLowerInvariant();
                    if (slot.Contains(key) || slot.EndsWith(key.Replace("t00_", "")))
                    {
                        pick = kv.Value;
                        break;
                    }
                }
                if (pick == null && i < ordered.Length)
                    pick = ordered[i];
                next[i] = pick ?? slots[i];
            }
            renderer.sharedMaterials = next;
            renderer.enabled = true;
        }
    }

    static Dictionary<string, AnimationClip> ExtractClips(string fbxPath, string outDir)
    {
        Directory.CreateDirectory(outDir);
        var map = new Dictionary<string, AnimationClip>();
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(fbxPath).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string safe = Stem + ".ao_" + src.name.Replace('|', '_').Replace(' ', '_');
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
        if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith("ao_")) n = n.Substring(3);
        return n;
    }

    static AnimatorOverrideController LoadOrCreateOverride(AnimatorController basic)
    {
        string path = OverrideFolder + "/" + Stem + ".overrideController";
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
        AnimationClip walk = Find("walk_1hs", "walk");
        AnimationClip run = Find("run_1hs", "run");
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
            else if (slot.Contains("run"))
                o = run ?? walk ?? wait;
            else if (slot.Contains("walk"))
                o = walk ?? wait;
            else if (slot.Contains("spwait") || slot.Contains("atkwait") || slot.Contains("wait")
                     || slot.Contains("atk"))
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
