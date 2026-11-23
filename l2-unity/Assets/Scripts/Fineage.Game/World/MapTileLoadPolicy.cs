using System;

/// <summary>
/// Which map ids the world streamer must try to show.
/// A Build Settings scene is not a substitute for the Resources map prefab:
/// several town scenes are stubs, and several field tiles exist only as prefabs.
/// </summary>
public static class MapTileLoadPolicy
{
    public static bool IsMapTileName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length != 5 || name[2] != '_')
            return false;
        return char.IsDigit(name[0]) && char.IsDigit(name[1])
            && char.IsDigit(name[3]) && char.IsDigit(name[4]);
    }

    /// <summary>
    /// True when streaming should start a load. Map tile ids are always attempted;
    /// the loader logs a concrete error if neither a scene nor a Resources prefab exists.
    /// </summary>
    public static bool ShouldAttempt(string tile, bool sceneInBuild, bool manifestListed)
    {
        if (string.IsNullOrEmpty(tile))
            return false;
        if (sceneInBuild || manifestListed)
            return true;
        return IsMapTileName(tile);
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
