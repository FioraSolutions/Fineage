#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter : MonoBehaviour {
    public static string mapToConvert;
    public static int meshSubdivisions = 128;
    public static float uvScale = 150;
    private static bool saveAssets = true;
    private static bool optimizeMesh = true;
    private static bool convertTerrain = true;
    private static bool convertDecoLayer = true;
    private static bool createMapSafenet = true;

    private static TerrainToMesh terrainToMesh;
    private static Texture2DArrayGenerator texture2DArrayGenerator;
    private static Terrain terrainToConvert;


    [MenuItem("Shnok/6. [Terrain] Convert terrain to mesh")]
}
