#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Shared PSA/FBX → standalone Social_* .anim import for player races.
/// Config-driven (no per-race importer copies). Loop flags follow the FFighter
/// Social counterparts when present (all current FFighter Social clips are one-shots).
/// Source FBX is produced by psk_psa_to_fbx.py into Assets/L2ImportTmp/{Race}_anim.fbx.
/// Existing non-Social clips are never overwritten.
/// </summary>
public static class L2SocialAnimImporter
{
    const string Root = "Assets/Resources/Data/Animations/";
    const string FFighterClipDir = Root + "Fighter/FFighter/Clips/";

    struct Race
    {
        public string Name;
        public string FamilyFolder;
        public string ClipPrefix;
        public string RootBone;
        public string RefFbx;
    }

    static readonly Race[] Races =
    {
        new Race
        {
            Name = "MFighter",
            FamilyFolder = "Fighter",
            ClipPrefix = "MFighter_m000_b.ao_",
            RootBone = "MFighter_m000_b.ao",
            RefFbx = Root + "Fighter/MFighter/Models/anim/MFighter_anim.fbx",
        },
        new Race
        {
            Name = "MMagic",
            FamilyFolder = "Magic",
            ClipPrefix = "MMagic_M000_b.ao_",
            RootBone = "MMagic_M000_b.ao",
            RefFbx = Root + "Magic/MMagic/Models/anim/MMagic_anim.fbx",
        },
    };

    public static void RunCli()
    {
        string result;
        bool ok = true;
        try { result = Run("MFighter"); }
        catch (Exception e) { result = e.ToString(); ok = false; }
        Debug.Log("[SocialAnimImport] " + result);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    [MenuItem("Shnok/Characters/Import Social animations from L2ImportTmp")]
    public static void Menu() { Debug.Log("[SocialAnimImport] " + Run(null)); }

    static string Seq(string clipName, string prefix, string suffix)
    {
        string n = clipName;
        int i = n.LastIndexOf('|');
        if (i >= 0) n = n.Substring(i + 1);
        if (!string.IsNullOrEmpty(prefix) && n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            n = n.Substring(prefix.Length);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
        if (!string.IsNullOrEmpty(suffix) && n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            n = n.Substring(0, n.Length - suffix.Length);
        return n;
    }

    static bool IsSocialSeq(string seq)
    {
        return seq.StartsWith("Social_", StringComparison.OrdinalIgnoreCase);
    }

    static Dictionary<string, bool> FFighterSocialLoopMap()
    {
        var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (!AssetDatabase.IsValidFolder(FFighterClipDir.TrimEnd('/')))
            return map;
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { FFighterClipDir.TrimEnd('/') }))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            string seq = Seq(Path.GetFileNameWithoutExtension(path), "FFighter_m000_b.ao_", "_FFighter");
            if (!IsSocialSeq(seq)) continue;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;
            map[seq] = AnimationUtility.GetAnimationClipSettings(clip).loopTime;
        }
        return map;
    }

