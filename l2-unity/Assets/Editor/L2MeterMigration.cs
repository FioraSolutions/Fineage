#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Meter-contract migration on the Unity side (Docs/METER_SCALE_MIGRATION.md §4–§6).
///
/// Input: the rig's FBX already rewritten by tools/interlude/fbx_meter_patch.py
/// (UnitScaleFactor 100, node scale 1, data carrying the legacy 1.9 product).
///
/// Steps, all idempotent:
///   1. importers  — globalScale 1, useFileScale true, animationType Generic (and the anim
///                   FBX keeps importAnimation on);
///   2. clips      — m_LocalPosition curves ×100, .ao root m_LocalScale → 1 (§5);
///   3. prefabs    — anim prefabs get their skeleton transforms synced from the new FBX (§6)
///                   and armor/face/hair prefabs lose stale m_LocalScale 100 overrides.
///
/// Batch:
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterMigration.RunCli [all|imports|clips|prefabs] [rigFilter] [dry]
///
/// `rigFilter` is a substring of the rig id (`Elf/MElf`, `LineageNPCs/`), default: every
/// rig in `L2MeterRigs.Discover()`.
/// </summary>
public static class L2MeterMigration
{
    const string ResultPath = "/tmp/fineage-meter-migration.txt";

    /// <summary>Legacy world factor: 100 (node Lcl Scaling) × 0.019 (importer globalScale).</summary>
    const float PositionFactor = 100f;

    static MeterRig _rig;

    /// <summary>Opt-in: gear prefabs that hold their own copy of the skeleton.</summary>
    static bool RunEmbeddedSkeletonSync;

    [MenuItem("L2/Meter Contract/Migrate all rigs (import, clips, prefabs)")]
    public static void Menu()
    {
        Debug.Log("[MeterMigration] " + RunAll("all", null));
    }

