#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Audit and repair LineageWeapons mesh topology. Batchmode entry points mirror other L2 CLIs.
/// Unity -batchmode -nographics -quit -projectPath l2-unity
///   -executeMethod L2WeaponMeshPipeline.RunAuditCli
///   -executeMethod L2WeaponMeshPipeline.RunFixCli
///   -executeMethod L2WeaponMeshPipeline.RunRebakeListCli
/// Env:
///   FINEAGE_WEAPON_AUDIT_JSON=/tmp/weapon-mesh-audit.json
/// </summary>
public static class L2WeaponMeshPipeline
{
    const string ModelsFolder = "Assets/Resources/Data/Animations/LineageWeapons/Models";
    const string PrefabFolder = "Assets/Resources/Data/Animations/LineageWeapons";

    public struct WeaponMeshAuditRow
    {
        public string ModelKey;
        public string FbxPath;
        public string PrefabPath;
        public WeaponMeshTopology.Report Mesh;
        public WeaponImporterMeta Meta;
        public string LastGitCommit;
    }

    public struct WeaponImporterMeta
    {
        public float GlobalScale;
        public bool UseFileScale;
        public int MeshCompression;
        public bool WeldVertices;
        public int MeshOptimizationFlags;
        public ModelImporterNormals NormalMode;
        public bool IsReadable;
        public long AssetModifiedUtcTicks;
    }

    [MenuItem("L2/Weapons/Audit Mesh Topology (Weapongrp)")]
    public static void AuditMenu()
    {
        IList<WeaponMeshAuditRow> rows = CollectWeapongrpWeaponMeshes();
        WriteAuditSummary(rows, logSuccess: true);
    }

