#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// MFighter pilot: finish the meter-contract migration on the Unity side
/// (Docs/METER_SCALE_MIGRATION.md §4–§6).
///
/// Input: the 75 MFighter FBX already rewritten by tools/interlude/fbx_meter_patch.py
/// (UnitScaleFactor 100, node scale 1, data carrying the legacy 1.9 product).
///
/// Steps, all idempotent:
///   1. importers  — globalScale 1, useFileScale true, animationType Generic (and the anim
///                   FBX keeps importAnimation on);
///   2. clips      — m_LocalPosition curves ×100, .ao root m_LocalScale → 1 (§5);
///   3. prefabs    — anim prefabs get their skeleton subtree rebuilt from the new FBX (§6)
///                   and armor/face/hair prefabs lose stale m_LocalScale 100 overrides.
///
/// Batch:
///   Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterMigration.RunCli
/// Steps can be run alone: L2MeterMigration.RunCli imports|clips|prefabs
/// </summary>
public static class L2MeterMigration
{
    const string Models = "Assets/Resources/Data/Animations/Fighter/MFighter/Models";
    const string AnimFbx = Models + "/anim/MFighter_anim.fbx";
    const string AnimPrefab = Models + "/anim/MFighter_anim.prefab";
    const string AnimPlayer = "Assets/Resources/Data/Animations/Fighter/MFighter/MFighter_anim_player.prefab";
    const string AnimUser = "Assets/Resources/Data/Animations/Fighter/MFighter/MFighter_anim_user.prefab";
    const string ClipDir = "Assets/Resources/Data/Animations/Fighter/MFighter/Clips";
    const string RaceRoot = "Assets/Resources/Data/Animations/Fighter";
    const string HumanRoot = "Assets/Resources/Data/Animations/Human/MFighter";
    const string ResultPath = "/tmp/fineage-meter-migration.txt";

    /// <summary>
    /// Clips already rescaled (§5 asks for a re-runnable record instead of a heuristic).
    /// Written next to the harness baseline and committed, so a second run is a no-op.
    /// </summary>
    const string ConvertedClipsPath = "Assets/MeterContract/MFighter.clips-converted.json";

    /// <summary>Legacy world factor: 100 (node Lcl Scaling) × 0.019 (importer globalScale).</summary>
    const float PositionFactor = 100f;

    [MenuItem("L2/Meter Contract/Migrate MFighter (import, clips, prefabs)")]
    public static void Menu()
    {
        Debug.Log("[MeterMigration] " + Run("all"));
    }

    public static void RunCli()
    {
        // The batch step name is the last argument: -executeMethod L2MeterMigration.RunCli imports
        string step = Environment.GetCommandLineArgs().LastOrDefault() ?? "all";
        if (step.StartsWith("-", StringComparison.Ordinal))
            step = "all";
        try
        {
            string report = Run(step);
            File.AppendAllText(ResultPath, report + "\nOK\n");
            Debug.Log("[MeterMigration] " + report);
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    public static string Run(string step)
    {
        var log = new List<string>();
        File.WriteAllText(ResultPath, step + "\n");
        bool all = step == "all";
        if (all || step == "imports")
            ConfigureImporters(log);
        if (all || step == "clips")
            ConvertClips(log);
        if (all || step == "prefabs")
            RebuildPrefabs(log);
        AssetDatabase.SaveAssets();
        string report = string.Join(" | ", log);
        File.AppendAllText(ResultPath, report + "\n");
        return report;
    }

    static IEnumerable<string> ModelFbx(string folder = Models)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                yield return path;
        }
    }

