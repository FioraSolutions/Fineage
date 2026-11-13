// fineage-touch 1794037730.455447
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Male Orc fighter animations from Interlude Orc.ukx (same recipe as L2MElfAnimImporter):
/// MOrc_anim.psa + MOrc_m000_b.psk → Blender → Assets/L2ImportTmp/MOrc_anim.fbx
/// → Clips/ + MOrc.overrideController filled from MFighter by sequence name.
/// Also rewires Pawn/User/Player MOrc animators and rebuilds anim skeletons from Models/anim.
/// Unity -batchmode -quit -executeMethod L2MOrcAnimImporter.RunCli
/// </summary>
public static class L2MOrcAnimImporter
{
    const string Root = "Assets/Resources/Data/Animations/";
    const string DonorOc = Root + "Animator/Player/MFighter.overrideController";

    [MenuItem("Shnok/Characters/Import MOrc animations (Orc.ukx)")]
    public static void Menu() { Debug.Log(Run()); }

    public static void RunCli()
    {
        string r;
        bool ok = true;
        try { r = Run(); }
        catch (Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[MOrcAnim] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    static string Seq(string clipName, string prefix, string suffix)
    {
        string n = clipName;
        int i = n.LastIndexOf('|');
        if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(prefix.Length);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
        if (n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(0, n.Length - suffix.Length);
        return n.ToLowerInvariant();
    }

    static string SlotSeq(string n)
    {
        int i = n.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
        if (i >= 0) n = n.Substring(i + 4);
        int j = n.LastIndexOf('_');
        if (j > 0) n = n.Substring(0, j);
        n = n.ToLowerInvariant();
        if (n.EndsWith("_a")) n = n.Substring(0, n.Length - 2);
        return n;
    }

    static void CloseLoop(AnimationClip c, float dt)
    {
        var binds = AnimationUtility.GetCurveBindings(c);
        float end = 0f;
        foreach (var b in binds)
        {
            var cv = AnimationUtility.GetEditorCurve(c, b);
            if (cv.length > 0) end = Mathf.Max(end, cv[cv.length - 1].time);
        }
        float t = end + dt;
        var flip = new Dictionary<string, bool>();
        foreach (var g in binds.Where(b => b.propertyName.StartsWith("m_LocalRotation")).GroupBy(b => b.path))
        {
            float dot = 0f;
            foreach (var b in g)
            {
                var cv = AnimationUtility.GetEditorCurve(c, b);
                if (cv.length > 0) dot += cv[0].value * cv[cv.length - 1].value;
            }
            flip[g.Key] = dot < 0f;
        }
        foreach (var b in binds)
        {
            var cv = AnimationUtility.GetEditorCurve(c, b);
            if (cv.length == 0) continue;
            float v = cv[0].value;
            if (b.propertyName.StartsWith("m_LocalRotation") && flip.TryGetValue(b.path, out var f) && f)
                v = -v;
            cv.AddKey(new Keyframe(t, v));
            AnimationUtility.SetEditorCurve(c, b, cv);
        }
    }

    public static string Run() => RunRace("MOrc", "Orc");

    /// <summary>Female orc and both shamans: the fighter already has clips; these override controllers are empty.</summary>
    public static string RunFamily()
    {
        return "FOrc " + RunRace("FOrc", "Orc")
            + " || MShaman " + RunRace("MShaman", "Shaman")
            + " || FShaman " + RunRace("FShaman", "Shaman");
    }

    /// <summary>
    /// Same recipe as the male orc fighter: clips from {race}_anim.fbx, override filled from MFighter by sequence name.
    /// Clip pack stays under Orc/{race}/Clips (CharacterAnimPaths). Skeleton FBX lives in {package}/{race}/Models/anim.
    /// </summary>
    public static string RunRace(string race, string package)
    {
        string Src = Root + package + "/" + race + "/Models/anim/" + race + "_anim.fbx";
        string ClipDir = Root + "Orc/" + race + "/Clips/";
        string RefFbx = Src;
        string raceOc = Root + "Animator/Player/" + race + ".overrideController";
        string[] Prefabs =
        {
            Root + "Orc/" + race + "/Player_" + race + ".prefab",
            Root + "Orc/" + race + "/Pawn_" + race + ".prefab",
            Root + "Orc/" + race + "/User_" + race + ".prefab",
            Root + "Orc/" + race + "/" + race + "_anim_player.prefab",
            Root + "Orc/" + race + "/" + race + "_anim_user.prefab",
        };
        string meshAo = race + "_m000_b.ao";
        string meshAoCap = race + "_M000_b.ao";

        var log = new List<string>();
        Directory.CreateDirectory(ClipDir.TrimEnd('/').Replace("Assets/", Application.dataPath + "/"));

        if (AssetDatabase.LoadAssetAtPath<GameObject>(Src) == null)
            throw new Exception("missing " + Src + " — convert " + race + "_anim.psa first");
        if (AssetDatabase.LoadAssetAtPath<GameObject>(RefFbx) == null)
            throw new Exception("missing " + RefFbx);

        // Match importer settings to a known-good fighter anim FBX when present.
        string settingsDonor = Root + "Fighter/MFighter/Models/anim/MFighter_anim.fbx";
        // Project has a global AssetImporter base class for terrain tools — use global::UnityEditor.
        var donorImp = (ModelImporter)global::UnityEditor.AssetImporter.GetAtPath(settingsDonor);
        foreach (string path in new[] { Src, RefFbx })
        {
            var imp = (ModelImporter)global::UnityEditor.AssetImporter.GetAtPath(path);
            if (imp == null) continue;
            if (donorImp != null)
            {
                imp.globalScale = donorImp.globalScale;
                imp.useFileScale = donorImp.useFileScale;
                imp.useFileUnits = donorImp.useFileUnits;
                imp.bakeAxisConversion = donorImp.bakeAxisConversion;
                imp.animationCompression = donorImp.animationCompression;
            }
            imp.animationType = ModelImporterAnimationType.Generic;
            imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
            imp.importAnimation = true;
            imp.resampleCurves = true;
            imp.SaveAndReimport();
        }

        var refGo = AssetDatabase.LoadAssetAtPath<GameObject>(RefFbx);
        var map = new Dictionary<string, string>();
        foreach (var t in refGo.GetComponentsInChildren<Transform>(true))
            map[t.name.ToLowerInvariant()] = AnimationUtility.CalculateTransformPath(t, refGo.transform);

        // Donor clips (MFighter) for loop flags + AnimNotify events.
        var donor = new Dictionary<string, AnimationClip>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { Root + "Fighter/MFighter/Clips" }))
        {
            var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
            if (c == null) continue;
            donor[Seq(c.name, "MFighter_m000_b.ao_", "_MFighter")] = c;
            donor[Seq(c.name, "MFighter_M000_b.ao_", "_MFighter")] = c;
        }

        var existing = new Dictionary<string, string>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { ClipDir.TrimEnd('/') }))
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            string fileSeq = Seq(Path.GetFileNameWithoutExtension(p), meshAo + "_", "_" + race);
            if (fileSeq == Path.GetFileNameWithoutExtension(p).ToLowerInvariant())
                fileSeq = Seq(Path.GetFileNameWithoutExtension(p), meshAoCap + "_", "_" + race);
            existing[fileSeq] = p;
        }

        var morc = new Dictionary<string, AnimationClip>();
        int unmapped = 0;
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(Src).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, meshAo + "|", "_" + race);
            if (string.IsNullOrEmpty(seq) || seq == src.name.ToLowerInvariant())
                seq = Seq(src.name, meshAoCap + "|", "_" + race);
            // Blender PSA names are often just "ao_Wait_Hand_{race}" without the mesh prefix.
            if (string.IsNullOrEmpty(seq) || seq == src.name.ToLowerInvariant())
                seq = Seq(src.name, "", "_" + race);
            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_")) raw = raw.Substring(3);
            string raceSuffix = "_" + race;
            if (raw.EndsWith(raceSuffix, StringComparison.OrdinalIgnoreCase))
                raw = raw.Substring(0, raw.Length - raceSuffix.Length);

            var dst = new AnimationClip { frameRate = src.frameRate };
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0
                    ? meshAo
                    : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np))
                {
                    unmapped++;
                    continue;
                }
                var nb = b;
                nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
            }

            donor.TryGetValue(seq, out var donorRef);
            bool willLoop = donorRef != null
                ? AnimationUtility.GetAnimationClipSettings(donorRef).loopTime
                : (seq.StartsWith("wait") || seq.StartsWith("walk") || seq.StartsWith("run")
                   || seq.StartsWith("atkwait") || seq.Contains("sitwait")
                   || seq.Contains("deathwait") || seq.StartsWith("swim") || seq == "stand");
            if (willLoop && !seq.StartsWith("cast") && !seq.StartsWith("magic") && !seq.StartsWith("atk0"))
                CloseLoop(dst, 1f / Mathf.Max(1f, src.frameRate));
            var st = AnimationUtility.GetAnimationClipSettings(dst);
            st.loopTime = willLoop;
            st.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(dst, st);

            if (donorRef != null && donorRef.length > 0)
            {
                float k = dst.length / donorRef.length;
                var ev = AnimationUtility.GetAnimationEvents(donorRef).Select(e => new AnimationEvent
                {
                    functionName = e.functionName,
                    time = e.time * k,
                    floatParameter = e.floatParameter,
                    intParameter = e.intParameter,
                    stringParameter = e.stringParameter,
                    objectReferenceParameter = e.objectReferenceParameter
                }).ToArray();
                AnimationUtility.SetAnimationEvents(dst, ev);
            }

            string path = existing.TryGetValue(seq, out var ep)
                ? ep
                : ClipDir + meshAo + "_" + raw + "_" + race + ".anim";
            var old = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (old != null)
            {
                EditorUtility.CopySerialized(dst, old);
                old.name = Path.GetFileNameWithoutExtension(path);
                EditorUtility.SetDirty(old);
                morc[seq] = old;
            }
            else
            {
                dst.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(dst, path);
                morc[seq] = dst;
            }
        }
        log.Add("clips " + morc.Count + " (unmapped bindings " + unmapped + ")");

        var donorOc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(DonorOc);
        var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(raceOc);
        if (donorOc == null || oc == null)
            throw new Exception("missing override controllers");
        oc.runtimeAnimatorController = donorOc.runtimeAnimatorController;
        var donorPairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        donorOc.GetOverrides(donorPairs);
        var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        var miss = new List<string>();
        foreach (var kv in donorPairs)
        {
            AnimationClip o = null;
            string ms = kv.Value != null
                ? Seq(kv.Value.name, "MFighter_m000_b.ao_", "_MFighter")
                : "";
            if (string.IsNullOrEmpty(ms) && kv.Value != null)
                ms = Seq(kv.Value.name, "MFighter_M000_b.ao_", "_MFighter");
            if (kv.Value != null && morc.TryGetValue(ms, out var c))
                o = c;
            if (o == null && morc.TryGetValue(SlotSeq(kv.Key.name), out var c2))
                o = c2;
            if (o == null && kv.Value != null)
                miss.Add(kv.Value.name);
            pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o));
        }
        oc.ApplyOverrides(pairs);
        EditorUtility.SetDirty(oc);
        log.Add("override slots " + pairs.Count(p => p.Value != null) + "/" + pairs.Count
            + (miss.Count > 0 ? " missing: " + string.Join(",", miss.Take(8)) : ""));

        int fixedA = 0;
        foreach (string prefab in Prefabs)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefab) == null)
                continue;
            var pr = PrefabUtility.LoadPrefabContents(prefab);
            foreach (var a in pr.GetComponentsInChildren<Animator>(true))
            {
                a.runtimeAnimatorController = oc;
                fixedA++;
            }
            PrefabUtility.SaveAsPrefabAsset(pr, prefab);
            PrefabUtility.UnloadPrefabContents(pr);
        }
        log.Add("animators wired: " + fixedA);

        // Rebuild anim skeletons from the real anim FBX (not armor m001_u).
        try
        {
            L2OrcRacePrefabBuilder.BuildRaces(race);
            log.Add("skeletons rebuilt");
        }
        catch (Exception ex)
        {
            log.Add("skeleton rebuild skipped: " + ex.Message);
        }

        AssetDatabase.SaveAssets();
        return string.Join(" | ", log);
    }
}
#endif
