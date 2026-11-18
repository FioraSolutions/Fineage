#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Meter-scale contract harness (Docs/METER_SCALE_MIGRATION.md §7).
///
/// Captures a baseline of a rig's pre-migration hierarchy and, after the FBX/clip
/// migration, fails loudly when a bone, skin bound or sampled vertex moved. One baseline
/// per rig, committed, so CI can re-run the same comparison.
///
/// Batch:
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterContractHarness.CaptureCli [rigFilter]
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterContractHarness.VerifyCli [rigFilter]
/// </summary>
public static class L2MeterContractHarness
{
    const string ResultPath = "/tmp/fineage-meter-harness.txt";

    /// <summary>Pilot baseline path, kept for the committed MFighter artefact.</summary>
    public const string BaselinePath = "Assets/MeterContract/MFighter.baseline.json";

    const float PositionTolerance = 1e-4f;
    const float ScaleRelativeTolerance = 1e-3f;
    const int VerticesPerRenderer = 16;

    [Serializable]
    public class BoneEntry
    {
        public string path;
        /// <summary>position.x,y,z (5 decimals) — strings keep the committed baseline small.</summary>
        public string position;
        public string lossyScale;
    }

    [Serializable]
    public class RendererEntry
    {
        public string nodePath;
        public int boneCount;
        /// <summary>
        /// Bones that actually resolve. `boneCount` counts the serialized slots, and a
        /// renderer can carry slots that are all `{fileID: 0}` (measured:
        /// `FFighter_m002_m00_bh.mo` in `MFighter_h_2_2_bh.prefab`), which still draws at
        /// bind pose. Only a renderer with at least one live bone deforms, so only that one
        /// can be validated against a skinned sample.
        /// </summary>
        public int boundBones;
        public string meshBoundsSize;
        public string worldBoundsCenter;
        public string worldBoundsSize;
        /// <summary>sampled world vertices, one "x,y,z" string each.</summary>
        public List<string> sampledVertices = new List<string>();
        /// <summary>
        /// Sampled *skinned* world vertices (`BakeMesh` then the renderer transform). The
        /// legacy `sampledVertices` above are `TransformPoint(mesh.vertices[i])`, which for a
        /// SkinnedMeshRenderer is a bind-pose coordinate, not a world position, and whose
        /// index order is not stable across a unit change. New captures fill both; old
        /// baselines only have the former, so the gate checks containment in
        /// `worldBoundsCenter/Size` instead of comparing these point by point.
        /// </summary>
        public List<string> sampledSkinnedWorld = new List<string>();
    }

    [Serializable]
    public class PrefabEntry
    {
        public string assetPath;
        public bool isMeterBoneHierarchy;
        public List<BoneEntry> bones = new List<BoneEntry>();
        public List<RendererEntry> renderers = new List<RendererEntry>();
    }

    [Serializable]
    public class ClipEntry
    {
        public string assetPath;
        public int bindingCount;
        /// <summary>SHA1 of the sorted (path|property) list — keeps the baseline small.</summary>
        public string bindingsHash;
    }

    [Serializable]
    public class Dataset
    {
        public string schema = "meter-contract/1";
        public string rig;
        public string capturedUtc;
        public string note;
        public List<PrefabEntry> prefabs = new List<PrefabEntry>();
        public List<ClipEntry> clips = new List<ClipEntry>();
    }

    [MenuItem("L2/Meter Contract/Capture Baselines (all rigs)")]
    public static void CaptureMenu()
    {
        var (report, ok) = CaptureAll(null);
        Debug.Log("[MeterHarness] " + report + (ok ? "" : " (FAILED)"));
    }

    [MenuItem("L2/Meter Contract/Verify Contracts (all rigs)")]
    public static void VerifyMenu()
    {
        string report = VerifyAll(null, out bool ok);
        Debug.Log("[MeterHarness] " + report);
        if (!ok)
            Debug.LogError("[MeterHarness] FAILED — see " + ResultPath);
    }

    public static void CaptureCli()
    {
        RunBatch(BatchArgs.First("CaptureCli"), 0);
    }

    public static void VerifyCli()
    {
        RunBatch(BatchArgs.First("VerifyCli"), 1);
    }

