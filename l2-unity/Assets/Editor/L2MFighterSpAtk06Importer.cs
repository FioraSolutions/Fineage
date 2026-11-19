#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Human male fighter (MFighter) is the only player race without the self-buff gesture
/// `SpAtk06_Hand` (Ultimate Evasion / Dash / Focus Chance). `AnimationCombo_Classic.txt`
/// tag `X` is `SpAtk06AnimName`; in the retail `MFighter_anim.psa` that is the unarmed
/// `SpAtk06_Hand_MFighter` take (46 frames, hands-up buff pose, no hit/shoot notifies).
///
/// The committed `Fighter/MFighter/Models/anim/MFighter_anim.fbx` is a stripped skeleton
/// (no takes), so the sequence is taken from the PSA conversion
/// (`MFighter_anim.psa` + `MFighter_m000_b.psk` -> Blender `psk_psa_to_fbx.py`) staged at
/// `Assets/L2ImportTmp/MFighter_anim.fbx`.
///
/// The imported take is remapped onto the skeleton paths the committed clips already use,
/// then calibrated against the committed `SpAtk01_1HS` clip of the same take family so the
/// result lands on the meter contract (positions ~0.4 m at `bip01`, `.ao` scale 1).
///
///   Unity -batchmode -nographics -quit -projectPath l2-unity \
///     -executeMethod L2MFighterSpAtk06Importer.RunCli [calibrate]
/// </summary>
public static class L2MFighterSpAtk06Importer
{
    const string Src = "Assets/L2ImportTmp/MFighter_anim.fbx";
    const string Root = "Assets/Resources/Data/Animations/";
    const string ClipDir = Root + "Fighter/MFighter/Clips/";
    const string RefClip = ClipDir + "MFighter_m000_b.ao_SpAtk01_1HS_MFighter.anim";
    const string DestClip = ClipDir + "MFighter_m000_b.ao_SpAtk06_Hand_MFighter.anim";
    const string AnimFbx = Root + "Fighter/MFighter/Models/anim/MFighter_anim.fbx";
    const string ConvertedMarker = "Assets/MeterContract/MFighter.clips-converted.json";
    const string RootBone = "MFighter_m000_b.ao";

    public static void RunCli()
    {
        string[] args = BatchArgs.After("L2MFighterSpAtk06Importer.RunCli");
        bool calibrateOnly = args.Contains("calibrate", StringComparer.OrdinalIgnoreCase);
        string report;
        int code = 0;
        try
        {
            report = calibrateOnly ? Calibrate() : Build();
        }
        catch (Exception error)
        {
            report = error.ToString();
            code = 1;
        }
        Debug.Log("[SpAtk06] " + report);
        File.WriteAllText("/tmp/fineage-spatk06.txt", report);
        EditorApplication.Exit(code);
    }

