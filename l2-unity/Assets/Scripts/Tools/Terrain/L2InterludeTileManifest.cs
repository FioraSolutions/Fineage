#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class L2InterludeTileManifestFile
{
    public string schemaVersion = "1";
    public string updatedUtc;
    public string clientRootHint = "(local only — not stored)";
    public int clientMapCount;
    public int importedFullCount;
    public int importedPartialCount;
    public List<L2InterludeTileManifestEntry> tiles = new List<L2InterludeTileManifestEntry>();
}

[Serializable]
public class L2InterludeTileManifestEntry
{
    public string id;
    public bool inClient;
    public bool inRepo;
    public string importStatus;
    public List<string> repoArtifacts = new List<string>();
    public string notes;
}

public static class L2InterludeTileManifest
{
    public const string ManifestRepoPath = "Docs/INTERLUDE_TILE_MANIFEST.json";
    static readonly Regex TileIdPattern = new Regex(@"^(\d{2}_\d{2})\.unr$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string ManifestAbsolutePath =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ManifestRepoPath));

    public static L2InterludeTileManifestFile Build(string clientRootOverride = null)
    {
        string clientRoot = string.IsNullOrEmpty(clientRootOverride)
            ? L2InterludeClientSettings.ClientRoot
            : clientRootOverride;

        HashSet<string> clientTiles = ScanClientTiles(clientRoot);
        HashSet<string> repoTiles = ScanRepoTiles();

        HashSet<string> all = new HashSet<string>(clientTiles);
        all.UnionWith(repoTiles);

        List<L2InterludeTileManifestEntry> entries = new List<L2InterludeTileManifestEntry>();
        int full = 0;
        int partial = 0;

        foreach (string id in all.OrderBy(x => x, StringComparer.Ordinal))
        {
            L2InterludeTileManifestEntry entry = EvaluateTile(id, clientTiles.Contains(id), repoTiles.Contains(id));
            entries.Add(entry);
            if (entry.importStatus == "full")
            {
                full++;
            }
            else if (entry.importStatus == "partial")
            {
                partial++;
            }
        }

        return new L2InterludeTileManifestFile
        {
            updatedUtc = DateTime.UtcNow.ToString("o"),
            clientMapCount = clientTiles.Count,
            importedFullCount = full,
            importedPartialCount = partial,
            tiles = entries
        };
    }

    public static void WriteToRepo(L2InterludeTileManifestFile manifest)
    {
        string path = ManifestAbsolutePath;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonUtility.ToJson(manifest, true));
    }

    public static HashSet<string> ScanClientTiles(string clientRoot)
    {
        HashSet<string> ids = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(clientRoot))
        {
            return ids;
        }

        string mapsDir = Path.Combine(clientRoot, "maps");
        if (!Directory.Exists(mapsDir))
        {
            return ids;
        }

        foreach (string file in Directory.GetFiles(mapsDir, "*.unr"))
        {
            Match match = TileIdPattern.Match(Path.GetFileName(file));
            if (match.Success)
            {
                ids.Add(match.Groups[1].Value);
            }
        }

        return ids;
    }

    public static HashSet<string> ScanRepoTiles()
    {
        HashSet<string> ids = new HashSet<string>();
        string mapsRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "Resources", "Data", "Maps"));
        if (!Directory.Exists(mapsRoot))
        {
            return ids;
        }

        foreach (string dir in Directory.GetDirectories(mapsRoot))
        {
            string name = Path.GetFileName(dir);
            if (TileIdPattern.IsMatch(name + ".unr"))
            {
                ids.Add(name);
            }
        }

        return ids;
    }

    public static L2InterludeTileManifestEntry EvaluateTile(string tileId, bool inClient, bool inRepo)
    {
        L2InterludeTileManifestEntry entry = new L2InterludeTileManifestEntry
        {
            id = tileId,
            inClient = inClient,
            inRepo = inRepo
        };

        if (!inRepo)
        {
            entry.importStatus = "missing";
            return entry;
        }

        List<string> artifacts = new List<string>();
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        void Track(string relativeAssetPath)
        {
            string full = Path.GetFullPath(Path.Combine(projectRoot, relativeAssetPath));
            if (File.Exists(full))
            {
                artifacts.Add(relativeAssetPath.Replace("\\", "/"));
            }
        }

        Track(L2InterludeMapPaths.MetaFolder(tileId) + "/TerrainInfo0.txt");
        Track(L2InterludeMapPaths.T3dAssetPath(tileId));
        Track(L2InterludeMapPaths.TileAssetFolder(tileId) + "/" + tileId + ".prefab");
        Track(L2InterludeMapPaths.TileAssetFolder(tileId) + "/" + tileId + "_Mesh.asset");
        Track(L2InterludeMapPaths.TileAssetFolder(tileId) + "/UnityTerrain/" + tileId + ".prefab");
        Track(L2InterludeMapPaths.TileAssetFolder(tileId) + "/StaticMeshes.prefab");

        entry.repoArtifacts = artifacts;

        bool hasMeta = artifacts.Any(a => a.EndsWith("TerrainInfo0.txt", StringComparison.Ordinal));
        bool hasTerrainPrefab = artifacts.Any(a => a.EndsWith(".prefab", StringComparison.Ordinal) && a.Contains("UnityTerrain/"));
        bool hasConvertedMesh = artifacts.Any(a => a.EndsWith("_Mesh.asset", StringComparison.Ordinal));

        if (hasMeta && (hasTerrainPrefab || hasConvertedMesh))
        {
            entry.importStatus = "full";
        }
        else if (hasMeta || artifacts.Count > 0)
        {
            entry.importStatus = "partial";
        }
        else
        {
            entry.importStatus = "missing";
        }

        return entry;
    }

    public static List<string> FilterTiles(
        IEnumerable<string> source,
        string allowlistCsv,
        string denylistCsv)
    {
        HashSet<string> allow = ParseList(allowlistCsv);
        HashSet<string> deny = ParseList(denylistCsv);

        List<string> result = new List<string>();
        foreach (string id in source)
        {
            if (deny.Contains(id))
            {
                continue;
            }

            if (allow.Count > 0 && !allow.Contains(id))
            {
                continue;
            }

            result.Add(id);
        }

        return result;
    }

    static HashSet<string> ParseList(string csv)
    {
        HashSet<string> set = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(csv))
        {
            return set;
        }

        string[] parts = csv.Split(new[] { ',', ';', ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            set.Add(parts[i].Trim());
        }

        return set;
    }
}
#endif
