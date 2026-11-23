using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Per-tile ZoneInfo / SkyZoneInfo / NSun rows from the derived zone-render catalog.
/// Empty cells are unauthored. This type does not log; the caller logs the error list.
/// </summary>
public sealed class L2ZoneRenderTable
{
    /// <summary>
    /// Engine.ZoneInfo defaultproperties DistanceFogStart. The map dump omits a property
    /// that still has the class default, so the catalog cell stays empty. The runtime
    /// applies this value; it is not a per-tile guess.
    /// </summary>
    public const float EngineZoneInfoDistanceFogStartUu = 3000f;

    public struct Actor
    {
        public string Map;
        public string ClassName;
        public string Name;
        public string ZoneTag;
        public string DistanceFog;
        public string TerrainZone;
        public string FogStart;
        public string FogEnd;
        public string FogR;
        public string FogG;
        public string FogB;
        public string FogA;
        public string PanU;
        public string PanV;
        public string LensFlares;
        public string Radius;
        /// <summary>TerrainInfo this zone lists in Terrains[]. Empty means it lists none.</summary>
        public string Terrains;
    }

    public enum TerrainDraw
    {
        Unknown = 0,
        Draw = 1,
        Hide = 2,
    }

    public struct DistanceFog
    {
        public string Actor;
        public bool HasStart;
        public float StartUu;
        public bool HasEnd;
        public float EndUu;
        public bool HasColor;
        public int R;
        public int G;
        public int B;
        public int A;
    }

    const int Columns = 26;

    readonly Dictionary<string, List<Actor>> _byMap = new Dictionary<string, List<Actor>>(StringComparer.OrdinalIgnoreCase);
    readonly List<Actor> _all = new List<Actor>();

    public IReadOnlyList<Actor> Actors => _all;

