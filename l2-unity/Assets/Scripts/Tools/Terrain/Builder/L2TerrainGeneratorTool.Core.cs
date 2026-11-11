#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGeneratorTool : MonoBehaviour
{
    [MenuItem("Shnok/4. [Terrain] Generate terrain")]
    static void GenerateTerrain() {
        GenerateTerrainForMap("l2_lobby");
    }

    [MenuItem("Shnok/3. [StaticMeshes] Generate staticmeshes")]
    static void GenerateStaticMeshes() {
        GenerateStaticMeshesForMap("l2_lobby");
    }
}
#endif
