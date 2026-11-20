#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Data-driven import of official Interlude town/village NPC FBX + textures.
/// Meter FBX (pskimport bScaleDown=True) then <see cref="L2TownNpcFbxImport.EnsureMeterContract"/>.
/// Wires Npc_Basic like the Dark Elf / Orc village builders. No per-npc gameplay branches.
/// </summary>
public static class L2NpcImportBatch
{
    const string AnimRoot = "Assets/Resources/Data/Animations";
    const string TexRoot = "Assets/Resources/Data/SysTextures/LineageNpcsTex";
    const string BasicController = "Assets/Resources/Data/Animations/Animator/_Template/Npc_Basic.controller";
    const string OverrideFolder = "Assets/Resources/Data/Animations/Animator/Npc";
    const string ActorShader = "Assets/Resources/Data/Shaders/World/L2ActorModulate2X.shader";
    static readonly string[] Suffixes = { "_t00_b00", "_t00_b01", "_t00_f", "_t00_h", "_t00_wand", "_t00", "_t01", "_t02", "_t03" };

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
        public bool skip_mesh;
        public bool humanoid = true;
    }

    [MenuItem("L2/NPCs/Import Batch Manifest")]
    public static void RunCli()
    {
        string path = Environment.GetEnvironmentVariable("L2_NPC_IMPORT_MANIFEST");
        if (string.IsNullOrEmpty(path))
            path = "/tmp/npc-audit/batch_town_npcs_unity.json";
        Run(path);
    }

    public static void Run(string manifestPath)
    {
        if (!File.Exists(manifestPath))
        {
            Debug.LogError("[NpcImport] missing manifest " + manifestPath);
            return;
        }

        Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(manifestPath));
        if (manifest == null || manifest.items == null)
        {
            Debug.LogError("[NpcImport] empty manifest");
            return;
        }

        AnimatorController basic = AssetDatabase.LoadAssetAtPath<AnimatorController>(BasicController);
        if (basic == null)
        {
            Debug.LogError("[NpcImport] missing " + BasicController);
            return;
        }

        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ActorShader);
        int ok = 0;
        for (int i = 0; i < manifest.items.Length; i++)
        {
            Item item = manifest.items[i];
            try
            {
                if (item.skip_mesh)
                {
                    EnsureMaterials(item, shader);
                    Debug.Log("[NpcImport] textures " + item.asset);
                    ok++;
                    continue;
                }

                string line = BuildOne(item, basic, shader);
                Debug.Log("[NpcImport] " + line);
                ok++;
            }
            catch (Exception e)
            {
                Debug.LogError("[NpcImport] FAIL " + item.package + "." + item.asset + " " + e);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[NpcImport] built " + ok + "/" + manifest.items.Length);
    }

    static string BuildOne(Item item, AnimatorController basic, Shader shader)
    {
        string folder = AnimRoot + "/" + item.package + "/" + item.asset;
        string fbxPath = folder + "/Model/" + item.asset + ".fbx";
        if (!File.Exists(Abs(fbxPath)))
            throw new Exception("FBX missing " + fbxPath);

        AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate);
        L2TownNpcFbxImport.EnsureMeterContract(fbxPath);
        GameObject src = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (src == null)
            throw new Exception("cannot load " + fbxPath);

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(src);
        instance.name = item.asset;
        int entityLayer = LayerMask.NameToLayer("Entity");
        if (entityLayer >= 0)
            SetLayer(instance, entityLayer);

        if (item.humanoid)
        {
            try { L2TownNpcFbxImport.AssertHumanoidHeight(instance, item.asset); }
            catch (Exception e)
            {
                UnityEngine.Object.DestroyImmediate(instance);
                throw e;
            }
        }

        Dictionary<string, Material> mats = BindMaterials(item, shader);
        AssignMaterials(instance, mats);
        EnsureNpcComponents(instance, item.humanoid);
        var clips = ExtractClips(item.asset, fbxPath, folder + "/Model");
        Animator animator = instance.GetComponent<Animator>();
        if (animator != null && clips.Count > 0)
        {
            AnimatorOverrideController ov = LoadOrCreateOverride(item.asset, basic);
            ApplyOverrides(ov, clips);
            animator.runtimeAnimatorController = ov;
            animator.applyRootMotion = false;
        }

        string prefabPath = folder + "/" + item.asset + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        UnityEngine.Object.DestroyImmediate(instance);
        return item.asset + " clips=" + clips.Count + " mats=" + mats.Count;
    }

    static void EnsureMaterials(Item item, Shader shader)
    {
        BindMaterials(item, shader);
    }

    static Dictionary<string, Material> BindMaterials(Item item, Shader shader)
    {
        var map = new Dictionary<string, Material>();
        Directory.CreateDirectory(TexRoot + "/Materials");
        foreach (string suf in Suffixes)
        {
            string matPath = TexRoot + "/Materials/" + item.asset + suf + ".mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            Texture2D tex = LoadTex(item, suf);
            if (tex == null && mat == null)
                continue;
            if (mat == null)
            {
                if (shader == null || tex == null)
                    continue;
                mat = new Material(shader) { name = item.asset + suf };
                if (mat.HasProperty("_BaseMap"))
                    mat.SetTexture("_BaseMap", tex);
                if (mat.HasProperty("_MainTex"))
                    mat.SetTexture("_MainTex", tex);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            else
            {
                if (shader != null)
                    mat.shader = shader;
                if (tex != null)
                {
                    if (mat.HasProperty("_BaseMap"))
                        mat.SetTexture("_BaseMap", tex);
                    if (mat.HasProperty("_MainTex"))
                        mat.SetTexture("_MainTex", tex);
                }
                EditorUtility.SetDirty(mat);
            }

            map[suf] = mat;
        }

        // Also create leaf-named mats in the npcgrp package so GetNpc can bind them.
        if (item.textures != null)
        {
            for (int i = 0; i < item.textures.Length; i++)
            {
                string key = item.textures[i];
                if (!SysTextureKey.TrySplit(key, out string pkg, out string asset))
                    continue;
                string texPath = FindTexAsset("Assets/Resources/Data/SysTextures/" + pkg, asset);
                Texture2D tex = texPath != null ? AssetDatabase.LoadAssetAtPath<Texture2D>(texPath) : null;
                if (tex == null)
                    continue;
                string matDir = "Assets/Resources/Data/SysTextures/" + pkg + "/Materials";
                Directory.CreateDirectory(matDir);
                string matPath = matDir + "/" + asset + ".mat";
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (mat == null && shader != null)
                {
                    mat = new Material(shader) { name = asset };
                    if (mat.HasProperty("_BaseMap"))
                        mat.SetTexture("_BaseMap", tex);
                    if (mat.HasProperty("_MainTex"))
                        mat.SetTexture("_MainTex", tex);
                    AssetDatabase.CreateAsset(mat, matPath);
                }
            }
        }

        return map;
    }

    static Texture2D LoadTex(Item item, string suf)
    {
        string[] folders =
        {
            TexRoot,
            "Assets/Resources/Data/SysTextures/LineageNPCsTex",
            "Assets/Resources/Data/SysTextures/LineageNpcsTex3"
        };
        string[] stems = { item.asset + suf };
        string meshBase = MeshBaseName(item.asset);
        if (!string.Equals(meshBase, item.asset, StringComparison.Ordinal))
            stems = Append(stems, meshBase + suf);
        if (item.textures != null)
        {
            for (int i = 0; i < item.textures.Length; i++)
            {
                if (!SysTextureKey.TrySplit(item.textures[i], out _, out string leaf))
                    continue;
                string leafAlbedo = StripShaderSuffix(leaf);
                if (leaf.EndsWith(suf, StringComparison.OrdinalIgnoreCase) ||
                    leafAlbedo.EndsWith(suf, StringComparison.OrdinalIgnoreCase))
                {
                    stems = Append(stems, leaf);
                    stems = Append(stems, leafAlbedo);
                }
            }
        }

        for (int f = 0; f < folders.Length; f++)
        {
            for (int s = 0; s < stems.Length; s++)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(folders[f] + "/" + stems[s] + ".png");
                if (tex != null)
                    return tex;
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(folders[f] + "/" + stems[s] + ".tga");
                if (tex != null)
                    return tex;
            }
        }

        return null;
    }

    static string StripShaderSuffix(string leaf)
    {
        if (leaf.EndsWith("_sh", StringComparison.OrdinalIgnoreCase))
            return leaf.Substring(0, leaf.Length - 3);
        return leaf;
    }

    static string MeshBaseName(string asset)
    {
        int cut = asset.LastIndexOf('_');
        if (cut <= 0 || cut >= asset.Length - 1)
            return asset;
        if (asset[cut + 1] != 'm' && asset[cut + 1] != 'M')
            return asset;
        for (int i = cut + 2; i < asset.Length; i++)
        {
            if (!char.IsDigit(asset[i]))
                return asset;
        }
        return asset.Substring(0, cut);
    }

    static string[] Append(string[] src, string extra)
    {
        for (int i = 0; i < src.Length; i++)
            if (src[i] == extra)
                return src;
        var next = new string[src.Length + 1];
        Array.Copy(src, next, src.Length);
        next[src.Length] = extra;
        return next;
    }

    static string FindTexAsset(string folder, string asset)
    {
        string png = folder + "/" + asset + ".png";
        if (File.Exists(Abs(png)))
            return png;
        string tga = folder + "/" + asset + ".tga";
        if (File.Exists(Abs(tga)))
            return tga;
        return null;
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
            else if (n.Contains("_t00") || n.EndsWith("t00"))
                mats.TryGetValue("_t00", out pick);
            else if (n.Contains("_t01") || n.EndsWith("t01"))
                mats.TryGetValue("_t01", out pick);

            if (pick == null && mats.TryGetValue("_t00_b00", out var body))
                pick = body;
            if (pick == null && mats.TryGetValue("_t00", out var t00))
                pick = t00;
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
        AnimationClip walk = Find("walk");
        AnimationClip run = Find("run");

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
            else if (slot.Contains("walk"))
                o = walk ?? wait;
            else if (slot.Contains("run"))
                o = run ?? walk ?? wait;
            else if (slot.Contains("spwait") || slot.Contains("atkwait") || slot.Contains("wait")
                     || slot.Contains("atk"))
                o = (slot.Contains("spwait") ? spwait : null) ?? wait;
            next.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o));
        }

        ov.ApplyOverrides(next);
        EditorUtility.SetDirty(ov);
    }

    static void EnsureNpcComponents(GameObject root, bool humanoid)
    {
        if (root.GetComponent<Animator>() == null)
            root.AddComponent<Animator>();
        if (root.GetComponent<NetworkAnimationController>() == null)
            root.AddComponent<NetworkAnimationController>();
        if (root.GetComponent<CharacterController>() == null)
        {
            var cc = root.AddComponent<CharacterController>();
            cc.height = humanoid ? 1.8f : 0.6f;
            cc.radius = humanoid ? 0.35f : 0.25f;
            cc.center = new Vector3(0f, humanoid ? 0.9f : 0.3f, 0f);
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
            go.transform.localPosition = new Vector3(0f, humanoid ? 0.9f : 0.3f, 0f);
            var col = go.AddComponent<CapsuleCollider>();
            col.height = humanoid ? 1.8f : 0.6f;
            col.radius = humanoid ? 0.4f : 0.3f;
            col.isTrigger = true;
            go.tag = "Npc";
            int layer = LayerMask.NameToLayer("EntityClick");
            go.layer = layer >= 0 ? layer : 0;
        }
        else
            click.gameObject.tag = "Npc";
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

    static string Abs(string assetPath)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), assetPath);
    }
}
#endif
