using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public static partial class EnchantGlow
{
    public const string DefaultLevels = @"# Interlude enchant glow by level. No glow below the first listed level for that kind.
# kind  level  r  g  b  intensity
# Official Interlude weapon steps: blue (+4-6), purple (+7-9), red (+10-12), gold (+13-16+).
# Armor uses the same hues at a lower intensity (Interlude has no per-armor Enchanted mesh).
weapon 4  0.35 0.55 1.00 0.45
weapon 5  0.40 0.60 1.00 0.55
weapon 6  0.48 0.65 1.00 0.65
weapon 7  0.70 0.35 1.00 0.75
weapon 8  0.76 0.28 1.00 0.82
weapon 9  0.82 0.22 1.00 0.88
weapon 10 1.00 0.25 0.22 0.92
weapon 11 1.00 0.18 0.16 0.96
weapon 12 1.00 0.14 0.10 1.00
weapon 13 1.00 0.55 0.12 1.05
weapon 14 1.00 0.70 0.18 1.10
weapon 15 1.00 0.88 0.28 1.20
weapon 16 1.00 0.95 0.55 1.25
armor 4  0.35 0.55 1.00 0.22
armor 5  0.40 0.60 1.00 0.26
armor 6  0.48 0.65 1.00 0.30
armor 7  0.70 0.35 1.00 0.34
armor 8  0.76 0.28 1.00 0.38
armor 9  0.82 0.22 1.00 0.42
armor 10 1.00 0.25 0.22 0.46
armor 11 1.00 0.18 0.16 0.50
armor 12 1.00 0.14 0.10 0.54
armor 13 1.00 0.55 0.12 0.58
armor 14 1.00 0.70 0.18 0.62
armor 15 1.00 0.88 0.28 0.70
armor 16 1.00 0.95 0.55 0.75
";

    static Dictionary<string, List<EnchantGlowStyle>> _tables;

    public static void Reset()
    {
        _tables = null;
    }

    public static void LoadText(string text)
    {
        _tables = ParseLevels(text);
    }

    public static bool TryLoadFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;
        LoadText(File.ReadAllText(path));
        return true;
    }

    public static bool TryStyle(string kind, int level, out EnchantGlowStyle style)
    {
        style = default;
        level = Cap(level);
        if (level <= 0)
            return false;

        EnsureLoaded();
        string key = NormalizeKind(kind);
        if (!_tables.TryGetValue(key, out List<EnchantGlowStyle> rows) || rows == null || rows.Count == 0)
            return false;
        if (level < rows[0].Level)
            return false;

        EnchantGlowStyle chosen = rows[0];
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Level > level)
                break;
            chosen = rows[i];
        }
        style = chosen;
        return true;
    }

    public static Dictionary<string, List<EnchantGlowStyle>> ParseLevels(string text)
    {
        var tables = new Dictionary<string, List<EnchantGlowStyle>>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(text))
            return tables;

        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0 || line[0] == '#')
                continue;
            string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 6)
                continue;
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int level))
                continue;
            if (!TryFloat(parts[2], out float r) || !TryFloat(parts[3], out float g) ||
                !TryFloat(parts[4], out float b) || !TryFloat(parts[5], out float intensity))
                continue;

            string key = NormalizeKind(parts[0]);
            if (!tables.TryGetValue(key, out List<EnchantGlowStyle> rows))
            {
                rows = new List<EnchantGlowStyle>();
                tables[key] = rows;
            }
            rows.Add(new EnchantGlowStyle(Cap(level), r, g, b, intensity));
        }

        foreach (List<EnchantGlowStyle> rows in tables.Values)
            rows.Sort((a, b) => a.Level.CompareTo(b.Level));
        return tables;
    }

    static void EnsureLoaded()
    {
        if (_tables == null)
            _tables = ParseLevels(DefaultLevels);
    }

    static string NormalizeKind(string kind)
    {
        if (string.IsNullOrEmpty(kind))
            return KindWeapon;
        if (kind.Equals("armor", StringComparison.OrdinalIgnoreCase) ||
            kind.Equals("shield", StringComparison.OrdinalIgnoreCase) ||
            kind.Equals("jewel", StringComparison.OrdinalIgnoreCase))
            return KindArmor;
        return KindWeapon;
    }

    static bool TryFloat(string text, out float value)
    {
        return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
