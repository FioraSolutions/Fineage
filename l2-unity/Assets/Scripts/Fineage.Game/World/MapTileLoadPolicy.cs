using System;

/// <summary>
/// Map tiles load from Resources/Data/Maps/{id}. A Build Settings scene is not a tile.
/// The catalog is <see cref="Catalog"/>.
/// </summary>
public static class MapTileLoadPolicy
{
    public const string Catalog = "Resources/Settings/TileStreamingManifest";
    public const string MapRoot = "Resources/Data/Maps";

    public static bool IsMapTileName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length != 5 || name[2] != '_')
            return false;
        return char.IsDigit(name[0]) && char.IsDigit(name[1])
            && char.IsDigit(name[3]) && char.IsDigit(name[4]);
    }

    public static string ExpectedPrefabResourcePath(string tileId)
    {
        return "Data/Maps/" + tileId + "/" + tileId;
    }

    /// <summary>
    /// A map tile loads only when the catalog lists it. Other scenes load from the build.
    /// </summary>
    public static bool ShouldAttempt(string tile, bool sceneInBuild, bool manifestListed)
    {
        if (string.IsNullOrEmpty(tile))
            return false;
        if (IsMapTileName(tile))
            return manifestListed;
        return sceneInBuild || manifestListed;
    }

    public static MapTileMissingException Missing(string tileId)
    {
        return new MapTileMissingException(tileId, ExpectedPrefabResourcePath(tileId), Catalog);
    }

    /// <summary>
    /// Day/night lighting globals stay current only while the clock hour and the
    /// active map are unchanged. A teleport that keeps the same hour still needs a push.
    /// </summary>
    public static bool LightingIsCurrent(
        bool applied,
        float hours,
        float appliedHours,
        string map,
        string appliedMap)
    {
        if (!applied)
            return false;
        if (Math.Abs(hours - appliedHours) >= 0.0001f)
            return false;
        return string.Equals(map ?? string.Empty, appliedMap ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