    public static void RunAuditCli()
    {
        IList<WeaponMeshAuditRow> rows = CollectWeapongrpWeaponMeshes();
        string jsonPath = Environment.GetEnvironmentVariable("FINEAGE_WEAPON_AUDIT_JSON");
        if (!string.IsNullOrEmpty(jsonPath))
            File.WriteAllText(jsonPath, SerializeAuditJson(rows), Encoding.UTF8);
        bool ok = WriteAuditSummary(rows, logSuccess: true);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static void RunFixCli()
    {
        int fixedCount = 0;
        IList<WeaponMeshAuditRow> rows = CollectWeapongrpWeaponMeshes();
        for (int i = 0; i < rows.Count; i++)
        {
            if (!rows[i].Mesh.IsDegenerate)
                continue;
            if (TryFixFbxMeshes(rows[i].FbxPath))
            {
                fixedCount++;
                L2WeaponPrefabBuilder.TryRebuildPrefabForName(Path.GetFileNameWithoutExtension(rows[i].FbxPath));
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[WeaponMeshPipeline] fixed " + fixedCount + " FBX asset(s)");
        EditorApplication.Exit(fixedCount > 0 ? 0 : rows.Any(r => r.Mesh.IsDegenerate) ? 1 : 0);
    }

    public static void RunRebakeListCli()
    {
        var needsRebake = new List<string>();
        IList<WeaponMeshAuditRow> rows = CollectWeapongrpWeaponMeshes();
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Mesh.IsDegenerate)
                needsRebake.Add(rows[i].ModelKey);
        }

        needsRebake.Sort(StringComparer.Ordinal);
        string listPath = Environment.GetEnvironmentVariable("FINEAGE_WEAPON_REBAKE_LIST");
        if (string.IsNullOrEmpty(listPath))
            listPath = Path.GetFullPath("tools/interlude/.work/weapon-meshes-needing-rebake.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(listPath) ?? ".");
        File.WriteAllLines(listPath, needsRebake);
        Debug.Log(
            "[WeaponMeshPipeline] meshes needing rebake/solidify: "
            + needsRebake.Count
            + " -> "
            + listPath);
        foreach (string line in needsRebake)
            Debug.Log("[WeaponMeshPipeline] REBAKE " + line);
        EditorApplication.Exit(needsRebake.Count > 0 ? 1 : 0);
    }

    static bool WriteAuditSummary(IList<WeaponMeshAuditRow> rows, bool logSuccess)
    {
        var flat = new List<string>();
        int audited = 0;
        for (int i = 0; i < rows.Count; i++)
        {
            WeaponMeshAuditRow row = rows[i];
            audited++;
            if (row.Mesh.IsDegenerate)
            {
                flat.Add(
                    row.ModelKey
                    + " (" + row.FbxPath + "): "
                    + row.Mesh.FailureReason
                    + " git=" + (row.LastGitCommit ?? "?"));
            }
        }

        if (flat.Count > 0)
        {
            Debug.LogError(
                "[WeaponMeshPipeline] FLAT " + flat.Count + "/" + audited + " weapongrp meshes:\n"
                + string.Join("\n", flat));
            return false;
        }

        if (logSuccess)
            Debug.Log("[WeaponMeshPipeline] PASS all " + audited + " weapongrp weapon meshes");
        return true;
    }

    public static IList<WeaponMeshAuditRow> CollectWeapongrpWeaponMeshes()
    {
        var keys = CollectWeapongrpModelKeys();
        var rows = new List<WeaponMeshAuditRow>(keys.Count);
        foreach (string key in keys)
        {
            string shortName = key.StartsWith("LineageWeapons.", StringComparison.Ordinal)
                ? key.Substring("LineageWeapons.".Length)
                : key;
            string fbxPath = ModelsFolder + "/" + shortName + ".fbx";
            string prefabPath = PrefabFolder + "/" + shortName + ".prefab";
            foreach (WeaponMeshAuditRow row in AuditFbxPath(key, fbxPath, prefabPath))
                rows.Add(row);
        }

        return rows;
    }

    static IEnumerable<WeaponMeshAuditRow> AuditFbxPath(string modelKey, string fbxPath, string prefabPath)
    {
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        if (assets == null || assets.Length == 0)
        {
            var missing = new WeaponMeshTopology.Report
            {
                AssetName = modelKey,
                IsDegenerate = true,
            };
            yield return new WeaponMeshAuditRow
            {
                ModelKey = modelKey,
                FbxPath = fbxPath,
                PrefabPath = prefabPath,
                Mesh = missing,
                Meta = ReadImporterMeta(fbxPath),
                LastGitCommit = TryGitLastCommit(fbxPath),
            };
            yield break;
        }

        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is not Mesh mesh || mesh.vertexCount == 0)
                continue;
            yield return new WeaponMeshAuditRow
            {
                ModelKey = modelKey,
                FbxPath = fbxPath,
                PrefabPath = prefabPath,
                Mesh = WeaponMeshTopology.Analyze(mesh, modelKey),
                Meta = ReadImporterMeta(fbxPath),
                LastGitCommit = TryGitLastCommit(fbxPath),
            };
        }
    }

    static HashSet<string> CollectWeapongrpModelKeys()
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        WeapongrpTable table = WeapongrpTable.Instance;
        table.Initialize();
        if (table.WeaponGrps != null)
        {
            foreach (KeyValuePair<int, Weapongrp> pair in table.WeaponGrps)
            {
                Weapongrp grp = pair.Value;
                if (grp == null)
                    continue;
                foreach (string model in WeapongrpHandMeshes.EnumerateModelKeys(grp))
                {
                    if (string.IsNullOrEmpty(model))
                        continue;
                    if (!model.StartsWith("LineageWeapons.", StringComparison.Ordinal))
                        continue;
                    keys.Add(model);
                }
            }
        }

