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
/// Read-only diagnostic for the meter contract: prints the imported scale of an FBX and the
/// local/lossy scale chain of an instantiated prefab, so a failing harness comparison can be
/// traced to the FBX, the importer or a stale prefab override instead of guessed at.
///
/// Batch:
///   Unity -batchmode -quit -projectPath l2-unity \
///     -executeMethod L2MeterChainDiag.RunCli &lt;filter&gt;
/// </summary>
public static class L2MeterChainDiag
{
    const string ResultPath = "/tmp/fineage-meter-chain.txt";

    /// <summary>
    /// Dump one prefab by asset path, plus the import scale of the FBX its meshes come from.
    ///   -executeMethod L2MeterChainDiag.DumpPrefabCli Assets/.../FDwarf_m001_u.prefab
    /// </summary>
    public static void DumpPrefabCli()
    {
        string path = BatchArgs.First("L2MeterChainDiag.DumpPrefabCli");
        var log = new StringBuilder();
        try
        {
            log.AppendLine("-- prefab " + path);
            DumpPrefab(log, path, 40);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                try
                {
                    foreach (SkinnedMeshRenderer skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    {
                        if (skin.sharedMesh == null)
                            continue;
                        string source = AssetDatabase.GetAssetPath(skin.sharedMesh);
                        log.AppendLine($"   skin {PathOf(instance.transform, skin.transform)}" +
                                       $" mesh={source} bounds={V(skin.bounds.size)}@{V(skin.bounds.center)}");
                        var first = skin.bones.Length > 0 ? skin.bones[0] : null;
                        log.AppendLine($"      bone0={(first ? PathOf(instance.transform, first) : "(none)")}");
                        Vector3[] verts = skin.sharedMesh.vertices;
                        if (verts.Length > 0)
                            log.AppendLine($"      v0 local={V(verts[0])} world={V(skin.transform.TransformPoint(verts[0]))}");
                    }
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(instance);
                }
            }
            File.WriteAllText(ResultPath, log.ToString());
            Debug.Log(log.ToString());
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    public static void RunCli()
    {
        string filter = BatchArgs.First("L2MeterChainDiag.RunCli") ?? "Orc/MOrc";
        var log = new StringBuilder();
        try
        {
            foreach (MeterRig rig in L2MeterMigration.SelectRigs(filter))
            {
                log.AppendLine("=== " + rig.Id);
                foreach (string fbx in new[] { rig.AnimFbx }
                             .Concat(L2MeterRigs.ModelFbx(rig))
                             .Where(p => !string.IsNullOrEmpty(p))
                             .Distinct()
                             .Take(4))
                    DumpImporter(log, fbx);
                foreach (string prefab in new[] { rig.AnimPrefab }
                             .Concat(rig.AnimPrefabs).Concat(rig.ActorPrefabs)
                             .Where(p => !string.IsNullOrEmpty(p) && File.Exists(p)).Take(6))
                {
                    log.AppendLine("-- prefab " + prefab);
                    DumpPrefab(log, prefab);
                }
            }
            File.WriteAllText(ResultPath, log.ToString());
            Debug.Log(log.ToString());
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    static void DumpImporter(StringBuilder log, string path)
    {
        if (!(AssetImporter.GetAtPath(path) is ModelImporter importer))
            return;
        log.AppendLine($"-- fbx {path}");
        log.AppendLine($"   globalScale={importer.globalScale} useFileScale={importer.useFileScale}" +
                       $" animationType={importer.animationType} importAnimation={importer.importAnimation}");
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (asset == null)
        {
            log.AppendLine("   (asset did not load)");
            return;
        }
        var instance = UnityEngine.Object.Instantiate(asset);
        try
        {
            log.AppendLine($"   assetRoot localScale={V(instance.transform.localScale)}" +
                           $" lossy={V(instance.transform.lossyScale)}");
            foreach (Transform t in instance.GetComponentsInChildren<Transform>(true).Take(6))
            {
                log.AppendLine($"   {PathOf(instance.transform, t),-46} localPos={V(t.localPosition),-32}" +
                               $" localScale={V(t.localScale),-26} lossy={V(t.lossyScale),-28} w={V(t.position)}");
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(instance);
        }
    }

    static void DumpPrefab(StringBuilder log, string path)
    {
        DumpPrefab(log, path, 14);
    }

    static void DumpPrefab(StringBuilder log, string path, int limit)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            log.AppendLine("   (prefab did not load)");
            return;
        }
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        try
        {
            var chain = instance.GetComponentsInChildren<Transform>(true)
                .Where(t => t.name.EndsWith(".ao", StringComparison.OrdinalIgnoreCase) ||
                            t.name.EndsWith(".mo", StringComparison.OrdinalIgnoreCase) ||
                            t.name.StartsWith("Bip01", StringComparison.OrdinalIgnoreCase) ||
                            t.name.StartsWith("bip01", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (Transform t in chain.Take(limit))
            {
                log.AppendLine($"   {PathOf(instance.transform, t),-70} localPos={V(t.localPosition),-30}" +
                               $" localScale={V(t.localScale),-24} lossy={V(t.lossyScale),-26} w={V(t.position)}");
                // A scale that survives on an instanced node is either the source asset's own
                // value or a prefab override; naming it saves chasing the wrong file.
                if (Mathf.Abs(t.localScale.x - 1f) > 1e-3f)
                {
                    GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject);
                    string sourcePath = source != null ? AssetDatabase.GetAssetPath(source) : "(none)";
                    var mods = PrefabUtility.GetPropertyModifications(instance)
                        .Where(m => m.target == (UnityEngine.Object)t)
                        .Select(m => m.propertyPath + "=" + m.value);
                    log.AppendLine($"      ^ source={sourcePath} mods=[{string.Join(", ", mods)}]");
                }
            }
            if (chain.Count == 0)
                log.AppendLine("   (no .ao/.mo/Bip01 nodes)");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(instance);
        }
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

    static string V(Vector3 v) =>
        string.Format(CultureInfo.InvariantCulture, "{0:F3},{1:F3},{2:F3}", v.x, v.y, v.z);
}
#endif
