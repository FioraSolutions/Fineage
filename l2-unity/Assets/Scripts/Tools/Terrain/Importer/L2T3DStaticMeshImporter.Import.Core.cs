#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class L2T3DStaticMeshImporter
{
    [MenuItem("Shnok/1. [StaticMeshes] Import Textures and models")]
    static void ImportStaticMeshes() {
        string title = "Select StaticMeshes list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "txt,t3d";

        string dataFolder = L2InterludeClientSettings.UModelExportRoot;
        if (string.IsNullOrEmpty(dataFolder)) {
            dataFolder = @"D:\Stock\Projects\L2-Unity\umodel_win32\export";
        }
        bool overwrite = false;

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            ImportFromListFile(fileToProcess, overwrite);
        }
    }

    public static void ImportFromT3d(string t3dAbsolutePath, bool overwrite) {
        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadStaticMeshInfo(t3dAbsolutePath);
        ImportFromMeshInfo(terrainInfo, overwrite);
    }
}
#endif
