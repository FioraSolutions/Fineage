#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Player_MElf was a copy of the FElf player (inline FElf_m000_b.ao skeleton), so MElf clips
/// (paths MElf_m000_b.ao/...) never bound -> frozen body. Rebuild it the MMagic way:
/// MElf_anim_player.prefab = MElf_anim_user + PlayerAnimationController/CharacterAnimationAudioHandler (from MMagic_anim_player),
/// Player_MElf = Player_MMagic root with the nested MMagic_anim_player swapped for MElf_anim_player.
/// Unity -batchmode -quit -executeMethod L2MElfPlayerPrefabBuilder.RunCli
/// </summary>
public static class L2MElfPlayerPrefabBuilder
{
    const string R = "Assets/Resources/Data/Animations/";
    const string MElfUser = R + "Elf/MElf/Models/anim/MElf_anim_user.prefab";
    const string MElfAnimPlayer = R + "Elf/MElf/MElf_anim_player.prefab";
    const string MMagicAnimPlayer = R + "Magic/MMagic/MMagic_anim_player.prefab";
    const string PlayerMMagic = R + "Human/MMagic/Player_MMagic.prefab";
    const string PlayerMElf = R + "Elf/MElf/Player_MElf.prefab";
    const string MElfOc = R + "Animator/Player/MElf.overrideController";

    [MenuItem("Shnok/Characters/Rebuild Player_MElf (MMagic layout)")]
    public static void Menu() { Debug.Log(Run()); }

