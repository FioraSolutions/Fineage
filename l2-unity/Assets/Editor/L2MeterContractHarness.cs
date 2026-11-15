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
/// Captures a baseline of the pre-migration character hierarchy and, after the FBX/clip
/// migration, fails loudly when a bone, skin bound or sampled vertex moved. The baseline
/// is committed so CI can re-run the same comparison.
///
/// Batch:
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterContractHarness.CaptureCli   (writes the baseline JSON)
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterContractHarness.VerifyCli    (exit 1 on drift)
/// </summary>
public static class L2MeterContractHarness
{
    const string RaceRoot = "Assets/Resources/Data/Animations/Fighter";
    const string HumanRoot = "Assets/Resources/Data/Animations/Human/MFighter";
    const string ResultPath = "/tmp/fineage-meter-harness.txt";

    /// <summary>Committed baseline (small JSON, one entry per captured prefab).</summary>
    public const string BaselinePath = "Assets/MeterContract/MFighter.baseline.json";

    const float PositionTolerance = 1e-4f;
    const float ScaleRelativeTolerance = 1e-3f;
    const int VerticesPerRenderer = 16;

    [Serializable]
    public class BoneEntry
    {
        public string path;
        public Vector3 position;
        public Vector3 lossyScale;
    }

    [Serializable]
    public class RendererEntry
    {
        public string nodePath;
        public int boneCount;
        public Vector3 meshBoundsSize;
        public Vector3 worldBoundsCenter;
        public Vector3 worldBoundsSize;
        public List<Vector3> sampledVertices = new List<Vector3>();
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
        public List<string> bindings = new List<string>();
    }

    [Serializable]
    public class Dataset
    {
        public string schema = "meter-contract/1";
        public string capturedUtc;
        public string note;
        public List<PrefabEntry> prefabs = new List<PrefabEntry>();
        public List<ClipEntry> clips = new List<ClipEntry>();
    }

    [MenuItem("L2/Meter Contract/Capture MFighter Baseline")]
    public static void CaptureMenu()
    {
        var (report, ok) = Capture();
        Debug.Log("[MeterHarness] " + report + (ok ? "" : " (FAILED)"));
    }

    [MenuItem("L2/Meter Contract/Verify MFighter Contract")]
    public static void VerifyMenu()
    {
        string report = Verify(out bool ok);
        Debug.Log("[MeterHarness] " + report);
        if (!ok)
            Debug.LogError("[MeterHarness] FAILED — see " + ResultPath);
    }

    public static void CaptureCli()
    {
        RunBatch(Capture, 0);
    }

    public static void VerifyCli()
    {
        RunBatch(() =>
        {
            string report = Verify(out bool ok);
            return (report, ok);
        }, 1);
    }

    static void RunBatch(Func<(string report, bool ok)> work, int failCode)
    {
        try
        {
            var (report, ok) = work();
            File.AppendAllText(ResultPath, report + "\n" + (ok ? "OK" : "FAILED") + "\n");
            Debug.Log("[MeterHarness] " + report);
            EditorApplication.Exit(ok ? 0 : failCode);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error.ToString() + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    public static (string report, bool ok) Capture()
    {
        Dataset data = BuildDataset();
        data.capturedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        string absolute = Path.GetFullPath(Path.Combine(Application.dataPath, "..", BaselinePath));
        Directory.CreateDirectory(Path.GetDirectoryName(absolute));
        File.WriteAllText(absolute, JsonUtility.ToJson(data, true));
        AssetDatabase.Refresh();
        Debug.Log("[MeterHarness] baseline -> " + absolute);

        // Capture also records whether the shipped tree is still the legacy x100 contract.
        int legacy = data.prefabs.Count(p => !p.isMeterBoneHierarchy);
        return ($"captured prefabs={data.prefabs.Count} clips={data.clips.Count} legacyScale={legacy}", true);
    }

    public static string Verify(out bool ok)
    {
        string absolute = Path.GetFullPath(Path.Combine(Application.dataPath, "..", BaselinePath));
        if (!File.Exists(absolute))
            throw new FileNotFoundException("No committed baseline at " + BaselinePath +
                                            ". Run CaptureCli on the pre-migration tree first.");
        Dataset baseline = JsonUtility.FromJson<Dataset>(File.ReadAllText(absolute));
        Dataset current = BuildDataset();

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
            if (!clipsByPath.ContainsKey(clip.assetPath))
                failures.Add("missing clip " + clip.assetPath);
        }

        // Every clip binding must still resolve to a transform in the anim prefab.
        var animPrefabs = new[]
        {
            RaceRoot + "/MFighter/MFighter_anim_player.prefab",
            RaceRoot + "/MFighter/MFighter_anim_user.prefab",
            HumanRoot + "/Player_MFighter.prefab",
            HumanRoot + "/User_MFighter.prefab",
            HumanRoot + "/Pawn_MFighter.prefab",
        }.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(p => p != null).ToArray();
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

        int stillLegacy = current.prefabs.Count(p => !p.isMeterBoneHierarchy);
        var report = new StringBuilder();
        report.Append($"verified prefabs={current.prefabs.Count} clips={current.clips.Count} ");
        report.Append($"legacyRootScale={stillLegacy} failures={failures.Count}");
        foreach (string failure in failures.Take(40))
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
            if (!Near(bone.position, was.position, PositionTolerance))
                failures.Add($"{tag}: bone {bone.path} moved {Fmt(was.position)} -> {Fmt(bone.position)}");
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
            if (!Near(skin.worldBoundsCenter, was.worldBoundsCenter, PositionTolerance) ||
                !Near(skin.worldBoundsSize, was.worldBoundsSize, PositionTolerance))
                failures.Add($"{tag}: {skin.nodePath} bounds {Fmt(was.worldBoundsSize)}@{Fmt(was.worldBoundsCenter)}" +
                             $" -> {Fmt(skin.worldBoundsSize)}@{Fmt(skin.worldBoundsCenter)}");
            int count = Math.Min(skin.sampledVertices.Count, was.sampledVertices.Count);
            for (int i = 0; i < count; i++)
            {
                if (!Near(skin.sampledVertices[i], was.sampledVertices[i], PositionTolerance))
                {
                    failures.Add($"{tag}: {skin.nodePath} vertex[{i}] {Fmt(was.sampledVertices[i])}" +
                                 $" -> {Fmt(skin.sampledVertices[i])}");
                    break;
                }
            }
        }
    }

