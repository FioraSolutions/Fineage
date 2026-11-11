#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    static void ConvertTerrainToMesh(L2TerrainInfo terrainInfo) {
        terrainToMesh = new TerrainToMesh();
        texture2DArrayGenerator = new Texture2DArrayGenerator();

        string saveFolder = Path.Combine("Assets", "Resources", "Data", "Maps", mapToConvert);
        if(saveAssets) {
            if(!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);
            }
        }

        string mapFolder = L2InterludeMapPaths.ResolveTerrainPrefabAssetFolder(mapToConvert);
        GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapToConvert + ".prefab"));
        if(map != null) {
            map = Instantiate(map);
            terrainToConvert = map.GetComponent<Terrain>();
        } else {
            // Debug.LogWarning("Could not find map " + mapToConvert + " prefab.");
            return;
        }

        if(convertTerrain) {
            Material terrainMat = GenerateMaterial(saveFolder, terrainInfo);

            GameObject destObject = GenerateMesh(terrainToConvert, terrainMat, true);

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

        if(createMapSafenet) {
            GameObject destObject = GenerateMesh(terrainToConvert, null, false);
            SaveMesh(saveFolder, destObject, true);
        }

        // Generate decolayer
        if (convertDecoLayer) {
            List<L2DecoLayer> decoLayers = terrainInfo.decoLayers;
            GameObject decoLayer = DecoToMesh.ConvertDecoLayers(decoLayers, terrainToConvert);
            string decoInstancesPath = Path.Combine(saveFolder, mapToConvert + "_DecoInstances.asset").Replace("\\", "/");
            L2InstancedPropBaker.ReplaceHierarchyWithInstances(
                decoLayer,
                decoInstancesPath,
                castShadows: false,
                maxDrawDistance: 30f,
                layer: LayerMask.NameToLayer("Deco"));

            if (saveAssets) {
                string decoLayerPrefabPath = Path.Combine(saveFolder, mapToConvert + "_DecoLayer.prefab");
                PrefabUtility.SaveAsPrefabAsset(decoLayer, decoLayerPrefabPath);
            }
        }

        // Disable initial terrain
        terrainToConvert.gameObject.SetActive(false);
    }
}
#endif
