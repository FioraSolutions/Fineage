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
    static string _zoneActor;
    static L2ZoneRenderTable.TerrainDraw _terrainDraw;
    static bool _hasFog;
    static L2ZoneRenderTable.DistanceFog _fog;
    static bool _hasSky;
    static float _panU;
    static float _panV;
    static float _sunScale = 1f;
    static float _moonScale = 1f;
    static readonly HashSet<string> _logged = new HashSet<string>(StringComparer.Ordinal);

    public static string ActiveMap => _active;

    public static string ActiveZone => _zoneActor;

    /// <summary>
    /// Draw, hide, or leave the active map's terrain. Unknown until the camera zone is set.
    /// </summary>
    public static L2ZoneRenderTable.TerrainDraw ActiveTerrainDraw => _terrainDraw;

    public static void SetActiveMap(string map)
    {
        SetActiveMap(map, null);
    }

    /// <param name="zoneActor">
    /// BSP zone for the active camera when the map has more than one terrain
    /// ZoneInfo. Null keeps the single-zone catalog rule.
    /// </param>
    public static void SetActiveMap(string map, string zoneActor)
    {
        if (string.Equals(_active, map, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(_zoneActor, zoneActor ?? "", StringComparison.Ordinal))
            return;
        _active = map;
        _zoneActor = zoneActor ?? "";
        _terrainDraw = L2ZoneRenderTable.TerrainDraw.Unknown;
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
        _hasFog = string.IsNullOrEmpty(zoneActor)
            ? _table.TryDistanceFog(map, out _fog, errors)
            : _table.TryNamedDistanceFog(map, zoneActor, out _fog, errors);
        if (!string.IsNullOrEmpty(zoneActor))
            _terrainDraw = _table.TerrainDrawFor(map, zoneActor, PortalZones(map, zoneActor), errors);
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

    /// <summary>
    /// Camera clear when a zone DistanceFogColor is active. Gaps that miss
    /// geometry show this, not the prefab light blue.
    /// </summary>
    public static bool TryFogClearColor(out Color color)
    {
        color = default;
        if (!TryActiveFog(out L2ZoneRenderTable.DistanceFog fog) || !fog.HasColor)
            return false;
        color = new Color(fog.R / 255f, fog.G / 255f, fog.B / 255f, 1f);
        return true;
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

    /// <summary>
    /// Zone named by every row of camera-zones/&lt;map&gt;.tsv. Disagreeing rows
    /// are an error and return null — the caller does not pick one.
    /// </summary>
    public static string AgreedCameraZone(string map)
    {
        if (string.IsNullOrEmpty(map))
            return null;
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta/World/camera-zones", map + ".tsv");
        if (!File.Exists(path))
            return null;
        string[] lines = File.ReadAllText(path).Split('\n');
        string agreed = null;
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0)
                continue;
            string[] cells = line.Split('\t');
            if (cells.Length < 5 || string.IsNullOrEmpty(cells[4]))
            {
                LogOnce("[ZoneFog] map " + map + " camera-zones row " + (i + 1) + " has no zone");
                return null;
            }
            if (agreed == null)
                agreed = cells[4];
            else if (!string.Equals(agreed, cells[4], StringComparison.Ordinal))
            {
                LogOnce("[ZoneFog] map " + map + " camera zones disagree " + agreed + " vs " + cells[4]);
                return null;
            }
        }
        return agreed;
    }

    /// <summary>
    /// Zones the camera zone can see through a portal, from
    /// Data/Meta/World/zone-portals/&lt;map&gt;.tsv (`from`, `to`). The property dump
    /// has no BSP portal graph, so a missing file means the camera zone only.
    /// </summary>
    static List<string> PortalZones(string map, string cameraZone)
    {
        var seen = new List<string>();
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta/World/zone-portals", map + ".tsv");
        if (!File.Exists(path))
            return seen;
        string[] lines = File.ReadAllText(path).Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0)
                continue;
            string[] cells = line.Split('\t');
            if (cells.Length < 2 || string.IsNullOrEmpty(cells[0]) || string.IsNullOrEmpty(cells[1]))
            {
                LogOnce("[ZoneTerrain] map " + map + " zone-portals row " + (i + 1) + " needs from and to");
                continue;
            }
            if (string.Equals(cells[0], cameraZone, StringComparison.Ordinal))
                seen.Add(cells[1]);
        }
        return seen;
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
