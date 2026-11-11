#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class L2BrushBuilder
{
    [MenuItem("Shnok/[Brush] (JSON) Build brushes")]
    static void ImportBrushTextures() {
        string title = "Select Brush list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);

            Brush[] brushes = L2JSONBrushImporter.ParseBrushFile(fileToProcess);

            Build(brushes);
        }
    }

    [MenuItem("Shnok/5. [Brush] (T3D) Build brushes")]
    static void ImportBrushTexturesT3D() {
        string title = "Select T3D file";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            BuildBrushesFromT3d(fileToProcess);
        }
    }
}
#endif