#if (UNITY_EDITOR)
using UnityEngine;

[System.Serializable]
public class L2DecoLayer {
    public bool showOnTerrain { get; set; }
    public bool hasShowOnTerrain { get; set; }
    public string densityMapPath { get; set; }
    public string densityMapRef { get; set; }
    public string scaleMapRef { get; set; }
    public string colorMapRef { get; set; }
    public string staticMeshRef { get; set; }
    public Texture2D densityMap { get; set; }
    public GameObject staticMesh { get; set; }
    public float minHeight { get; set; }
    public float maxHeight { get; set; }
    public float minWidth { get; set; }
    public float maxWidth { get; set; }
    public float scaleXMin { get; set; }
    public float scaleXMax { get; set; }
    public float scaleYMin { get; set; }
    public float scaleYMax { get; set; }
    public float scaleZMin { get; set; }
    public float scaleZMax { get; set; }
    public bool hasScale { get; set; }
    public float fadeMinUu { get; set; }
    public float fadeMaxUu { get; set; }
    public bool hasFade { get; set; }
    public float densityMin { get; set; }
    public float densityMax { get; set; }
    public bool hasDensity { get; set; }
    public int maxPerQuad { get; set; }
    public bool hasMaxPerQuad { get; set; }
    public int seed { get; set; }
    public bool hasSeed { get; set; }
    public bool alignToTerrain { get; set; }
    public bool hasAlign { get; set; }
    public bool randomYaw { get; set; }
    public bool hasRandomYaw { get; set; }
    public bool showOnInvisibleTerrain { get; set; }
    public bool hasShowOnInvisible { get; set; }
    public bool litDirectional { get; set; }
    public bool disregardTerrainLighting { get; set; }
    public bool forceRender { get; set; }
    public int drawOrder { get; set; }
    public bool hasDrawOrder { get; set; }

    /// <summary>
    /// Official density: DensityMultiplier lerp on the density-map alpha, then MaxPerQuad.
    /// A zero sample places nothing. Shared with <see cref="L2DecoGenerator.CountForDensity"/>.
    /// </summary>
    public int CountForDensity(int alpha)
    {
        if (!hasDensity || !hasMaxPerQuad)
            return 0;
        return L2DecoGenerator.CountForDensity(densityMin, densityMax, maxPerQuad, alpha);
    }
}
#endif
