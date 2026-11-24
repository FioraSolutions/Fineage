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

        // Unity's heightmap vertex count is 2^n+1. The extra vertex repeats the
        // last texel so the sheet is not moved by a sector. Vertex i sits on
        // texel i of L2Ue2Terrain (file row 0 is the +Y edge).
        int verts = resolution + 1;
        terrainData.heightmapResolution = verts;
        float[,] heights = new float[verts, verts];

        using (BinaryReader reader = new BinaryReader(new MemoryStream(terrainMap))) {
            reader.ReadBytes(54);

            for (int fileRow = 0; fileRow < resolution; fileRow++)
                for (int j = 0; j < resolution; j++)
                    heights[j, resolution - 1 - fileRow] = reader.ReadUInt16() / (float)ushort.MaxValue;
        }

        for (int i = 0; i < verts; i++) {
            heights[i, resolution] = heights[i, resolution - 1];
            heights[resolution, i] = heights[resolution - 1, i];
        }

        terrainData.SetHeights(0, 0, heights);

    }
}
#endif