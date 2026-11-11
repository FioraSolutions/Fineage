#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    private static GameObject GenerateMesh(Terrain map, Material terrainMat, bool generateHeightmap) {
        // Generate mesh
        GameObject destObject = new GameObject(terrainToConvert.gameObject.name);
        Vector3 initialPosition = map.transform.position;
        map.transform.position = Vector3.zero;
        terrainToMesh.meshSubdivisions = meshSubdivisions;
        terrainToMesh.terrain = terrainToConvert;
        terrainToMesh.dest = destObject;
        terrainToMesh.BuildMeshBase();
        if(generateHeightmap) {
            terrainToMesh.ConvertTerrain(optimizeMesh);
        }
        map.transform.position = initialPosition;

        // Move terrain to original position
        destObject.transform.position = initialPosition;
        destObject.isStatic = true;


        // Apply material to terrain
        if(terrainMat != null) {
            destObject.GetComponent<Renderer>().material = terrainMat;
        }

        return destObject;
    }
}
#endif
