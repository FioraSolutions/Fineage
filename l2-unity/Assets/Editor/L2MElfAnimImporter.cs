#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Light elf male (MElf) animations from the original client, same layout as MMagic:
/// Elf.ukx MElf_anim.psa + MElf_m000_b.psk -> (umodel, psk_psa_to_fbx.py) -> Assets/L2ImportTmp/MElf_anim.fbx
/// -> standalone Clips/MElf_m000_b.ao_&lt;Seq&gt;.anim (paths remapped onto MElf_anim_user skeleton),
/// AnimNotify-style events copied from the matching MMagic clip (time-scaled),
/// MElf.overrideController filled from MMagic.overrideController by sequence name, Player_MElf -> MElf controller.
/// Unity -batchmode -quit -executeMethod L2MElfAnimImporter.RunCli
/// </summary>
public static class L2MElfAnimImporter
{
    const string Src = "Assets/L2ImportTmp/MElf_anim.fbx";
    const string Root = "Assets/Resources/Data/Animations/";
    const string ClipDir = Root + "Elf/MElf/Clips/";
    const string RefFbx = Root + "Elf/MElf/Models/anim/MElf_anim_user.fbx";
    const string MMagicOc = Root + "Animator/Player/MMagic.overrideController";
    const string MElfOc = Root + "Animator/Player/MElf.overrideController";
    const string PlayerPrefab = Root + "Elf/MElf/Player_MElf.prefab";

    [MenuItem("Shnok/Characters/Import MElf animations (Elf.ukx)")]
    public static void Menu() { Debug.Log(Run()); }