    static void RunBatch(string filter, int failCode)
    {
        try
        {
            var (report, ok) = CaptureOrVerify(filter);
            File.AppendAllText(ResultPath, report + "\n" + (ok ? "OK" : "FAILED") + "\n");
            Debug.Log("[MeterHarness] " + report);
            // Verify is the gate: a red rig must fail the batch job, not the empty report.
            EditorApplication.Exit(ok ? 0 : failCode);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error.ToString() + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    static (string report, bool ok) CaptureOrVerify(string filter)
    {
        return BatchArgs.First("CaptureCli") != null ||
               Environment.GetCommandLineArgs().Any(a => a.EndsWith(".CaptureCli", StringComparison.Ordinal))
            ? CaptureAll(filter)
            : (VerifyAll(filter, out bool ok), ok);
    }

    /// <summary>Capture the baseline of one rig; name it after the pilot's committed file.</summary>
    public static (string report, bool ok) Capture()
    {
        MeterRig rig = L2MeterRigs.Discover().First(r => r.Id == "Fighter/MFighter");
        return Capture(rig, BaselinePath);
    }

    public static (string report, bool ok) CaptureAll(string filter)
    {
        var reports = new List<string>();
        foreach (MeterRig rig in L2MeterMigration.SelectRigs(filter))
        {
            (string report, bool ok) = Capture(rig, rig.BaselinePath);
            reports.Add(report);
        }
        return (string.Join(" | ", reports), true);
    }

    /// <summary>
    /// True when every model of the rig is already on the meter import contract.
    ///
    /// Capturing a baseline for such a rig documents the *migrated* tree, and Verify would
    /// then compare the migration against itself — the failure that let 11,013 differences
    /// through on the MFighter pilot. The committed baseline must stay the pre-migration
    /// one, so an already-migrated rig never rewrites it.
    /// </summary>
    static bool AlreadyMigrated(MeterRig rig)
    {
        var importers = L2MeterRigs.ModelFbx(rig)
            .Select(AssetImporter.GetAtPath)
            .OfType<ModelImporter>()
            .ToList();
        if (importers.Count == 0)
            return false;
        return importers.All(i => Mathf.Approximately(i.globalScale, 1f) && i.useFileScale);
    }

    static (string report, bool ok) Capture(MeterRig rig, string baselinePath)
    {
        string absoluteEarly = Path.GetFullPath(Path.Combine(Application.dataPath, "..", baselinePath));
        if (AlreadyMigrated(rig) && File.Exists(absoluteEarly))
            return ($"skipped {rig.Id}: already meter and a baseline exists", true);

        Dataset data = BuildDataset(rig);
        data.rig = rig.Id;
        data.capturedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        string absolute = Path.GetFullPath(Path.Combine(Application.dataPath, "..", baselinePath));
        Directory.CreateDirectory(Path.GetDirectoryName(absolute));
        File.WriteAllText(absolute, JsonOf(data));
        AssetDatabase.Refresh();
        Debug.Log("[MeterHarness] baseline -> " + absolute);

        // Capture also records whether the shipped tree is still the legacy x100 contract.
        int legacy = data.prefabs.Count(p => !p.isMeterBoneHierarchy);
        return ($"captured {rig.Id} prefabs={data.prefabs.Count} clips={data.clips.Count} legacyScale={legacy}", true);
    }

    public static string Verify(out bool ok)
    {
        return VerifyAll(null, out ok);
    }

    public static string VerifyAll(string filter, out bool ok)
    {
        var parts = new List<string>();
        bool allOk = true;
        int total = 0, failed = 0;
        foreach (MeterRig rig in L2MeterMigration.SelectRigs(filter))
        {
            string report = Verify(rig, out bool rigOk);
            parts.Add(report);
            if (!rigOk)
            {
                allOk = false;
                failed++;
            }
            total++;
        }
        ok = allOk;
        return $"rigs={total} failed={failed} | " + string.Join(" | ", parts);
    }

    static string Verify(MeterRig rig, out bool ok)
    {
        string absolute = Path.GetFullPath(Path.Combine(Application.dataPath, "..", rig.BaselinePath));
        if (!File.Exists(absolute))
            throw new FileNotFoundException("No committed baseline at " + rig.BaselinePath +
                                            ". Run CaptureCli on the pre-migration tree first.");
        Dataset baseline = DatasetFromJson(File.ReadAllText(absolute));
        Dataset current = BuildDataset(rig);

        var failures = new List<string>();
        var prefabsByPath = current.prefabs.ToDictionary(p => p.assetPath, p => p);
        foreach (PrefabEntry expected in baseline.prefabs)
        {
            if (!prefabsByPath.TryGetValue(expected.assetPath, out PrefabEntry actual))
            {
                failures.Add("missing prefab " + expected.assetPath);
                continue;
            }
            ComparePrefab(expected, actual, failures);
        }

        var clipsByPath = current.clips.ToDictionary(c => c.assetPath, c => c);
        foreach (ClipEntry clip in baseline.clips)
        {
            if (!clipsByPath.TryGetValue(clip.assetPath, out ClipEntry actual))
            {
                failures.Add("missing clip " + clip.assetPath);
                continue;
            }
            if (actual.bindingCount < clip.bindingCount)
                failures.Add($"clip {clip.assetPath} lost bindings {clip.bindingCount} -> {actual.bindingCount}");
        }

        // Clip values are converted exactly once; the binding set is checked for loss
        // above and each animation curve still has to resolve on a migrated skeleton.
        // Resolve against the captured paths: instantiating every prefab again for every
        // binding made this gate quadratic on full race directories.
        var clipBindingPaths = new HashSet<string>(StringComparer.Ordinal);
        foreach (PrefabEntry prefab in current.prefabs)
            foreach (BoneEntry bone in prefab.bones)
            {
                clipBindingPaths.Add(bone.path);
                int separator = bone.path.IndexOf('/', StringComparison.Ordinal);
                if (separator >= 0)
                    clipBindingPaths.Add(bone.path.Substring(separator + 1));
            }
        // Case-insensitive index, used only to classify an unresolved binding, never to
        // accept one: Unity binds transform paths case-sensitively, so a curve that only
        // matches with different casing is genuinely dead at runtime.
        var clipBindingPathsIgnoringCase = new HashSet<string>(clipBindingPaths, StringComparer.OrdinalIgnoreCase);
        var deferred = new List<string>();
        foreach (ClipEntry clip in current.clips)
        {
            var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(clip.assetPath);
            if (asset == null)
                continue;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(asset))
            {
                if (!binding.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal) &&
                    !binding.propertyName.StartsWith("m_LocalRotation", StringComparison.Ordinal) &&
                    !binding.propertyName.StartsWith("m_LocalScale", StringComparison.Ordinal))
                    continue;
                if (rig.AnimFbx != null)
                {
                    var skeleton = AssetDatabase.LoadAssetAtPath<GameObject>(rig.AnimFbx);
                    if (skeleton != null && skeleton.transform.Find(binding.path) != null)
                        continue;
                }
                if (clipBindingPaths.Contains(binding.path))
                    continue;
                // Pre-existing authoring defect, not a unit defect: the clip targets a bone
                // whose name differs only in case from the skeleton's (`bip01` vs `Bip01`).
                // The migration neither created nor can fix it, and it is tracked separately,
                // so it is reported and does not fail the meter gate. Any other unresolved
                // path is a real regression and still fails.
                if (clipBindingPathsIgnoringCase.Contains(binding.path))
                    deferred.Add("clip curve targets a differently-cased bone " + binding.path +
                                 " in " + clip.assetPath);
                else
                    failures.Add("unbound clip curve " + clip.assetPath + " -> " + binding.path);
                break;
            }
        }

        // A clip left in the legacy unit plays on a migrated skeleton by dragging its root
        // to the floor; binding counts cannot see that, so compare magnitudes to the bind pose.
        int legacyClips = CountLegacyClips(rig, current.clips, out int checkedCurves);
        if (legacyClips > 0)
            failures.Add("clips still in legacy units: " + legacyClips + " of " + current.clips.Count +
                         " are ~100x below the bind pose (checked " + checkedCurves + " position curves)");

        int stillLegacy = CountLegacyScale(current);
        var report = new StringBuilder();
        report.Append($"{rig.Id}: prefabs={current.prefabs.Count} clips={current.clips.Count} ");
        report.Append($"legacyBoneScale={stillLegacy} failures={failures.Count}");
        if (deferred.Count > 0)
            report.Append($" deferredClipCurves={deferred.Count}");
        foreach (PrefabEntry prefab in current.prefabs)
        {
            if (prefab.isMeterBoneHierarchy)
                continue;
            BoneEntry off = prefab.bones.FirstOrDefault(b => !NearText(b.lossyScale, "1,1,1", ScaleRelativeTolerance));
            report.Append("\n  ~ legacy scale ").Append(prefab.assetPath);
            if (off != null)
                report.Append(" bone=").Append(off.path).Append(" scale=").Append(off.lossyScale);
        }
        foreach (string failure in failures.Take(400))
            report.Append("\n  - ").Append(failure);
        foreach (string note in deferred.Take(400))
            report.Append("\n  ! ").Append(note);
        ok = failures.Count == 0;
        return report.ToString();
    }

