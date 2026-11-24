using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class MapTileStubAuditEditModeTests
{
    [Test]
    public void NoStubTileSceneRemainsAndBuildSettingsPointAtRealScenes()
    {
        string assets = Application.dataPath;
        string project = Directory.GetParent(assets).FullName;
        var failures = new List<string>();

        foreach (string file in Directory.EnumerateFiles(assets, "*.unity", SearchOption.AllDirectories))
        {
            string name = Path.GetFileNameWithoutExtension(file);
            if (MapTileLoadPolicy.IsMapTileName(name))
                failures.Add("tile scene still present " + Rel(project, file));
        }

        ScanSceneList(project, Path.Combine(project, "ProjectSettings/EditorBuildSettings.asset"), failures);
        string profiles = Path.Combine(assets, "Settings/Build Profiles");
        if (Directory.Exists(profiles))
        {
            foreach (string asset in Directory.EnumerateFiles(profiles, "*.asset", SearchOption.AllDirectories))
                ScanSceneList(project, asset, failures);
        }

        if (failures.Count > 0)
            Assert.Fail(string.Join("\n", failures));
    }

    [Test]
    public void EveryCatalogTilePrefabHasAMeshAndMatchesTheMapFolders()
    {
        string maps = Path.Combine(Application.dataPath, "Resources/Data/Maps");
        var folders = new HashSet<string>();
        foreach (string dir in Directory.GetDirectories(maps))
        {
            string id = Path.GetFileName(dir);
            if (MapTileLoadPolicy.IsMapTileName(id))
                folders.Add(id);
        }

        var catalog = new HashSet<string>(TileStreamingManifest.Ids);
        var failures = new List<string>();
        foreach (string id in folders)
        {
            if (!catalog.Contains(id))
                failures.Add(id + " map folder is not in " + MapTileLoadPolicy.Catalog);
            AuditPrefab(id, failures);
        }

        foreach (string id in catalog)
        {
            if (!folders.Contains(id))
                failures.Add(id + " is in " + MapTileLoadPolicy.Catalog + " but Resources/Data/Maps/" + id + " is missing");
        }

        if (failures.Count > 0)
            Assert.Fail(string.Join("\n", failures));
    }

    static void AuditPrefab(string tileId, List<string> failures)
    {
        string project = Directory.GetParent(Application.dataPath).FullName;
        string prefab = Path.Combine(
            Application.dataPath,
            "Resources/Data/Maps",
            tileId,
            tileId + ".prefab");
        if (!File.Exists(prefab))
        {
            failures.Add(tileId + " prefab missing " + Rel(project, prefab));
            return;
        }

        string text = File.ReadAllText(prefab);
        Match mesh = Regex.Match(
            text,
            @"m_Mesh:\s*\{fileID:\s*(\d+),\s*guid:\s*([0-9a-fA-F]{32}),\s*type:\s*\d+\}");
        if (!mesh.Success || mesh.Groups[1].Value == "0")
        {
            failures.Add(tileId + " prefab has no mesh at " + Rel(project, prefab));
            return;
        }

        string guid = mesh.Groups[2].Value;
        if (guid == "00000000000000000000000000000000")
        {
            failures.Add(tileId + " prefab mesh guid is empty");
            return;
        }

        string meshAsset = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(meshAsset) || !File.Exists(Path.Combine(project, meshAsset)))
        {
            failures.Add(tileId + " prefab mesh asset missing guid " + guid);
            return;
        }

        long bytes = new FileInfo(Path.Combine(project, meshAsset)).Length;
        if (bytes < 1024)
            failures.Add(tileId + " prefab mesh is empty (" + bytes + " bytes) " + meshAsset);
    }

    static void ScanSceneList(string project, string listPath, List<string> failures)
    {
        if (!File.Exists(listPath))
            return;
        string text = File.ReadAllText(listPath);
        foreach (Match match in Regex.Matches(text, @"(?:path|m_path):\s*(\S+)"))
        {
            string rel = match.Groups[1].Value.Trim();
            if (!rel.EndsWith(".unity"))
                continue;
            string name = Path.GetFileNameWithoutExtension(rel);
            if (MapTileLoadPolicy.IsMapTileName(name))
                failures.Add(Rel(project, listPath) + " references tile scene " + rel);
            else if (!File.Exists(Path.Combine(project, rel)))
                failures.Add(Rel(project, listPath) + " references missing scene " + rel);
        }
    }

    static string Rel(string project, string path)
    {
        return path.StartsWith(project) ? path.Substring(project.Length + 1) : path;
    }
}
