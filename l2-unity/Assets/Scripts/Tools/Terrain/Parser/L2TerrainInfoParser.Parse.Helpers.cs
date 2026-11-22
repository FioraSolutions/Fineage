#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2TerrainInfoParser
{
    private static L2DecoLayer UpdateDecoLayerScale(L2DecoLayer layer, string scaleMultiplier) {
        if (string.IsNullOrEmpty(scaleMultiplier))
            return layer;
        bool x = TryRange(scaleMultiplier, "X", out float xMin, out float xMax);
        bool y = TryRange(scaleMultiplier, "Y", out float yMin, out float yMax);
        bool z = TryRange(scaleMultiplier, "Z", out float zMin, out float zMax);
        if (!x || !y || !z)
            return layer;
        layer.hasScale = true;
        layer.scaleXMin = xMin;
        layer.scaleXMax = xMax;
        layer.scaleYMin = yMin;
        layer.scaleYMax = yMax;
        layer.scaleZMin = zMin;
        layer.scaleZMax = zMax;
        // Unity detail prototypes: X is width, Z is height.
        layer.minWidth = xMin;
        layer.maxWidth = xMax;
        layer.minHeight = zMin;
        layer.maxHeight = zMax;
        return layer;
    }

    static bool TryRange(string text, string axis, out float min, out float max)
    {
        min = 0f;
        max = 0f;
        var m = System.Text.RegularExpressions.Regex.Match(
            text, axis + @"=\(Min=([-0-9.]+),Max=([-0-9.]+)\)");
        if (!m.Success)
            return false;
        return float.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out min)
            && float.TryParse(m.Groups[2].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out max);
    }

    public const int QuadVisibilityWordCount = 2048;
    public const int QuadVisibilitySize = 256;

    public static bool TryParseQuadVisibilityLine(string line, out int index, out int word)
    {
        index = 0;
        word = 0;
        const string prefix = "QuadVisibilityBitmap(";
        if (line == null || !line.StartsWith(prefix))
        {
            return false;
        }

        int close = line.IndexOf(')', prefix.Length);
        int eq = close >= 0 ? line.IndexOf('=', close) : -1;
        if (close < 0 || eq < 0)
        {
            return false;
        }

        return int.TryParse(line.Substring(prefix.Length, close - prefix.Length), out index)
            && int.TryParse(line.Substring(eq + 1).Trim(), out word);
    }

    public static int[] FinalizeQuadVisibility(string sourcePath, Dictionary<int, int> words)
    {
        if (words == null || words.Count == 0)
        {
            Debug.LogError("[Interlude terrain] QuadVisibilityBitmap missing in " + sourcePath
                + " (need 2048 words / 65536 bits).");
            return null;
        }

        if (words.Count != QuadVisibilityWordCount)
        {
            Debug.LogError("[Interlude terrain] QuadVisibilityBitmap in " + sourcePath
                + " has " + words.Count + " words, need " + QuadVisibilityWordCount
                + " (256x256). Truncated Meta must not be used.");
            return null;
        }

        int[] arr = new int[QuadVisibilityWordCount];
        for (int i = 0; i < QuadVisibilityWordCount; i++)
        {
            if (!words.TryGetValue(i, out arr[i]))
            {
                Debug.LogError("[Interlude terrain] QuadVisibilityBitmap in " + sourcePath
                    + " is missing word " + i + ".");
                return null;
            }
        }

        return arr;
    }

    /// <summary>Official bit index is qy * 256 + qx (LSB of word 0 is quad 0). True means the quad is visible.</summary>
    public static bool IsQuadVisible(int[] words, int qx, int qy)
    {
        if (words == null || words.Length != QuadVisibilityWordCount)
        {
            return true;
        }

        if (qx < 0 || qy < 0 || qx >= QuadVisibilitySize || qy >= QuadVisibilitySize)
        {
            return true;
        }

        int i = qy * QuadVisibilitySize + qx;
        return ((words[i / 32] >> (i % 32)) & 1) != 0;
    }

    /// <summary>Unity X tracks UE Y, Unity Z tracks UE X (same swap as terrain placement).</summary>
    public static bool IsUnityQuadVisible(int[] words, int unityX, int unityZ)
    {
        return IsQuadVisible(words, unityZ, unityX);
    }

    public static AmbientSound[] ParseAmbientSoundFile(string path) {
        try {
            string json = File.ReadAllText(path);
            AmbientDataContainer sounds = JsonUtility.FromJson<AmbientDataContainer>(json);
            return sounds.data;

        } catch (IOException) {
            // Debug.LogError("An error occurred: " + e.Message);
        }

        return null;
    }
}
#endif
