#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Pulls the female sequences that were never imported (FFighter, FMagic, FDarkElf)
/// from Assets/L2ImportTmp/{Race}_anim.fbx onto the bone paths already used by that
/// race's clips, then fills the player override controller from the male template.
/// </summary>
public static class L2FemaleAnimImporter
{
    const string Root = "Assets/Resources/Data/Animations/";

    struct Race
    {
        public string Name;
        public string ClipDir;
        public string MaleClipPrefix;
        public string MaleClipSuffix;
        public string MaleOc;
        public string DestOc;
        public string PlayerPrefab;
        public string RootClip;
    }

    static readonly Race[] Races =
    {
        new Race
        {
            Name = "FFighter",
            ClipDir = Root + "Fighter/FFighter/Clips/",
            MaleClipPrefix = "MFighter_m000_b.ao_",
            MaleClipSuffix = "_MFighter",
            MaleOc = Root + "Animator/Player/MFighter.overrideController",
            DestOc = Root + "Animator/Player/FFighter.overrideController",
            PlayerPrefab = Root + "Fighter/FFighter/Player_FFighter.prefab"
        },
        new Race
        {
            Name = "FMagic",
            ClipDir = Root + "Magic/FMagic/Clips/",
            MaleClipPrefix = "MMagic_M000_b.ao_",
            MaleClipSuffix = "_MMagic",
            MaleOc = Root + "Animator/Player/MMagic.overrideController",
            DestOc = Root + "Animator/Player/FMagic.overrideController",
            PlayerPrefab = Root + "Magic/FMagic/Player_FMagic.prefab"
        },
        new Race
        {
            Name = "FDarkElf",
            ClipDir = Root + "DarkElf/FDarkElf/Clips/",
            MaleClipPrefix = "MDarkElf_m000_b.ao_",
            MaleClipSuffix = "_MDarkElf",
            MaleOc = Root + "Animator/Player/MDarkElf.overrideController",
            DestOc = Root + "Animator/Player/FDarkElf.overrideController",
            PlayerPrefab = Root + "DarkElf/FDarkElf/Player_FDarkElf.prefab"
        },
        new Race
        {
            Name = "FDwarf",
            ClipDir = Root + "Dwarf/FDwarf/Clips/",
            MaleClipPrefix = "MDwarf_m000_b.ao_",
            MaleClipSuffix = "_MDwarf",
            MaleOc = Root + "Animator/Player/MDwarf.overrideController",
            DestOc = Root + "Animator/Player/FDwarf.overrideController",
            PlayerPrefab = Root + "Dwarf/FDwarf/Player_FDwarf.prefab",
            RootClip = "FDwarf_m001_b.ao"
        },
        new Race
        {
            Name = "MDwarf",
            ClipDir = Root + "Dwarf/MDwarf/Clips/",
            MaleClipPrefix = "MDwarf_m000_b.ao_",
            MaleClipSuffix = "_MDwarf",
            MaleOc = Root + "Animator/Player/MDwarf.overrideController",
            DestOc = Root + "Animator/Player/MDwarf.overrideController",
            PlayerPrefab = Root + "Dwarf/MDwarf/Player_MDwarf.prefab",
            RootClip = "MDwarf_m000_b.ao"
        }
    };

