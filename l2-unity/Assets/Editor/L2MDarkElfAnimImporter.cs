#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Dark elf male (MDarkElf) animations from the original client DarkElf.ukx.
/// Sequence names match the other races; the curves are the dark elf skeleton.
/// Hit events and override slots come from FDarkElf, which already has this set.
/// Unity -batchmode -quit -executeMethod L2MDarkElfAnimImporter.RunCli
/// </summary>
public static class L2MDarkElfAnimImporter
{
    const string Src = "Assets/L2ImportTmp/MDarkElf_anim.fbx";
    const string Root = "Assets/Resources/Data/Animations/";
    const string ClipDir = Root + "DarkElf/MDarkElf/Clips/";
    const string RefFbx = Root + "DarkElf/MDarkElf/Models/anim/MDarkElf_anim.fbx";
    const string SourceOc = Root + "Animator/Player/FDarkElf.overrideController";
    const string DestOc = Root + "Animator/Player/MDarkElf.overrideController";
    const string PlayerPrefab = Root + "DarkElf/MDarkElf/Player_MDarkElf.prefab";

    [MenuItem("Shnok/Characters/Import MDarkElf animations (DarkElf.ukx)")]
    public static void Menu() { Debug.Log(Run()); }

    public static void RunCli()
    {
        string r; bool ok = true;
        try { r = Run(); } catch (Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[MDarkElfAnim] " + r);
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

    static string SlotSeq(string n)
    {
        int i = n.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase); if (i >= 0) n = n.Substring(i + 4);
        int j = n.LastIndexOf('_'); if (j > 0) n = n.Substring(0, j);
        n = n.ToLowerInvariant();
        if (n.EndsWith("_a")) n = n.Substring(0, n.Length - 2);
        return n;
    }

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
        var refImp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(RefFbx);
        var imp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(Src);
        if (imp == null) throw new Exception("missing " + Src);
        imp.globalScale = refImp.globalScale; imp.useFileScale = refImp.useFileScale; imp.useFileUnits = refImp.useFileUnits;
        imp.bakeAxisConversion = refImp.bakeAxisConversion;
        imp.animationType = ModelImporterAnimationType.Generic; imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true; imp.resampleCurves = true; imp.animationCompression = refImp.animationCompression;
        imp.SaveAndReimport();

        var refGo = AssetDatabase.LoadAssetAtPath<GameObject>(RefFbx);
        var map = new Dictionary<string, string>();
        foreach (var t in refGo.GetComponentsInChildren<Transform>(true))
            map[t.name.ToLowerInvariant()] = AnimationUtility.CalculateTransformPath(t, refGo.transform);

        var male = new Dictionary<string, AnimationClip>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { Root + "DarkElf/FDarkElf/Clips" }))
        {
            var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
            male[Seq(c.name, "FDarkElf_m000_b.ao_", "_FDarkElf")] = c;
        }
        var existing = new Dictionary<string, string>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { ClipDir.TrimEnd('/') }))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            existing[Seq(Path.GetFileNameWithoutExtension(p), "MDarkElf_m000_b.ao_", "_MDarkElf")] = p;
        }

        var felf = new Dictionary<string, AnimationClip>();
        int unmapped = 0;
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(Src).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, "MDarkElf_m000_b.ao|", "_MDarkElf");
            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_")) raw = raw.Substring(3);
            var dst = new AnimationClip { frameRate = src.frameRate };
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0 ? "MDarkElf_m000_b.ao" : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np)) { unmapped++; continue; }
                var nb = b; nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
            }
            AnimationClip maleRef; male.TryGetValue(seq, out maleRef);
            bool willLoop = maleRef != null ? AnimationUtility.GetAnimationClipSettings(maleRef).loopTime
                : (seq.StartsWith("wait") || seq.StartsWith("walk") || seq.StartsWith("run") || seq.StartsWith("atkwait")
                   || seq.Contains("sitwait") || seq.Contains("deathwait") || seq.StartsWith("swim"));
            if (willLoop && !seq.StartsWith("cast") && !seq.StartsWith("magic")) CloseLoop(dst, 1f / Mathf.Max(1f, src.frameRate));
            var st = AnimationUtility.GetAnimationClipSettings(dst);
            st.loopTime = willLoop; st.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(dst, st);
            if (male.TryGetValue(seq, out var mm) && mm.length > 0)
            {
                float k = dst.length / mm.length;
                var ev = AnimationUtility.GetAnimationEvents(mm).Select(e => new AnimationEvent
                {
                    functionName = e.functionName, time = e.time * k, floatParameter = e.floatParameter,
                    intParameter = e.intParameter, stringParameter = e.stringParameter, objectReferenceParameter = e.objectReferenceParameter
                }).ToArray();
                AnimationUtility.SetAnimationEvents(dst, ev);
            }
            string path = existing.TryGetValue(seq, out var ep) ? ep : ClipDir + "MDarkElf_m000_b.ao_" + raw + ".anim";
            var old = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (old != null) { EditorUtility.CopySerialized(dst, old); old.name = Path.GetFileNameWithoutExtension(path); EditorUtility.SetDirty(old); felf[seq] = old; }
            else { dst.name = Path.GetFileNameWithoutExtension(path); AssetDatabase.CreateAsset(dst, path); felf[seq] = dst; }
        }
        log.Add("clips " + felf.Count + " (unmapped bindings " + unmapped + ")");

        var srcOc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(SourceOc);
        var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(DestOc);
        oc.runtimeAnimatorController = srcOc.runtimeAnimatorController;
        var srcPairs = new List<KeyValuePair<AnimationClip, AnimationClip>>(); srcOc.GetOverrides(srcPairs);
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>(); var miss = new List<string>();
        foreach (var kv in srcPairs)
        {
            AnimationClip o = null;
            string ms = kv.Value != null ? Seq(kv.Value.name, "FDarkElf_m000_b.ao_", "_FDarkElf") : "";
            // Female package uses SpAtk001; DarkElf.ukx uses SpAtk01.
            string alias = ms.Replace("spatk00", "spatk0");
            if (kv.Value != null && (felf.TryGetValue(ms, out var c) || felf.TryGetValue(alias, out c))) o = c;
            if (o == null && felf.TryGetValue(SlotSeq(kv.Key.name), out var c2)) o = c2;
            if (o == null && kv.Value != null) miss.Add(kv.Value.name);
            pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o));
        }
        oc.ApplyOverrides(pairs); EditorUtility.SetDirty(oc);
        log.Add("override slots " + pairs.Count(p => p.Value != null) + "/" + pairs.Count + (miss.Count > 0 ? " missing: " + string.Join(",", miss) : ""));

        var pr = PrefabUtility.LoadPrefabContents(PlayerPrefab);
        int fixedA = 0;
        foreach (var a in pr.GetComponentsInChildren<Animator>(true)) { a.runtimeAnimatorController = oc; fixedA++; }
        PrefabUtility.SaveAsPrefabAsset(pr, PlayerPrefab); PrefabUtility.UnloadPrefabContents(pr);
        log.Add("Player_MDarkElf animators -> MDarkElf: " + fixedA);

        AssetDatabase.SaveAssets();
        AssetDatabase.DeleteAsset(Src);
        return string.Join(" | ", log);
    }
}
#endif
