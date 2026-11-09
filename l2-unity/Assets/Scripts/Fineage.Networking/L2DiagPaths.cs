using System.IO;
using UnityEngine;

/// <summary>
/// Portable paths for diagnostic dumps. Replaces hardcoded
/// <c>C:\unity\l2 client\decompile\...</c> paths from the upstream Windows machine.
/// Files land under <c>{project}/logs/diag/</c> (gitignored via logs/).
/// </summary>
public static class L2DiagPaths
{
    public static string DiagDir
    {
        get
        {
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string dir = Path.Combine(projectRoot, "logs", "diag");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    /// <summary>Path under <see cref="DiagDir"/>. Named DiagFile so it does not shadow <see cref="System.IO.File"/>.</summary>
    public static string DiagFile(string fileName)
    {
        return Path.Combine(DiagDir, fileName);
    }

    /// <summary>
    /// Optional external capture/extract tree (bridge bake, umodel dumps).
    /// Set env <c>L2_DECOMPILE_ROOT</c>, or place files under <c>{project}/External/decompile/</c>.
    /// </summary>
    public static string DecompileRoot
    {
        get
        {
            string env = System.Environment.GetEnvironmentVariable("L2_DECOMPILE_ROOT");
            if (!string.IsNullOrEmpty(env) && Directory.Exists(env))
                return env;

            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string local = Path.Combine(projectRoot, "External", "decompile");
            if (Directory.Exists(local))
                return local;

            return local;
        }
    }

    public static string DecompileFile(params string[] parts)
    {
        string path = DecompileRoot;
        for (int i = 0; i < parts.Length; i++)
            path = Path.Combine(path, parts[i]);
        return path;
    }

    /// <summary>umodel extract tree. Env <c>L2_UMODEL_ROOT</c> or <c>External/umodel</c>.</summary>
    public static string UmodelRoot
    {
        get
        {
            string env = System.Environment.GetEnvironmentVariable("L2_UMODEL_ROOT");
            if (!string.IsNullOrEmpty(env))
                return env;
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            return Path.Combine(projectRoot, "External", "umodel");
        }
    }

    public static string UmodelFile(params string[] parts)
    {
        string path = UmodelRoot;
        for (int i = 0; i < parts.Length; i++)
            path = Path.Combine(path, parts[i]);
        return path;
    }

    /// <summary>Blender for psk/psa import. Env <c>L2_BLENDER</c> or PATH <c>blender</c>.</summary>
    public static string BlenderExe
    {
        get
        {
            string env = System.Environment.GetEnvironmentVariable("L2_BLENDER");
            if (!string.IsNullOrEmpty(env))
                return env;
#if UNITY_EDITOR_OSX
            const string macApp = "/Applications/Blender.app/Contents/MacOS/Blender";
            if (File.Exists(macApp))
                return macApp;
#endif
            return "blender";
        }
    }
}
