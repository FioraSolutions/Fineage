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
    /// <summary>
    /// True when the FBX at this path already carries the meter import contract. The stale
    /// scale rule needs this: a node at 100 is only wrong when its mesh data is meter.
    /// </summary>
    public static bool IsMeterImporter(string fbxPath)
    {
        return AssetImporter.GetAtPath(fbxPath) is ModelImporter importer
               && Mathf.Approximately(importer.globalScale, 1f)
               && importer.useFileScale;
    }

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
        GameObject rigFbx = null;
        if (!string.IsNullOrEmpty(_rig.AnimFbx))
        {
            rigFbx = AssetDatabase.LoadAssetAtPath<GameObject>(_rig.AnimFbx);
            if (rigFbx == null)
                throw new FileNotFoundException("missing anim FBX", _rig.AnimFbx);
        }

        var targets = new List<string>();
        if (!string.IsNullOrEmpty(_rig.AnimPrefab))
            targets.Add(_rig.AnimPrefab);
        targets.AddRange(_rig.AnimPrefabs);
        // `Player_`/`User_`/`Pawn_` are not always nested instances of the anim prefab: for
        // DarkElf, Dwarf, Elf and Magic the Player prefab owns its own flattened skeleton
        // (measured: 0 PrefabInstance blocks, `.ao` still at scale 100 while every other
        // rig reported 1). Those copies have to follow the FBX too, otherwise §11's
        // `lossyScale ~= 1` never holds for the prefab the game actually spawns.
        targets.AddRange(_rig.ActorPrefabs);
        if (targets.Count == 0)
        {
            log.Add($"{_rig.Id}: no anim prefab, prefabs left alone");
            return 0;
        }
        foreach (string path in targets.Distinct())
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
            {
                log.Add($"{_rig.Id}: missing {Path.GetFileName(path)}");
                continue;
            }
            // A flattened skeleton prefab is not always a copy of the rig's anim FBX: the
            // FFighter pair draws `Models/experiment/test_ffigter_anim.fbx`, and that rig has
            // no anim FBX at all, so the renderer's own mesh is the only correct reference.
            //
            // The anim FBX wins whenever it owns the prefab's skeleton node. Actor prefabs
            // (`Player_`/`User_`/`Pawn_`) are built on the *anim* skeleton, which has its own
            // bind pose: resolving them through the body FBX by name (measured:
            // `Player_MDarkElf` -> `MDarkElf_m000_b.fbx`) moved every bone, because that file
            // rests the pelvis at 0.4848 instead of 0.8153.
            string sourcePath = AnimSkeletonFor(path, _rig) ?? SourceFbxFor(path, _rig) ?? _rig.AnimFbx;
            GameObject source = string.IsNullOrEmpty(sourcePath)
                ? null
                : AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source == null)
            {
                log.Add($"{_rig.Id}: no source FBX for {Path.GetFileName(path)}");
                continue;
            }
            synced += SyncSkeletonTransforms(path, source);
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
            if (targetRoot == null)
                throw new InvalidOperationException("no .ao root in " + prefabPath);
            // A body FBX can hold only the carrier mesh (`FFighter_m000_b.fbx` has a `.mo`
            // and no `.ao`); that rig's skeleton is owned by the armor prefabs, so there is
            // nothing to copy here and it is not an error.
            if (sourceRoot == null)
                return 0;

            int changed = 0;
            // The root itself, then every descendant by relative path.
            if (Apply(targetRoot, sourceRoot, prefabPath))
                changed++;
            foreach (Transform source in sourceRoot.GetComponentsInChildren<Transform>(true))
            {
                if (source == sourceRoot)
                    continue;
                Transform target = targetRoot.Find(RelativePath(sourceRoot, source));
                if (target == null)
                    continue;
                if (Apply(target, source, prefabPath))
                    changed++;
            }

            // The carrier mesh (`.mo`) sits beside the `.ao` in the FBX, but the prefab nests
            // both under a carrier object (`model/FElf_anim_user/`), so a root-level `Find`
            // missed it and the node kept scale 100. Match it relative to the skeleton's
            // parent instead, which is where the FBX puts it.
            Transform sourceParent = sourceRoot.parent;
            Transform targetParent = targetRoot.parent;
            if (sourceParent != null && targetParent != null)
            {
                foreach (Transform source in sourceParent.GetComponentsInChildren<Transform>(true))
                {
                    if (!source.name.EndsWith(".mo", StringComparison.OrdinalIgnoreCase))
                        continue;
                    Transform target = targetParent.Find(RelativePath(sourceParent, source));
                    if (target != null && Apply(target, source, prefabPath))
                        changed++;
                }
            }

            changed += SyncRendererCaches(root, fbxInstance);

            // Only touch the prefab when something actually moved: saving unconditionally
            // rewrites every file on every run, which makes the tool non-idempotent in git.
            bool rootScaleFixed = root.transform.localScale != Vector3.one;
            if (changed == 0 && !rootScaleFixed)
                return 0;
            root.transform.localScale = Vector3.one;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            return changed + (rootScaleFixed ? 1 : 0);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
            UnityEngine.Object.DestroyImmediate(fbxInstance);
        }
    }

    static bool Apply(Transform target, Transform source, string prefabPath = null)
    {
        // A transform that comes from a *nested* prefab instance is not ours to edit: writing
        // to it creates an override on the carrier (and `Player_`/`User_`/`Pawn_` already
        // override the carrier renderer by fileID). The nested prefab is synced on its own.
        //
        // The comparison has to be against the prefab being edited, not "is there a source
        // at all": objects loaded by `LoadPrefabContents` report the prefab asset itself as
        // their corresponding source, so a null check silently blocked every owned object
        // (measured: MDwarf_anim_player stayed at scale 100 with syncedTransforms=0).
        if (prefabPath != null)
        {
            GameObject origin = PrefabUtility.GetCorrespondingObjectFromSource(target.gameObject);
            if (origin != null && AssetDatabase.GetAssetPath(origin) != prefabPath)
                return false;
        }
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
    /// A prefab's *own* object can also hold 100 legitimately — when its mesh is still stored
    /// in raw units, clearing the scale shrinks the part 100x. So an own object only
    /// qualifies when its mesh now comes from a migrated FBX: the mesh data is meter, and the
    /// 100 is the stale half of the old pair. `MFighter_h_2_2_bh` is exactly that case — a
    /// stray `FFighter_m002_m00_bh.mo` whose mesh grew 100x on reimport while its node kept
    /// the scale, so the hair draws 100x too large.
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
                    if (!IsMigratedFbxInstance(t, rig) && !HasMigratedFbxMesh(t))
                        continue;
                    t.localScale = Vector3.one;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(t);
                    dirty = true;
                }
                // Dropping the carrier changes the renderer's space, so its serialized
                // `localBounds` is now wrong by the same factor and the part gets culled.
                if (RefreshOwnRendererBounds(root))
                    dirty = true;
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

    /// <summary>
    /// Recompute `localBounds` for renderers that draw a migrated FBX mesh. The FBX renderer
    /// is authoritative: it is the same mesh in the same (now meter) space.
    /// </summary>
    static bool RefreshOwnRendererBounds(GameObject root)
    {
        var cache = new Dictionary<Mesh, Bounds>();
        bool dirty = false;
        foreach (SkinnedMeshRenderer skin in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            Mesh mesh = skin.sharedMesh;
            if (mesh == null)
                continue;
            string meshPath = AssetDatabase.GetAssetPath(mesh);
            if (string.IsNullOrEmpty(meshPath) ||
                !meshPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) ||
                !IsMeterImporter(meshPath))
                continue;
            if (!cache.TryGetValue(mesh, out Bounds bounds))
            {
                bounds = FbxLocalBounds(meshPath, mesh, skin.transform.lossyScale);
                cache[mesh] = bounds;
            }
            if ((skin.localBounds.center - bounds.center).sqrMagnitude < 1e-12f &&
                (skin.localBounds.size - bounds.size).sqrMagnitude < 1e-12f)
                continue;
            skin.localBounds = bounds;
            dirty = true;
        }
        return dirty;
    }

    static Bounds FbxLocalBounds(string fbxPath, Mesh mesh, Vector3 nodeScale)
    {
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx != null)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
            try
            {
                foreach (SkinnedMeshRenderer skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if (skin.sharedMesh == mesh)
                        return skin.localBounds;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
        // No FBX renderer to copy: the mesh bounds are the bind-pose local bounds.
        return mesh.bounds;
    }

    /// <summary>
    /// True when the node's own renderer draws a mesh stored in a migrated FBX. Such a mesh
    /// is already meter-scaled, so a local scale of 100 is the stale half of the old pair.
    /// </summary>
    static bool HasMigratedFbxMesh(Transform t)
    {
        var skin = t.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = skin != null ? skin.sharedMesh : null;
        if (mesh == null)
        {
            var filter = t.GetComponent<MeshFilter>();
            mesh = filter != null ? filter.sharedMesh : null;
        }
        if (mesh == null)
            return false;
        string path = AssetDatabase.GetAssetPath(mesh);
        if (string.IsNullOrEmpty(path) || !path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            return false;
        return L2MeterMigration.IsMeterImporter(path);
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
    /// of the prefab, with `m_LocalScale = 100` on both and the mesh stored in raw units.
    /// `ClearStaleScaleOverrides` deliberately skips those (they are not instances), so the
    /// FBX reimport passed them by.
    ///
    /// The FBX is resolved from the renderer's **mesh**, not from the prefab's file name:
    /// `FDarkElf_h_1_0_ah.prefab` draws `FDarkElf_m001_m00_ah.fbx`, and matching by name
    /// silently skipped 100 of these prefabs (measured: 223 renderers drawing 100x large).
    ///
    /// The prefab copy keeps the legacy basis: scale 100 on `.ao`/`.mo` with the children
    /// already divided by 100, so its *world* size was right while the mesh was small. The
    /// FBX is now the opposite — scale 1 with a mesh 100x bigger — so the prefab's own
    /// objects have to take the FBX basis. Copying the FBX local transforms by path is
    /// exact: measured `FDarkElf_h_1_0_ah` and `FDarkElf_m001_m00_ah.fbx` share all 111
    /// transform paths, so no hierarchy reconstruction is needed.
    /// </summary>
    static int SyncEmbeddedSkeletons(MeterRig rig, List<string> log)
    {
        int synced = 0;
        foreach (string path in L2MeterRigs.GearedPrefabs(rig))
        {
            string fbxPath = SourceFbxFor(path, rig);
            if (fbxPath == null)
                continue;
            var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbx == null)
                continue;
            var source = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (!HasOwnSkeleton(root))
                    continue;
                int changed = SyncOwnSkeleton(root, source);
                if (changed == 0)
                    continue;
                SyncRendererCaches(root, source);
                root.transform.localScale = Vector3.one;
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

    /// <summary>
    /// The rig's anim FBX when it actually contains the skeleton node this prefab owns.
    /// The anim skeleton and the body FBX have different bind poses for the same bone
    /// names, so picking the wrong one silently re-poses the whole character.
    /// </summary>
    static string AnimSkeletonFor(string prefabPath, MeterRig rig)
    {
        if (string.IsNullOrEmpty(rig.AnimFbx))
            return null;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
            return null;
        Transform skeleton = FindSkeletonRoot(prefab.transform);
        if (skeleton == null)
            return null;
        return FbxHasNode(rig.AnimFbx, skeleton.name) ? rig.AnimFbx : null;
    }

    /// <summary>
    /// The FBX a gear prefab draws, taken from the mesh its renderer references. Falls back
    /// to a name match for prefabs whose renderer is overridden to `m_Mesh: {fileID: 0}`.
    /// </summary>
    static string SourceFbxFor(string prefabPath, MeterRig rig)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            foreach (SkinnedMeshRenderer skin in prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (skin.sharedMesh == null)
                    continue;
                string path = AssetDatabase.GetAssetPath(skin.sharedMesh);
                if (!string.IsNullOrEmpty(path) &&
                    path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                    return path;
            }
            // `Player_`/`User_`/`Pawn_` override the carrier renderer to `m_Mesh: {fileID: 0}`
            // to hide the placeholder body, so the mesh names no FBX. The skeleton node does:
            // `FElf_m000_b.ao` is imported from `FElf_m000_b.fbx`.
            Transform skeleton = FindSkeletonRoot(prefab.transform);
            if (skeleton != null)
            {
                string stem = skeleton.name;
                if (stem.EndsWith(".ao", StringComparison.OrdinalIgnoreCase))
                    stem = stem.Substring(0, stem.Length - 3);
                string match = L2MeterRigs.ModelFbx(rig)
                    .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p)
                        .Equals(stem, StringComparison.OrdinalIgnoreCase));
                if (match != null && FbxHasNode(match, skeleton.name))
                    return match;
                // A name match is not enough: `FFighter_m000_b.fbx` holds only the `.mo`
                // carrier, while the `.ao` this prefab owns was imported from
                // `Models/experiment/test_ffigter_anim.fbx`. Resolve the FBX that actually
                // contains the skeleton node, otherwise the prefab keeps the legacy scale.
                string owner = FbxOwningSkeleton(rig, skeleton.name);
                if (owner != null)
                    return owner;
                if (match != null)
                    return match;
            }
        }
        string prefabStem = Path.GetFileNameWithoutExtension(prefabPath);
        return L2MeterRigs.ModelFbx(rig)
            .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p)
                .Equals(prefabStem, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// The model FBX whose imported hierarchy actually contains the given skeleton node.
    /// Needed where the body FBX carries only the carrier mesh (`FFighter_m000_b.fbx`), so
    /// the `.ao` a prefab owns was imported from a different file in the same rig.
    /// </summary>
    static string FbxOwningSkeleton(MeterRig rig, string skeletonName)
    {
        foreach (string path in L2MeterRigs.ModelFbx(rig))
            if (FbxHasNode(path, skeletonName))
                return path;
        return null;
    }

    /// <summary>True when the imported FBX hierarchy contains a transform of this name.</summary>
    static bool FbxHasNode(string fbxPath, string nodeName)
    {
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbx == null)
            return false;
        foreach (Transform t in fbx.GetComponentsInChildren<Transform>(true))
            if (t.name == nodeName)
                return true;
        return false;
    }

    /// <summary>
    /// True when the prefab owns its skeleton objects instead of nesting the FBX. A nested
    /// instance's transform has a corresponding source object; an owned one does not.
    /// </summary>
    static bool HasOwnSkeleton(GameObject root)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.EndsWith(".ao", StringComparison.OrdinalIgnoreCase))
                return PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) == null;
        }
        return false;
    }

    /// <summary>
    /// Copy the FBX's local transforms onto the prefab's own skeleton objects, matched by
    /// path from the `.ao` root. Every path that exists in both trees is applied; a path
    /// that exists only in one is left alone rather than invented.
    /// </summary>
    static int SyncOwnSkeleton(GameObject root, GameObject source)
    {
        Transform sourceRoot = FindSkeletonRoot(source.transform);
        Transform targetRoot = FindSkeletonRoot(root.transform);
        if (sourceRoot == null || targetRoot == null)
            return 0;

        int changed = 0;
        string prefabPath = AssetDatabase.GetAssetPath(root);
        if (Apply(targetRoot, sourceRoot, prefabPath))
            changed++;
        foreach (Transform target in targetRoot.GetComponentsInChildren<Transform>(true))
        {
            if (target == targetRoot)
                continue;
            Transform match = sourceRoot.Find(RelativePath(targetRoot, target));
            if (match != null && Apply(target, match, prefabPath))
                changed++;
        }

        // The carrier mesh (`.mo`) is a sibling of `.ao`. Its scale has to follow the FBX
        // too: the prefab kept 100 there for a raw-unit mesh, and the mesh is now meter.
        foreach (Transform sourceMesh in source.transform.GetComponentsInChildren<Transform>(true))
        {
            if (!sourceMesh.name.EndsWith(".mo", StringComparison.OrdinalIgnoreCase))
                continue;
            Transform targetMesh = root.transform.Find(sourceMesh.name);
            if (targetMesh != null && Apply(targetMesh, sourceMesh, prefabPath))
                changed++;
        }
        return changed;
    }
}
#endif
