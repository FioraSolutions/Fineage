#if UNITY_EDITOR
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class L2AmbientSoundBuilder : MonoBehaviour
{
    [MenuItem("Shnok/[AmbientSound] (JSON) Build ambient sounds")]
    static void BuildSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);
            AmbientSound[] sounds = L2TerrainInfoParser.ParseAmbientSoundFile(fileToProcess);
            BuildAmbientSounds(sounds);
        }
    }

    [MenuItem("Shnok/9.1. [AmbientSound] (T3D) Build ambient sounds")]
    static void BuildSoundsMenuT3D() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            BuildAmbientSoundsFromT3d(fileToProcess);
        }
    }
}
#endif
