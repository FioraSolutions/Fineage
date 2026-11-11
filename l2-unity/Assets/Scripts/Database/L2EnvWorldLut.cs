using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// TimeEnv0.INT hourly keys, or live TimeSweep CSV from GetTerrainAmbientColor.
/// Actor ambient stays in L2DayNightLut. This is lighting on geometry, not Yebis.
/// </summary>
public static partial class L2EnvWorldLut
{
    public struct Sample
    {
        public Color terrainAmbient;
        public Color staticMeshAmbient;
        public Color bspAmbient;
        public Color hsvTerrainLight;
        public Color hsvStaticMeshLight;
    }

    const string SweepLutFileName = "L2WorldAmbientLUT.csv";

    const string IntensityLutFileName = "L2TerrainIntensityLUT.csv";

    const string MetaFileName = "TimeEnv0.int";

    const float DefaultTerrainIntensity01 = 156f / 255f;

    static readonly int TerrainAmbientId = Shader.PropertyToID("_L2TerrainAmbient");

    static readonly int StaticMeshAmbientId = Shader.PropertyToID("_L2StaticMeshAmbient");

    static readonly int BspAmbientId = Shader.PropertyToID("_L2BspAmbient");

    static readonly int ActorAmbientId = Shader.PropertyToID("_L2ActorAmbient");

    static readonly int HsvTerrainLightId = Shader.PropertyToID("_L2HsvTerrainLight");

    static readonly int HsvStaticMeshLightId = Shader.PropertyToID("_L2HsvStaticMeshLight");

    static readonly int FogColorId = Shader.PropertyToID("_L2FogColor");

    static readonly int FogEndId = Shader.PropertyToID("_L2FogEnd");

    static readonly int FogScaleId = Shader.PropertyToID("_L2FogScale");

    static readonly int FogEnableId = Shader.PropertyToID("_L2FogEnable");

    static readonly int TerrainColor0ScaleId = Shader.PropertyToID("_L2TerrainColor0Scale");

    static readonly int TerrainIntensityId = Shader.PropertyToID("_L2TerrainIntensity");

    static readonly int TerrainIntensityMapId = Shader.PropertyToID("_L2TerrainIntensityMap");

    static readonly int TerrainIntensityOriginId = Shader.PropertyToID("_L2TerrainIntensityOrigin");

    static readonly int TerrainIntensityMap1Id = Shader.PropertyToID("_L2TerrainIntensityMap1");

    static readonly int TerrainIntensityOrigin1Id = Shader.PropertyToID("_L2TerrainIntensityOrigin1");

    static readonly int TerrainIntensityMap2Id = Shader.PropertyToID("_L2TerrainIntensityMap2");

    static readonly int TerrainIntensityOrigin2Id = Shader.PropertyToID("_L2TerrainIntensityOrigin2");

    static readonly int TerrainLightDebugId = Shader.PropertyToID("_L2TerrainLightDebug");

    const bool ShowTerrainLightDebug = false;

    const bool UseStoredClientIntensityGrid = true;

    static bool LoadStoredIntensityMasks = true;

    static bool LoadBaked17_25Intensity = false;

    const string IntensityGridFileName = "L2TerrainIntensityGrid.bin";

    const string StoredIntensityGridFileName = "L2TerrainIntensityGrid_stored.bin";

    const string Baked17_25IntensityFileName = "L2TerrainIntensityGrid_17_25_baked.bin";

    const int IntensityGridSize = 257;

    static readonly Vector4 IntensityOrigin = new Vector4(-98304f, 229376f, 128f, IntensityGridSize);

    static readonly int StaticMeshMaskSlot0Id = Shader.PropertyToID("_L2StaticMeshMaskSlot0");

    static readonly int StaticMeshMaskSlot1Id = Shader.PropertyToID("_L2StaticMeshMaskSlot1");

    static readonly int StaticMeshMaskBlendId = Shader.PropertyToID("_L2StaticMeshMaskBlend");

    static readonly int StaticMeshLightDirectionId = Shader.PropertyToID("_L2StaticMeshLightDirection");

    public const float FogEndUu = 12288f;

    public const float FogRangeUu = 8192f;

    static readonly List<KeyColor> TerrainAmbient = new List<KeyColor>(32);

    static readonly List<KeyColor> StaticMeshAmbient = new List<KeyColor>(32);

    static readonly List<KeyColor> BspAmbient = new List<KeyColor>(32);

    static readonly List<KeyHsv> HsvTerrainLight = new List<KeyHsv>(32);

    static readonly Regex ColorRe = new Regex(

        @"T\s*=\s*([0-9.]+)\s*,\s*R\s*=\s*([0-9.]+)\s*,\s*G\s*=\s*([0-9.]+)\s*,\s*B\s*=\s*([0-9.]+)",

        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static readonly Regex HsvRe = new Regex(

        @"T\s*=\s*([0-9.]+)\s*,\s*Hue\s*=\s*([0-9.]+)\s*,\s*Sat\s*=\s*([0-9.]+)\s*,\s*Bri\s*=\s*([0-9.]+)",

        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static readonly List<SweepKey> SweepKeys = new List<SweepKey>(320);

    static readonly List<KeyFloat> TerrainIntensity = new List<KeyFloat>(320);

    static bool _triedLoad;

    public static bool IsReady =>

        SweepKeys.Count >= 2 || (TerrainAmbient.Count >= 2 && StaticMeshAmbient.Count >= 2);

    static float Parse(string s) => float.Parse(s, CultureInfo.InvariantCulture);

    struct SweepKey
    {
        public float tHours;
        public Color terrain;
        public Color staticMesh;
        public Color bsp;
        public Color hsvTerrain;
        public Color hsvStatic;
    }

    struct KeyFloat
    {
        public float tHours;
        public float value;
    }

    struct KeyColor
    {
        public float tHours;
        public Color color;
    }

    struct KeyHsv
    {
        public float tHours;
        public float hue;
        public float sat;
        public float bri;
    }

    struct IntensitySlice
    {
        public float tHours;
        public byte[] intensity;
        public byte[] valid;
    }

    static readonly List<IntensitySlice> IntensitySlices = new List<IntensitySlice>(8);

    static Texture2D _intensityTex;

    static bool _gridTried;

    static bool _openIntensityPushed;

    static byte[] _unityTreePreview;

    static bool _unityTreePreviewUploaded;

    static int _gridSliceA = -2;

    static int _gridSliceB = -2;

    static byte _gridFill = 255;

    sealed class NeighborIntensityGrid
    {
        public string fileName;
        public Vector4 origin;
        public int textureId;
        public int originId;
        public readonly List<IntensitySlice> slices = new List<IntensitySlice>(8);
        public Texture2D tex;
        public bool tried;
        public int sliceA = -2;
        public int sliceB = -2;
        public byte fill = 255;
    }

    static readonly NeighborIntensityGrid[] NeighborGrids =

    {

        new NeighborIntensityGrid

        {

            fileName = "L2TerrainIntensityGrid_16_25_stored.bin",

            origin = new Vector4(-131072f, 229376f, 128f, IntensityGridSize),

            textureId = TerrainIntensityMap1Id,

            originId = TerrainIntensityOrigin1Id

        },

        new NeighborIntensityGrid

        {

            fileName = "L2TerrainIntensityGrid_16_24_stored.bin",

            origin = new Vector4(-131072f, 196608f, 128f, IntensityGridSize),

            textureId = TerrainIntensityMap2Id,

            originId = TerrainIntensityOrigin2Id

        }

    };

    static bool _baked17Tried;

    static bool _bakedNeighborsCleared;
}
