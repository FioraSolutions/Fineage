#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2TerrainInfoParser
{
    public static L2TerrainInfo ParseTerrainInfo0(string mapID) {
        string dataPath = "Assets/Data/Maps/Meta/" + mapID + "/TerrainInfo0.txt";
        if (!File.Exists(dataPath)) {
            // Debug.LogWarning("File not found: " + dataPath);
            return null;
        }

        var terrainInfo = new L2TerrainInfo();
        terrainInfo.mapName = mapID;
        terrainInfo.uvLayers = new List<L2TerrainLayer>();
        terrainInfo.decoLayers = new List<L2DecoLayer>();

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("TerrainMap=")) {
                    terrainInfo.terrainMapPath = TextureUtils.GetHeightMapPath(line);
                } else if (line.StartsWith("GeneratedSectorCounter=")) {
                    terrainInfo.generatedSectorCounter = ParseGeneratedSectorCounter(line);
                } else if (line.StartsWith("TerrainScale=")) {
                    terrainInfo.terrainScale = ParseVector3(line);
                } else if (line.StartsWith("Location=")) {
                    terrainInfo.location = ParseVector3(line);
                } else if (line.StartsWith("Layer")) {
                    L2TerrainLayer layer = ParseL2TerrainLayer(line);
                    if (layer != null) {
                        terrainInfo.uvLayers.Add(layer);
                    }
                } else if (line.StartsWith("DecoLayers")) {
                    L2DecoLayer layer = ParseL2DecoLayer(line);
                    if (layer != null) {
                        terrainInfo.decoLayers.Add(layer);
                    }
                }
            }
        }

        return terrainInfo;
    }

    private static int ParseGeneratedSectorCounter(string line) {
        // TODO: Handle parsing of GeneratedSectorCounter if needed
        return 256; // Temporary value
    }

    private static Vector3 ParseVector3(string line) {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');
        float x = float.Parse(valueParts[0].Substring(valueParts[0].IndexOf('=') + 1));
        float y = float.Parse(valueParts[1].Substring(valueParts[1].IndexOf('=') + 1));
        float z = float.Parse(valueParts[2].Substring(valueParts[2].IndexOf('=') + 1));
        return new Vector3(x, y, z);
    }
}
#endif
