#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract partial class L2TerrainAssetImporter
{
    protected static void ImportFiles(string inputFolder, List<string> files, bool overwrite) {
        // Brush import calls RequireBrushTexturesPresent first and throws there.
        // This copy loop is the shared "already in Assets, or copy from the export"
        // step for brushes and static meshes. A miss that reaches here is still an
        // error (logged with package, texture, and path); it does not invent a file.
        foreach(var filePath in files) {
            if (BrushTextureSatisfied(inputFolder, filePath, overwrite, out string source, out string destination, out string missing)) {
                if (source == null || destination == null) {
                    continue;
                }
            } else {
                Debug.LogError(
                    "L2TerrainAssetImporter: brush texture file missing after rectification; " + missing);
                continue;
            }

            if(!Directory.Exists(Path.GetDirectoryName(destination))) {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }

            try {
                File.Copy(source, destination, overwrite);
            } catch (IOException) {
                // Debug.LogWarning(destination + " already exists or " + e.Message);
            }
        }
    }

    // True when the texture is already under Assets or the export has the PNG
    // (flat {package}/{name}.png, or umodel {package}/Texture/{name}.png).
    // missing names package, texture file, and the rectified path.
    protected static bool BrushTextureSatisfied(
        string inputFolder,
        string filePath,
        bool overwrite,
        out string sourcePath,
        out string destination,
        out string missing)
    {
        sourcePath = null;
        destination = GetAssetsDestination(inputFolder, filePath);
        missing = null;
        // Already committed: the step is done. A vanished export dir is not an error.
        if (!overwrite && destination != null && AssetFileExists(destination)) {
            return true;
        }

        if (File.Exists(filePath)) {
            sourcePath = filePath;
            return true;
        }

        string relativePath = GetRelativeExportPath(inputFolder, filePath);
        string[] segments = SplitExportPath(relativePath);
        if (segments.Length < 2)
        {
            missing = $"package='', texture='', expectedPath='{filePath}', originalPath='{filePath}', "
                + $"inputFolder='{inputFolder}', relative='{relativePath}'";
            return false;
        }

        string package = segments[0];
        string textureFile = segments[segments.Length - 1];
        string expected = Path.Combine(inputFolder, package, "Texture", textureFile);
        missing = $"package='{package}', texture='{textureFile}', expectedPath='{expected}', originalPath='{filePath}'";
        if (File.Exists(expected)) {
            sourcePath = expected;
            return true;
        }

        return false;
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
