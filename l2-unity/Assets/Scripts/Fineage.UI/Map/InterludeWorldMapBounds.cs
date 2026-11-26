using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Interlude world-map L2 plane coverage (loaded from StreamingAssets, not hand-tuned C# constants).
/// </summary>
public static class InterludeWorldMapBounds
{
    public const string RelativePath = "Data/Map/world_map_interlude.json";

    static bool _loaded;
    static WorldMapBoundsRecord _record;

    [Serializable]
    sealed class WorldMapBoundsRecord
    {
        public string source;
        public float pixelWidth = 1812f;
        public float pixelHeight = 2620f;
        public float l2MinX;
        public float l2MaxX;
        public float l2MinY;
        public float l2MaxY;
    }

    public static float PixelWidth => EnsureLoaded().pixelWidth;
    public static float PixelHeight => EnsureLoaded().pixelHeight;
    public static float L2MinX => EnsureLoaded().l2MinX;
    public static float L2MaxX => EnsureLoaded().l2MaxX;
    public static float L2MinY => EnsureLoaded().l2MinY;
    public static float L2MaxY => EnsureLoaded().l2MaxY;

    static WorldMapBoundsRecord EnsureLoaded()
    {
        if (_loaded)
            return _record;

        _loaded = true;
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            Debug.LogError(
                "[InterludeWorldMapBounds] Missing StreamingAssets/" + RelativePath
                + "; world-map player marker projection is disabled.");
            _record = new WorldMapBoundsRecord();
            return _record;
        }

        try
        {
            string json = File.ReadAllText(path);
            _record = JsonUtility.FromJson<WorldMapBoundsRecord>(json);
            if (_record == null)
            {
                Debug.LogError("[InterludeWorldMapBounds] Parse failed: " + path);
                _record = new WorldMapBoundsRecord();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[InterludeWorldMapBounds] Read failed " + path + ": " + ex.Message);
            _record = new WorldMapBoundsRecord();
        }

        return _record;
    }

    public static void ResetForEditModeTests()
    {
        _loaded = false;
        _record = null;
    }
}