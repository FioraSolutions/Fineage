using System;
using UnityEngine;

/// <summary>
/// Draws a map's heightmap only when <see cref="L2ZoneRender.ActiveTerrainDraw"/>
/// says the camera zone, or a zone visible through a portal, lists that terrain.
/// Unknown leaves the mesh as placed. Colliders stay.
/// </summary>
public static class L2ZoneTerrainDraw
{
    public static string ObjectName(string catalogMap)
    {
        if (string.Equals(catalogMap, LobbyMapInterludeContract.ZoneCatalogMap, StringComparison.OrdinalIgnoreCase))
            return LobbyMapInterludeContract.AdditiveLobbySceneName;
        return catalogMap;
    }

    public static void Apply(string catalogMap)
    {
        if (string.IsNullOrEmpty(catalogMap))
            return;
        if (!string.Equals(catalogMap, L2ZoneRender.ActiveMap, StringComparison.OrdinalIgnoreCase))
            return;
        L2ZoneRenderTable.TerrainDraw draw = L2ZoneRender.ActiveTerrainDraw;
        if (draw == L2ZoneRenderTable.TerrainDraw.Unknown)
            return;
        SetDrawn(ObjectName(catalogMap), draw == L2ZoneRenderTable.TerrainDraw.Draw);
    }

    static void SetDrawn(string objectName, bool drawn)
    {
        Renderer[] renderers = UnityEngine.Object.FindObjectsByType<Renderer>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || !string.Equals(renderer.gameObject.name, objectName, StringComparison.Ordinal))
                continue;
            renderer.forceRenderingOff = !drawn;
            renderer.enabled = drawn;
        }

        Terrain[] terrains = UnityEngine.Object.FindObjectsByType<Terrain>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            if (terrain == null || !string.Equals(terrain.gameObject.name, objectName, StringComparison.Ordinal))
                continue;
            terrain.drawHeightmap = drawn;
            terrain.enabled = true;
        }
    }
}
