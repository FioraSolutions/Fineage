#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Imports every retail take that a player race/gender is still missing as a standalone
/// `Clips/*.anim`, from the PSA conversion staged at `Assets/L2ImportTmp/{Race}_anim.fbx`
/// (`{Race}_anim.psa` + `{Race}_m000_b.psk` -> Blender `psk_psa_to_fbx.py`).
///
/// Same contract as <see cref="L2MFighterSpAtk06Importer"/>, generalised to all takes:
/// * remap onto the transform paths the committed clips of that race already use;
/// * strip the duplicate Euler rotation channels (`L2AnimClipPipeline`);
/// * renormalise positions onto the meter contract from a bone *length* ratio against the
///   race's committed reference clip, and reset the `.ao` x100 scale carrier;
/// * reject a clip whose calf length misses the committed rig by more than 2 %;
/// * loop flag copied from the same sequence of the opposite gender (or any race) that is
///   already committed, one-shot when no committed counterpart exists;
/// * register the clip in the meter migration ledger so it is never scaled twice.
///
/// A take is a gameplay sequence when its name ends with `_{Race}` (the retail naming); takes
/// without the suffix (e.g. the authoring `test` take) are reported and left out.
/// Existing clips are never overwritten.
///
///   Unity -batchmode -nographics -quit -projectPath l2-unity \
///     -executeMethod L2RaceTakeGapImporter.RunCli [Race ...]
/// </summary>
public static class L2RaceTakeGapImporter
{
    const string Root = "Assets/Resources/Data/Animations/";
    const string StageDir = "Assets/L2ImportTmp/";
    const string ReportPath = "/tmp/fineage-race-take-gap.txt";

    sealed class Race
    {
        public string Name;
        public string FamilyFolder;
        public string RootBone;
        public string ReferenceSeq;
        public string Ledger;
        public string ClipDir => Root + FamilyFolder + "/" + Name + "/Clips/";
        public string ClipPrefix => RootBone + "_";
        public string AnimFbx => Root + FamilyFolder + "/" + Name + "/Models/anim/" + Name + "_anim.fbx";
        public string Staged => StageDir + Name + "_anim.fbx";
    }

    static readonly Race[] Races =
    {
        new Race
        {
            Name = "MFighter",
            FamilyFolder = "Fighter",
            RootBone = "MFighter_m000_b.ao",
            ReferenceSeq = "SpAtk01_1HS",
            Ledger = "Assets/MeterContract/MFighter.clips-converted.json",
        },
    };

    public static void RunCli()
    {
        string[] only = BatchArgs.After("L2RaceTakeGapImporter.RunCli");
        string report;
        int code = 0;
        try
        {
            report = Run(only);
            if (report.Contains(" FAILED "))
                code = 1;
        }
        catch (Exception error)
        {
            report = error.ToString();
            code = 1;
        }
        Debug.Log("[RaceTakeGap] " + report);
        File.WriteAllText(ReportPath, report);
        EditorApplication.Exit(code);
    }

    [MenuItem("Shnok/Characters/Import missing race takes from L2ImportTmp")]
    public static void Menu() => Debug.Log("[RaceTakeGap] " + Run(Array.Empty<string>()));

    public static string Run(string[] only)
    {
        var lines = new List<string>();
        foreach (Race race in Races)
        {
            if (only.Length > 0 && !only.Contains(race.Name, StringComparer.OrdinalIgnoreCase))
                continue;
            lines.Add(ImportRace(race));
        }
        AssetDatabase.SaveAssets();
        return string.Join("\n", lines);
    }