    static Dataset BuildDataset()
    {
        var data = new Dataset();
        foreach (string path in CharacterPrefabPaths())
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

        foreach (string guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { RaceRoot }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                continue;
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            data.clips.Add(new ClipEntry
            {
                assetPath = path,
                bindings = AnimationUtility.GetCurveBindings(clip)
                    .Select(b => b.path + "|" + b.propertyName)
                    .Distinct()
                    .OrderBy(s => s, StringComparer.Ordinal)
                    .ToList(),
            });
        }

        return data;
    }

    static IEnumerable<string> CharacterPrefabPaths()
    {
        var paths = new List<string>
        {
            HumanRoot + "/Player_MFighter.prefab",
            HumanRoot + "/User_MFighter.prefab",
            HumanRoot + "/Pawn_MFighter.prefab",
        };
        foreach (string folder in new[] { RaceRoot, HumanRoot + "/Hair", HumanRoot + "/Faces" })
        {
            if (!AssetDatabase.IsValidFolder(folder))
                continue;
            foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { folder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileName(path);
                if (!name.StartsWith("MFighter_", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("F_MFighter_", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("Player_MFighter", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("User_MFighter", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("Pawn_MFighter", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (name.Contains("_anim_"))
                    continue;
                paths.Add(path);
            }
        }
        return paths.Distinct().OrderBy(p => p, StringComparer.Ordinal);
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
                position = t.position,
                lossyScale = scale,
            });
        }

        if (entry.bones.Count > 0 && entry.bones.All(b => Mathf.Abs(b.lossyScale.x - 1f) < 1e-3f))
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
                meshBoundsSize = mesh.bounds.size,
                worldBoundsCenter = skin.bounds.center,
                worldBoundsSize = skin.bounds.size,
            };
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length > 0)
            {
                int step = Mathf.Max(1, vertices.Length / VerticesPerRenderer);
                for (int i = 0; i < vertices.Length && renderer.sampledVertices.Count < VerticesPerRenderer; i += step)
                    renderer.sampledVertices.Add(skin.transform.TransformPoint(vertices[i]));
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
                meshBoundsSize = mesh.bounds.size,
                worldBoundsCenter = meshRenderer.bounds.center,
                worldBoundsSize = meshRenderer.bounds.size,
            };
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length > 0)
            {
                int step = Mathf.Max(1, vertices.Length / VerticesPerRenderer);
                for (int i = 0; i < vertices.Length && renderer.sampledVertices.Count < VerticesPerRenderer; i += step)
                    renderer.sampledVertices.Add(meshRenderer.transform.TransformPoint(vertices[i]));
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

    static bool Near(Vector3 a, Vector3 b, float tolerance)
    {
        return Mathf.Abs(a.x - b.x) <= tolerance &&
               Mathf.Abs(a.y - b.y) <= tolerance &&
               Mathf.Abs(a.z - b.z) <= tolerance;
    }

    static bool NearRelative(Vector3 a, Vector3 b, float tolerance)
    {
        return Mathf.Abs(a.x - b.x) <= tolerance * Mathf.Max(1f, Mathf.Abs(b.x)) &&
               Mathf.Abs(a.y - b.y) <= tolerance * Mathf.Max(1f, Mathf.Abs(b.y)) &&
               Mathf.Abs(a.z - b.z) <= tolerance * Mathf.Max(1f, Mathf.Abs(b.z));
    }

    /// <summary>Every bone must report lossyScale ~= 1 after the migration.</summary>
    static int CountLegacyScale(Dataset data)
    {
        int legacy = 0;
        foreach (PrefabEntry prefab in data.prefabs)
        {
            foreach (BoneEntry bone in prefab.bones)
            {
                if (!NearRelative(bone.lossyScale, Vector3.one, ScaleRelativeTolerance))
                {
                    legacy++;
                    break;
                }
            }
        }
        return legacy;
    }

    static string Fmt(Vector3 v)
    {
        return string.Format(CultureInfo.InvariantCulture, "({0:F4},{1:F4},{2:F4})", v.x, v.y, v.z);
    }
}
#endif
