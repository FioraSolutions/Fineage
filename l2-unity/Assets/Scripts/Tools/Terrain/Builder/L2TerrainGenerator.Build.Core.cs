#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGenerator
{
    private void GenerateHeightmaps(TerrainData terrainData, L2TerrainInfo terrainInfo) {
        byte[] terrainMap = File.ReadAllBytes(terrainInfo.terrainMapPath);

        // Calculate the resolution based on the file size
        int resolution = (int)Mathf.Sqrt(terrainMap.Length / 2); // each height is 2 bytes (16 bits)

        terrainData.heightmapResolution = resolution + 1; // Set the resolution of the heightmap

        // Create a new array for the heightmap
        float[,] heights = new float[resolution + 1, resolution + 1];

        // Read the heights from the file
        using (BinaryReader reader = new BinaryReader(new MemoryStream(terrainMap))) {
            reader.ReadBytes(54);

            for (int i = resolution - 1; i >= 0; i--)
                for (int j = 0; j < resolution; j++) {
                    // Unity uses a value between 0 and 1 for the heightmap data
                    // ushort.MaxValue is 65535
                    heights[j + 1, i + 1] = reader.ReadUInt16() / (float)ushort.MaxValue;
                }
        }

        //Filling out the terrain seam.
        for (int i = 0; i < resolution + 1; i++) {
            heights[0, i] = heights[1, i];
        }
        for (int i = 0; i < resolution + 1; i++) {
            heights[i, 0] = heights[i, 1];
        }

        terrainData.heightmapResolution = resolution;
        terrainData.SetHeights(0, 0, heights);

    }
}
#endif