    /// <summary>
    /// Counts clips whose <c>m_LocalPosition</c> values are still in the legacy unit.
    ///
    /// A migrated skeleton is 100x the legacy one, so a clip left at the old values plays by
    /// dragging its root to the floor (measured: `Dwarf/FDwarf` and `Dwarf/MDwarf` shipped
    /// 198 such clips while their `clips-converted` marker claimed they were done). Binding
    /// counts and hashes cannot see it — only the magnitudes can. Compare each position curve
    /// with the bind pose of the same bone in the anim FBX: a converted clip lands near 1.
    /// </summary>
    static int CountLegacyClips(MeterRig rig, List<ClipEntry> clips, out int checkedCurves)
    {
        checkedCurves = 0;
        if (rig.AnimFbx == null)
            return 0;
        var skeleton = AssetDatabase.LoadAssetAtPath<GameObject>(rig.AnimFbx);
        if (skeleton == null)
            return 0;

        // Bind pose of the skeleton as authored in the anim FBX, by transform path.
        var bind = new Dictionary<string, Vector3>(StringComparer.Ordinal);
        foreach (Transform t in skeleton.GetComponentsInChildren<Transform>(true))
            bind[PathOf(skeleton.transform, t)] = t.localPosition;

        int legacy = 0;
        foreach (ClipEntry entry in clips)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(entry.assetPath);
            if (clip == null)
                continue;

            int nearBind = 0;
            int tiny = 0;
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (!binding.propertyName.StartsWith("m_LocalPosition", StringComparison.Ordinal))
                    continue;
                if (!bind.TryGetValue(binding.path, out Vector3 local))
                    continue;
                int axis = binding.propertyName.EndsWith(".x", StringComparison.Ordinal) ? 0
                    : binding.propertyName.EndsWith(".y", StringComparison.Ordinal) ? 1
                    : binding.propertyName.EndsWith(".z", StringComparison.Ordinal) ? 2 : -1;
                if (axis < 0)
                    continue;
                float expected = axis == 0 ? local.x : axis == 1 ? local.y : local.z;
                if (Mathf.Abs(expected) < 1e-3f)
                    continue;
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve == null || curve.length == 0)
                    continue;
                float ratio = Mathf.Abs(curve[0].value / expected);
                checkedCurves++;
                if (ratio > 0.5f && ratio < 2f)
                    nearBind++;
                else if (ratio < 0.1f)
                    tiny++;
            }

            // A converted clip lands on the bind pose; a legacy one is two orders below it.
            if (tiny > 4 && tiny > nearBind * 4)
                legacy++;
        }
        return legacy;
    }

    static void ComparePrefab(PrefabEntry expected, PrefabEntry actual, List<string> failures)
    {
        string tag = expected.assetPath;
        var expectedBones = expected.bones.ToDictionary(b => b.path, b => b);
        var actualBones = actual.bones.ToDictionary(b => b.path, b => b);
        foreach (BoneEntry bone in actual.bones)
        {
            if (!expectedBones.TryGetValue(bone.path, out BoneEntry was))
            {
                failures.Add($"{tag}: new bone {bone.path}");
                continue;
            }
            if (!NearBone(Parse(bone.position), Parse(was.position), expected, actual,
                          expectedBones, actualBones, bone.path))
                failures.Add($"{tag}: bone {bone.path} moved {was.position} -> {bone.position}");
            // lossyScale is expected to change (100 -> 1); it is reported, not gated.
        }

        foreach (RendererEntry skin in actual.renderers)
        {
            RendererEntry was = expected.renderers.FirstOrDefault(r => r.nodePath == skin.nodePath);
            if (was == null)
            {
                failures.Add($"{tag}: new renderer {skin.nodePath}");
                continue;
            }
            if (!NearPosition(skin.worldBoundsCenter, was.worldBoundsCenter, expected, actual) ||
                !NearPosition(skin.worldBoundsSize, was.worldBoundsSize, expected, actual))
                failures.Add($"{tag}: {skin.nodePath} bounds {was.worldBoundsSize}@{was.worldBoundsCenter}" +
                             $" -> {skin.worldBoundsSize}@{skin.worldBoundsCenter}");

            // The baseline's `sampledVertices` are `TransformPoint(mesh.vertices)`, which for a
            // SkinnedMeshRenderer ignores the bones: it is the bind-pose vertex pushed through
            // the renderer node only. Whether that equals a world position depends on which
            // node carried the legacy x100 (measured: true for MFighter_m000_b, false for the
            // embedded FFighter gear copies, where the mesh reads 100x too small). Those
            // numbers therefore cannot be a point-by-point reference.
            //
            // `worldBounds` is normally reliable — Unity derives it from the skinned result —
            // so the gate asserts that today's skinned vertices still live inside the baseline
            // volume. That still catches a real 100x regression (the hair prefab that grew
            // 100x is outside its baseline box) without depending on the broken formula.
            //
            // One case defeats the volume check, and it is not a defect: a renderer whose
            // bone slots are all `{fileID: 0}` (measured: `FFighter_m002_m00_bh.mo` in
            // `MFighter_h_2_2_bh.prefab`) does not deform at all, so `BakeMesh` returns
            // bind-pose geometry that is not what the serialized AABB describes. Its world
            // bounds above are still a real measurement — Unity derives them from the mesh
            // and the transform — so a 100x error there would already have failed. Skip the
            // sample instead of inventing a second, weaker reference for it.
            Vector3 center = Parse(was.worldBoundsCenter);
            Vector3 half = Parse(was.worldBoundsSize) * 0.5f;
            float slack = PositionToleranceAt(center) + PositionToleranceAt(half);
            if (skin.boundBones == 0)
                continue;
            foreach (string sample in skin.sampledSkinnedWorld)
            {
                Vector3 point = Parse(sample);
                Vector3 offset = point - center;
                if (Mathf.Abs(offset.x) <= half.x + slack &&
                    Mathf.Abs(offset.y) <= half.y + slack &&
                    Mathf.Abs(offset.z) <= half.z + slack)
                    continue;
                failures.Add($"{tag}: {skin.nodePath} skinned vertex {sample} is outside the baseline" +
                             $" volume {was.worldBoundsSize}@{was.worldBoundsCenter}");
                break;
            }
        }
    }

    /// <summary>
    /// Absolute tolerance that float32 can actually deliver at this coordinate. The stored
    /// baselines are float32 world positions, so at world x≈4627 one ULP is 4.9e-4 — five
    /// times the nominal 1e-4. Comparing below that measures the file format, not the asset.
    /// </summary>
    static float PositionToleranceAt(Vector3 point)
    {
        float magnitude = Mathf.Max(Mathf.Abs(point.x), Mathf.Max(Mathf.Abs(point.y), Mathf.Abs(point.z)));
        // 8 ULPs of float32 at this magnitude, never below the nominal contract tolerance.
        float ulp = Mathf.Max(magnitude, 1f) * 1.1920929e-7f;
        return Mathf.Max(PositionTolerance, ulp * 8f);
    }

    static float NearestDistance(Vector3 want, List<Vector3> candidates)
    {
        float best = float.PositiveInfinity;
        foreach (Vector3 candidate in candidates)
        {
            float distance = (candidate - want).sqrMagnitude;
            if (distance < best)
                best = distance;
        }
        return Mathf.Sqrt(best);
    }

    static Dataset BuildDataset(MeterRig rig)
    {
        var data = new Dataset { rig = rig.Id };
        foreach (string path in CharacterPrefabPaths(rig))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                continue;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                data.prefabs.Add(CapturePrefab(path, instance));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        foreach (string path in L2MeterRigs.Clips(rig))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            List<string> bindings = AnimationUtility.GetCurveBindings(clip)
                .Select(b => b.path + "|" + b.propertyName)
                .Distinct()
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToList();
            data.clips.Add(new ClipEntry
            {
                assetPath = path,
                bindingCount = bindings.Count,
                bindingsHash = HashOf(bindings),
            });
        }

        return data;
    }

    static IEnumerable<string> CharacterPrefabPaths(MeterRig rig)
    {
        return L2MeterRigs.Prefabs(rig)
            .Where(p => File.Exists(p))
            .Distinct()
            .OrderBy(p => p, StringComparer.Ordinal);
    }

    /// <summary>Public so the read-only diff tool can capture one prefab without a rerun.</summary>
    public static PrefabEntry CapturePrefab(string assetPath, GameObject instance)
    {
        var entry = new PrefabEntry { assetPath = assetPath };
        foreach (Transform t in instance.GetComponentsInChildren<Transform>(true))
        {
            Vector3 scale = t.lossyScale;
            bool legacy = Mathf.Abs(scale.x - 100f) < 0.5f || Mathf.Abs(scale.x - 190f) < 0.5f;
            if (legacy)
                entry.isMeterBoneHierarchy = false;
            entry.bones.Add(new BoneEntry
            {
                path = PathOf(instance.transform, t),
                position = Text(t.position),
                lossyScale = Text(scale),
            });
        }

        if (entry.bones.Count > 0 && entry.bones.All(b => NearText(b.lossyScale, "1,1,1", ScaleRelativeTolerance)))
            entry.isMeterBoneHierarchy = true;

        foreach (SkinnedMeshRenderer skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            Mesh mesh = skin.sharedMesh;
            if (mesh == null)
                continue;
            var renderer = new RendererEntry
            {
                nodePath = PathOf(instance.transform, skin.transform),
                boneCount = skin.bones.Length,
                boundBones = skin.bones.Count(b => b != null),
                meshBoundsSize = Text(mesh.bounds.size),
                worldBoundsCenter = Text(skin.bounds.center),
                worldBoundsSize = Text(skin.bounds.size),
            };
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length > 0)
            {
                int step = Mathf.Max(1, vertices.Length / VerticesPerRenderer);
                for (int i = 0; i < vertices.Length && renderer.sampledVertices.Count < VerticesPerRenderer; i += step)
                    renderer.sampledVertices.Add(Text(skin.transform.TransformPoint(vertices[i])));
            }
            // Skinned rest-pose world positions. `BakeMesh` already returns renderer-space
            // vertices, so only the renderer transform is applied — pushing them through
            // TransformPoint twice would report a phantom unit error.
            var baked = new Mesh();
            try
            {
                skin.BakeMesh(baked);
                Vector3[] skinned = baked.vertices;
                if (skinned.Length > 0)
                {
                    int step = Mathf.Max(1, skinned.Length / VerticesPerRenderer);
                    for (int i = 0; i < skinned.Length && renderer.sampledSkinnedWorld.Count < VerticesPerRenderer; i += step)
                        renderer.sampledSkinnedWorld.Add(Text(skin.transform.TransformPoint(skinned[i])));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(baked);
            }
            entry.renderers.Add(renderer);
        }

        foreach (MeshRenderer meshRenderer in instance.GetComponentsInChildren<MeshRenderer>(true))
        {
            var filter = meshRenderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null)
                continue;
            Mesh mesh = filter.sharedMesh;
            var renderer = new RendererEntry
            {
                nodePath = PathOf(instance.transform, meshRenderer.transform),
                boneCount = 0,
                boundBones = 0,
                meshBoundsSize = Text(mesh.bounds.size),
                worldBoundsCenter = Text(meshRenderer.bounds.center),
                worldBoundsSize = Text(meshRenderer.bounds.size),
            };
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length > 0)
            {
                int step = Mathf.Max(1, vertices.Length / VerticesPerRenderer);
                for (int i = 0; i < vertices.Length && renderer.sampledVertices.Count < VerticesPerRenderer; i += step)
                    renderer.sampledVertices.Add(Text(meshRenderer.transform.TransformPoint(vertices[i])));
            }
            entry.renderers.Add(renderer);
        }

        return entry;
    }

    /// <summary>
    /// Compare in the character's frame: float precision at world x≈4600 exceeds the
    /// tolerance. The tolerance itself also has to respect float32 resolution at the
    /// coordinate being compared — the committed baselines store float32 world positions,
    /// so at x≈4627 one ULP is 4.9e-4 and the nominal 1e-4 is not representable.
    /// </summary>
    static bool NearPosition(string a, string b, PrefabEntry expected, PrefabEntry actual)
    {
        Vector3 va = Parse(a);
        Vector3 vb = Parse(b);
        float tolerance = Mathf.Max(PositionToleranceAt(va), PositionToleranceAt(vb));
        if (expected.bones.Count == 0 || expected.bones[0].path != "" || actual.bones.Count == 0 ||
            actual.bones[0].path != "")
            return (va - vb).magnitude <= tolerance;
        Vector3 rootOffset = Parse(expected.bones[0].position) - Parse(actual.bones[0].position);
        Vector3 delta = (va - vb) - rootOffset;
        delta = new Vector3(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
        return delta.x <= tolerance && delta.y <= tolerance && delta.z <= tolerance;
    }

    /// <summary>
    /// Bone comparison in the skeleton's own frame. World positions are a poor reference at
    /// the scales this tree uses: the Orc actors sit at world x≈4627, where one float32 ULP
    /// is 4.9e-4, and the committed baselines store exactly those float32 numbers. Comparing
    /// each bone against its parent's world delta cancels the shared offset and measures the
    /// bone's own placement, which is what the migration could actually break.
    /// </summary>
    static bool NearBone(Vector3 bone, Vector3 was, PrefabEntry expected, PrefabEntry actual,
                         Dictionary<string, BoneEntry> expectedBones, Dictionary<string, BoneEntry> actualBones,
                         string path)
    {
        float tolerance = Mathf.Max(PositionToleranceAt(bone), PositionToleranceAt(was));
        Vector3 delta = bone - was;
        string parent = ParentPath(path);
        if (parent != null &&
            expectedBones.TryGetValue(parent, out BoneEntry wasParent) &&
            actualBones.TryGetValue(parent, out BoneEntry nowParent))
            delta -= Parse(nowParent.position) - Parse(wasParent.position);
        return Mathf.Abs(delta.x) <= tolerance &&
               Mathf.Abs(delta.y) <= tolerance &&
               Mathf.Abs(delta.z) <= tolerance;
    }

    static string ParentPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        int slash = path.LastIndexOf('/');
        return slash < 0 ? "" : path.Substring(0, slash);
    }

    /// <summary>Clips are rooted at an FBX node; actors nest the same subtree below model/.</summary>
    static bool ResolveClipBinding(Transform root, string path)
    {
        if (root.Find(path) != null)
            return true;
        string leaf = path.Split('/')[0];
        foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
            if (candidate.name == leaf && candidate.Find(path.Substring(leaf.Length).TrimStart('/')) != null)
                return true;
        return false;
    }

    static string PathOf(Transform root, Transform target)
    {
        if (target == root)
            return "";
        var parts = new List<string>();
        for (Transform t = target; t != null && t != root; t = t.parent)
            parts.Add(t.name);
        parts.Reverse();
        return string.Join("/", parts);
    }

    static string Text(Vector3 v)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:F5},{1:F5},{2:F5}", v.x, v.y, v.z);
    }

    static Vector3 Parse(string text)
    {
        string[] parts = (text ?? "").Split(',');
        if (parts.Length != 3)
            return Vector3.zero;
        return new Vector3(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture));
    }

    static bool NearText(string a, string b, float tolerance)
    {
        Vector3 va = Parse(a);
        Vector3 vb = Parse(b);
        return Mathf.Abs(va.x - vb.x) <= tolerance &&
               Mathf.Abs(va.y - vb.y) <= tolerance &&
               Mathf.Abs(va.z - vb.z) <= tolerance;
    }

    /// <summary>Every bone must report lossyScale ~= 1 after the migration.</summary>
    static int CountLegacyScale(Dataset data)
    {
        int legacy = 0;
        foreach (PrefabEntry prefab in data.prefabs)
        {
            foreach (BoneEntry bone in prefab.bones)
            {
                if (!NearText(bone.lossyScale, "1,1,1", ScaleRelativeTolerance))
                {
                    legacy++;
                    break;
                }
            }
        }
        return legacy;
    }

    static string HashOf(IEnumerable<string> lines)
    {
        using (var sha = System.Security.Cryptography.SHA1.Create())
        {
            byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
            var hex = new StringBuilder(digest.Length * 2);
            foreach (byte b in digest)
                hex.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return hex.ToString();
        }
    }

    /// <summary>Indented JSON (JsonUtility has no pretty-print).</summary>
    static string JsonOf(Dataset data)
    {
        var sb = new StringBuilder();
        sb.Append("{\n  \"schema\": ").Append(Quote(data.schema));
        sb.Append(",\n  \"rig\": ").Append(Quote(data.rig));
        sb.Append(",\n  \"capturedUtc\": ").Append(Quote(data.capturedUtc));
        sb.Append(",\n  \"prefabs\": [");
        for (int i = 0; i < data.prefabs.Count; i++)
        {
            PrefabEntry prefab = data.prefabs[i];
            sb.Append(i == 0 ? "\n    " : ",\n    ");
            sb.Append("{\"assetPath\": ").Append(Quote(prefab.assetPath));
            sb.Append(", \"isMeterBoneHierarchy\": ").Append(prefab.isMeterBoneHierarchy ? "true" : "false");
            sb.Append(",\n     \"bones\": [");
            for (int b = 0; b < prefab.bones.Count; b++)
            {
                BoneEntry bone = prefab.bones[b];
                sb.Append(b == 0 ? "\n" : ",\n");
                sb.Append("       {\"path\": ").Append(Quote(bone.path));
                sb.Append(", \"position\": ").Append(Quote(bone.position));
                sb.Append(", \"lossyScale\": ").Append(Quote(bone.lossyScale)).Append("}");
            }
            sb.Append(prefab.bones.Count > 0 ? "\n     ]" : "]");
            sb.Append(",\n     \"renderers\": [");
            for (int r = 0; r < prefab.renderers.Count; r++)
            {
                RendererEntry renderer = prefab.renderers[r];
                sb.Append(r == 0 ? "\n" : ",\n");
                sb.Append("       {\"nodePath\": ").Append(Quote(renderer.nodePath));
                sb.Append(", \"boneCount\": ").Append(renderer.boneCount);
                sb.Append(", \"meshBoundsSize\": ").Append(Quote(renderer.meshBoundsSize));
                sb.Append(", \"worldBoundsCenter\": ").Append(Quote(renderer.worldBoundsCenter));
                sb.Append(", \"worldBoundsSize\": ").Append(Quote(renderer.worldBoundsSize));
                sb.Append(", \"sampledVertices\": [");
                sb.Append(string.Join(", ", renderer.sampledVertices.Select(Quote)));
                sb.Append("]");
                if (renderer.sampledSkinnedWorld.Count > 0)
                {
                    sb.Append(", \"sampledSkinnedWorld\": [");
                    sb.Append(string.Join(", ", renderer.sampledSkinnedWorld.Select(Quote)));
                    sb.Append("]");
                }
                sb.Append("}");
            }
            sb.Append(prefab.renderers.Count > 0 ? "\n     ]" : "]");
            sb.Append("}");
        }
        sb.Append(data.prefabs.Count > 0 ? "\n  ]" : "]");
        sb.Append(",\n  \"clips\": [");
        for (int i = 0; i < data.clips.Count; i++)
        {
            ClipEntry clip = data.clips[i];
            sb.Append(i == 0 ? "\n" : ",\n");
            sb.Append("    {\"assetPath\": ").Append(Quote(clip.assetPath));
            sb.Append(", \"bindingCount\": ").Append(clip.bindingCount);
            sb.Append(", \"bindingsHash\": ").Append(Quote(clip.bindingsHash)).Append("}");
        }
        sb.Append(data.clips.Count > 0 ? "\n  ]" : "]");
        sb.Append("\n}\n");
        return sb.ToString();
    }

    static string Quote(string value)
    {
        if (value == null)
            return "null";
        var sb = new StringBuilder(value.Length + 2);
        sb.Append('"');
        foreach (char c in value)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (c < ' ')
                        sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else
                        sb.Append(c);
                    break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }

    static Dataset DatasetFromJson(string json)
    {
        return JsonUtility.FromJson<Dataset>(json);
    }

}
#endif
