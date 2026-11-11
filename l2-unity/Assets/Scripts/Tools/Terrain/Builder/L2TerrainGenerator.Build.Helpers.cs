#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGenerator
{
    public void GenerateUVLayers(string mapID, TerrainData terrainData, L2TerrainInfo terrainInfo) {
        // Create terrain layers
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainInfo.uvLayers.Count];
        terrainData.terrainLayers = new TerrainLayer[terrainInfo.uvLayers.Count];

        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.diffuseTexture = terrainInfo.uvLayers[i].texture;
            terrainLayer.metallic = 0;
            terrainLayer.specular = Color.black;
            terrainLayer.smoothness = 0;
            //terrainLayer.smoothnessSource = TerrainLayerSmoothnessSource.Constant;
            terrainLayer.tileOffset = Vector2.zero;
            terrainLayer.tileSize = new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale) * L2TerrainGeneratorTool.MAP_SCALE * L2TerrainGeneratorTool.UV_TILE_SIZE;

            string savePath = Path.Combine("Assets", "Resources", "Data", "Maps", mapID, "TerrainData", mapID + "_layer_" + i + "_" + terrainInfo.uvLayers[i].texture.name + ".asset");
            AssetDatabase.CreateAsset(terrainLayer, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            terrainLayers[i] = AssetDatabase.LoadAssetAtPath<TerrainLayer>(savePath);
        }

        // Set terrain layers to terrain data
        terrainData.terrainLayers = terrainLayers;

        // Flip vertically
        Texture2D[] flippedAlphaMaps = new Texture2D[terrainInfo.uvLayers.Count];
        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            if (terrainInfo.uvLayers[i].alphaMap != null) {
                flippedAlphaMaps[i] = TextureUtils.FlipTextureVertically(terrainInfo.uvLayers[i].alphaMap);
            }
        }

        float uvMultiplier = 256f / 257f;

        // Now you can set up your splatmap using your masks
        float[,,] map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainInfo.uvLayers.Count];
        for (int y = 0; y < terrainData.alphamapHeight; y++) {
            for (int x = 0; x < terrainData.alphamapWidth; x++) {

                // Initialize all weights to zero
                for (int i = 0; i < terrainInfo.uvLayers.Count; i++)
                    map[x, y, i] = 0;

                float remainingWeight = 1; // keep track of the remaining weight available

                for (int i = terrainInfo.uvLayers.Count - 1; i >= 0; i--) {
                    float u = (x) / (float)(terrainData.alphamapWidth);
                    float v = (y) / (float)(terrainData.alphamapHeight);

                    float weight = 0;

                    if (flippedAlphaMaps[i] != null) {
                        float maskValue = flippedAlphaMaps[i].GetPixelBilinear(u * uvMultiplier, v * uvMultiplier).grayscale;

                        // Calculate the weight for this layer, ensuring that it doesn't exceed the remaining available weight
                        weight = Mathf.Min(maskValue, remainingWeight);
                    }

                    map[x, y, i] = weight;

                    // Subtract the weight assigned to this layer from the remaining available weight
                    remainingWeight -= weight;
                }
            }

            terrainData.SetAlphamaps(0, 0, map);
        }
    }
}
#endif