    public static void RunCli()
    {
        string r; bool ok = true;
        try { r = Run(); } catch (Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[MElfPlayer] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static string Run()
    {
        var log = new List<string>();
        var oc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(MElfOc);

        // 0) MElf_anim_user.prefab was an empty holder (no skeleton/mesh) -> SkinnedMeshSync NRE + clips bind nothing.
        //    Add MElf_m000_b.ao + MElf_m000_b.mo from MElf_anim_user.fbx, like MMagic_anim_* (root children .ao/.mo).
        {
            var u = PrefabUtility.LoadPrefabContents(MElfUser);
            try { log.Add("MElf_anim_user skeleton: " + EnsureSkeleton(u)); PrefabUtility.SaveAsPrefabAsset(u, MElfUser); }
            finally { PrefabUtility.UnloadPrefabContents(u); }
        }

        // 1) MElf_anim_player
        var mmAp = AssetDatabase.LoadAssetAtPath<GameObject>(MMagicAnimPlayer);
        var ap = PrefabUtility.LoadPrefabContents(MElfUser);
        try
        {
            foreach (var src in mmAp.GetComponents<MonoBehaviour>())
            {
                if (src == null) continue;
                var t = src.GetType();
                var dst = ap.GetComponent(t) ?? ap.AddComponent(t);
                EditorUtility.CopySerialized(src, dst);
            }
            var anim = ap.GetComponent<Animator>();
            if (anim == null) throw new Exception("MElf_anim_user has no Animator on root");
            anim.runtimeAnimatorController = oc;
            var mmAnim = mmAp.GetComponent<Animator>();
            if (mmAnim != null) { anim.applyRootMotion = mmAnim.applyRootMotion; anim.cullingMode = mmAnim.cullingMode; anim.updateMode = mmAnim.updateMode; }
            // re-point object refs that pointed into MMagic_anim_player at our own components
            foreach (var mb in ap.GetComponents<MonoBehaviour>()) Repoint(mb, mmAp, ap);
            ap.name = "MElf_anim_player";
            PrefabUtility.SaveAsPrefabAsset(ap, MElfAnimPlayer);
            log.Add("MElf_anim_player: " + string.Join(",", ap.GetComponents<Component>().Select(c => c.GetType().Name)));
        }
        finally { PrefabUtility.UnloadPrefabContents(ap); }

        // 2) Player_MElf from Player_MMagic
        var apAsset = AssetDatabase.LoadAssetAtPath<GameObject>(MElfAnimPlayer);
        var root = PrefabUtility.LoadPrefabContents(PlayerMMagic);
        try
        {
            GameObject oldInst = null;
            foreach (Transform t in root.transform)
                if (PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) != null &&
                    AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject)) == MMagicAnimPlayer)
                { oldInst = t.gameObject; break; }
            if (oldInst == null) throw new Exception("MMagic_anim_player instance not found in Player_MMagic");
            var ni = (GameObject)PrefabUtility.InstantiatePrefab(apAsset, root.transform);
            ni.transform.localPosition = oldInst.transform.localPosition;
            ni.transform.localRotation = oldInst.transform.localRotation;
            ni.transform.localScale = oldInst.transform.localScale;
            ni.transform.SetSiblingIndex(oldInst.transform.GetSiblingIndex());
            ni.tag = oldInst.tag; ni.layer = oldInst.layer;
            int rp = 0;
            foreach (var mb in root.GetComponents<MonoBehaviour>()) rp += Repoint(mb, oldInst, ni);
            UnityEngine.Object.DestroyImmediate(oldInst);
            int raceSet = 0;
            foreach (var mb in root.GetComponents<MonoBehaviour>())
            {
                if (mb == null) continue;
                var so = new SerializedObject(mb);
                var p = so.FindProperty("_race"); if (p != null) { p.intValue = (int)CharacterRace.Elf; raceSet++; }
                var q = so.FindProperty("_raceId"); if (q != null) { q.intValue = (int)CharacterRaceAnimation.MElf; raceSet++; }
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            root.name = "Player_MElf";
            PrefabUtility.SaveAsPrefabAsset(root, PlayerMElf);
            log.Add("Player_MElf rebuilt (refs repointed " + rp + ", race fields " + raceSet + ")");
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
        AssetDatabase.SaveAssets();
        return string.Join(" | ", log);
    }

    const string MElfFbx = R + "Elf/MElf/Models/anim/MElf_anim_user.fbx";
    static string EnsureSkeleton(GameObject root)
    {
        if (root.GetComponentInChildren<SkinnedMeshRenderer>(true) != null) return "already";
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(MElfFbx);
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        PrefabUtility.UnpackPrefabInstance(inst, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        var kids = new List<Transform>(); foreach (Transform c in inst.transform) kids.Add(c);
        foreach (var c in kids) c.SetParent(root.transform, false);
        UnityEngine.Object.DestroyImmediate(inst);
        return "added " + string.Join(",", kids.Select(k => k.name));
    }

    // object refs into `from` hierarchy -> same-named/same-type object under `to`
    static int Repoint(UnityEngine.Object obj, GameObject from, GameObject to)
    {
        if (obj == null) return 0;
        int n = 0;
        var so = new SerializedObject(obj); var it = so.GetIterator();
        while (it.NextVisible(true))
        {
            if (it.propertyType != SerializedPropertyType.ObjectReference || it.objectReferenceValue == null) continue;
            var v = it.objectReferenceValue;
            GameObject g = v is GameObject go ? go : (v as Component)?.gameObject;
            if (g == null) continue;
            var src = PrefabUtility.GetCorrespondingObjectFromSource(g) ?? g;
            bool inFrom = g.transform.IsChildOf(from.transform) || (src != null && src.transform.root.gameObject == from);
            if (!inFrom) continue;
            Transform tgt = g == from || src == from ? to.transform : FindDeep(to.transform, g.name);
            if (tgt == null) continue;
            UnityEngine.Object nv = v is GameObject ? (UnityEngine.Object)tgt.gameObject : tgt.GetComponent(v.GetType());
            if (nv != null) { it.objectReferenceValue = nv; n++; }
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        return n;
    }

    static Transform FindDeep(Transform t, string name)
    {
        string a = name.Replace("MMagic", "MElf").ToLowerInvariant();
        foreach (var c in t.GetComponentsInChildren<Transform>(true))
            if (c.name.ToLowerInvariant() == a || c.name.ToLowerInvariant() == name.ToLowerInvariant()) return c;
        return null;
    }
}
#endif
