#if (UNITY_EDITOR)
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    static bool ConvertTerrainToMesh(L2TerrainInfo terrainInfo) {
        currentTerrainInfo = terrainInfo;
        terrainToConvert = null;
        if (terrainInfo != null && terrainInfo.quadVisibilityBitmap != null
            && terrainInfo.quadVisibilityBitmap.Length == L2TerrainInfoParser.QuadVisibilityWordCount) {
            meshSubdivisions = L2TerrainInfoParser.QuadVisibilitySize;
        }
        terrainToMesh = new TerrainToMesh();
        texture2DArrayGenerator = new Texture2DArrayGenerator();

        string saveFolder = Path.Combine("Assets", "Resources", "Data", "Maps", mapToConvert);
        if(saveAssets) {
            if(!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);
            }
        }

        bool terrainOk = true;
        bool needTerrainPrefab = convertTerrain || createMapSafenet;
        if (needTerrainPrefab) {
            string mapFolder = L2InterludeMapPaths.ResolveTerrainPrefabAssetFolder(mapToConvert);
            GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapToConvert + ".prefab"));
            if (map == null) {
                Debug.LogError("[Interlude terrain] " + mapToConvert + ": terrain prefab missing");
                terrainOk = false;
            } else {
                map = Instantiate(map);
                terrainToConvert = map.GetComponent<Terrain>();
                if (terrainToConvert == null) {
                    Debug.LogError("[Interlude terrain] " + mapToConvert + ": prefab has no Terrain");
                    terrainOk = false;
                }
            }
        }

        if(terrainOk && convertTerrain) {
            Material terrainMat = GenerateMaterial(saveFolder, terrainInfo);

            GameObject destObject = GenerateMesh(terrainToConvert, terrainMat, true, applyHoles: true);

            if(saveAssets) {
                // Persist heightmap mesh and bind it before prefab save; otherwise prefab
                // keeps MeshFilter.m_Mesh {fileID:0} (runtime-only mesh) => white void.
                Mesh savedMesh = SaveMesh(saveFolder, destObject, false);
                if (savedMesh != null) {
                    destObject.GetComponent<MeshFilter>().sharedMesh = savedMesh;
                    MeshCollider mc = destObject.GetComponent<MeshCollider>();
                    if (mc != null) mc.sharedMesh = savedMesh;
                }
                SaveMaterial(saveFolder, terrainMat);
                SaveTerrain(saveFolder, destObject);
            }
        }

        if(terrainOk && createMapSafenet) {
            GameObject destObject = GenerateMesh(terrainToConvert, null, false, applyHoles: false);
            SaveMesh(saveFolder, destObject, true);
        }

        bool decoOk = true;
        if (convertDecoLayer)
            decoOk = L2DecoArchiveBake.WriteForMap(mapToConvert);

        if (terrainToConvert != null)
            terrainToConvert.gameObject.SetActive(false);
        return terrainOk && decoOk;
    }
}
#endif
