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
}