    public static void RunCli()
    {
        string[] extra = BatchArgs.After("L2MeterMigration.RunCli");
        string step = extra.Length > 0 ? extra[0] : "all";
        string filter = extra.Length > 1 ? extra[1] : null;
        bool dry = extra.Contains("dry", StringComparer.OrdinalIgnoreCase);
        RunEmbeddedSkeletonSync = extra.Contains("gearcopies", StringComparer.OrdinalIgnoreCase);
        try
        {
            string report = RunAll(step, filter, dry);
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

    /// <summary>Rig ids that match the CLI filter, resolved once per batch run.</summary>
    public static List<MeterRig> SelectRigs(string filter)
    {
        List<MeterRig> all = L2MeterRigs.Discover();
        return string.IsNullOrEmpty(filter)
            ? all
            : all.Where(r => r.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
    }

    public static string RunAll(string step, string filter, bool dry = false)
    {
        var log = new List<string>();
        File.WriteAllText(ResultPath, step + " filter=" + (filter ?? "*") + "\n");
        List<MeterRig> rigs = SelectRigs(filter);
        if (rigs.Count == 0)
            throw new InvalidOperationException("no rig matched filter '" + filter + "'");
        log.Add("rigs=" + rigs.Count);
        bool all = step == "all";
        int imported = 0, clips = 0, prefabs = 0, cleared = 0, syncedCopies = 0;
        if (step == "rigs")
        {
            foreach (MeterRig rig in rigs)
            {
                log.Add($"{rig.Id} | models={rig.Models} | animFbx={Path.GetFileName(rig.AnimFbx ?? "-")} " +
                        $"| animPrefab={Path.GetFileName(rig.AnimPrefab ?? "-")} " +
                        $"| clipDirs={rig.ClipDirs.Count} clips={L2MeterRigs.Clips(rig).Count()} " +
                        $"| prefabs={L2MeterRigs.Prefabs(rig).Count()}");
            }
            return string.Join("\n", log);
        }
        foreach (MeterRig rig in rigs)
        {
            _rig = rig;
            if (all || step == "imports")
                imported += ConfigureImporters(log);
            if (all || step == "clips")
                clips += ConvertClips(log);
            if (all || step == "prefabs")
                prefabs += RebuildPrefabs(log, out int dropped);
            cleared += ClearStaleScaleOverrides(rig);
            if (RunEmbeddedSkeletonSync)
                syncedCopies += SyncEmbeddedSkeletons(rig, log);
        }

        // Model folders no rig claims (duplicate face meshes, monster2, effect static
        // meshes) still carry UnitScaleFactor 1 and globalScale 0.019, so anything that
        // instances them keeps the legacy 100x bone scale.
        if (all || step == "imports")
        {
            int extra = 0;
            foreach (string folder in L2MeterRigs.UnclaimedModelFolders(rigs))
                extra += ConfigureImportersIn(folder, null);
            if (extra > 0)
                log.Add($"unclaimedImporters={extra}");
            imported += extra;
        }
        AssetDatabase.SaveAssets();
        if (syncedCopies > 0)
            log.Add($"gearCopiesSynced total={syncedCopies}");
        string report = string.Join(" | ", log);
        File.AppendAllText(ResultPath, report + "\n");
        return report;
    }

    /// <summary>Legacy entry point kept so existing batch invocations keep working.</summary>
    public static string Run(string step)
    {
        return RunAll(step, null);
    }

    static IEnumerable<string> ModelFbx(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder))
            yield break;
        foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { folder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                yield return path;
        }
    }

    // --------------------------------------------------------------- §4 import settings
    static int ConfigureImporters(List<string> log)
    {
        int changed = ConfigureImportersIn(_rig.Models, _rig.AnimFbx);
        log.Add($"{_rig.Id}: importers={changed}");
        return changed;
    }

    /// <summary>
    /// Force the meter import contract on every FBX under a folder. Only the unit contract
    /// is touched; animation flags are forced on the anim FBX, whose clips the rig samples.
    /// </summary>
    static int ConfigureImportersIn(string folder, string animFbx)
    {
        int changed = 0;
        if (!AssetDatabase.IsValidFolder(folder))
            return 0;
        var paths = ModelFbx(folder).Distinct().ToList();
        if (!string.IsNullOrEmpty(animFbx) && !paths.Contains(animFbx))
            paths.Add(animFbx);
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (string path in paths)
            {
                if (!(AssetImporter.GetAtPath(path) is ModelImporter importer))
                    continue;
                bool anim = !string.IsNullOrEmpty(animFbx) && path == animFbx;
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
        return changed;
    }

    // ------------------------------------------------------------------- §5 anim clips
    static int ConvertClips(List<string> log)
    {
        string markerPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", _rig.ConvertedClipsPath));
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
        foreach (string path in L2MeterRigs.Clips(_rig))
        {
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
        AssetDatabase.ImportAsset(_rig.ConvertedClipsPath);
        log.Add($"{_rig.Id}: clips={converted} skipped={skipped} positionCurves={positionCurves} scaleCurves={scaleCurves}");
        return converted;
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
    static int RebuildPrefabs(List<string> log, out int cleared)
    {
        cleared = 0;
        // §6 permits rebuilding the subtree, but destroying and re-instantiating it churns
        // prefab fileIDs. Player_/User_/Pawn_ override the carrier SkinnedMeshRenderer by
        // fileID (m_Mesh -> null), so a rebuild silently resurrects the skeleton mesh.
        // The FBX patch is numerically exact, so copying the new local transforms onto the
        // existing objects keeps every fileID and every external override intact.
        int synced = 0;
        if (string.IsNullOrEmpty(_rig.AnimFbx))
        {
            log.Add($"{_rig.Id}: no anim FBX, prefabs left alone");
            return 0;
        }
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(_rig.AnimFbx);
        if (fbx == null)
            throw new FileNotFoundException("missing anim FBX", _rig.AnimFbx);

        var targets = new List<string>();
        if (!string.IsNullOrEmpty(_rig.AnimPrefab))
            targets.Add(_rig.AnimPrefab);
        targets.AddRange(_rig.AnimPrefabs);
        foreach (string path in targets.Distinct())
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                log.Add($"{_rig.Id}: missing {Path.GetFileName(path)}");
                continue;
            }
            synced += SyncSkeletonTransforms(path, fbx);
        }
        cleared = ClearStaleScaleOverrides(_rig);
        log.Add($"{_rig.Id}: animPrefabs={targets.Count} syncedTransforms={synced} clearedScaleOverrides={cleared}");
        return synced;
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
    static int ClearStaleScaleOverrides(MeterRig rig)
    {
        int cleared = 0;
        foreach (string path in L2MeterRigs.GearedPrefabs(rig))
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                bool dirty = false;
                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                {
                    if (Mathf.Abs(t.localScale.x - 100f) > 0.5f)
                        continue;
                    if (!IsMigratedFbxInstance(t, rig))
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
        return cleared;
    }

    static bool IsMigratedFbxInstance(Transform t, MeterRig rig)
    {
        GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject);
        if (source == null)
            return false;
        string path = AssetDatabase.GetAssetPath(source);
        return path.StartsWith(rig.Models, StringComparison.Ordinal)
               && path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gear prefabs are not all nested instances of a migrated FBX. A face, hair or armor
    /// prefab may hold its own copy of the skeleton — the `.ao` and `.mo` are real objects
    /// of the prefab, with `m_LocalScale = 100` on the root and every direct child's
    /// `m_LocalPosition` divided by 100. `ClearStaleScaleOverrides` deliberately skips those
    /// (they are not instances), so the FBX reimport passed them by and the mesh draws 100x:
    /// a dwarf glove at y = 56 instead of y = 0.56.
    ///
    /// Copying the FBX's local transforms wholesale is wrong — the prefab copy keeps the
    /// bind hierarchy in the legacy basis, so its children are not the FBX's children.
    /// The change is a basis swap that has to preserve world space: drop the root to scale 1
    /// and multiply each *direct* child's local position by the scale being removed. A
    /// grandchild is already in a scale-1 frame, so it is untouched. That reproduces the
    /// FBX numbers exactly: `FDwarf_m001_u.ao/bip01` goes from y = -0.00022975808 to
    /// -0.022975808, which is the FBX's `Lcl Translation.y`.
    /// </summary>
    static int SyncEmbeddedSkeletons(MeterRig rig, List<string> log)
    {
        int synced = 0;
        // These flattened prefabs serialize old importer-local translations, not the raw
        // FBX `Lcl Translation`: the root is divided by 100, and descendants keep the
        // old importer's basis. Preserve the world transform when removing the carrier's
        // 100x scale, then repair positions only where the old basis actually drifted.
        foreach (string path in L2MeterRigs.GearedPrefabs(rig))
        {
            if (!rig.NamePrefixes.Any(p => Path.GetFileNameWithoutExtension(path)
                    .StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                continue;
            string fbxPath = L2MeterRigs.ModelFbx(rig)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p)
                    .Equals(Path.GetFileNameWithoutExtension(path), StringComparison.OrdinalIgnoreCase));
            if (fbxPath == null)
                continue;
            var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbx == null)
                continue;
            var source = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Transform sourceRoot = FindSkeletonRoot(source.transform);
                Transform targetRoot = FindSkeletonRoot(root.transform);
                if (sourceRoot == null || targetRoot == null ||
                    Mathf.Abs(targetRoot.localScale.x - 100f) > 0.5f ||
                    PrefabUtility.GetCorrespondingObjectFromSource(targetRoot.gameObject) != null)
                    continue;

                // Drop the x100 carrier and preserve each old world point. This makes the
                // complete flattened hierarchy meter-scaled even where its FBX layout differs.
                var original = targetRoot.GetComponentsInChildren<Transform>(true)
                    .ToDictionary(t => RelativePath(targetRoot, t), t => t.position);
                targetRoot.localScale = Vector3.one;
                foreach (Transform target in targetRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (target == targetRoot)
                        continue;
                    target.position = original[RelativePath(targetRoot, target)];
                }
                foreach (Transform sourceMesh in source.GetComponentsInChildren<Transform>(true))
                {
                    if (!sourceMesh.name.EndsWith(".mo", StringComparison.OrdinalIgnoreCase))
                        continue;
                    Transform targetMesh = root.transform.Find(sourceMesh.name);
                    if (targetMesh != null &&
                        PrefabUtility.GetCorrespondingObjectFromSource(targetMesh.gameObject) == null &&
                        Mathf.Abs(targetMesh.localScale.x - 100f) < 0.5f)
                        targetMesh.localScale = Vector3.one;
                }

                // The old flattened prefab stored its direct children in a different basis.
                // The matching migrated FBX nodes are authoritative for those translations.
                foreach (Transform target in targetRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (target == targetRoot)
                        continue;
                    Transform match = sourceRoot.Find(RelativePath(targetRoot, target));
                    if (match != null)
                        target.localPosition = match.localPosition;
                }
                SyncRendererCaches(root, source);
                PrefabUtility.SaveAsPrefabAsset(root, path);
                synced++;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
                UnityEngine.Object.DestroyImmediate(source);
            }
        }
        if (synced > 0)
            log.Add($"{rig.Id}: gearCopiesSynced={synced}");
        return synced;
    }
}
#endif
