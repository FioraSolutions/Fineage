using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static partial class L2HazeLut
{
    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2HazeLut] File not found: " + dataPath);
            return;
        }

        LoadCsv(File.ReadAllText(dataPath));
    }

    public static void LoadCsv(string csvText)
    {
        Samples.Clear();
        if (string.IsNullOrEmpty(csvText))
        {
            return;
        }

        var lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            if (c.Length < 4)
            {
                continue;
            }

            float tHours = Parse(c[0]);
            // TimeSweep last key t=24 is a wrap spike (#B349C0); skip it.
            if (tHours >= 23.999f)
            {
                continue;
            }

            Samples.Add(new Sample
            {
                tHours = tHours,
                haze = new Color(Parse(c[1]), Parse(c[2]), Parse(c[3]), 1f)
            });
        }

        Samples.Sort((a, b) => a.tHours.CompareTo(b.tHours));
        // Debug.Log("[L2HazeLut] Loaded " + Samples.Count + " GetHazeColor keys from " + MetaFileName);
    }
}
