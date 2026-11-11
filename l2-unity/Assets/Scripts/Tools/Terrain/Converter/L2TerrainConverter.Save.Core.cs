#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class TerrainConverter
{
    private static Mesh SaveMesh(string saveFolder, GameObject destObject, bool safenet) {
        Mesh convertedTerrainMesh = destObject.GetComponent<MeshFilter>().sharedMesh;
        string filename = mapToConvert + "_Mesh.asset";
        if(safenet) {
            filename = "Safenet_Mesh.asset";
        }
        string saveMeshPath = Path.Combine(saveFolder, filename).Replace("\\", "/");
        if (AssetDatabase.LoadAssetAtPath<Mesh>(saveMeshPath) != null) {
            AssetDatabase.DeleteAsset(saveMeshPath);
        }
        MeshSaverEditor.SaveMeshToPath(convertedTerrainMesh, saveMeshPath, false, true);
        return AssetDatabase.LoadAssetAtPath<Mesh>(saveMeshPath);
    }

    private static Material GenerateMaterial(string saveFolder, L2TerrainInfo terrainInfo) {
        // Create material
        Material terrainMat = new Material(Shader.Find("Shader Graphs/TerrainAlphamapDouble"));

        // Generate alphamap
        Texture2D[] alphaMaps = new Texture2D[terrainInfo.uvLayers.Count - 1];
        Texture2D[] layers = new Texture2D[terrainInfo.uvLayers.Count - 1];

        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            //if(i > 10) {
            //    Debug.LogWarning("Too many layers (" + terrainInfo.uvLayers.Count + ") for the material.");
            //}

            if (i > 0) {
                alphaMaps[i - 1] = terrainInfo.uvLayers[i].alphaMap;
                layers[i - 1] = terrainInfo.uvLayers[i].texture;
                terrainMat.SetFloat("_Layer_" + i + "_Enabled", 1.0f);
            } else {
                terrainMat.SetTexture("_Layer_" + i, terrainInfo.uvLayers[i].texture);
            }

            //terrainMat.SetTexture("_Layer_" + i, terrainInfo.uvLayers[i].texture);
            //terrainMat.SetTextureScale("_Layer_" + i, new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale));
            L2TerrainLayer uvl = terrainInfo.uvLayers[i];
            float us = uvl.uScale == 0 ? 1f : uvl.uScale, vs = uvl.vScale == 0 ? 1f : uvl.vScale;
            terrainMat.SetVector("_Layer_" + i + "_UV", new Vector4(uvl.uScale, uvl.vScale, uvl.uOffset / us, uvl.vOffset / vs));
        }

        // Build Texture2D array for textures
        if (terrainInfo.uvLayers.Count - 1 > 0) {
            texture2DArrayGenerator.sourceTextures = layers;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Layer.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            terrainMat.SetTexture("_Layers", alphamaps);
        }

        // Build Texture2D array for alphamaps
        if (alphaMaps.Length > 0) {
            texture2DArrayGenerator.sourceTextures = alphaMaps;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Alphamap.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            terrainMat.SetTexture("_Splatmaps", alphamaps);
        }

        terrainMat.SetVector("_Global_UV_Size", new Vector2(uvScale, uvScale));

        return terrainMat;
    }
}
#endif
