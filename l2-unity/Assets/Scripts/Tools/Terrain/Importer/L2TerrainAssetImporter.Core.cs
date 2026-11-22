#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract partial class L2TerrainAssetImporter
{
    protected static void ImportFiles(string inputFolder, List<string> files, bool overwrite) {
        foreach(var filePath in files) {
            // The import's job is to make the asset exist under Assets; when it is
            // already there the step is satisfied — never an error, copy or not.
            // (A tile whose textures are committed but whose umodel export dir is gone
            // must not log spurious "missing" errors, e.g. the lobby Brushes.json list.)
            string destination = GetAssetsDestination(inputFolder, filePath);
            if (!overwrite && destination != null && AssetFileExists(destination)) {
                continue;
            }

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

    // Assets destination for an export-side file, without needing the source to
    // exist: umodel writes PNGs under a Texture/ folder that is stripped so files
    // land at Textures|StaticMeshes/{package}/{name} (Shnok lookup layout).
    static string GetAssetsDestination(string inputFolder, string filePath)
    {
        string flattened = filePath.Replace("\\Texture", string.Empty).Replace("/Texture", string.Empty);
        string assetRelativePath = Path.GetRelativePath(inputFolder, flattened);
        string ext = Path.GetExtension(filePath).ToLower();
        if (ext == ".fbx") {
            return Path.Combine("Assets", "Resources", "Data", "StaticMeshes", assetRelativePath);
        }
        if (ext == ".png" || ext == ".txt") {
            return Path.Combine("Assets", "Resources", "Data", "Textures", assetRelativePath);
        }
        return null;
    }

    // File.Exists is case-sensitive on some filesystems while UE2 package case in
    // the t3d/JSON does not always match the committed folder case
    // (Castle_Kent_t vs castle_kent_t). Resolve each path component
    // case-insensitively as a fallback.
    static bool AssetFileExists(string assetRelativePath)
    {
        if (File.Exists(assetRelativePath)) return true;
        string[] parts = assetRelativePath.Split(
            new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
            StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || !Directory.Exists(parts[0])) return false;
        string cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            bool last = i == parts.Length - 1;
            string hit = null;
            foreach (string entry in last ? Directory.GetFiles(cur) : Directory.GetDirectories(cur))
            {
                if (string.Equals(Path.GetFileName(entry), parts[i], StringComparison.OrdinalIgnoreCase))
                {
                    hit = entry;
                    break;
                }
            }
            if (hit == null) return false;
            cur = hit;
        }
        return true;
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