    static string SeqOfClipFile(string fileName, Race race)
    {
        string n = fileName;
        if (n.StartsWith(race.ClipPrefix, StringComparison.OrdinalIgnoreCase))
            n = n.Substring(race.ClipPrefix.Length);
        string suffix = "_" + race.Name;
        if (n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            n = n.Substring(0, n.Length - suffix.Length);
        return n;
    }

    static string TakeName(AnimationClip src)
    {
        string n = src.name;
        int bar = n.LastIndexOf('|');
        if (bar >= 0)
            n = n.Substring(bar + 1);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase))
            n = n.Substring(3);
        return n;
    }

    static string ImportRace(Race race)
    {
        if (!File.Exists(race.Staged))
            return race.Name + " FAILED missing " + race.Staged + " (stage the PSA conversion first)";
        if (!AssetDatabase.IsValidFolder(race.ClipDir.TrimEnd('/')))
            return race.Name + " FAILED missing clip dir " + race.ClipDir;

        var have = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string g in AssetDatabase.FindAssets("t:AnimationClip", new[] { race.ClipDir.TrimEnd('/') }))
            have.Add(SeqOfClipFile(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(g)), race));

        string suffix = "_" + race.Name;
        string refTake = race.ReferenceSeq + suffix;
        ConfigureImporter(race, have, refTake);

        AnimationClip reference = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            race.ClipDir + race.ClipPrefix + refTake + ".anim");
        if (reference == null)
            return race.Name + " FAILED missing reference clip " + race.ClipPrefix + refTake;

        Dictionary<string, string> map = SkeletonPaths(race);
        Dictionary<string, bool> loops = CommittedLoopFlags(race);

        var added = new List<string>();
        var failed = new List<string>();
        var ignored = new List<string>();
        var marker = File.Exists(race.Ledger)
            ? File.ReadAllLines(race.Ledger).Where(l => l.Trim().Length > 0).ToList()
            : new List<string>();

        AnimationClip[] takes = AssetDatabase.LoadAllAssetsAtPath(race.Staged)
            .OfType<AnimationClip>()
            .Where(c => !c.name.StartsWith("__preview__", StringComparison.Ordinal))
            .ToArray();
        AnimationClip refSrc = takes.FirstOrDefault(t => TakeName(t).Equals(refTake, StringComparison.OrdinalIgnoreCase));
        if (refSrc == null)
            return race.Name + " FAILED staged FBX has no reference take " + refTake;
        float factor = MeterFactor(Remap(refSrc, map, race, out _), reference, race);
        if (float.IsNaN(factor))
            return race.Name + " FAILED cannot measure meter factor on " + refTake;

        foreach (AnimationClip src in takes)
        {
            string take = TakeName(src);
            if (!take.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                ignored.Add(take);
                continue;
            }
            string seq = take.Substring(0, take.Length - suffix.Length);
            if (have.Contains(seq))
                continue;

            AnimationClip dst = Remap(src, map, race, out int unmapped);
            int positions = ToMeter(dst, factor);
            string calf = CalfCheck(dst, reference, race, out bool calfOk);
            if (!calfOk)
            {
                failed.Add(take + "(" + calf + ")");
                Debug.LogError("[RaceTakeGap] " + race.Name + " " + take + " rejected: " + calf);
                continue;
            }

            bool loop = loops.TryGetValue(seq, out bool l) && l;
            L2AnimClipPipeline.ApplyStandardLoopSettings(dst, loop);
            AnimationUtility.SetAnimationEvents(dst, Array.Empty<AnimationEvent>());

            string path = race.ClipDir + race.ClipPrefix + take + ".anim";
            dst.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(dst, path);
            if (marker.All(m => !m.Trim().TrimEnd(',').Trim('"').Equals(path, StringComparison.Ordinal)))
                marker.Add(path);
            added.Add(take + "(len=" + dst.length.ToString("F3") + ",pos=" + positions + ",unmapped=" + unmapped +
                      ",loop=" + loop + "," + calf + ")");
        }

        File.WriteAllLines(race.Ledger, marker.OrderBy(p => p, StringComparer.Ordinal));
        AssetDatabase.ImportAsset(race.Ledger);
        AssetDatabase.SaveAssets();

        return race.Name + (failed.Count > 0 ? " FAILED " : " ok ") +
               "factor=" + factor.ToString("F5") +
               " added=" + added.Count + " [" + string.Join(", ", added) + "]" +
               " failed=" + failed.Count + " [" + string.Join(", ", failed) + "]" +
               " ignoredNonRaceTakes=[" + string.Join(", ", ignored) + "]";
    }

    /// <summary>Import only the missing takes plus the calibration take: the conversion carries every retail take.</summary>
    static void ConfigureImporter(Race race, HashSet<string> have, string refTake)
    {
        var imp = (ModelImporter)AssetImporter.GetAtPath(race.Staged);
        if (imp == null)
        {
            AssetDatabase.ImportAsset(race.Staged);
            imp = (ModelImporter)AssetImporter.GetAtPath(race.Staged);
        }
        if (imp == null)
            throw new InvalidOperationException("ModelImporter null for " + race.Staged);
        var refImp = (ModelImporter)AssetImporter.GetAtPath(race.AnimFbx);
        if (refImp == null)
            throw new InvalidOperationException("missing committed rig " + race.AnimFbx);
        imp.globalScale = refImp.globalScale;
        imp.useFileScale = refImp.useFileScale;
        imp.useFileUnits = refImp.useFileUnits;
        imp.bakeAxisConversion = refImp.bakeAxisConversion;
        imp.animationCompression = refImp.animationCompression;
        imp.animationType = ModelImporterAnimationType.Generic;
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.resampleCurves = true;

        string suffix = "_" + race.Name;
        ModelImporterClipAnimation[] all = imp.defaultClipAnimations;
        if (all == null || all.Length == 0)
            throw new InvalidOperationException(race.Staged + " has no takes");
        imp.clipAnimations = all.Where(t =>
        {
            string n = t.takeName.Substring(t.takeName.LastIndexOf('|') + 1);
            if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
            if (n.Equals(refTake, StringComparison.OrdinalIgnoreCase)) return true;
            if (!n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) return false;
            return !have.Contains(n.Substring(0, n.Length - suffix.Length));
        }).ToArray();
        imp.SaveAndReimport();
    }

    /// <summary>Leaf (lower case) -> transform path used by the committed clips of this race.</summary>
    static Dictionary<string, string> SkeletonPaths(Race race)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { [race.RootBone] = race.RootBone };
        foreach (string g in AssetDatabase.FindAssets("t:AnimationClip", new[] { race.ClipDir.TrimEnd('/') }))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g));
            if (clip == null) continue;
            foreach (EditorCurveBinding b in AnimationUtility.GetCurveBindings(clip))
            {
                if (string.IsNullOrEmpty(b.path)) continue;
                if (!b.path.Equals(race.RootBone, StringComparison.Ordinal) &&
                    !b.path.StartsWith(race.RootBone + "/", StringComparison.Ordinal))
                    continue;
                map[b.path.Substring(b.path.LastIndexOf('/') + 1)] = b.path;
            }
        }
        if (map.Count <= 1)
            throw new InvalidOperationException(race.Name + " has no committed clips to read skeleton paths from");
        return map;
    }

    /// <summary>Loop flag per sequence from every committed player clip (any race/gender).</summary>
    static Dictionary<string, bool> CommittedLoopFlags(Race self)
    {
        var flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (string g in AssetDatabase.FindAssets("t:AnimationClip", new[] { Root.TrimEnd('/') }))
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            if (!path.Contains("/Clips/")) continue;
            string file = Path.GetFileNameWithoutExtension(path);
            int ao = file.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
            if (ao < 0) continue;
            string body = file.Substring(ao + 4);
            int last = body.LastIndexOf('_');
            if (last <= 0) continue;
            string seq = body.Substring(0, last);
            if (flags.ContainsKey(seq)) continue;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;
            flags[seq] = AnimationUtility.GetAnimationClipSettings(clip).loopTime;
        }
        return flags;
    }

    static AnimationClip Remap(AnimationClip src, Dictionary<string, string> map, Race race, out int unmapped)
    {
        var dst = new AnimationClip { frameRate = src.frameRate };
        unmapped = 0;
        foreach (EditorCurveBinding b in AnimationUtility.GetCurveBindings(src))
        {
            string leaf = b.path.Length == 0 ? race.RootBone : b.path.Substring(b.path.LastIndexOf('/') + 1);
            if (!map.TryGetValue(leaf, out string path))
            {
                unmapped++;
                continue;
            }
            EditorCurveBinding t = b;
            t.path = path;
            AnimationUtility.SetEditorCurve(dst, t, AnimationUtility.GetEditorCurve(src, b));
        }
        L2AnimClipPipeline.StripRedundantEulerCurves(dst);
        return dst;
    }

    /// <summary>Committed / imported bone length of the reference take (meter contract, METER_SCALE_MIGRATION.md §5).</summary>
    static float MeterFactor(AnimationClip probe, AnimationClip reference, Race race)
    {
        foreach (string bone in new[] { "Bip01_L_Calf", "Bip01_L_Forearm" })
        {
            Vector3? a = BoneOffset(probe, bone, race);
            Vector3? b = BoneOffset(reference, bone, race);
            if (!a.HasValue || !b.HasValue || a.Value.magnitude < 1e-5f || b.Value.magnitude < 1e-5f)
                continue;
            return b.Value.magnitude / a.Value.magnitude;
        }
        return float.NaN;
    }

    static int ToMeter(AnimationClip clip, float factor)
    {
        int positions = 0;
        foreach (EditorCurveBinding b in AnimationUtility.GetCurveBindings(clip))
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, b);
            if (curve == null || curve.length == 0) continue;
            if (b.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal))
            {
                var keys = curve.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].value *= factor;
                    keys[i].inTangent *= factor;
                    keys[i].outTangent *= factor;
                }
                AnimationUtility.SetEditorCurve(clip, b, new AnimationCurve(keys));
                positions++;
            }
            else if (b.propertyName.StartsWith("m_LocalScale", StringComparison.Ordinal) && IsSkeletonRootPath(b.path))
            {
                var keys = curve.keys;
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].value = 1f;
                    keys[i].inTangent = 0f;
                    keys[i].outTangent = 0f;
                    keys[i].inWeight = 0f;
                    keys[i].outWeight = 0f;
                }
                AnimationUtility.SetEditorCurve(clip, b, new AnimationCurve(keys));
            }
        }
        return positions;
    }

    static string CalfCheck(AnimationClip clip, AnimationClip reference, Race race, out bool ok)
    {
        Vector3? mine = BoneOffset(clip, "Bip01_L_Calf", race);
        Vector3? reff = BoneOffset(reference, "Bip01_L_Calf", race);
        if (!mine.HasValue || !reff.HasValue)
        {
            ok = false;
            return "calf=n/a";
        }
        float expect = reff.Value.magnitude;
        float actual = mine.Value.magnitude;
        ok = Mathf.Abs(actual - expect) <= expect * 0.02f;
        return "calf=" + actual.ToString("F4") + "/" + expect.ToString("F4");
    }

    static Vector3? BoneOffset(AnimationClip clip, string leaf, Race race)
    {
        string path = null;
        foreach (EditorCurveBinding b in AnimationUtility.GetCurveBindings(clip))
        {
            if (string.IsNullOrEmpty(b.path) || !b.path.StartsWith(race.RootBone + "/", StringComparison.Ordinal))
                continue;
            if (b.path.Substring(b.path.LastIndexOf('/') + 1).Equals(leaf, StringComparison.OrdinalIgnoreCase))
            {
                path = b.path;
                break;
            }
        }
        if (path == null) return null;
        var v = new float[3];
        string[] props = { "m_LocalPosition.x", "m_LocalPosition.y", "m_LocalPosition.z" };
        for (int i = 0; i < 3; i++)
        {
            AnimationCurve c = AnimationUtility.GetEditorCurve(clip, new EditorCurveBinding
            {
                path = path, propertyName = props[i], type = typeof(Transform)
            });
            if (c == null || c.length == 0) return null;
            v[i] = c[0].value;
        }
        return new Vector3(v[0], v[1], v[2]);
    }

    static bool IsSkeletonRootPath(string path)
    {
        return !string.IsNullOrEmpty(path) && !path.Contains("/") &&
               (path.EndsWith(".ao", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".mo", StringComparison.OrdinalIgnoreCase));
    }
}
#endif
