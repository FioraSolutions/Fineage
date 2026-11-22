#if (UNITY_EDITOR)
using UnityEngine;
using System;

[System.Serializable]
public class L2DecoLayer {
    public bool showOnTerrain { get; set; }
    public string densityMapPath { get; set; }
    public Texture2D densityMap { get; set; }
    public GameObject staticMesh { get; set; }
    public float minHeight { get; set; }
    public float maxHeight { get; set; }
    public float minWidth { get; set; }
    public float maxWidth { get; set; }
    public float fadeMinUu { get; set; }
    public float fadeMaxUu { get; set; }
    public float densityMin { get; set; }
    public float densityMax { get; set; }
    public int maxPerQuad { get; set; }
    public int seed { get; set; }
    public bool alignToTerrain { get; set; }
    public bool randomYaw { get; set; } = true;

    /// <summary>
    /// Official density: remap the density-map sample (0-255) through DensityMultiplier,
    /// then cap at MaxPerQuad. A zero sample places nothing.
    /// </summary>
    public int CountForDensity(int packed)
    {
        if (packed <= 0)
        {
            return 0;
        }

        float t = packed / 255f;
        float lo = densityMin;
        float hi = densityMax > 0f ? densityMax : packed;
        int count = Mathf.RoundToInt(Mathf.Lerp(lo, hi, t));
        if (count < 1)
        {
            count = 1;
        }
        if (maxPerQuad > 0 && count > maxPerQuad)
        {
            count = maxPerQuad;
        }
        return count;
    }
}
#endif
