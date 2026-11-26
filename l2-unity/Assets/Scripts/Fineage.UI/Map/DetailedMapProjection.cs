using UnityEngine;

/// <summary>
/// Interlude NCMinimapWnd::WorldToMap projection for world and detailed (town/dungeon) maps.
/// Detailed maps use scale 0.05 and subtract origin; world map uses MapPanner world bounds.
/// </summary>
public static class DetailedMapProjection
{
    public const float DefaultDetailedScale = 0.05f;
    public const int HotspotButtonSize = 26;

    public static float WorldMapL2MinX => InterludeWorldMapBounds.L2MinX;
    public static float WorldMapL2MaxX => InterludeWorldMapBounds.L2MaxX;
    public static float WorldMapL2MinY => InterludeWorldMapBounds.L2MinY;
    public static float WorldMapL2MaxY => InterludeWorldMapBounds.L2MaxY;
    public static float WorldMapPixelWidth => InterludeWorldMapBounds.PixelWidth;
    public static float WorldMapPixelHeight => InterludeWorldMapBounds.PixelHeight;

    public static bool ContainsWorldPosition(DetailedMapZoneRecord zone, float l2X, float l2Y)
    {
        if (zone == null)
            return false;

        return l2X >= zone.WorldMinL2X && l2X <= zone.WorldMaxL2X
            && l2Y >= zone.WorldMinL2Y && l2Y <= zone.WorldMaxL2Y;
    }

    public static Vector2 ProjectWorldMapPixel(float l2X, float l2Y)
    {
        float nx = Mathf.InverseLerp(WorldMapL2MinX, WorldMapL2MaxX, l2X);
        float ny = Mathf.InverseLerp(WorldMapL2MinY, WorldMapL2MaxY, l2Y);
        return new Vector2(nx * WorldMapPixelWidth, ny * WorldMapPixelHeight);
    }

    public static Vector2 ProjectDetailedMapPixel(DetailedMapZoneRecord zone, float l2X, float l2Y)
    {
        float scale = zone.scale > 0f ? zone.scale : DefaultDetailedScale;
        float px = (l2X - zone.originL2X) * scale;
        float py = (l2Y - zone.originL2Y) * scale;
        return new Vector2(px, py);
    }

    public static Vector2 ProjectDetailedToUi(
        DetailedMapZoneRecord zone,
        float l2X,
        float l2Y,
        float uiWidth,
        float uiHeight)
    {
        Vector2 mapPx = ProjectDetailedMapPixel(zone, l2X, l2Y);
        float nx = zone.mapPixelWidth > 0 ? mapPx.x / zone.mapPixelWidth : 0f;
        float ny = zone.mapPixelHeight > 0 ? mapPx.y / zone.mapPixelHeight : 0f;
        return new Vector2(
            Mathf.Clamp01(nx) * uiWidth,
            Mathf.Clamp01(ny) * uiHeight);
    }
}