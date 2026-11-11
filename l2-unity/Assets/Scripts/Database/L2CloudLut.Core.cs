using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2CloudLut
{
    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2CloudLut] File not found: " + dataPath);
            return;
        }

        LoadCsv(File.ReadAllText(dataPath));
    }

    public static void LoadCsv(string csvText)
    {
        Samples.Clear();
        if (string.IsNullOrEmpty(csvText))
            return;

        var lines = csvText.Split(
            new[] { '\r', '\n' },
            System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            if (c.Length < 7)
                continue;

            float t = Parse(c[0]);
            if (t >= 23.999f)
                continue;

            Samples.Add(new Sample
            {
                tHours = t,
                sheet = new Color(Parse(c[1]), Parse(c[2]), Parse(c[3]), 1f),
                silhouette = new Color(Parse(c[4]), Parse(c[5]), Parse(c[6]), 1f)
            });
        }

        Samples.Sort((a, b) => a.tHours.CompareTo(b.tHours));
        // Debug.Log("[L2CloudLut] Loaded " + Samples.Count + " GetCloudColor keys from " + MetaFileName);
    }
}
