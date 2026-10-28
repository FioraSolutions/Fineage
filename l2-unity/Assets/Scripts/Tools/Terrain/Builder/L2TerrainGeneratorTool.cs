#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2TerrainGeneratorTool : MonoBehaviour {
    public static int UV_TEXTURE_SIZE = 256;
    public static int UV_LAYER_ALPHAMAP_SIZE = 1024;
    public static int DECO_LAYER_ALPHAMAP_SIZE = 512;
    public static float UV_TILE_SIZE = 5f;
    public static float MAP_SCALE = 1f;

    public int uvTextureSize = 256;
    public int uvLayerAlphaMapSize = 1024;
    public int decoLayerAlphaMapSize = 512;
    public float uvTileSize = 5f;
    public float mapScale = 1f;

    public bool generateMap = true;
    public static List<L2TerrainInfo> terrainInfos;
    public List<L2StaticMeshActor> meshActors;
    public static List<Terrain> terrains;
    public static Dictionary<string, Terrain> terrainsDict;
    public List<MapGenerationData> maps;

    [MenuItem("Shnok/4. [Terrain] Generate terrain")]
    static void GenerateTerrain() {
        GenerateTerrainForMap("l2_lobby");
    }

    [MenuItem("Shnok/3. [StaticMeshes] Generate staticmeshes")]
    static void GenerateStaticMeshes() {
        GenerateStaticMeshesForMap("l2_lobby");
    }

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