    public static void RunCli()
    {
        string r; bool ok = true;
        try { r = Run(); } catch (Exception e) { r = e.ToString(); ok = false; }
        Debug.Log("[FemaleAnim] " + r);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    static string Seq(string clipName, string prefix, string suffix)
    {
        string n = clipName;
        int i = n.LastIndexOf('|'); if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(prefix.Length);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
        if (!string.IsNullOrEmpty(suffix) && n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            n = n.Substring(0, n.Length - suffix.Length);
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
            if (b.propertyName.StartsWith("m_LocalRotation") && flip.TryGetValue(b.path, out var f) && f) v = -v;
            cv.AddKey(new Keyframe(t, v));
            AnimationUtility.SetEditorCurve(c, b, cv);
        }
    }

    static Dictionary<string, string> BonePaths(string clipDir, string rootName)
    {
        var map = new Dictionary<string, string> { [rootName.ToLowerInvariant()] = rootName };
        if (!AssetDatabase.IsValidFolder(clipDir.TrimEnd('/')))
            return map;
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { clipDir.TrimEnd('/') }))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
            if (clip == null) continue;
            foreach (var b in AnimationUtility.GetCurveBindings(clip))
            {
                if (string.IsNullOrEmpty(b.path)) continue;
                // Clips must address the skeleton root (FDarkElf_m000_b.ao). A donor
                // path under m001 never hits that hierarchy and the body stays in bind.
                if (!b.path.Equals(rootName, StringComparison.Ordinal)
                    && !b.path.StartsWith(rootName + "/", StringComparison.Ordinal))
                    continue;
                string leaf = b.path.Substring(b.path.LastIndexOf('/') + 1);
                map[leaf.ToLowerInvariant()] = b.path;
            }
        }
        return map;
    }

    static string ImportRace(Race race)
    {
        string srcPath = "Assets/L2ImportTmp/" + race.Name + "_anim.fbx";
        var imp = (ModelImporter)AssetImporter.GetAtPath(srcPath);
        if (imp == null) return race.Name + " missing " + srcPath;
        imp.animationType = ModelImporterAnimationType.Generic;
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.resampleCurves = true;
        imp.SaveAndReimport();

        string rootName = string.IsNullOrEmpty(race.RootClip) ? race.Name + "_m000_b.ao" : race.RootClip;
        var map = BonePaths(race.ClipDir, rootName);
        var male = new Dictionary<string, AnimationClip>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { Root }))
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            string file = Path.GetFileNameWithoutExtension(p);
            if (!file.StartsWith(race.MaleClipPrefix, StringComparison.OrdinalIgnoreCase)) continue;
            var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(p);
            if (c != null) male[Seq(c.name, race.MaleClipPrefix, race.MaleClipSuffix)] = c;
        }

        var existing = new Dictionary<string, string>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { race.ClipDir.TrimEnd('/') }))
        {
            string p = AssetDatabase.GUIDToAssetPath(g);
            existing[Seq(Path.GetFileNameWithoutExtension(p), rootName + "_", "_" + race.Name)] = p;
        }

        var made = new Dictionary<string, AnimationClip>();
        int unmapped = 0, curves = 0;
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(srcPath).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, rootName + "|", "_" + race.Name);
            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_")) raw = raw.Substring(3);
            var dst = new AnimationClip { frameRate = src.frameRate };
            int hit = 0;
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0 ? rootName : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np)) { unmapped++; continue; }
                var nb = b; nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
                hit++; curves++;
            }
            if (hit == 0) continue;
            bool willLoop = male.TryGetValue(seq, out var maleRef)
                ? AnimationUtility.GetAnimationClipSettings(maleRef).loopTime
                : (seq.StartsWith("wait") || seq.StartsWith("walk") || seq.StartsWith("run") || seq.StartsWith("atkwait")
                   || seq.Contains("sitwait") || seq.Contains("deathwait") || seq.StartsWith("swim"));
            if (willLoop && !seq.StartsWith("cast") && !seq.StartsWith("magic"))
                CloseLoop(dst, 1f / Mathf.Max(1f, src.frameRate));
            var st = AnimationUtility.GetAnimationClipSettings(dst);
            st.loopTime = willLoop;
            AnimationUtility.SetAnimationClipSettings(dst, st);
            if (maleRef != null && maleRef.length > 0 && dst.length > 0)
            {
                float k = dst.length / maleRef.length;
                var ev = AnimationUtility.GetAnimationEvents(maleRef).Select(e => new AnimationEvent
                {
                    functionName = e.functionName, time = e.time * k, floatParameter = e.floatParameter,
                    intParameter = e.intParameter, stringParameter = e.stringParameter,
                    objectReferenceParameter = e.objectReferenceParameter
                }).ToArray();
                AnimationUtility.SetAnimationEvents(dst, ev);
            }
            string path = existing.TryGetValue(seq, out var ep) ? ep : race.ClipDir + rootName + "_" + raw + ".anim";
            var old = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (old != null)
            {
                EditorUtility.CopySerialized(dst, old);
                old.name = Path.GetFileNameWithoutExtension(path);
                EditorUtility.SetDirty(old);
                made[seq] = old;
            }
            else
            {
                Directory.CreateDirectory(race.ClipDir);
                dst.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(dst, path);
                made[seq] = dst;
            }
        }

        int slots = 0, filled = 0;
        var srcOc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(race.MaleOc);
        var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(race.DestOc);
        if (made.Count >= 20 && srcOc != null && oc != null)
        {
            oc.runtimeAnimatorController = srcOc.runtimeAnimatorController;
            var srcPairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            srcOc.GetOverrides(srcPairs);
            var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var kv in srcPairs)
            {
                AnimationClip o = null;
                string ms = kv.Value != null ? Seq(kv.Value.name, race.MaleClipPrefix, race.MaleClipSuffix) : "";
                if (kv.Value != null && made.TryGetValue(ms, out var c)) o = c;
                if (o == null && made.TryGetValue(SlotSeq(kv.Key.name), out var c2)) o = c2;
                if (o != null) filled++;
                pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(kv.Key, o ?? kv.Value));
            }
            oc.ApplyOverrides(pairs);
            EditorUtility.SetDirty(oc);
            slots = pairs.Count;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(race.PlayerPrefab) != null)
        {
            var pr = PrefabUtility.LoadPrefabContents(race.PlayerPrefab);
            if (oc != null)
            {
                foreach (var a in pr.GetComponentsInChildren<Animator>(true))
                    a.runtimeAnimatorController = oc;
            }
            PrefabUtility.SaveAsPrefabAsset(pr, race.PlayerPrefab);
            PrefabUtility.UnloadPrefabContents(pr);
        }

        if (made.Count >= 20)
            AssetDatabase.DeleteAsset(srcPath);
        return race.Name + " clips=" + made.Count + " curves=" + curves + " unmapped=" + unmapped
            + " bones=" + map.Count + " override=" + filled + "/" + slots;
    }

    public static string Run()
    {
        var log = new List<string>();
        foreach (var race in Races)
            log.Add(ImportRace(race));
        log.Add(L2OrcRacePrefabBuilder.RebuildHairSlots());
        AssetDatabase.SaveAssets();
        return string.Join(" || ", log);
    }
}
#endif
