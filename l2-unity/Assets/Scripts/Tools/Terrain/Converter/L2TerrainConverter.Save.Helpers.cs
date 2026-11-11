#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    private static void SaveMaterial(string saveFolder, Material terrainMat) {
        // Save material
        string materialPath = Path.Combine(saveFolder, mapToConvert + "_Mat.mat");

        // Save the material as an asset
        AssetDatabase.CreateAsset(terrainMat, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SaveTerrain(string saveFolder, GameObject destObject) {
        // Save terrain Prefab
        string prefabPath = Path.Combine(saveFolder, mapToConvert + ".prefab");
        // Ground collider for client-side gravity (mobs/NPC snap); non-convex, uses persisted mesh.
        MeshFilter mf = destObject.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null) {
            MeshCollider mc = destObject.GetComponent<MeshCollider>();
            if (mc == null) mc = destObject.AddComponent<MeshCollider>();
            mc.convex = false;
            mc.sharedMesh = mf.sharedMesh;
        }
        PrefabUtility.SaveAsPrefabAsset(destObject, prefabPath);
    }
}
#endif
