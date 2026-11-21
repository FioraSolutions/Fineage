using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Mobius map-region grid for client zone-name banners (<c>map-regions.json</c>, same contract as Fineage-Backend).
/// Cell index: <c>(x >> 15) + 20</c>, <c>(y >> 15) + 18</c>.
/// </summary>
public static class ClientMapRegionCatalog
{
    public const string RelativePath = "Data/Map/client-map-region-cells.json";

    static bool _loaded;
    static readonly Dictionary<(int X, int Y), string> TownByCell = new();
    static string _fallbackTown = "Talking Island Village";

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;
        _loaded = true;
        TownByCell.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            Debug.LogError("[ZoneTransition] missing catalog StreamingAssets/" + RelativePath);
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var root = JsonUtility.FromJson<RootDto>(json);
            if (!string.IsNullOrEmpty(root?.defaultTown))
                _fallbackTown = root.defaultTown;
            if (root?.cells == null)
                return;

            foreach (CellDto cell in root.cells)
            {
                if (cell == null || string.IsNullOrEmpty(cell.town))
                    continue;
                TownByCell[(cell.x, cell.y)] = cell.town;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[ZoneTransition] parse failed " + RelativePath + ": " + ex.Message);
        }
    }

    public static string ResolveTownName(float l2X, float l2Y)
    {
        EnsureLoaded();
        VectorUtils.GetSwMapTile(new Vector3(l2X, l2Y, 0f), out int cellX, out int cellY);
        if (TownByCell.TryGetValue((cellX, cellY), out string town) && !string.IsNullOrEmpty(town))
            return town;

        DetailedMapZoneRecord detailed = DetailedMapCatalog.FindByWorldPosition(l2X, l2Y);
        if (detailed != null && !string.IsNullOrEmpty(detailed.name))
            return detailed.name;

        Debug.LogError(
            "[ZoneTransition] no town label for L2 (" + l2X.ToString("F0") + "," + l2Y.ToString("F0") +
            ") cell (" + cellX + "," + cellY + ")");
        return _fallbackTown;
    }

    public static bool TryResolveCell(int cellX, int cellY, out string town)
    {
        EnsureLoaded();
        return TownByCell.TryGetValue((cellX, cellY), out town) && !string.IsNullOrEmpty(town);
    }

    [Serializable]
    sealed class RootDto
    {
        public string defaultTown;
        public CellDto[] cells;
    }

    [Serializable]
    sealed class CellDto
    {
        public int x;
        public int y;
        public string town;
    }
}