    // --------------------------------------------------------------- §4 import settings
    static void ConfigureImporters(List<string> log)
    {
        int changed = 0;
        var paths = ModelFbx().Concat(ModelFbx(Models + "/anim")).Distinct().ToList();
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (string path in paths)
            {
                if (!(AssetImporter.GetAtPath(path) is ModelImporter importer))
                    continue;
                bool anim = path == AnimFbx;
                ModelImporterAnimationType type = anim ? ModelImporterAnimationType.Generic
                                                       : importer.animationType;
                // Only the unit contract is forced for meshes; the rest of their settings stay.
                bool dirty = !Mathf.Approximately(importer.globalScale, 1f)
                             || !importer.useFileScale
                             || (anim && importer.animationType != type)
                             || (anim && !importer.importAnimation)
                             || (anim && importer.avatarSetup != ModelImporterAvatarSetup.NoAvatar);
                if (!dirty)
                    continue;
                importer.globalScale = 1f;
                importer.useFileScale = true;
                if (anim)
                {
                    importer.animationType = ModelImporterAnimationType.Generic;
                    importer.importAnimation = true;
                    importer.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
                }
                importer.SaveAndReimport();
                changed++;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        log.Add($"importers={changed}/{paths.Count}");
    }

    // ------------------------------------------------------------------- §5 anim clips
    static void ConvertClips(List<string> log)
    {
        string markerPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ConvertedClipsPath));
        var already = new HashSet<string>(StringComparer.Ordinal);
        if (File.Exists(markerPath))
        {
            foreach (string line in File.ReadAllLines(markerPath))
            {
                string trimmed = line.Trim().TrimEnd(',').Trim('"');
                if (trimmed.Length > 0 && trimmed.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                    already.Add(trimmed);
            }
        }

        int converted = 0;
        int positionCurves = 0;
        int scaleCurves = 0;
        int skipped = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { ClipDir }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                continue;
            // §5: no value heuristic. A recorded clip is never converted twice.
            if (already.Contains(path))
            {
                skipped++;
                continue;
            }
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            bool dirty = false;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    if (curve == null || curve.length == 0)
                        continue;
                    AnimationUtility.SetEditorCurve(clip, binding, Scaled(curve, PositionFactor));
                    positionCurves++;
                    dirty = true;
                }
                else if (binding.propertyName.StartsWith("m_LocalScale", StringComparison.Ordinal)
                         && IsSkeletonRootPath(binding.path))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                    if (curve == null || curve.length == 0)
                        continue;
                    AnimationUtility.SetEditorCurve(clip, binding, UnitScaleCurve(curve));
                    scaleCurves++;
                    dirty = true;
                }
            }
            if (!dirty)
                continue;
            EditorUtility.SetDirty(clip);
            already.Add(path);
            converted++;
        }
        AssetDatabase.SaveAssets();

        Directory.CreateDirectory(Path.GetDirectoryName(markerPath));
        File.WriteAllLines(markerPath, already.OrderBy(p => p, StringComparer.Ordinal));
        AssetDatabase.ImportAsset(ConvertedClipsPath);
        log.Add($"clips={converted} skipped={skipped} positionCurves={positionCurves} scaleCurves={scaleCurves}");
    }

    static AnimationCurve Scaled(AnimationCurve curve, float factor)
    {
        var keys = new Keyframe[curve.length];
        for (int i = 0; i < curve.length; i++)
        {
            // Keyframe is a struct that also carries tangentMode/weightedMode, so copying it
            // and scaling value+slopes keeps the curve shape identical at 100× amplitude.
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

    /// <summary>Bare `.ao` / `.mo` node path — the legacy ×100 carrier.</summary>
    static bool IsSkeletonRootPath(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Contains("/"))
            return false;
        return path.EndsWith(".ao", StringComparison.OrdinalIgnoreCase)
               || path.EndsWith(".mo", StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------------- §6 prefabs
    static void RebuildPrefabs(List<string> log)
    {
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(AnimFbx);
        if (fbx == null)
            throw new FileNotFoundException("missing anim FBX", AnimFbx);

        // §6 permits rebuilding the subtree, but destroying and re-instantiating it churns
        // prefab fileIDs. Player_/User_/Pawn_ override the carrier SkinnedMeshRenderer by
        // fileID (m_Mesh -> null), so a rebuild silently resurrects the skeleton mesh.
        // The FBX patch is numerically exact, so copying the new local transforms onto the
        // existing objects keeps every fileID and every external override intact.
        int synced = 0;
        foreach (string path in new[] { AnimPrefab, AnimPlayer, AnimUser })
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                log.Add("missing " + Path.GetFileName(path));
                continue;
            }
            synced += SyncSkeletonTransforms(path, fbx);
        }
        int cleared = ClearStaleScaleOverrides();
        log.Add($"animPrefabs=3 syncedTransforms={synced} clearedScaleOverrides={cleared}");
    }

    /// <summary>
    /// Copy the FBX's local transforms onto the matching objects of an anim prefab, matched
    /// by path from the `.ao` root, so the flattened skeleton follows the new FBX without
    /// changing a single prefab fileID.
    /// </summary>
    static int SyncSkeletonTransforms(string prefabPath, GameObject fbx)
    {
        var fbxInstance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
        try
        {
            Transform sourceRoot = FindSkeletonRoot(fbxInstance.transform);
            Transform targetRoot = FindSkeletonRoot(root.transform);
            if (sourceRoot == null || targetRoot == null)
                throw new InvalidOperationException("no .ao root in " + prefabPath);

            int changed = 0;
            // The root itself, then every descendant by relative path.
            if (Apply(targetRoot, sourceRoot))
                changed++;
            foreach (Transform source in sourceRoot.GetComponentsInChildren<Transform>(true))
            {
                if (source == sourceRoot)
                    continue;
                Transform target = targetRoot.Find(RelativePath(sourceRoot, source));
                if (target == null)
                    continue;
                if (Apply(target, source))
                    changed++;
            }

            // The carrier mesh (`.mo`) is a sibling of `.ao`, i.e. a root child.
            foreach (Transform source in fbxInstance.transform.GetComponentsInChildren<Transform>(true))
            {
                if (!source.name.EndsWith(".mo", StringComparison.OrdinalIgnoreCase))
                    continue;
                Transform target = root.transform.Find(source.name);
                if (target != null && Apply(target, source))
                    changed++;
            }

            changed += SyncRendererCaches(root, fbxInstance);

            root.transform.localScale = Vector3.one;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            return changed;
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
            UnityEngine.Object.DestroyImmediate(fbxInstance);
        }
    }

    static bool Apply(Transform target, Transform source)
    {
        bool dirty = (target.localPosition - source.localPosition).sqrMagnitude > 1e-12f
                     || Quaternion.Angle(target.localRotation, source.localRotation) > 1e-4f
                     || (target.localScale - source.localScale).sqrMagnitude > 1e-12f;
        if (!dirty)
            return false;
        target.localPosition = source.localPosition;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;
        return true;
    }

    /// <summary>
    /// `SkinnedMeshRenderer.localBounds` is a serialized cache of the mesh bounds in the
    /// renderer's space. Rebuilding the skeleton invalidates it silently: bounds drop 100×
    /// and the character is culled away while every transform looks right.
    /// </summary>
    static int SyncRendererCaches(GameObject target, GameObject source)
    {
        var sourceByPath = new Dictionary<string, SkinnedMeshRenderer>(StringComparer.Ordinal);
        foreach (SkinnedMeshRenderer skin in source.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            sourceByPath[RelativePath(source.transform, skin.transform)] = skin;

        int changed = 0;
        foreach (SkinnedMeshRenderer skin in target.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (!sourceByPath.TryGetValue(RelativePath(target.transform, skin.transform), out var from))
                continue;
            if (skin.sharedMesh != from.sharedMesh)
                continue;
            if ((skin.localBounds.center - from.localBounds.center).sqrMagnitude < 1e-12f &&
                (skin.localBounds.size - from.localBounds.size).sqrMagnitude < 1e-12f)
                continue;
            skin.localBounds = from.localBounds;
            changed++;
        }
        return changed;
    }

    static Transform FindSkeletonRoot(Transform root)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.EndsWith(".ao", StringComparison.OrdinalIgnoreCase))
                return t;
        }
        return null;
    }

    static string RelativePath(Transform root, Transform target)
    {
        var parts = new List<string>();
        for (Transform t = target; t != null && t != root; t = t.parent)
            parts.Add(t.name);
        parts.Reverse();
        return string.Join("/", parts);
    }

    /// <summary>
    /// Armor/face/hair prefabs nest the FBX, so a reimport propagates the transforms. What
    /// stays behind is an explicit `m_LocalScale = 100` override on the instance's `.ao`/`.mo`
    /// nodes; drop those so the gear reports scale 1.
    ///
    /// Only nested-instance nodes of a migrated FBX qualify. A prefab's own local object can
    /// legitimately carry 100 — `MFighter_h_2_2_bh` has one holding a raw-unit mesh, and
    /// clearing it shrank the hair 100×.
    /// </summary>
    static int ClearStaleScaleOverrides()
    {
        int cleared = 0;
        foreach (string folder in new[] { RaceRoot, HumanRoot + "/Faces", HumanRoot + "/Hair" })
        {
            if (!AssetDatabase.IsValidFolder(folder))
                continue;
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { folder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!Path.GetFileName(path).StartsWith("MFighter_", StringComparison.OrdinalIgnoreCase))
                    continue;
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    bool dirty = false;
                    foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                    {
                        if (Mathf.Abs(t.localScale.x - 100f) > 0.5f)
                            continue;
                        if (!IsMigratedFbxInstance(t))
                            continue;
                        t.localScale = Vector3.one;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                        dirty = true;
                    }
                    if (!dirty)
                        continue;
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    cleared++;
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }
        return cleared;
    }

    static bool IsMigratedFbxInstance(Transform t)
    {
        GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject);
        if (source == null)
            return false;
        string path = AssetDatabase.GetAssetPath(source);
        return path.StartsWith(Models, StringComparison.Ordinal)
               && path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase);
    }
}
#endif
