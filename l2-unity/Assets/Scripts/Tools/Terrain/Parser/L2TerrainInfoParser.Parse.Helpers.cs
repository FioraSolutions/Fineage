#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2TerrainInfoParser
{
    private static L2DecoLayer UpdateDecoLayerScale(L2DecoLayer layer, string scaleMultiplier) {
        int equalsIndex = scaleMultiplier.IndexOf('=');
        string valueString = scaleMultiplier.Substring(equalsIndex + 1, scaleMultiplier.Length - equalsIndex - 2);

        valueString = valueString
            .Replace("(", String.Empty)
            .Replace(")", String.Empty)
            .Replace("Min", String.Empty)
            .Replace("Max", String.Empty)
            .Replace("=", String.Empty)
            .Replace("X", String.Empty)
            .Replace("Y", String.Empty)
            .Replace("Z", String.Empty);

        string[] parts = valueString.Split(",");

        layer.minWidth = float.Parse(parts[0]);
        layer.maxWidth = float.Parse(parts[1]);
        layer.minHeight = float.Parse(parts[2]);
        layer.maxHeight = float.Parse(parts[3]);

        return layer;
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