    public static L2ZoneRenderTable Parse(string text, List<string> errors)
    {
        var table = new L2ZoneRenderTable();
        if (string.IsNullOrEmpty(text))
        {
            errors?.Add("[ZoneFog] zone catalog missing");
            return table;
        }
        string[] lines = text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0)
                continue;
            string[] cells = line.Split('\t');
            if (cells.Length != Columns)
            {
                errors?.Add("[ZoneFog] zone catalog row " + (i + 1) + " has " + cells.Length + " columns");
                continue;
            }
            var actor = new Actor
            {
                Map = cells[0],
                ClassName = cells[1],
                Name = cells[2],
                ZoneTag = cells[3],
                DistanceFog = cells[4],
                TerrainZone = cells[5],
                FogStart = cells[6],
                FogEnd = cells[7],
                FogR = cells[8],
                FogG = cells[9],
                FogB = cells[10],
                FogA = cells[11],
                PanU = cells[12],
                PanV = cells[13],
                LensFlares = cells[14],
                Radius = cells[17],
                Terrains = cells[25],
            };
            table._all.Add(actor);
            if (!table._byMap.TryGetValue(actor.Map, out List<Actor> list))
            {
                list = new List<Actor>();
                table._byMap.Add(actor.Map, list);
            }
            list.Add(actor);
        }
        return table;
    }

    public bool TryDistanceFog(string map, out DistanceFog fog, List<string> errors)
    {
        fog = default;
        if (string.IsNullOrEmpty(map) || !_byMap.TryGetValue(map, out List<Actor> rows))
        {
            errors?.Add("[ZoneFog] map " + map + " missing");
            return false;
        }
        if (!TrySelect(rows, out Actor selected, out string ambiguous))
        {
            if (ambiguous != null)
                errors?.Add("[ZoneFog] map " + map + " distance fog ambiguous " + ambiguous);
            else
                errors?.Add("[ZoneFog] map " + map + " distance fog missing");
            return false;
        }
        fog.Actor = selected.Name;
        if (!TryFloat(selected.FogStart, out fog.StartUu))
            fog.StartUu = EngineZoneInfoDistanceFogStartUu;
        fog.HasStart = true;
        if (!TryFloat(selected.FogEnd, out fog.EndUu))
            errors?.Add("[ZoneFog] map " + map + " actor " + selected.Name + " DistanceFogEnd missing");
        else
            fog.HasEnd = true;
        if (fog.HasStart && fog.HasEnd && fog.StartUu >= fog.EndUu)
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + selected.Name + " DistanceFogStart past DistanceFogEnd");
            fog.HasEnd = false;
        }
        if (TryByte(selected.FogR, out fog.R) && TryByte(selected.FogG, out fog.G) &&
            TryByte(selected.FogB, out fog.B) && TryByte(selected.FogA, out fog.A))
            fog.HasColor = true;
        return true;
    }

    /// <summary>
    /// Draw the map terrain when the camera zone, or a zone visible through a
    /// portal, lists a TerrainInfo. An empty camera zone is Unknown: the caller
    /// leaves the mesh as placed. bTerrainZone without a Terrains entry does not own one.
    /// </summary>
    public TerrainDraw TerrainDrawFor(string map, string cameraZone, IList<string> portalZones, List<string> errors)
    {
        if (string.IsNullOrEmpty(cameraZone))
            return TerrainDraw.Unknown;
        if (string.IsNullOrEmpty(map) || !_byMap.TryGetValue(map, out List<Actor> rows))
        {
            errors?.Add("[ZoneTerrain] map " + map + " missing");
            return TerrainDraw.Unknown;
        }
        if (!ZoneListed(rows, cameraZone))
        {
            errors?.Add("[ZoneTerrain] map " + map + " camera zone " + cameraZone + " missing");
            return TerrainDraw.Unknown;
        }
        if (ZoneListsTerrain(rows, cameraZone))
            return TerrainDraw.Draw;
        if (portalZones != null)
        {
            for (int i = 0; i < portalZones.Count; i++)
            {
                string seen = portalZones[i];
                if (string.IsNullOrEmpty(seen))
                    continue;
                if (!ZoneListed(rows, seen))
                {
                    errors?.Add("[ZoneTerrain] map " + map + " portal zone " + seen + " missing");
                    continue;
                }
                if (ZoneListsTerrain(rows, seen))
                    return TerrainDraw.Draw;
            }
        }
        return TerrainDraw.Hide;
    }

    static bool ZoneListed(List<Actor> rows, string actorName)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (!string.Equals(rows[i].Name, actorName, StringComparison.Ordinal))
                continue;
            if (rows[i].ClassName == "ZoneInfo" || rows[i].ClassName == "SkyZoneInfo")
                return true;
        }
        return false;
    }

    static bool ZoneListsTerrain(List<Actor> rows, string actorName)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (!string.Equals(rows[i].Name, actorName, StringComparison.Ordinal))
                continue;
            if (rows[i].ClassName != "ZoneInfo" && rows[i].ClassName != "SkyZoneInfo")
                continue;
            return !string.IsNullOrEmpty(rows[i].Terrains);
        }
        return false;
    }

    /// <summary>
    /// The zone the BSP query named for this map's cameras. Used when several
    /// terrain ZoneInfo rows disagree, so the catalog does not drop fog.
    /// </summary>
    public bool TryNamedDistanceFog(string map, string actorName, out DistanceFog fog, List<string> errors)
    {
        fog = default;
        if (string.IsNullOrEmpty(actorName))
        {
            errors?.Add("[ZoneFog] map " + map + " camera zone missing");
            return false;
        }
        if (string.IsNullOrEmpty(map) || !_byMap.TryGetValue(map, out List<Actor> rows))
        {
            errors?.Add("[ZoneFog] map " + map + " missing");
            return false;
        }
        Actor selected = default;
        bool found = false;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!string.Equals(rows[i].Name, actorName, StringComparison.Ordinal))
                continue;
            if (!string.Equals(rows[i].ClassName, "ZoneInfo", StringComparison.Ordinal))
                continue;
            selected = rows[i];
            found = true;
            break;
        }
        if (!found)
        {
            errors?.Add("[ZoneFog] map " + map + " camera zone " + actorName + " missing");
            return false;
        }
        if (selected.DistanceFog != "1")
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + actorName + " DistanceFog off");
            return false;
        }
        fog.Actor = selected.Name;
        if (!TryFloat(selected.FogStart, out fog.StartUu))
            fog.StartUu = EngineZoneInfoDistanceFogStartUu;
        fog.HasStart = true;
        if (!TryFloat(selected.FogEnd, out fog.EndUu))
            errors?.Add("[ZoneFog] map " + map + " actor " + selected.Name + " DistanceFogEnd missing");
        else
            fog.HasEnd = true;
        if (fog.HasStart && fog.HasEnd && fog.StartUu >= fog.EndUu)
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + selected.Name + " DistanceFogStart past DistanceFogEnd");
            fog.HasEnd = false;
        }
        if (TryByte(selected.FogR, out fog.R) && TryByte(selected.FogG, out fog.G) &&
            TryByte(selected.FogB, out fog.B) && TryByte(selected.FogA, out fog.A))
            fog.HasColor = true;
        return fog.HasEnd;
    }

    public bool TrySkyPan(string map, out float u, out float v, List<string> errors)
    {
        u = 0f;
        v = 0f;
        if (!TryOne(map, "SkyZoneInfo", out Actor sky, errors))
            return false;
        bool ok = true;
        if (!TryFloat(sky.PanU, out u))
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + sky.Name + " TexUPanSpeed missing");
            ok = false;
        }
        if (!TryFloat(sky.PanV, out v))
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + sky.Name + " TexVPanSpeed missing");
            ok = false;
        }
        return ok;
    }

    public string[] LensFlares(string map)
    {
        if (!TryOne(map, "SkyZoneInfo", out Actor sky, null) || string.IsNullOrEmpty(sky.LensFlares))
            return Array.Empty<string>();
        return sky.LensFlares.Split('|');
    }

    public float BodyRadiusScale(string map, string actorClass, List<string> errors)
    {
        if (!TryModeRadius(actorClass, out float mode, errors))
            return 1f;
        if (!_byMap.TryGetValue(map ?? "", out List<Actor> rows))
        {
            errors?.Add("[ZoneFog] map " + map + " " + actorClass + " missing");
            return 1f;
        }
        Actor found = default;
        int count = 0;
        bool disagree = false;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!string.Equals(rows[i].ClassName, actorClass, StringComparison.Ordinal))
                continue;
            if (count > 0 && !string.Equals(rows[i].Radius, found.Radius, StringComparison.Ordinal))
                disagree = true;
            found = rows[i];
            count++;
        }
        if (count == 0 || disagree)
        {
            errors?.Add("[ZoneFog] map " + map + " " + actorClass + (count == 0 ? " missing" : " ambiguous"));
            return 1f;
        }
        if (!TryFloat(found.Radius, out float radius))
        {
            errors?.Add("[ZoneFog] map " + map + " actor " + found.Name + " Radius missing");
            return 1f;
        }
        return radius / mode;
    }

    public static void DistanceFogGlobals(DistanceFog fog, float uuToUnity, out float endMeters, out float scale, out bool enable)
    {
        endMeters = 0f;
        scale = 0f;
        float rangeUu = fog.HasStart ? fog.EndUu - fog.StartUu : fog.EndUu;
        enable = fog.HasEnd && uuToUnity > 1e-8f && rangeUu > 1e-3f;
        if (!enable)
            return;
        endMeters = fog.EndUu * uuToUnity;
        scale = 1f / (rangeUu * uuToUnity);
    }

    bool TryModeRadius(string actorClass, out float mode, List<string> errors)
    {
        mode = 0f;
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        string bestKey = null;
        int best = 0;
        bool tie = false;
        for (int i = 0; i < _all.Count; i++)
        {
            Actor actor = _all[i];
            if (!string.Equals(actor.ClassName, actorClass, StringComparison.Ordinal) || actor.Radius.Length == 0)
                continue;
            counts.TryGetValue(actor.Radius, out int n);
            n++;
            counts[actor.Radius] = n;
            if (n > best)
            {
                best = n;
                bestKey = actor.Radius;
                tie = false;
            }
            else if (n == best)
                tie = true;
        }
        if (bestKey == null)
        {
            errors?.Add("[ZoneFog] " + actorClass + " radius mode missing");
            return false;
        }
        if (tie)
        {
            errors?.Add("[ZoneFog] " + actorClass + " radius mode ambiguous");
            return false;
        }
        return TryFloat(bestKey, out mode);
    }

    bool TryOne(string map, string className, out Actor actor, List<string> errors)
    {
        actor = default;
        if (string.IsNullOrEmpty(map) || !_byMap.TryGetValue(map, out List<Actor> rows))
        {
            errors?.Add("[ZoneFog] map " + map + " missing");
            return false;
        }
        int count = 0;
        bool disagree = false;
        for (int i = 0; i < rows.Count; i++)
        {
            if (!string.Equals(rows[i].ClassName, className, StringComparison.Ordinal))
                continue;
            if (count > 0 && !SameSky(actor, rows[i]))
                disagree = true;
            actor = rows[i];
            count++;
        }
        if (count == 1 || (count > 1 && !disagree))
            return true;
        errors?.Add("[ZoneFog] map " + map + " " + className + (count == 0 ? " missing" : " ambiguous"));
        return false;
    }

    static bool TrySelect(List<Actor> rows, out Actor selected, out string ambiguous)
    {
        if (TryTier(rows, "ZoneInfo", true, out selected, out ambiguous))
            return ambiguous == null;
        if (ambiguous != null)
            return false;
        if (TryTier(rows, "LevelInfo", false, out selected, out ambiguous))
            return ambiguous == null;
        if (ambiguous != null)
            return false;
        if (TryTier(rows, "ZoneInfo", false, out selected, out ambiguous))
            return ambiguous == null;
        return false;
    }

    static bool TryTier(List<Actor> rows, string className, bool terrain, out Actor selected, out string ambiguous)
    {
        selected = default;
        ambiguous = null;
        int count = 0;
        var names = new List<string>();
        for (int i = 0; i < rows.Count; i++)
        {
            Actor actor = rows[i];
            if (!string.Equals(actor.ClassName, className, StringComparison.Ordinal))
                continue;
            if (actor.DistanceFog != "1")
                continue;
            if (terrain && actor.TerrainZone != "1")
                continue;
            selected = actor;
            names.Add(actor.Name);
            count++;
        }
        if (count == 0)
            return false;
        if (count > 1 && !SameFog(rows, names))
            ambiguous = string.Join(", ", names);
        else
            selected = FirstNamed(rows, names[0]);
        return true;
    }

    static bool SameSky(Actor a, Actor b)
    {
        return string.Equals(a.PanU, b.PanU, StringComparison.Ordinal) &&
               string.Equals(a.PanV, b.PanV, StringComparison.Ordinal) &&
               string.Equals(a.LensFlares, b.LensFlares, StringComparison.Ordinal);
    }

    static bool SameFog(List<Actor> rows, List<string> names)
    {
        Actor first = FirstNamed(rows, names[0]);
        for (int i = 1; i < names.Count; i++)
        {
            Actor other = FirstNamed(rows, names[i]);
            if (!string.Equals(first.FogStart, other.FogStart, StringComparison.Ordinal) ||
                !string.Equals(first.FogEnd, other.FogEnd, StringComparison.Ordinal) ||
                !string.Equals(first.FogR, other.FogR, StringComparison.Ordinal) ||
                !string.Equals(first.FogG, other.FogG, StringComparison.Ordinal) ||
                !string.Equals(first.FogB, other.FogB, StringComparison.Ordinal) ||
                !string.Equals(first.FogA, other.FogA, StringComparison.Ordinal))
                return false;
        }
        return true;
    }

    static Actor FirstNamed(List<Actor> rows, string name)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Name == name)
                return rows[i];
        }
        return default;
    }

    static bool TryFloat(string text, out float value)
    {
        return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    static bool TryByte(string text, out int value)
    {
        return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
