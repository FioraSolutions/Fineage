using System;

/// <summary>
/// A map tile was requested and its catalog entry has no content.
/// The message names the tile id, the Resources path that should hold the prefab,
/// and the catalog that was consulted.
/// </summary>
public sealed class MapTileMissingException : Exception
{
    public string TileId { get; }
    public string ExpectedPath { get; }
    public string Catalog { get; }

    public MapTileMissingException(string tileId, string expectedPath, string catalog)
        : base(
            "Map tile '" + (tileId ?? "")
            + "' is missing. Expected Resources prefab '"
            + (expectedPath ?? "")
            + "'. Catalog: " + (catalog ?? "") + ".")
    {
        TileId = tileId ?? "";
        ExpectedPath = expectedPath ?? "";
        Catalog = catalog ?? "";
    }
}
