using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string sweepPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", SweepLutFileName);
        if (File.Exists(sweepPath))
        {
            LoadSweepCsv(File.ReadAllText(sweepPath));
            if (SweepKeys.Count >= 2)
            {
                // Debug.Log("[L2EnvWorldLut] Loaded " + SweepKeys.Count + " GetTerrainAmbient keys from " + SweepLutFileName);
                LoadIntensityLut();
                return;
            }
        }

        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2EnvWorldLut] File not found: " + dataPath);
            LoadIntensityLut();
            return;
        }

        LoadInt(File.ReadAllText(dataPath));
        LoadIntensityLut();
    }

    static void LoadIntensityLut()
    {
        TerrainIntensity.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, "Data/Meta", IntensityLutFileName);
        if (!File.Exists(path))
        {
            // Debug.LogWarning("[L2EnvWorldLut] Intensity LUT missing (" + IntensityLutFileName + "), using I=" + DefaultTerrainIntensity01.ToString("0.###"));
            return;
        }

        var lines = File.ReadAllText(path).Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            if (c.Length < 2)
                continue;
            float tHours = Parse(c[0]);
            if (tHours >= 23.999f)
                continue;
            TerrainIntensity.Add(new KeyFloat
            {
                tHours = tHours,
                value = Mathf.Clamp01(Parse(c[1]))
            });
        }

        TerrainIntensity.Sort((a, b) => a.tHours.CompareTo(b.tHours));
        //if (TerrainIntensity.Count >= 2)
            //Debug.Log("[L2EnvWorldLut] Loaded " + TerrainIntensity.Count + " intensity keys from " + IntensityLutFileName);
    }

    public static void LoadSweepCsv(string csvText)
    {
        SweepKeys.Clear();
        if (string.IsNullOrEmpty(csvText))
            return;

        var lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            if (c.Length < 10)
                continue;

            float tHours = Parse(c[0]);
            if (tHours >= 23.999f)
                continue;

            var key = new SweepKey
            {
                tHours = tHours,
                terrain = new Color(Parse(c[1]), Parse(c[2]), Parse(c[3]), 1f),
                staticMesh = new Color(Parse(c[4]), Parse(c[5]), Parse(c[6]), 1f),
                bsp = new Color(Parse(c[7]), Parse(c[8]), Parse(c[9]), 1f),
                hsvTerrain = Color.black,
                hsvStatic = Color.black
            };
            if (c.Length >= 14)
                key.hsvTerrain = new Color(Parse(c[10]), Parse(c[11]), Parse(c[12]), 1f);
            if (c.Length >= 18)
                key.hsvStatic = new Color(Parse(c[14]), Parse(c[15]), Parse(c[16]), 1f);
            SweepKeys.Add(key);
        }

        SweepKeys.Sort((a, b) => a.tHours.CompareTo(b.tHours));
    }

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
