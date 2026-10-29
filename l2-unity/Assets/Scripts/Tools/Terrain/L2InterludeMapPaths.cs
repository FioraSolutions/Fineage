#if UNITY_EDITOR
using System.IO;
using UnityEngine;

public static class L2InterludeMapPaths
{
    public static string MapsAssetRoot => Path.Combine("Assets", "Resources", "Data", "Maps");

    public static string TileAssetFolder(string tileId) =>
        Path.Combine(MapsAssetRoot, tileId).Replace("\\", "/");

    public static string MetaFolder(string tileId) =>
        Path.Combine(TileAssetFolder(tileId), "Meta").Replace("\\", "/");

    public static string T3dAssetPath(string tileId) =>
        Path.Combine(MetaFolder(tileId), tileId + ".t3d").Replace("\\", "/");

    public static string T3dAbsolutePath(string tileId)
    {
        string assetPath = T3dAssetPath(tileId);
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
    }

    public static string BrushesJsonAssetPath(string tileId) =>
        Path.Combine(MetaFolder(tileId), "Brushes.json").Replace("\\", "/");

    public static string StaticMeshListAssetPath(string tileId)
    {
        string individuals = Path.Combine(MetaFolder(tileId), "StaticMeshActorIndividuals.txt");
        if (File.Exists(Path.GetFullPath(Path.Combine(Application.dataPath, "..", individuals))))
        {
            return individuals;
        }

        string typo = Path.Combine(MetaFolder(tileId), "StaticMeshActorInvidivuals.txt");
        if (File.Exists(Path.GetFullPath(Path.Combine(Application.dataPath, "..", typo))))
        {
            return typo;
        }

        return T3dAssetPath(tileId);
    }

    public static string ResolveTerrainPrefabAssetFolder(string tileId)
    {
        string unityTerrain = Path.Combine(TileAssetFolder(tileId), "UnityTerrain").Replace("\\", "/");
        string unityPrefab = Path.Combine(unityTerrain, tileId + ".prefab");
        if (File.Exists(Path.GetFullPath(Path.Combine(Application.dataPath, "..", unityPrefab))))
        {
            return unityTerrain;
        }

        return Path.Combine(TileAssetFolder(tileId), "TerrainData").Replace("\\", "/");
    }
}
#endif
