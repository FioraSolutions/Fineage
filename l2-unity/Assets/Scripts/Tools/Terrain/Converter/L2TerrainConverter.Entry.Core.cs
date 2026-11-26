#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;

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
        return ConvertTerrainToMeshForMap(mapName, true, false);
    }

    /// <summary>
    /// <paramref name="writeDeco"/> stores DecoLayer generator inputs (<c>XX_YY_Deco.bytes</c>).
    /// It does not need the terrain prefab. Terrain mesh export still does.
    /// </summary>
    public static bool ConvertTerrainToMeshForMap(string mapName, bool writeTerrain, bool writeDeco) {
        mapToConvert = mapName;
        convertTerrain = writeTerrain;
        convertDecoLayer = writeDeco;
        createMapSafenet = writeTerrain;
        optimizeMesh = true;

        if (writeDeco && !writeTerrain)
            return L2DecoArchiveBake.WriteForMap(mapName);

        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);
        if (terrainInfo == null) {
            return false;
        }

        return ConvertTerrainToMesh(terrainInfo);
    }
}
#endif
