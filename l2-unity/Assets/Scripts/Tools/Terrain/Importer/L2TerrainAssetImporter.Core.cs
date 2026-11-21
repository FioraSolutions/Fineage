#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract partial class L2TerrainAssetImporter
{
    protected static void ImportFiles(string inputFolder, List<string> files, bool overwrite) {
        foreach(var filePath in files) {
            string path = filePath;
            if(!File.Exists(path)) {
                string relativePath = GetRelativeExportPath(inputFolder, path);
                string[] segments = SplitExportPath(relativePath);
                if (segments.Length < 2)
                {
                    Debug.LogError(
                        "L2TerrainAssetImporter: cannot resolve missing brush texture; "
                        + $"inputFolder='{inputFolder}', texturePath='{filePath}', relative='{relativePath}'.");
                    continue;
                }

                string package = segments[0];
                string textureFile = segments[segments.Length - 1];
                path = Path.Combine(inputFolder, package, "Texture", textureFile);

                if(!File.Exists(path)) {
                    Debug.LogError(
                        "L2TerrainAssetImporter: brush texture file missing after rectification; "
                        + $"package='{package}', texture='{textureFile}', "
                        + $"expectedPath='{path}', originalPath='{filePath}'.");
                    continue;
                }
            }

            // UModel writes PNGs under a Texture/ folder. Windows paths use
            // backslashes; Mac batch bakes use forward slashes. Strip either so
            // files land at Textures/{package}/{name}.png (Shnok lookup layout).
            string flattened = path.Replace("\\Texture", string.Empty).Replace("/Texture", string.Empty);
            string assetRelativePath = Path.GetRelativePath(inputFolder, flattened);
            string destination = null;
            if(Path.GetExtension(path).ToLower() == ".fbx") {
                destination = Path.Combine("Assets", "Resources", "Data", "StaticMeshes", assetRelativePath);
                // Debug.Log(destination);
            } else if(Path.GetExtension(path).ToLower() == ".png") {
                destination = Path.Combine("Assets", "Resources", "Data", "Textures", assetRelativePath);
                // Debug.Log(destination);
            } else if(Path.GetExtension(path).ToLower() == ".txt") {
                destination = Path.Combine("Assets", "Resources", "Data", "Textures", assetRelativePath);
                // Debug.Log(destination);
            }

            if(destination != null) {
                if(!Directory.Exists(Path.GetDirectoryName(destination))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                }

                try {
                    File.Copy(path, destination, overwrite);
                } catch (IOException) {
                    // Debug.LogWarning(destination + " already exists or " + e.Message);
                }

            }
        }
    }

    static string GetRelativeExportPath(string inputFolder, string absolutePath)
    {
        if (absolutePath.StartsWith(inputFolder, StringComparison.Ordinal))
            return absolutePath.Substring(inputFolder.Length);

        return absolutePath;
    }

    static string[] SplitExportPath(string path)
    {
        string trimmed = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return trimmed.Split(
            new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
            StringSplitOptions.RemoveEmptyEntries);
    }

    protected static string FindInSubDirectories(string baseFolder, string fileName) {
        try {
            string[] files = Directory.GetFiles(baseFolder, fileName, SearchOption.AllDirectories);

            if(files.Length > 0) {
                foreach(string file in files) {
                    return file;
                }
            } else {
                // Debug.Log("File not found in the specified directory.");
            }
        } catch (DirectoryNotFoundException) {
            // Debug.Log("Directory not found: " + e.Message);
        } catch (Exception) {
            // Debug.LogError("An error occurred: " + ex.Message);
        }

        return null;
    }
}
#endif
