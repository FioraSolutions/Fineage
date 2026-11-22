#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

[System.Serializable]
public class DataContainer {
    public Brush[] data;
}

public class L2JSONBrushImporter : L2TerrainAssetImporter {

    [MenuItem("Shnok/[Brush] Import Textures")]
    static void ImportBrushTextures() {
        string title = "Select Brush list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string dataFolder = L2InterludeClientSettings.UModelExportRoot;
        if (string.IsNullOrEmpty(dataFolder)) {
            dataFolder = @"D:\Stock\Projects\L2-Unity\Tools\umodel_win32\export";
        }
        bool overwrite = false;

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            ImportBrushTexturesFromJson(fileToProcess, overwrite);
        }
    }

    public static void ImportBrushTexturesFromJson(string brushJsonAbsolutePath, bool overwrite) {
        string dataFolder = L2InterludeClientSettings.UModelExportRoot;
        if (string.IsNullOrEmpty(dataFolder)) {
            dataFolder = @"D:\Stock\Projects\L2-Unity\Tools\umodel_win32\export";
        }

        RequireBrushTexturesPresent(brushJsonAbsolutePath, dataFolder);
        List<string> files = ProcessDataFile(dataFolder, brushJsonAbsolutePath);
        ImportFiles(dataFolder, files, overwrite);
    }

    // Pre-bake gate. Every non-shader brush/BSP texture must already be an asset
    // or be present in the export ({package}/{name}.png or {package}/Texture/{name}.png).
    // One exception names every missing package and texture; the bake stops here
    // instead of continuing into material binding.
    public static void RequireBrushTexturesPresent(string brushJsonAbsolutePath, string exportRoot) {
        List<string> files = ProcessDataFile(exportRoot, brushJsonAbsolutePath);
        var missing = new List<string>();
        foreach (string filePath in files) {
            if (BrushTextureSatisfied(exportRoot, filePath, false, out _, out _, out string detail)) {
                continue;
            }
            missing.Add(detail);
        }

        if (missing.Count > 0) {
            throw new InvalidOperationException(
                "[Interlude bake] brush texture missing (" + missing.Count + "):\n"
                + string.Join("\n", missing));
        }
    }

    private static List<string> ProcessDataFile(string dataFolder, string fileToProcess) {
        List<string> files = new List<string>();

        Brush[] brushes = ParseBrushFile(fileToProcess);
        foreach(Brush brush in brushes) {
            if(brush.model != null) {
                if(brush.model.poly != null) {
                    for(int i = 0; i < brush.model.poly.polyData.Length; i++) {
                        if(!string.IsNullOrEmpty(brush.model.poly.polyData[i].texture)) {
                            string textureName = brush.model.poly.polyData[i].texture;
                            if(textureName.Contains("Shaders")) {
                                // Debug.Log("Skipping shaders " + textureName);
                                continue;
                            }

                            string[] parts = textureName.Split('.');
                            string folder = parts[0];
                            string file = parts[parts.Length - 1];

                            files.Add(Path.Combine(dataFolder, folder, file + ".png"));
                        }
                    }
                }
            }
        }

        files = files.Distinct().ToList();
        // Debug.LogWarning("Total files to import: " + files.Count);

        return files;
    }

    public static Brush[] ParseBrushFile(string path) {
        string json = File.ReadAllText(path);
        DataContainer brushes = JsonUtility.FromJson<DataContainer>(json);
        if (brushes == null || brushes.data == null) {
            throw new InvalidOperationException(
                "[Interlude bake] Brushes.json has no brush array: " + path);
        }
        return brushes.data;
    }
}
#endif
