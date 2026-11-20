using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DetailedMapCatalog
{
    const string RelativePath = "Data/Map/detailed_maps_interlude.json";

    static DetailedMapZoneRecord[] _zones = Array.Empty<DetailedMapZoneRecord>();
    static Dictionary<int, DetailedMapZoneRecord> _byZoneId = new();
    static bool _loaded;

    public static IReadOnlyList<DetailedMapZoneRecord> Zones
    {
        get
        {
            EnsureLoaded();
            return _zones;
        }
    }

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;

        _loaded = true;
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            Debug.LogError($"[DetailedMap] Missing catalog JSON at StreamingAssets/{RelativePath}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var root = JsonUtility.FromJson<DetailedMapCatalogJson>(json);
            _zones = root?.zones ?? Array.Empty<DetailedMapZoneRecord>();
            _byZoneId = new Dictionary<int, DetailedMapZoneRecord>(_zones.Length);
            foreach (DetailedMapZoneRecord zone in _zones)
            {
                if (zone?.zoneIds == null)
                    continue;
                foreach (int id in zone.zoneIds)
                {
                    if (id <= 0)
                        continue;
                    _byZoneId[id] = zone;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DetailedMap] Failed to parse {RelativePath}: {ex.Message}");
            _zones = Array.Empty<DetailedMapZoneRecord>();
        }
    }

    public static DetailedMapZoneRecord FindByZoneId(int zoneId)
    {
        EnsureLoaded();
        return _byZoneId.TryGetValue(zoneId, out DetailedMapZoneRecord zone) ? zone : null;
    }

    public static DetailedMapZoneRecord FindByWorldPosition(float l2X, float l2Y)
    {
        EnsureLoaded();
        DetailedMapZoneRecord best = null;
        float bestArea = float.MaxValue;
        foreach (DetailedMapZoneRecord zone in _zones)
        {
            if (zone == null || zone.needsCoordsFromZonenameDat)
                continue;
            if (!DetailedMapProjection.ContainsWorldPosition(zone, l2X, l2Y))
                continue;

            float area = (zone.WorldMaxL2X - zone.WorldMinL2X) * (zone.WorldMaxL2Y - zone.WorldMinL2Y);
            if (area < bestArea)
            {
                bestArea = area;
                best = zone;
            }
        }

        return best;
    }

    public static DetailedMapZoneRecord FindById(string id)
    {
        EnsureLoaded();
        if (string.IsNullOrEmpty(id))
            return null;
        foreach (DetailedMapZoneRecord zone in _zones)
        {
            if (zone != null && zone.id == id)
                return zone;
        }

        return null;
    }
}