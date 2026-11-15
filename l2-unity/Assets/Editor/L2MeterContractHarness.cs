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
        public string meshBoundsSize;
        public string worldBoundsCenter;
        public string worldBoundsSize;
        /// <summary>sampled world vertices, one "x,y,z" string each.</summary>
        public List<string> sampledVertices = new List<string>();
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

        // Every clip binding must still resolve to a transform in the anim prefab.
        var animPrefabs = rig.AnimPrefabs
            .Concat(new[] { rig.AnimPrefab })
            .Concat(rig.ActorPrefabs)
            .Where(p => !string.IsNullOrEmpty(p))
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(p => p != null).ToArray();
        if (animPrefabs.Length > 0)
        {
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
                    bool resolved = animPrefabs.Any(root => root.transform.Find(binding.path) != null);
                    if (!resolved)
                    {
                        failures.Add("unbound clip curve " + clip.assetPath + " -> " + binding.path);
                        break;
                    }
                }
            }
        }

        int stillLegacy = CountLegacyScale(current);
        var report = new StringBuilder();
        report.Append($"{rig.Id}: prefabs={current.prefabs.Count} clips={current.clips.Count} ");
        report.Append($"legacyBoneScale={stillLegacy} failures={failures.Count}");
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
        ok = failures.Count == 0;
        return report.ToString();
    }

    static void ComparePrefab(PrefabEntry expected, PrefabEntry actual, List<string> failures)
    {
        string tag = expected.assetPath;
        var expectedBones = expected.bones.ToDictionary(b => b.path, b => b);
        foreach (BoneEntry bone in actual.bones)
        {
            if (!expectedBones.TryGetValue(bone.path, out BoneEntry was))
            {
                failures.Add($"{tag}: new bone {bone.path}");
                continue;
            }
            if (!NearText(bone.position, was.position, PositionTolerance))
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
            if (!NearText(skin.worldBoundsCenter, was.worldBoundsCenter, PositionTolerance) ||
                !NearText(skin.worldBoundsSize, was.worldBoundsSize, PositionTolerance))
                failures.Add($"{tag}: {skin.nodePath} bounds {was.worldBoundsSize}@{was.worldBoundsCenter}" +
                             $" -> {skin.worldBoundsSize}@{skin.worldBoundsCenter}");
            int count = Math.Min(skin.sampledVertices.Count, was.sampledVertices.Count);
            for (int i = 0; i < count; i++)
            {
                if (!NearText(skin.sampledVertices[i], was.sampledVertices[i], PositionTolerance))
                {
                    failures.Add($"{tag}: {skin.nodePath} vertex[{i}] {was.sampledVertices[i]}" +
                                 $" -> {skin.sampledVertices[i]}");
                    break;
                }
            }
        }
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

    static PrefabEntry CapturePrefab(string assetPath, GameObject instance)
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
                sb.Append("]}");
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