        return keys;
    }

    static WeaponImporterMeta ReadImporterMeta(string assetPath)
    {
        var meta = new WeaponImporterMeta();
        var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (importer == null)
            return meta;
        meta.GlobalScale = importer.globalScale;
        meta.UseFileScale = importer.useFileScale;
        meta.MeshCompression = (int)importer.meshCompression;
        meta.WeldVertices = importer.weldVertices;
        meta.MeshOptimizationFlags = (int)importer.meshOptimizationFlags;
        meta.NormalMode = importer.importNormals;
        meta.IsReadable = importer.isReadable;
        meta.AssetModifiedUtcTicks = File.Exists(assetPath)
            ? File.GetLastWriteTimeUtc(assetPath).Ticks
            : 0;
        return meta;
    }

    static string TryGitLastCommit(string assetPath)
    {
        string fullRepo = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        string rel = assetPath.Replace('\\', '/');
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                WorkingDirectory = fullRepo,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Arguments = "log -1 --format=%h -- \"" + rel + "\"",
            };
            using System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi);
            if (proc == null)
                return null;
            string output = proc.StandardOutput.ReadToEnd().Trim();
            proc.WaitForExit(2000);
            return string.IsNullOrEmpty(output) ? null : output;
        }
        catch
        {
            return null;
        }
    }

    public static bool TryFixFbxMeshes(string fbxPath)
    {
        UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        if (assets == null)
        {
            Debug.LogError("[WeaponMeshPipeline] missing asset " + fbxPath);
            return false;
        }

        bool changed = false;
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is not Mesh mesh)
                continue;
            WeaponMeshTopology.Report before = WeaponMeshTopology.Analyze(mesh, mesh.name);
            if (!before.IsDegenerate)
                continue;
            Mesh fixedMesh = WeaponMeshTopology.BuildSolidifiedMesh(mesh);
            if (fixedMesh == null)
                continue;
            mesh.Clear();
            mesh.vertices = fixedMesh.vertices;
            mesh.triangles = fixedMesh.triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            EditorUtility.SetDirty(mesh);
            changed = true;
            WeaponMeshTopology.Report after = WeaponMeshTopology.Analyze(mesh, mesh.name);
            Debug.Log(
                "[WeaponMeshPipeline] solidified "
                + fbxPath
                + " mesh="
                + mesh.name
                + " opp "
                + before.OppositeNormalPairRatio.ToString("F3")
                + " -> "
                + after.OppositeNormalPairRatio.ToString("F3"));
        }

        if (changed)
            AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate);
        return changed;
    }

    static string SerializeAuditJson(IList<WeaponMeshAuditRow> rows)
    {
        var sb = new StringBuilder();
        sb.Append("{\"weapons\":[");
        for (int i = 0; i < rows.Count; i++)
        {
            if (i > 0)
                sb.Append(',');
            WeaponMeshAuditRow row = rows[i];
            sb.Append('{');
            sb.Append("\"modelKey\":").Append(JsonString(row.ModelKey));
            sb.Append(",\"fbxPath\":").Append(JsonString(row.FbxPath));
            sb.Append(",\"degenerate\":").Append(row.Mesh.IsDegenerate ? "true" : "false");
            sb.Append(",\"oppRatio\":").Append(row.Mesh.OppositeNormalPairRatio.ToString("F4"));
            sb.Append(",\"thickRatio\":").Append(row.Mesh.ThicknessRatio.ToString("F4"));
            sb.Append(",\"vertexCount\":").Append(row.Mesh.VertexCount);
            sb.Append(",\"triangleCount\":").Append(row.Mesh.TriangleCount);
            sb.Append(",\"materialCull\":null");
            sb.Append(",\"submeshCount\":0");
            sb.Append(",\"globalScale\":").Append(row.Meta.GlobalScale.ToString("F4"));
            sb.Append(",\"useFileScale\":").Append(row.Meta.UseFileScale ? "true" : "false");
            sb.Append(",\"meshCompression\":").Append(row.Meta.MeshCompression);
            sb.Append(",\"weldVertices\":").Append(row.Meta.WeldVertices ? "true" : "false");
            sb.Append(",\"gitCommit\":").Append(JsonString(row.LastGitCommit));
            sb.Append('}');
        }

        sb.Append("]}");
        return sb.ToString();
    }

    static string JsonString(string value)
    {
        if (value == null)
            return "null";
        return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }
}
#endif