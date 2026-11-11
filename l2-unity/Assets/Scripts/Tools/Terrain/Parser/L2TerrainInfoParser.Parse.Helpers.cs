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
