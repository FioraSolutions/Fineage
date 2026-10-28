#if (UNITY_EDITOR) 
using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AmbientDataContainer {
    public AmbientSound[] data;
}

public class L2JSONAmbientSoundImporter : AssetImporter {

    static string ResolveSoundExportFolder() {
        if (!string.IsNullOrEmpty(L2InterludeClientSettings.UModelSoundExportRoot)) {
            return L2InterludeClientSettings.UModelSoundExportRoot;
        }

        return @"D:\Stock\Projects\L2-Unity\umodel_win32\export_sound";
    }

    static string ResolveSoundUnityExportFolder() {
        if (!string.IsNullOrEmpty(L2InterludeClientSettings.UModelSoundUnityExportRoot)) {
            return L2InterludeClientSettings.UModelSoundUnityExportRoot;
        }

        return Path.Combine(ResolveSoundExportFolder(), "unityexport");
    }

    [MenuItem("Shnok/[AmbientSound] (JSON) Import sounds")]
    static void ImportSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            // Debug.Log("Selected file: " + fileToProcess);
            AmbientSound[] sounds = L2TerrainInfoParser.ParseAmbientSoundFile(fileToProcess);
            ImportAmbientSoundFiles(sounds, ResolveSoundExportFolder());
        }
    }


    [MenuItem("Shnok/9. [AmbientSound] (T3D) Import sounds")]
    static void ImportSoundsMenuT3D() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            ImportFromT3d(fileToProcess);
        }
    }

    public static void ImportFromT3d(string t3dAbsolutePath) {
        List<AmbientSound> sounds = L2T3DInfoParser.ParseAmbientSounds(t3dAbsolutePath);
        ImportAmbientSoundFiles(sounds.ToArray(), ResolveSoundExportFolder());
    }

    private static void ImportAmbientSoundFiles(AmbientSound[] sounds, string dataFolder) {
        List<string> soundNames = new List<string>();
        foreach(AmbientSound sound in sounds) {
            // Debug.Log(sound);
            // Debug.Log(sound.ambientSoundName);
            string[] parts = sound.ambientSoundName.Split(".");
            soundNames.Add(parts[0].Trim() + "." + parts[2].Trim());
        }

        soundNames = soundNames.Distinct().ToList();
        foreach(string soundname in soundNames) {
            // Debug.Log(soundname);
            string[] parts = soundname.Split(".");
            string fileToCopy = Path.Combine(dataFolder, parts[0], "Sound", parts[1] + ".wav");
            // Debug.Log(fileToCopy);
            if(!File.Exists(fileToCopy)) {
                // Debug.LogError($"File missing at {fileToCopy}");
            } else {
                string outputFolder = Path.Combine(ResolveSoundUnityExportFolder(), parts[0]);
                string outputFile = Path.Combine(outputFolder, parts[1] + ".wav");
                if(!Directory.Exists(outputFolder)) {
                    Directory.CreateDirectory(outputFolder);
                }
                File.Copy(fileToCopy, outputFile, true);
            }
        }
        // Debug.LogWarning($"Need to {soundNames.Count} load ambient sounds.");

        // Debug.LogWarning($"Ambient {soundNames.Count} sound files individually copied into folder {scriptExportFolder}");
    }

}
#endif