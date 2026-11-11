#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGenerator
{
    public Terrain InstantiateTerrain(MapGenerationData generationData, L2TerrainInfo terrainInfo) {
        string directoryPath = Path.Combine("Assets", "Resources", "Data", "Maps", generationData.mapName, "TerrainData");
        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        if (generationData.generateStaticMeshes) {
            GenerateStaticMeshes(terrainInfo);
        }

        if (!generationData.generateDecoLayers && !generationData.generateUVLayers && !generationData.generateHeightmaps) {
            return null;
        }

        // Create the terrain object
        GameObject terrainObj = Terrain.CreateTerrainGameObject(new TerrainData());
        terrainObj.name = terrainContainerName + terrainInfo.mapName;

        // Get the Terrain component and TerrainData
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        terrain.heightmapPixelError = 3;
        terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        terrain.drawInstanced = true;
        terrain.detailObjectDistance = 150;

        TerrainData terrainData = terrain.terrainData;
        terrainData.baseMapResolution = L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE;
        terrainData.alphamapResolution = L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE;

        terrainData.SetDetailResolution(512, 32);

        // Just to initialize
        terrainData.size = new Vector3(1015f, 603f, 1015f);

        // Save the terrainData asset
        string savePath = Path.Combine(directoryPath, terrainInfo.mapName + ".asset");
        AssetDatabase.CreateAsset(terrainData, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign the saved asset to the terrain object
        terrain.terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(savePath);

        if (generationData.generateUVLayers) {
            // Debug.Log(terrainInfo.mapName);
            // Debug.Log(terrainData);
            // Debug.Log(terrainInfo);
            GenerateUVLayers(terrainInfo.mapName, terrainData, terrainInfo);
        }

        if (generationData.generateHeightmaps) {
            GenerateHeightmaps(terrainData, terrainInfo);
        }

        if (generationData.generateDecoLayers) {
            GenerateDecoLayers(terrainData, terrainInfo);
        }

        float tx = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.y;
        float ty = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.z;
        float tz = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.x;
        terrainData.size = new Vector3(tx, ty, tz) * ueToUnityUnitScale * L2TerrainGeneratorTool.MAP_SCALE;

        // Debug.Log("TerrainData Size:" + terrainData.size);

        var uxHalfTerrainWidthAdjustment = (float)tx * 0.5f;
        var uyHalfTerrainWidthAdjustment = (float)ty * 0.5F;
        var uzHalfTerrainWidthAdjustment = (float)tz * 0.5F;

        // Terrain is shifted by one sector size to accomodate the terrain seam.
        var unityPos = new Vector3(
            terrainInfo.location.y - uxHalfTerrainWidthAdjustment - terrainInfo.terrainScale.y,
            terrainInfo.location.z - uyHalfTerrainWidthAdjustment,
            terrainInfo.location.x - uzHalfTerrainWidthAdjustment - terrainInfo.terrainScale.x
        ) * ueToUnityUnitScale * L2TerrainGeneratorTool.MAP_SCALE * worldPositionOffset;

        terrain.transform.position = unityPos;
        terrain.transform.name = terrainInfo.mapName;

        return terrain;
    }
}
#endif