using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    public static void LoadInt(string text)
    {
        TerrainAmbient.Clear();
        StaticMeshAmbient.Clear();
        BspAmbient.Clear();
        HsvTerrainLight.Clear();
        if (string.IsNullOrEmpty(text))
            return;

        string section = "";
        var lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = StripComment(lines[i]).Trim();
            if (line.Length == 0)
                continue;

            if (line[0] == '[' && line[line.Length - 1] == ']')
            {
                section = line.Substring(1, line.Length - 2);
                continue;
            }

            if (section == "TerrainAmbient")
                TryAddColor(TerrainAmbient, line);
            else if (section == "StaticMeshAmbient")
                TryAddColor(StaticMeshAmbient, line);
            else if (section == "BSPAmbient")
                TryAddColor(BspAmbient, line);
            else if (section == "HSVTerrainLight")
                TryAddHsv(HsvTerrainLight, line);
        }

        SortByTime(TerrainAmbient);
        SortByTime(StaticMeshAmbient);
        SortByTime(BspAmbient);
        HsvTerrainLight.Sort((a, b) => a.tHours.CompareTo(b.tHours));
    }

    static string StripComment(string line)
    {
        int cut = line.IndexOf(';');
        return cut >= 0 ? line.Substring(0, cut) : line;
    }

    static void TryAddColor(List<KeyColor> list, string line)
    {
        Match m = ColorRe.Match(line);
        if (!m.Success)
            return;
        list.Add(new KeyColor
        {
            tHours = Parse(m.Groups[1].Value),
            color = ByteRgb(m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value)
        });
    }

    static void TryAddHsv(List<KeyHsv> list, string line)
    {
        Match m = HsvRe.Match(line);
        if (!m.Success)
            return;
        list.Add(new KeyHsv
        {
            tHours = Parse(m.Groups[1].Value),
            hue = Parse(m.Groups[2].Value),
            sat = Parse(m.Groups[3].Value),
            bri = Parse(m.Groups[4].Value)
        });
    }
}
