#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGeneratorTool : MonoBehaviour
{
    public static Terrain GenerateTerrainForMap(string mapName) {
        List<MapGenerationData> mapsToGenerate = new List<MapGenerationData>();
        MapGenerationData data = new MapGenerationData();
        data.mapName = mapName;
        data.generateDecoLayers = true;
        data.generateUVLayers = true;
        data.generateHeightmaps = true;
        data.generateStaticMeshes = false;
        data.generationMode = GenerationMode.Generate;
        mapsToGenerate.Add(data);
        return GenerateMap(mapsToGenerate);
    }

    public static void GenerateStaticMeshesForMap(string mapName) {
        List<MapGenerationData> mapsToGenerate = new List<MapGenerationData>();
        MapGenerationData data = new MapGenerationData();
        data.mapName = mapName;
        data.generateDecoLayers = false;
        data.generateUVLayers = false;
        data.generateHeightmaps = false;
        data.generateStaticMeshes = true;
        data.generationMode = GenerationMode.Generate;
        mapsToGenerate.Add(data);
        GenerateMap(mapsToGenerate);
    }

    private static Terrain GenerateMap(List<MapGenerationData> mapsToGenerate) {
        L2TerrainGenerator generator = new L2TerrainGenerator();
        terrainInfos = new List<L2TerrainInfo>();
        terrains = new List<Terrain>();
        terrainsDict = new Dictionary<string, Terrain>();

        Terrain lastTerrain = null;
        for (int i = 0; i < mapsToGenerate.Count; i++) {

            if (!mapsToGenerate[i].enabled) {
                continue;
            }

            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapsToGenerate[i].mapName);
            terrainInfos.Add(terrainInfo);

            Terrain terrain = generator.InstantiateTerrain(mapsToGenerate[i], terrainInfo);

            terrains.Add(terrain);
            lastTerrain = terrain;

            terrainsDict.Add(mapsToGenerate[i].mapName, terrain);
        }

        // generator.StitchTerrainSeams(terrainsDict);
        //   generator.StitchTerrainSeams(terrainsDict);
        return lastTerrain;
    }
}
#endif
