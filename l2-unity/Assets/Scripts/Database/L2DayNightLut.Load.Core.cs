using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2DayNightLut
{
    public static string PhaseName(int phase)
    {
        switch (phase)
        {
            case 0: return "morning";
            case 1: return "day";
            case 2: return "evening";
            case 3: return "night";
            default: return "phase" + phase;
        }
    }

    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2DayNightLut] File not found: " + dataPath + " — using DayNightCycle fallback colors.");
            return;
        }

        LoadCsv(File.ReadAllText(dataPath));
    }
}
