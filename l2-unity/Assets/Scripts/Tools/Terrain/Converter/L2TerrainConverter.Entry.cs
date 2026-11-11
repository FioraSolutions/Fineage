#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    [MenuItem("Shnok/6. [Terrain] Convert terrain to mesh")]
    static void ConvertTerrainMenu() {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);
            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);

            mapToConvert = mapName;

            ConvertTerrainToMesh(terrainInfo);
        }
    }
    public static bool ConvertTerrainToMeshForMap(string mapName) {
        string t3dAbs = L2InterludeMapPaths.T3dAbsolutePath(mapName);
        if (!File.Exists(t3dAbs)) {
            return false;
        }

        mapToConvert = mapName;
        convertTerrain = true;
        convertDecoLayer = false;
        createMapSafenet = true;
        optimizeMesh = true;

        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);
        if (terrainInfo == null) {
            return false;
        }

        ConvertTerrainToMesh(terrainInfo);
        return true;
    }
    public static bool GenerateDecoLayerMeshForMap(string mapName) {
        string t3dAbs = L2InterludeMapPaths.T3dAbsolutePath(mapName);
        if (!File.Exists(t3dAbs)) {
            return false;
        }

        mapToConvert = mapName;
        optimizeMesh = false;
        convertTerrain = false;
        convertDecoLayer = true;
        createMapSafenet = false;

        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);
        if (terrainInfo == null) {
            return false;
        }

        ConvertTerrainToMesh(terrainInfo);
        return true;
    }
    [MenuItem("Shnok/7. [Terrain] Generate deco layer mesh")]
    static void DecoLayers() {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);
            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);

            mapToConvert = mapName;

            optimizeMesh = false;
            convertTerrain = false;
            convertDecoLayer = false;
            createMapSafenet = false;
            convertDecoLayer = true;

            ConvertTerrainToMesh(terrainInfo);
        }
    }
}
#endif
