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
        File.WriteAllText(absolute, JsonOf(data));
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
        Dataset baseline = DatasetFromJson(File.ReadAllText(absolute));
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
            if (!clipsByPath.TryGetValue(clip.assetPath, out ClipEntry actual))
            {
                failures.Add("missing clip " + clip.assetPath);
                continue;
            }
            if (actual.bindingCount < clip.bindingCount)
                failures.Add($"clip {clip.assetPath} lost bindings {clip.bindingCount} -> {actual.bindingCount}");
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

        int stillLegacy = CountLegacyScale(current);
        var report = new StringBuilder();
        report.Append($"verified prefabs={current.prefabs.Count} clips={current.clips.Count} ");
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
            // MSE7 scope: MFighter only (Fighter/ also holds FFighter clips, which bind to
            // FFighter_m000_b.ao and cannot resolve in an MFighter prefab).
            if (path.IndexOf("/MFighter", StringComparison.OrdinalIgnoreCase) < 0)
                continue;
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
