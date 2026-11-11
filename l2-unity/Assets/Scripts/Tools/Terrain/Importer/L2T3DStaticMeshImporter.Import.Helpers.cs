#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class L2T3DStaticMeshImporter
{
    public static void ImportFromListFile(string listAbsolutePath, bool overwrite) {
        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadStaticMeshInfo(listAbsolutePath);
        ImportFromMeshInfo(terrainInfo, overwrite);
    }

    static void ImportFromMeshInfo(L2TerrainInfo terrainInfo, bool overwrite) {
        string dataFolder = L2InterludeClientSettings.UModelExportRoot;
        if (string.IsNullOrEmpty(dataFolder)) {
            dataFolder = @"D:\Stock\Projects\L2-Unity\umodel_win32\export";
        }

        List<string> files = ProcessStaticMeshInfo(dataFolder, terrainInfo.staticMeshes);
        ImportFiles(dataFolder, files, overwrite);
    }

    private static List<string> ProcessStaticMeshInfo(string dataFolder, List<L2StaticMesh> staticMeshes) {
        List<string> files = new List<string>();
        int staticMeshCount = 0;
        int textureInfoCount = 0;
        int missingMeshCount = 0;
        int missingTextureInfoCount = 0;

        foreach (L2StaticMesh mesh in staticMeshes) {
            if (mesh.staticMesh == null || mesh.staticMesh.Length == 0) continue;
            string[] parts = mesh.staticMesh.Split('.');
            string folder = parts[0];
            string file = parts[parts.Length - 1];

            string staticMeshPath = Path.Combine(dataFolder, folder, file + ".fbx");

            if (!File.Exists(staticMeshPath)) {
                missingMeshCount++;
                // Debug.LogWarning("Mesh missing:" + staticMeshPath);
            } else {
                staticMeshCount++;
                files.Add(staticMeshPath);
                string textureInfoPath = Path.Combine(dataFolder, folder, "StaticMesh", file + ".props.txt");
                if (!File.Exists(textureInfoPath)) {
                    missingTextureInfoCount++;
                    // Debug.LogWarning("Texture info missing:" + textureInfoPath);
                } else {
                    List<string> textures = ParseTextureInfo(dataFolder, textureInfoPath);
                    textureInfoCount+= textures.Count;
                    files.AddRange(textures);
                }
            }

       
        }

        files = files.Distinct().ToList();

        //files.ForEach((f) => Debug.Log(f)); 

        // Debug.Log($"{staticMeshCount} staticmesh(es) FBX will be imported.");
        // Debug.Log($"{missingMeshCount} missing staticmesh(es) FBX.");
        // Debug.Log($"{textureInfoCount} texture(s) will be imported.");
        // Debug.Log($"{missingTextureInfoCount} missing staticmesh(es) textureinfo.");

        // Debug.LogWarning($"{files.Count} distinct files(s) will be imported.");

        return files;
    }
}
#endif