    static Dictionary<string, string> BonePaths(string clipDir, string rootName, string refFbx)
    {
        var map = new Dictionary<string, string> { [rootName.ToLowerInvariant()] = rootName };
        if (AssetDatabase.IsValidFolder(clipDir.TrimEnd('/')))
        {
            foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { clipDir.TrimEnd('/') }))
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
                if (clip == null) continue;
                foreach (var b in AnimationUtility.GetCurveBindings(clip))
                {
                    if (string.IsNullOrEmpty(b.path)) continue;
                    if (!b.path.Equals(rootName, StringComparison.Ordinal)
                        && !b.path.StartsWith(rootName + "/", StringComparison.Ordinal))
                        continue;
                    string leaf = b.path.Substring(b.path.LastIndexOf('/') + 1);
                    map[leaf.ToLowerInvariant()] = b.path;
                }
            }
        }

        if (map.Count <= 1 && !string.IsNullOrEmpty(refFbx))
        {
            var refGo = AssetDatabase.LoadAssetAtPath<GameObject>(refFbx);
            if (refGo != null)
            {
                foreach (var t in refGo.GetComponentsInChildren<Transform>(true))
                    map[t.name.ToLowerInvariant()] = AnimationUtility.CalculateTransformPath(t, refGo.transform);
            }
        }
        return map;
    }

    static void ApplyImportSettings(string srcPath, string refFbx)
    {
        var imp = (ModelImporter)AssetImporter.GetAtPath(srcPath);
        if (imp == null)
        {
            AssetDatabase.ImportAsset(srcPath);
            imp = (ModelImporter)AssetImporter.GetAtPath(srcPath);
        }
        if (imp == null)
            throw new InvalidOperationException("importer null for " + srcPath);

        var refImp = !string.IsNullOrEmpty(refFbx)
            ? (ModelImporter)AssetImporter.GetAtPath(refFbx)
            : null;
        if (refImp != null)
        {
            imp.globalScale = refImp.globalScale;
            imp.useFileScale = refImp.useFileScale;
            imp.useFileUnits = refImp.useFileUnits;
            imp.bakeAxisConversion = refImp.bakeAxisConversion;
            imp.animationCompression = refImp.animationCompression;
        }
        imp.animationType = ModelImporterAnimationType.Generic;
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.resampleCurves = true;
        imp.SaveAndReimport();
    }

    static string ImportRace(Race race, Dictionary<string, bool> ffLoop)
    {
        string srcPath = "Assets/L2ImportTmp/" + race.Name + "_anim.fbx";
        if (!File.Exists(srcPath))
            return race.Name + " missing " + srcPath;

        string clipDir = Root + race.FamilyFolder + "/" + race.Name + "/Clips/";
        if (!AssetDatabase.IsValidFolder(clipDir.TrimEnd('/')))
            return race.Name + " missing clip dir " + clipDir;

        ApplyImportSettings(srcPath, race.RefFbx);
        var map = BonePaths(clipDir, race.RootBone, race.RefFbx);
        if (map.Count <= 1)
            return race.Name + " no bone paths from existing clips or " + race.RefFbx;

        var have = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { clipDir.TrimEnd('/') }))
            have.Add(Seq(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(g)), race.ClipPrefix, "_" + race.Name));

        var added = new List<string>();
        var failed = new List<string>();
        var skipped = new List<string>();
        int unmapped = 0;
        var seenSocial = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(srcPath).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, race.RootBone + "|", "_" + race.Name);
            if (!IsSocialSeq(seq)) continue;
            seenSocial.Add(seq);
            if (have.Contains(seq))
            {
                skipped.Add(seq + "(exists)");
                continue;
            }

            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) raw = raw.Substring(3);

            var dst = new AnimationClip { frameRate = src.frameRate };
            int hit = 0;
            int miss = 0;
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0 ? race.RootBone : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np))
                {
                    miss++;
                    unmapped++;
                    continue;
                }
                var nb = b;
                nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
                hit++;
            }
            if (hit == 0)
            {
                failed.Add(seq + "(no mapped curves, misses=" + miss + ")");
                continue;
            }

            L2AnimClipPipeline.StripRedundantEulerCurves(dst);
            bool loop = ffLoop.TryGetValue(seq, out var ff) && ff;
            L2AnimClipPipeline.ApplyStandardLoopSettings(dst, loop);

            string path = clipDir + race.ClipPrefix + raw + ".anim";
            dst.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(dst, path);
            added.Add(raw + "(curves=" + hit + ",unmapped=" + miss + ",loop=" + loop + ")");
        }

        string[] expected =
        {
            "Social_Atk", "Social_victory", "Social_no", "Social_nod", "Social_yes",
            "Social_unaware", "Social_waiting_a", "Social_laugh", "Social_dance",
            "Social_bow", "Social_SpWait01", "Social_SpWait02", "Social_clap", "Social_sad",
        };
        var missing = new List<string>();
        foreach (var exp in expected)
        {
            if (!seenSocial.Contains(exp) && !have.Contains(exp))
                missing.Add(exp);
        }

        AssetDatabase.SaveAssets();
        if (added.Count > 0)
            AssetDatabase.DeleteAsset(srcPath);

        return race.Name
            + " added=" + added.Count + " [" + string.Join(", ", added) + "]"
            + " skipped=" + skipped.Count + " [" + string.Join(", ", skipped) + "]"
            + " failed=" + failed.Count + " [" + string.Join(", ", failed) + "]"
            + " missingInFbx=" + missing.Count + " [" + string.Join(", ", missing) + "]"
            + " bones=" + map.Count + " unmappedBindings=" + unmapped;
    }

    public static string Run(string onlyRace)
    {
        var ffLoop = FFighterSocialLoopMap();
        var log = new List<string> { "ffighterSocialLoopRefs=" + ffLoop.Count };
        foreach (var race in Races)
        {
            if (!string.IsNullOrEmpty(onlyRace)
                && !string.Equals(race.Name, onlyRace, StringComparison.OrdinalIgnoreCase))
                continue;
            log.Add(ImportRace(race, ffLoop));
        }
        AssetDatabase.SaveAssets();
        return string.Join(" || ", log);
    }
}
#endif
