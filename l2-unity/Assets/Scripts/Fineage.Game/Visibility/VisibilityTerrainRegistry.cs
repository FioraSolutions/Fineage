/// <summary>
/// Terrain deco draw distance published by <c>VisibilityManager</c> (Fineage.Game consumers only).
/// </summary>
public static class VisibilityTerrainRegistry
{
    static float _terrainDecoDrawMeters;

    public static void PublishTerrainDecoDrawMeters(float meters) => _terrainDecoDrawMeters = meters;

    public static void Clear() => _terrainDecoDrawMeters = 0f;

  /// <summary>Zero when no manager has published yet (fall back to per-renderer clip).</summary>
    public static float TerrainDecoDrawMeters => _terrainDecoDrawMeters;
}