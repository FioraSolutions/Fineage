#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Package and extract locations used by the effect importer (usx/ukx/utx + umodel).
/// </summary>
public static class L2EffectPackageRoots
{
    public const string TextureDestFolder =
        "Assets/Resources/Data/SysTextures/LineageEffectsTextures";
    public const string MeshDestFolder =
        "Assets/Resources/Data/StaticMeshes/LineageEffectsStaticmeshes";

    // Optional local clones; discovery also walks parents for l2-viwer-effect.
    // Override with env L2_VIEWER_ROOT / L2_UMODEL_MESH_DIR / L2_UMODEL_TEX_DIR.
    public static string DefaultViewerRoot =>
        System.Environment.GetEnvironmentVariable("L2_VIEWER_ROOT")
        ?? Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "External", "l2-viwer-effect", "My project");
    public static string ExtraViewerRoot =>
        Path.GetDirectoryName(DefaultViewerRoot) ?? "";
    public static string ExtraMeshDir =>
        System.Environment.GetEnvironmentVariable("L2_UMODEL_MESH_DIR") ?? "";
    public static string ExtraTextureDir =>
        System.Environment.GetEnvironmentVariable("L2_UMODEL_TEX_DIR") ?? "";
    public static string ExtraSysTexturesDir =>
        System.Environment.GetEnvironmentVariable("L2_UMODEL_SYS_TEX_DIR") ?? ExtraTextureDir;

    public static string FindUModelExe()
    {
        string root = ViewerProjectRoot();
        string[] candidates =
        {
            Path.Combine(root, "Tools", "UModel", "umodel_64.exe"),
            Path.Combine(root, "Tools", "UModel", "umodel.exe")
        };
        for (int i = 0; i < candidates.Length; i++)
        {
            if (File.Exists(candidates[i]))
            {
                return candidates[i];
            }
        }

        return "";
    }

    public static string ViewerProjectRoot()
    {
        if (Directory.Exists(DefaultViewerRoot))
        {
            return DefaultViewerRoot;
        }

        string current = Application.dataPath;
        for (int i = 0; i < 8 && !string.IsNullOrEmpty(current); i++)
        {
            DirectoryInfo parent = Directory.GetParent(current);
            if (parent == null)
            {
                break;
            }

            string candidate = Path.Combine(parent.FullName, "l2-viwer-effect", "My project");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = parent.FullName;
        }

        return DefaultViewerRoot;
    }

    public static string ViewerDataDir()
    {
        return Path.Combine(ViewerProjectRoot(), "Assets", "StreamingAssets", "ViewerData");
    }

    public static string ExtractRoot()
    {
        string dir = Path.Combine(
            Directory.GetParent(Application.dataPath)!.FullName,
            "Temp",
            "L2ViewerExtract");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public static string UModelWorkDir()
    {
        string dir = Path.Combine(ExtractRoot(), "umodel-work");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public static string TextureSearchPath(string packagePath)
    {
        foreach (string dir in TextureSearchRoots())
        {
            if (Directory.Exists(dir))
            {
                return dir;
            }
        }

        string packageDir = Path.GetDirectoryName(packagePath);
        return Directory.Exists(packageDir) ? packageDir : ViewerDataDir();
    }

    public static IEnumerable<string> TextureSearchRoots()
    {
        yield return ViewerDataDir();
        yield return Path.Combine(ViewerDataDir(), "Textures");
        yield return ExtraSysTexturesDir;
        yield return ExtraTextureDir;
    }

    public static IEnumerable<string> MeshPackageDirs()
    {
        yield return Path.Combine(ViewerDataDir(), "Meshes");
        yield return ExtraMeshDir;
        yield return Path.Combine(ViewerDataDir(), "Textures");
        yield return ExtraViewerRoot;
    }

    public static IEnumerable<string> TexturePackageDirs()
    {
        yield return Path.Combine(ViewerDataDir(), "Textures");
        yield return ExtraTextureDir;
        yield return ExtraSysTexturesDir;
        yield return Path.Combine(ViewerDataDir(), "Meshes");
    }

    public static IEnumerable<string> PackageNameCandidates(string packageName, bool texture)
    {
        if (!string.IsNullOrEmpty(packageName))
        {
            yield return packageName;
        }

        if (texture)
        {
            yield return "LineageEffectsTextures";
            yield return "LineageEffectsTextures2";
            yield return "FX_E_T";
            yield return "FX_M_T";
            yield return "WarEffectsTextures";
            yield return "LineageEffectsTexturesCha";
            yield return "LineageEffectsTextures3";
        }
        else
        {
            yield return "LineageEffectsStaticmeshes2";
            yield return "LineageEffectsStaticmeshes";
            yield return "LineageEffectsStaticmeshes3";
            yield return "LineageItemStaticMeshs";
            yield return "LineageEffectMeshes";
            yield return "LineageEffectMeshes2";
            yield return "LineageEffectMeshes3";
            yield return "FX_E_S";
        }
    }
}
#endif