    public static void RunCli()
    {
        string r; bool ok = true;
        try { r = Run(); } catch (Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[MElfAnim] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    static string Seq(string clipName, string prefix, string suffix)
    {
        string n = clipName;
        int i = n.LastIndexOf('|'); if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(prefix.Length);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
        if (n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(0, n.Length - suffix.Length);
        return n.ToLowerInvariant();
    }

    // "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf" -> "magicshot"
    static string SlotSeq(string n)
    {
        int i = n.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase); if (i >= 0) n = n.Substring(i + 4);
        int j = n.LastIndexOf('_'); if (j > 0) n = n.Substring(0, j);
        n = n.ToLowerInvariant();
        if (n.EndsWith("_a")) n = n.Substring(0, n.Length - 2);
        return n;
    }

    // UE loops interpolate last->first; Unity stops at the last key. Append the first pose one frame later.
    static void CloseLoop(AnimationClip c, float dt)
    {
        var binds = AnimationUtility.GetCurveBindings(c);
        float end = 0f;
        foreach (var b in binds) { var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length > 0) end = Mathf.Max(end, cv[cv.length - 1].time); }
        float t = end + dt;
        var flip = new Dictionary<string, bool>();
        foreach (var g in binds.Where(b => b.propertyName.StartsWith("m_LocalRotation")).GroupBy(b => b.path))
        {
            float dot = 0f;
            foreach (var b in g) { var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length > 0) dot += cv[0].value * cv[cv.length - 1].value; }
            flip[g.Key] = dot < 0f;
        }
        foreach (var b in binds)
        {
            var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length == 0) continue;
            float v = cv[0].value;
            if (b.propertyName.StartsWith("m_LocalRotation") && flip.TryGetValue(b.path, out var f) && f) v = -v;
            cv.AddKey(new Keyframe(t, v));
            AnimationUtility.SetEditorCurve(c, b, cv);
        }
    }

    public static string Run()
    {
        var log = new List<string>();
        // importer settings copied from the existing MElf skeleton FBX
        var refImp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(RefFbx);
        var imp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(Src);
        if (imp == null) throw new Exception("missing " + Src);
        imp.globalScale = refImp.globalScale; imp.useFileScale = refImp.useFileScale; imp.useFileUnits = refImp.useFileUnits;
        imp.bakeAxisConversion = refImp.bakeAxisConversion;
        imp.animationType = ModelImporterAnimationType.Generic; imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true; imp.resampleCurves = true; imp.animationCompression = refImp.animationCompression;
        imp.SaveAndReimport();

        // skeleton path map from the reference anim FBX (leaf name, case-insensitive)
        var refGo = AssetDatabase.LoadAssetAtPath<GameObject>(RefFbx);
        var map = new Dictionary<string, string>();
        foreach (var t in refGo.GetComponentsInChildren<Transform>(true))
            map[t.name.ToLowerInvariant()] = AnimationUtility.CalculateTransformPath(t, refGo.transform);

        // MMagic clips by sequence (for events + override mapping)
        var mmagic = new Dictionary<string, AnimationClip>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { Root + "Magic/MMagic/Clips" }))
        {
            var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
            mmagic[Seq(c.name, "MMagic_M000_b.ao_", "_MMagic")] = c;
        }
        // existing MElf clips (keep their GUIDs, overwrite contents)
        var existing = new Dictionary<string, string>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { ClipDir.TrimEnd('/') }))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            existing[Seq(Path.GetFileNameWithoutExtension(p), "MElf_m000_b.ao_", "_MElf")] = p;
        }

        var melf = new Dictionary<string, AnimationClip>();
        int unmapped = 0;
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(Src).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, "MElf_m000_b.ao|", "_MElf");
            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_")) raw = raw.Substring(3);
            var dst = new AnimationClip { frameRate = src.frameRate };
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0 ? "MElf_m000_b.ao" : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np)) { unmapped++; continue; }
                var nb = b; nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
            }
            bool willLoop = seq.StartsWith("wait") || seq.StartsWith("walk") || seq.StartsWith("run") || seq.StartsWith("atkwait")
                        || seq.Contains("sitwait") || seq.Contains("deathwait") || seq.StartsWith("swim") || seq.Contains("loop")
                        || seq.StartsWith("castmid") || seq.StartsWith("castlong");
            if (willLoop) CloseLoop(dst, 1f / Mathf.Max(1f, src.frameRate));
            var st = AnimationUtility.GetAnimationClipSettings(dst);
            st.loopTime = willLoop; st.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(dst, st);
            if (mmagic.TryGetValue(seq, out var mm) && mm.length > 0)
            {
                float k = dst.length / mm.length;
                var ev = AnimationUtility.GetAnimationEvents(mm).Select(e => new AnimationEvent
                {
                    functionName = e.functionName, time = e.time * k, floatParameter = e.floatParameter,
                    intParameter = e.intParameter, stringParameter = e.stringParameter, objectReferenceParameter = e.objectReferenceParameter
                }).ToArray();
                AnimationUtility.SetAnimationEvents(dst, ev);
            }
            string path = existing.TryGetValue(seq, out var ep) ? ep : ClipDir + "MElf_m000_b.ao_" + raw + ".anim";
            var old = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (old != null) { EditorUtility.CopySerialized(dst, old); old.name = Path.GetFileNameWithoutExtension(path); EditorUtility.SetDirty(old); melf[seq] = old; }
            else { dst.name = Path.GetFileNameWithoutExtension(path); AssetDatabase.CreateAsset(dst, path); melf[seq] = dst; }
        }
        log.Add("clips " + melf.Count + " (unmapped bindings " + unmapped + ")");

        // override controller: same slots as MMagic, MElf clip by sequence
        var mmOc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(MMagicOc);
        var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(MElfOc);
        oc.runtimeAnimatorController = mmOc.runtimeAnimatorController;
        var mmPairs = new List<KeyValuePair<AnimationClip, AnimationClip>>(); mmOc.GetOverrides(mmPairs);
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>(); var miss = new List<string>();
        foreach (var kv in mmPairs)
        {
            AnimationClip o = null;
            string ms = kv.Value != null ? Seq(kv.Value.name, "MMagic_M000_b.ao_", "_MMagic") : "";
            // MMagic uses SpAtk001/002, the elf package SpAtk01/02
            string alias = ms.Replace("spatk00", "spatk0");
            if (kv.Value != null && (melf.TryGetValue(ms, out var c) || melf.TryGetValue(alias, out c))) o = c;
            if (o == null && melf.TryGetValue(SlotSeq(kv.Key.name), out var c2)) o = c2;
            if (o == null && melf.TryGetValue(SlotSeq(kv.Key.name).Replace("spatk00", "spatk0"), out var c3)) o = c3;
            if (o == null && kv.Value != null) miss.Add(kv.Value.name);
            pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o));
        }
        oc.ApplyOverrides(pairs); EditorUtility.SetDirty(oc);
        log.Add("override slots " + pairs.Count(p => p.Value != null) + "/" + pairs.Count + (miss.Count > 0 ? " missing: " + string.Join(",", miss) : ""));

        // Player_MElf used FElf.overrideController
        var pr = PrefabUtility.LoadPrefabContents(PlayerPrefab);
        int fixedA = 0;
        foreach (var a in pr.GetComponentsInChildren<Animator>(true)) { a.runtimeAnimatorController = oc; fixedA++; }
        PrefabUtility.SaveAsPrefabAsset(pr, PlayerPrefab); PrefabUtility.UnloadPrefabContents(pr);
        log.Add("Player_MElf animators -> MElf: " + fixedA);

        // sanity: compare pelvis first-key against the old hand-made wait_1HS
        AssetDatabase.SaveAssets();
        AssetDatabase.DeleteAsset("Assets/L2ImportTmp");
        return string.Join(" | ", log);
    }
}
#endif
