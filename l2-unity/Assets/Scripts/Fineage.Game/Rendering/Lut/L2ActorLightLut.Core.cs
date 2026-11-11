using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static partial class L2ActorLightLut
{
    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2ActorLightLut] File not found: " + dataPath);
            return;
        }

        LoadCsv(File.ReadAllText(dataPath));
        //if (Samples.Count >= 2)
        //    Debug.Log("[L2ActorLightLut] Loaded " + Samples.Count + " keys from " + MetaFileName);
    }

    public static void LoadCsv(string csvText)
    {
        Samples.Clear();
        if (string.IsNullOrEmpty(csvText))
            return;

        var lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            if (c.Length < 14)
                continue;

            float tHours = Parse(c[0]);
            if (tHours >= 23.999f)
                continue;

            Samples.Add(new Sample
            {
                tHours = tHours,
                ambient = new Color(Parse(c[1]), Parse(c[2]), Parse(c[3]), 1f),
                ambientHalf = new Color(Parse(c[4]), Parse(c[5]), Parse(c[6]), 1f),
                sunPlane = new Color(Parse(c[7]), Parse(c[8]), Parse(c[9]), Parse(c[10])),
                sunBase = new Color(Parse(c[11]), Parse(c[12]), Parse(c[13]), 1f)
            });
        }

        Samples.Sort((a, b) => a.tHours.CompareTo(b.tHours));
    }
}
