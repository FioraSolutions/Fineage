using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Active-tile ZoneInfo fog, SkyZoneInfo pan, and NSun/NMoon radius ratio.
/// The catalog is the derived zone-render table. A missing map, actor, or property is logged.
/// </summary>
public static class L2ZoneRender
{
    public const string RelativePath = "Data/Meta/World/zone-render.tsv";

    static L2ZoneRenderTable _table;
    static bool _loadAttempted;
    static string _active;
    static bool _hasFog;
    static L2ZoneRenderTable.DistanceFog _fog;
    static bool _hasSky;
    static float _panU;
    static float _panV;
    static float _sunScale = 1f;
    static float _moonScale = 1f;
    static readonly HashSet<string> _logged = new HashSet<string>(StringComparer.Ordinal);

    public static string ActiveMap => _active;

    public static void SetActiveMap(string map)
    {
        if (string.Equals(_active, map, StringComparison.OrdinalIgnoreCase))
            return;
        _active = map;
        if (string.IsNullOrEmpty(map))
            return;
        EnsureTable();
        var errors = new List<string>();
        if (_table == null)
        {
            _hasFog = false;
            _hasSky = false;
            _sunScale = 1f;
            _moonScale = 1f;
            return;
        }
        _hasFog = _table.TryDistanceFog(map, out _fog, errors);
        _hasSky = _table.TrySkyPan(map, out _panU, out _panV, errors);
        _sunScale = _table.BodyRadiusScale(map, "NSun", errors);
        _moonScale = _table.BodyRadiusScale(map, "NMoon", errors);
        for (int i = 0; i < errors.Count; i++)
            LogOnce(errors[i]);
        ReportFlares(map);
    }

    public static bool TryActiveFog(out L2ZoneRenderTable.DistanceFog fog)
    {
        fog = _fog;
        return !string.IsNullOrEmpty(_active) && _hasFog;
    }

    public static bool TryActiveSkyPan(out float u, out float v)
    {
        u = _panU;
        v = _panV;
        return !string.IsNullOrEmpty(_active) && _hasSky;
    }

    public static float ActiveBodyRadiusScale(string actorClass)
    {
        if (string.IsNullOrEmpty(_active))
            return 1f;
        if (actorClass == "NMoon")
            return _moonScale;
        if (actorClass == "NSun")
            return _sunScale;
        return 1f;
    }

    static void EnsureTable()
    {
        if (_loadAttempted)
            return;
        _loadAttempted = true;
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            LogOnce("[ZoneFog] zone catalog missing " + path);
            return;
        }
        var errors = new List<string>();
        _table = L2ZoneRenderTable.Parse(File.ReadAllText(path), errors);
        for (int i = 0; i < errors.Count; i++)
            LogOnce(errors[i]);
    }

    static void ReportFlares(string map)
    {
        string[] flares = _table.LensFlares(map);
        for (int i = 0; i < flares.Length; i++)
        {
            string name = flares[i];
            if (string.IsNullOrEmpty(name) || !_logged.Add("flare:" + name))
                continue;
            string resource = "Data/Textures/" + name.Replace('.', '/');
            if (Resources.Load<Texture>(resource) != null)
                continue;
            Debug.LogError("[ZoneFog] SkyZoneInfo " + map + " LensFlare texture " + name + " missing");
        }
    }

    static void LogOnce(string message)
    {
        if (string.IsNullOrEmpty(message) || !_logged.Add(message))
            return;
        Debug.LogError(message);
    }
}