    static void EnsureImporter()
    {
        var refImp = (ModelImporter)AssetImporter.GetAtPath(AnimFbx);
        var imp = (ModelImporter)AssetImporter.GetAtPath(Src);
        if (imp == null)
            throw new Exception("missing " + Src + " (stage the PSA conversion there first)");
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

        // The PSA conversion carries 114 takes; only the buff gesture and the 1HS take used for
        // calibrating the meter contract are needed, and importing the rest costs minutes.
        ModelImporterClipAnimation[] takes = imp.defaultClipAnimations;
        if (takes != null && takes.Length > 0)
        {
            ModelImporterClipAnimation[] wanted = takes
                .Where(t => t.name.IndexOf("SpAtk06_Hand_MFighter", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            t.name.IndexOf("SpAtk01_1HS_MFighter", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
            if (wanted.Length > 0 && (imp.clipAnimations == null || imp.clipAnimations.Length != wanted.Length))
                imp.clipAnimations = wanted;
        }
        imp.SaveAndReimport();
    }

    /// <summary>Leaf name (lower case) -> transform path used by the committed MFighter clips.</summary>
    static Dictionary<string, string> SkeletonPaths()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [RootBone.ToLowerInvariant()] = RootBone
        };
        var dir = ClipDir.TrimEnd('/');
        foreach (string guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { dir }))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid));
            if (clip == null)
                continue;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                string path = binding.path;
                if (string.IsNullOrEmpty(path))
                    continue;
                if (!path.Equals(RootBone, StringComparison.Ordinal) &&
                    !path.StartsWith(RootBone + "/", StringComparison.Ordinal))
                    continue;
                string leaf = path.Substring(path.LastIndexOf('/') + 1).ToLowerInvariant();
                map[leaf] = path;
            }
        }
        return map;
    }

    static AnimationClip SourceTake(string seqPart)
    {
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(Src))
        {
            if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__") &&
                clip.name.IndexOf(seqPart, StringComparison.OrdinalIgnoreCase) >= 0)
                return clip;
        }
        return null;
    }

    /// <summary>Copy a take onto the committed skeleton paths, keeping every curve as authored.</summary>
    static AnimationClip Remap(AnimationClip src, Dictionary<string, string> map, out int unmapped)
    {
        var dst = new AnimationClip { frameRate = src.frameRate };
        unmapped = 0;
        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(src))
        {
            string leaf = binding.path.Length == 0
                ? RootBone
                : binding.path.Substring(binding.path.LastIndexOf('/') + 1);
            if (!map.TryGetValue(leaf, out string path))
            {
                // `*_end` nub bones exist only on the committed skeleton; no skin weight binds them.
                unmapped++;
                continue;
            }
            var target = binding;
            target.path = path;
            AnimationUtility.SetEditorCurve(dst, target, AnimationUtility.GetEditorCurve(src, binding));
        }
        return dst;
    }

    static string Calibrate()
    {
        EnsureImporter();
        var map = SkeletonPaths();
        AnimationClip src = SourceTake("SpAtk01_1HS_MFighter");
        if (src == null)
            throw new Exception("no SpAtk01_1HS take in " + Src);
        int unmapped;
        AnimationClip probe = Remap(src, map, out unmapped);
        AnimationClip reference = AssetDatabase.LoadAssetAtPath<AnimationClip>(RefClip);
        if (reference == null)
            throw new Exception("missing reference " + RefClip);

        var lines = new List<string>
        {
            $"src={src.name} len={src.length:F3} frameRate={src.frameRate} unmapped={unmapped}",
            $"ref={reference.name} len={reference.length:F3}"
        };

        foreach (string bone in new[] { "bip01", "Bip01_Pelvis", "bip01_spine", "Bip01_L_UpperArm" })
        {
            string path = bone == "bip01" ? RootBone + "/bip01" : RootBone + "/bip01/" + spinePath(bone);
            lines.Add(diffLine(probe, reference, path, bone));
        }
        return string.Join("\n", lines);
    }

    static string spinePath(string bone) => bone;

    static string diffLine(AnimationClip probe, AnimationClip reference, string path, string label)
    {
        var sb = new System.Text.StringBuilder(label + ":");
        for (int axis = 0; axis < 3; axis++)
        {
            string prop = axis switch { 0 => "m_LocalPosition.x", 1 => "m_LocalPosition.y", _ => "m_LocalPosition.z" };
            var a = AnimationUtility.GetEditorCurve(probe, new EditorCurveBinding { path = path, propertyName = prop, type = typeof(Transform) });
            var b = AnimationUtility.GetEditorCurve(reference, new EditorCurveBinding { path = path, propertyName = prop, type = typeof(Transform) });
            float av = a != null && a.length > 0 ? a[0].value : float.NaN;
            float bv = b != null && b.length > 0 ? b[0].value : float.NaN;
            sb.Append($" pos[{prop.Substring(prop.Length - 1)}] probe={av:G6} ref={bv:G6} ratio={(Mathf.Abs(av) > 1e-9f ? (bv / av).ToString("G6") : "-")}");
        }
        for (int axis = 0; axis < 4; axis++)
        {
            string prop = axis switch
            {
                0 => "m_LocalRotation.x", 1 => "m_LocalRotation.y", 2 => "m_LocalRotation.z", _ => "m_LocalRotation.w"
            };
            var a = AnimationUtility.GetEditorCurve(probe, new EditorCurveBinding { path = path, propertyName = prop, type = typeof(Transform) });
            var b = AnimationUtility.GetEditorCurve(reference, new EditorCurveBinding { path = path, propertyName = prop, type = typeof(Transform) });
            float av = a != null && a.length > 0 ? a[0].value : float.NaN;
            float bv = b != null && b.length > 0 ? b[0].value : float.NaN;
            sb.Append($" rot[{prop.Substring(prop.Length - 1)}] probe={av:G6} ref={bv:G6}");
        }
        return sb.ToString();
    }

    static string Build()
    {
        EnsureImporter();
        var map = SkeletonPaths();
        AnimationClip src = SourceTake("SpAtk06_Hand_MFighter");
        if (src == null)
            throw new Exception("no SpAtk06_Hand take in " + Src);
        int unmapped;
        AnimationClip dst = Remap(src, map, out unmapped);

        // Meter contract (Docs/METER_SCALE_MIGRATION.md §5): local positions carry the legacy
        // x100 carrier that the committed clips already had folded out, and the `.ao` node keeps
        // its old 100 scale curve. Both are renormalised here so the new clip matches its siblings.
        int positions = 0, scales = 0;
        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(dst))
        {
            if (binding.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(dst, binding);
                if (curve == null || curve.length == 0)
                    continue;
                AnimationUtility.SetEditorCurve(dst, binding, Scaled(curve, MeterPositionFactor(src, dst)));
                positions++;
            }
            else if (binding.propertyName.StartsWith("m_LocalScale", StringComparison.Ordinal) &&
                     IsSkeletonRootPath(binding.path))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(dst, binding);
                if (curve == null || curve.length == 0)
                    continue;
                AnimationUtility.SetEditorCurve(dst, binding, UnitScaleCurve(curve));
                scales++;
            }
        }

        var settings = AnimationUtility.GetAnimationClipSettings(dst);
        settings.loopTime = false;
        settings.loopBlend = false;
        AnimationUtility.SetAnimationClipSettings(dst, settings);
        // SpAtk06 is a buff gesture: no OnAnimationHit / AttackShot notify in any retail race.
        AnimationUtility.SetAnimationEvents(dst, Array.Empty<AnimationEvent>());

        dst.name = Path.GetFileNameWithoutExtension(DestClip);
        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(DestClip);
        if (existing != null)
        {
            EditorUtility.CopySerialized(dst, existing);
            existing.name = dst.name;
            EditorUtility.SetDirty(existing);
            dst = existing;
        }
        else
        {
            AssetDatabase.CreateAsset(dst, DestClip);
        }
        AssetDatabase.SaveAssets();

        // The migration keeps a ledger of clips already on the meter contract; without this
        // entry a later `L2MeterMigration clips` pass would scale the new clip a second time.
        var marker = new List<string>();
        if (File.Exists(ConvertedMarker))
            marker.AddRange(File.ReadAllLines(ConvertedMarker).Where(l => l.Trim().Length > 0));
        if (marker.All(l => !l.Trim().TrimEnd(',').Trim('"').Equals(DestClip, StringComparison.Ordinal)))
            marker.Add(DestClip);
        File.WriteAllLines(ConvertedMarker, marker.OrderBy(p => p, StringComparer.Ordinal));
        AssetDatabase.ImportAsset(ConvertedMarker);

        return $"clip={DestClip} len={dst.length:F3} curves={positions} scaleCurves={scales} " +
               $"unmapped={unmapped} events={dst.events.Length}";
    }

    /// <summary>
    /// Ratio between the imported take and the committed siblings. Read off the reference take
    /// family (SpAtk01_1HS) instead of a constant: the PSA conversion can be re-run with a
    /// different Blender unit setup and a hard-coded factor would silently shrink the hero.
    /// </summary>
    static float MeterPositionFactor(AnimationClip src, AnimationClip dst)
    {
        AnimationClip reference = AssetDatabase.LoadAssetAtPath<AnimationClip>(RefClip);
        if (reference == null)
            return 100f;

        string path = RootBone + "/bip01";
        var binding = new EditorCurveBinding
        {
            path = path,
            propertyName = "m_LocalPosition.y",
            type = typeof(Transform)
        };
        AnimationCurve mine = AnimationUtility.GetEditorCurve(dst, binding);
        AnimationCurve reff = AnimationUtility.GetEditorCurve(reference, binding);
        if (mine == null || mine.length == 0 || reff == null || reff.length == 0)
            return 100f;
        float value = mine[0].value;
        if (Mathf.Abs(value) < 1e-6f)
            return 100f;
        return reff[0].value / value;
    }

    static AnimationCurve Scaled(AnimationCurve curve, float factor)
    {
        var keys = new Keyframe[curve.length];
        for (int i = 0; i < curve.length; i++)
        {
            Keyframe key = curve[i];
            key.value *= factor;
            key.inTangent *= factor;
            key.outTangent *= factor;
            keys[i] = key;
        }
        return new AnimationCurve(keys);
    }

    static AnimationCurve UnitScaleCurve(AnimationCurve curve)
    {
        var keys = new Keyframe[curve.length];
        for (int i = 0; i < curve.length; i++)
        {
            Keyframe key = curve[i];
            key.value = 1f;
            key.inTangent = 0f;
            key.outTangent = 0f;
            key.inWeight = 0f;
            key.outWeight = 0f;
            keys[i] = key;
        }
        return new AnimationCurve(keys);
    }

    /// <summary>Bare `.ao` / `.mo` node path — the legacy x100 carrier (meter migration §5).</summary>
    static bool IsSkeletonRootPath(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Contains("/"))
            return false;
        return path.EndsWith(".ao", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".mo", StringComparison.OrdinalIgnoreCase);
    }
}
#endif
