using System.IO;
using UnityEngine;

public static partial class L2CelestialLut
{
    public static void EnsureLoaded()
    {
        if (_triedLoad)
            return;

        _triedLoad = true;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta", MetaFileName);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("[L2CelestialLut] File not found: " + dataPath);
            return;
        }

        LoadCsv(File.ReadAllText(dataPath));
    }
}
